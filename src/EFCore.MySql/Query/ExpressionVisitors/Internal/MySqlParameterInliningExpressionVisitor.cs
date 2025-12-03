// Copyright (c) Pomelo Foundation. All rights reserved.
// Licensed under the MIT. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionTranslators.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;

/// <summary>
/// Inject parameter inlining expressions where parameters are not supported for some reason.
/// </summary>
public class MySqlParameterInliningExpressionVisitor : ExpressionVisitor
{
    private readonly IRelationalTypeMappingSource _typeMappingSource;
    private readonly ISqlExpressionFactory _sqlExpressionFactory;
    private readonly IMySqlOptions _options;

    private ParametersCacheDecorator _parametersDecorator;

    private bool _shouldInlineParameters;

    public MySqlParameterInliningExpressionVisitor(
        IRelationalTypeMappingSource typeMappingSource,
        ISqlExpressionFactory sqlExpressionFactory,
        IMySqlOptions options)
    {
        _typeMappingSource = typeMappingSource;
        _sqlExpressionFactory = sqlExpressionFactory;
        _options = options;
    }

    public virtual Expression Process(Expression expression, ParametersCacheDecorator parametersDecorator)
    {
        Check.NotNull(expression, nameof(expression));

        _parametersDecorator = parametersDecorator;
        _shouldInlineParameters = false;

        var result = Visit(expression);

        return result;
    }

    protected override Expression VisitExtension(Expression extensionExpression)
        => extensionExpression switch
        {
            MySqlJsonTableExpression jsonTableExpression => VisitJsonTable(jsonTableExpression),
            SelectExpression selectExpression => VisitSelect(selectExpression),
            SqlFunctionExpression sqlFunctionExpression => VisitSqlFunction(sqlFunctionExpression),
            SqlParameterExpression sqlParameterExpression => VisitSqlParameter(sqlParameterExpression),
            ShapedQueryExpression shapedQueryExpression => shapedQueryExpression.Update(
                Visit(shapedQueryExpression.QueryExpression),
                Visit(shapedQueryExpression.ShaperExpression)),
            _ => base.VisitExtension(extensionExpression)
        };

    protected virtual Expression VisitSqlFunction(SqlFunctionExpression sqlFunctionExpression)
    {
        // For LEAST/GREATEST functions, evaluate and replace with constant (MariaDB 11.6.2 doesn't support functions in LIMIT)
        if (sqlFunctionExpression.Name.Equals("LEAST", StringComparison.OrdinalIgnoreCase) ||
            sqlFunctionExpression.Name.Equals("GREATEST", StringComparison.OrdinalIgnoreCase))
        {
            return NewInlineParametersScope(
                inlineParameters: true,
                () => {
                    // Visit arguments to ensure they're inlined
                    var visitedArguments = new List<SqlExpression>();
                    foreach (var arg in sqlFunctionExpression.Arguments)
                    {
                        visitedArguments.Add((SqlExpression)Visit(arg));
                    }
                    
                    // Extract constant values from inlined parameters
                    var values = new List<long>();
                    foreach (var arg in visitedArguments)
                    {
                        if (arg is MySqlInlinedParameterExpression inlinedParam &&
                            inlinedParam.ValueExpression.Value != null)
                        {
                            values.Add(Convert.ToInt64(inlinedParam.ValueExpression.Value));
                        }
                        else if (arg is SqlConstantExpression constant &&
                                constant.Value != null)
                        {
                            values.Add(Convert.ToInt64(constant.Value));
                        }
                    }

                    // If we have values, evaluate LEAST/GREATEST and return constant
                    if (values.Count > 0)
                    {
                        var isLeast = sqlFunctionExpression.Name.Equals("LEAST", StringComparison.OrdinalIgnoreCase);
                        var result = isLeast ? values.Min() : values.Max();
                        
                        return _sqlExpressionFactory.Constant(result, sqlFunctionExpression.TypeMapping);
                    }
                    
                    // Fallback: return function with inlined arguments
                    return sqlFunctionExpression.Update(
                        sqlFunctionExpression.Instance,
                        visitedArguments);
                });
        }

        return base.VisitExtension(sqlFunctionExpression);
    }

    protected virtual Expression VisitSelect(SelectExpression selectExpression)
        => NewInlineParametersScope(
            inlineParameters: false,
            () => base.VisitExtension(selectExpression));

    // For test simplicity, we currently inline parameters even for non MySQL database engines (even though it should not be necessary
    // for e.g. MariaDB).
    // TODO: Use inlined parameters only if JsonTableImplementationUsingParameterAsSourceWithoutEngineCrash is true.
    protected virtual Expression VisitJsonTable(MySqlJsonTableExpression jsonTableExpression)
        => jsonTableExpression.Update(
            NewInlineParametersScope(
                inlineParameters: true,
                () => (SqlExpression)Visit(jsonTableExpression.JsonExpression)),
            jsonTableExpression.Path,
            jsonTableExpression.ColumnInfos);

    protected virtual Expression VisitSqlParameter(SqlParameterExpression sqlParameterExpression)
    {
        if (!_shouldInlineParameters)
        {
            return sqlParameterExpression;
        }

        var parameterValues = _parametersDecorator.GetAndDisableCaching();

        return new MySqlInlinedParameterExpression(
            sqlParameterExpression,
            (SqlConstantExpression)_sqlExpressionFactory.Constant(
                parameterValues[sqlParameterExpression.Name],
                sqlParameterExpression.TypeMapping));
    }

    protected virtual T NewInlineParametersScope<T>(bool inlineParameters, Func<T> func)
    {
        var parentShouldInlineParameters = _shouldInlineParameters;
        _shouldInlineParameters = inlineParameters;

        try
        {
            return func();
        }
        finally
        {
            _shouldInlineParameters = parentShouldInlineParameters;
        }
    }
}
