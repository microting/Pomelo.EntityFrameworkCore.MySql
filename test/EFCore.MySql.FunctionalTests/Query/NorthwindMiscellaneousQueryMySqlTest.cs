using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Internal;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public partial class NorthwindMiscellaneousQueryMySqlTest : NorthwindMiscellaneousQueryRelationalTestBase<
        NorthwindQueryMySqlFixture<NoopModelCustomizer>>
    {
        public NorthwindMiscellaneousQueryMySqlTest(
            NorthwindQueryMySqlFixture<NoopModelCustomizer> fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            ClearLog();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }

        [ConditionalTheory]
        public override async Task Take_Skip(bool async)
        {
            await base.Take_Skip(async);

        AssertSql(
"""
@p='10'
@p0='5'

SELECT `c0`.`CustomerID`, `c0`.`Address`, `c0`.`City`, `c0`.`CompanyName`, `c0`.`ContactName`, `c0`.`ContactTitle`, `c0`.`Country`, `c0`.`Fax`, `c0`.`Phone`, `c0`.`PostalCode`, `c0`.`Region`
FROM (
    SELECT `c`.`CustomerID`, `c`.`Address`, `c`.`City`, `c`.`CompanyName`, `c`.`ContactName`, `c`.`ContactTitle`, `c`.`Country`, `c`.`Fax`, `c`.`Phone`, `c`.`PostalCode`, `c`.`Region`
    FROM `Customers` AS `c`
    ORDER BY `c`.`ContactName`
    LIMIT @p
) AS `c0`
ORDER BY `c0`.`ContactName`
LIMIT 18446744073709551610 OFFSET @p0
""");
        }

        [ConditionalTheory]
        public override async Task Select_expression_references_are_updated_correctly_with_subquery(bool async)
        {
            await base.Select_expression_references_are_updated_correctly_with_subquery(async);

            AssertSql(
"""
@nextYear='2017'

SELECT DISTINCT EXTRACT(year FROM `o`.`OrderDate`)
FROM `Orders` AS `o`
WHERE `o`.`OrderDate` IS NOT NULL AND (EXTRACT(year FROM `o`.`OrderDate`) < @nextYear)
""");
        }

        public override Task Entity_equality_orderby_subquery(bool async)
        {
            // Ordering in the base test is arbitrary.
            return AssertQuery(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.Orders.OrderBy(o => o.OrderID).FirstOrDefault()).ThenBy(c => c.CustomerID),
                ss => ss.Set<Customer>().OrderBy(c => c.Orders.FirstOrDefault() == null ? (int?)null : c.Orders.OrderBy(o => o.OrderID).FirstOrDefault().OrderID).ThenBy(c => c.CustomerID),
                assertOrder: true);
        }

        public override Task Using_string_Equals_with_StringComparison_throws_informative_error(bool async)
        {
            return AssertTranslationFailedWithDetails(
                () => AssertQuery(
                    async,
                    ss => ss.Set<Customer>().Where(c => c.CustomerID.Equals("ALFKI", StringComparison.InvariantCulture))),
                MySqlStrings.QueryUnableToTranslateMethodWithStringComparison(nameof(String), nameof(string.Equals), nameof(MySqlDbContextOptionsBuilder.EnableStringComparisonTranslations)));
        }

        public override Task Using_static_string_Equals_with_StringComparison_throws_informative_error(bool async)
        {
            return AssertTranslationFailedWithDetails(
                () => AssertQuery(
                    async,
                    ss => ss.Set<Customer>().Where(c => string.Equals(c.CustomerID, "ALFKI", StringComparison.InvariantCulture))),
                MySqlStrings.QueryUnableToTranslateMethodWithStringComparison(nameof(String), nameof(string.Equals), nameof(MySqlDbContextOptionsBuilder.EnableStringComparisonTranslations)));
        }

        /// <summary>
        /// Needs explicit ordering of ProductIds to work with MariaDB.
        /// </summary>
        public override async Task Projection_skip_collection_projection(bool async)
        {
            // await base.Projection_skip_collection_projection(async);
            await AssertQuery(
                async,
                ss => ss.Set<Order>()
                    .Where(o => o.OrderID < 10300)
                    .OrderBy(o => o.OrderID)
                    .Select(o => new { Item = o })
                    .Skip(5)
                    .Select(e => new { e.Item.OrderID, ProductIds = e.Item.OrderDetails.OrderBy(od => od.ProductID).Select(od => od.ProductID).ToList() }), // added .OrderBy(od => od.ProductID)
                assertOrder: true,
                elementAsserter: (e, a) =>
                {
                    Assert.Equal(e.OrderID, a.OrderID);
                    AssertCollection(e.ProductIds, a.ProductIds, ordered: true, elementAsserter: (ie, ia) => Assert.Equal(ie, ia));
                });

        AssertSql(
"""
@p='5'

SELECT `o1`.`OrderID`, `o0`.`ProductID`, `o0`.`OrderID`
FROM (
    SELECT `o`.`OrderID`
    FROM `Orders` AS `o`
    WHERE `o`.`OrderID` < 10300
    ORDER BY `o`.`OrderID`
    LIMIT 18446744073709551610 OFFSET @p
) AS `o1`
LEFT JOIN `Order Details` AS `o0` ON `o1`.`OrderID` = `o0`.`OrderID`
ORDER BY `o1`.`OrderID`, `o0`.`ProductID`
""");
        }

        /// <summary>
        /// Needs explicit ordering of ProductIds to work with MariaDB.
        /// </summary>
        public override async Task Projection_skip_take_collection_projection(bool async)
        {
            // await base.Projection_skip_take_collection_projection(async);
            await AssertQuery(
                async,
                ss => ss.Set<Order>()
                    .Where(o => o.OrderID < 10300)
                    .OrderBy(o => o.OrderID)
                    .Select(o => new { Item = o })
                    .Skip(5)
                    .Take(10)
                    .Select(e => new { e.Item.OrderID, ProductIds = e.Item.OrderDetails.OrderBy(od => od.ProductID).Select(od => od.ProductID).ToList() }), // added .OrderBy(od => od.ProductID)
                assertOrder: true,
                elementAsserter: (e, a) =>
                {
                    Assert.Equal(e.OrderID, a.OrderID);
                    AssertCollection(e.ProductIds, a.ProductIds, ordered: true, elementAsserter: (ie, ia) => Assert.Equal(ie, ia));
                });

        AssertSql(
"""
@p0='10'
@p='5'

SELECT `o1`.`OrderID`, `o0`.`ProductID`, `o0`.`OrderID`
FROM (
    SELECT `o`.`OrderID`
    FROM `Orders` AS `o`
    WHERE `o`.`OrderID` < 10300
    ORDER BY `o`.`OrderID`
    LIMIT @p0 OFFSET @p
) AS `o1`
LEFT JOIN `Order Details` AS `o0` ON `o1`.`OrderID` = `o0`.`OrderID`
ORDER BY `o1`.`OrderID`, `o0`.`ProductID`
""");
        }

        /// <summary>
        /// Needs explicit ordering of ProductIds to work with MariaDB.
        /// </summary>
        public override async Task Projection_take_collection_projection(bool async)
        {
            // await base.Projection_take_collection_projection(async);
            await AssertQuery(
                async,
                ss => ss.Set<Order>()
                    .Where(o => o.OrderID < 10300)
                    .OrderBy(o => o.OrderID)
                    .Select(o => new { Item = o })
                    .Take(10)
                    .Select(e => new { e.Item.OrderID, ProductIds = e.Item.OrderDetails.OrderBy(od => od.ProductID).Select(od => od.ProductID).ToList() }), // added .OrderBy(od => od.ProductID)
                assertOrder: true,
                elementAsserter: (e, a) =>
                {
                    Assert.Equal(e.OrderID, a.OrderID);
                    AssertCollection(e.ProductIds, a.ProductIds, ordered: true, elementAsserter: (ie, ia) => Assert.Equal(ie, ia));
                });

        AssertSql(
"""
@p='10'

SELECT `o1`.`OrderID`, `o0`.`ProductID`, `o0`.`OrderID`
FROM (
    SELECT `o`.`OrderID`
    FROM `Orders` AS `o`
    WHERE `o`.`OrderID` < 10300
    ORDER BY `o`.`OrderID`
    LIMIT @p
) AS `o1`
LEFT JOIN `Order Details` AS `o0` ON `o1`.`OrderID` = `o0`.`OrderID`
ORDER BY `o1`.`OrderID`, `o0`.`ProductID`
""");
        }

        public override Task Complex_nested_query_doesnt_try_binding_to_grandparent_when_parent_returns_complex_result(bool async)
        {
            if (AppConfig.ServerVersion.Supports.OuterApply)
            {
                // MySql.Data.MySqlClient.MySqlException: Reference 'CustomerID' not supported (forward reference in item list)
                return Assert.ThrowsAsync<MySqlException>(
                    () => base.Complex_nested_query_doesnt_try_binding_to_grandparent_when_parent_returns_complex_result(async));
            }
            else
            {
                // The LINQ expression 'OUTER APPLY ...' could not be translated. Either...
                return Assert.ThrowsAsync<InvalidOperationException>(
                    () => base.Complex_nested_query_doesnt_try_binding_to_grandparent_when_parent_returns_complex_result(async));
            }
        }

        public override async Task Client_code_using_instance_method_throws(bool async)
        {
            Assert.Equal(
                CoreStrings.ClientProjectionCapturingConstantInMethodInstance(
                    "Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.NorthwindMiscellaneousQueryMySqlTest",
                    "InstanceMethod"),
                (await Assert.ThrowsAsync<InvalidOperationException>(
                    () => base.Client_code_using_instance_method_throws(async))).Message);

            AssertSql();
        }

        public override async Task Client_code_using_instance_in_static_method(bool async)
        {
            Assert.Equal(
                CoreStrings.ClientProjectionCapturingConstantInMethodArgument(
                    "Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.NorthwindMiscellaneousQueryMySqlTest",
                    "StaticMethod"),
                (await Assert.ThrowsAsync<InvalidOperationException>(
                    () => base.Client_code_using_instance_in_static_method(async))).Message);

            AssertSql();
        }

        public override async Task Client_code_using_instance_in_anonymous_type(bool async)
        {
            Assert.Equal(
                CoreStrings.ClientProjectionCapturingConstantInTree(
                    "Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.NorthwindMiscellaneousQueryMySqlTest"),
                (await Assert.ThrowsAsync<InvalidOperationException>(
                    () => base.Client_code_using_instance_in_anonymous_type(async))).Message);

            AssertSql();
        }

        public override async Task Client_code_unknown_method(bool async)
        {
            await AssertTranslationFailedWithDetails(
                () => base.Client_code_unknown_method(async),
                CoreStrings.QueryUnableToTranslateMethod(
                    "Microsoft.EntityFrameworkCore.Query.NorthwindMiscellaneousQueryTestBase<Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query.NorthwindQueryMySqlFixture<Microsoft.EntityFrameworkCore.TestUtilities.NoopModelCustomizer>>",
                    nameof(UnknownMethod)));

            AssertSql();
        }

        public override async Task Entity_equality_through_subquery_composite_key(bool async)
        {
            var message = (await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Entity_equality_through_subquery_composite_key(async))).Message;

            Assert.Equal(
                CoreStrings.EntityEqualityOnCompositeKeyEntitySubqueryNotSupported("==", nameof(OrderDetail)),
                message);

            AssertSql();
        }

        public override async Task Max_on_empty_sequence_throws(bool async)
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() => base.Max_on_empty_sequence_throws(async));

            AssertSql(
                @"SELECT (
    SELECT MAX(`o`.`OrderID`)
    FROM `Orders` AS `o`
    WHERE `c`.`CustomerID` = `o`.`CustomerID`) AS `Max`
FROM `Customers` AS `c`");
        }

        public override async Task
            Select_DTO_constructor_distinct_with_collection_projection_translated_to_server_with_binding_after_client_eval(bool async)
        {
            using var context = CreateContext();
            var actualQuery = context.Set<Order>()
                .Where(o => o.OrderID < 10300)
                .Select(o => new { A = new OrderCountDTO(o.CustomerID), o.CustomerID })
                .Distinct()
                .Select(e => new { e.A, Orders = context.Set<Order>().Where(o => o.CustomerID == e.CustomerID)
                    .OrderBy(o => o.OrderID) // <-- added
                    .ToList() });

            var actual = async
                ? (await actualQuery.ToListAsync()).OrderBy(e => e.A.Id).ToList()
                : actualQuery.ToList().OrderBy(e => e.A.Id).ToList();

            var expected = Fixture.GetExpectedData().Set<Order>()
                .Where(o => o.OrderID < 10300)
                .Select(o => new { A = new OrderCountDTO(o.CustomerID), o.CustomerID })
                .Distinct()
                .Select(e => new { e.A, Orders = Fixture.GetExpectedData().Set<Order>().Where(o => o.CustomerID == e.CustomerID)
                    .OrderBy(o => o.OrderID) // <-- added
                    .ToList() })
                .ToList().OrderBy(e => e.A.Id).ToList();

            Assert.Equal(expected.Count, actual.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].A.Id, actual[i].A.Id);
                Assert.True(expected[i].Orders?.SequenceEqual(actual[i].Orders) ?? true);
            }

        AssertSql(
"""
SELECT `o0`.`CustomerID`, `o1`.`OrderID`, `o1`.`CustomerID`, `o1`.`EmployeeID`, `o1`.`OrderDate`
FROM (
    SELECT DISTINCT `o`.`CustomerID`
    FROM `Orders` AS `o`
    WHERE `o`.`OrderID` < 10300
) AS `o0`
LEFT JOIN `Orders` AS `o1` ON `o0`.`CustomerID` = `o1`.`CustomerID`
ORDER BY `o0`.`CustomerID`, `o1`.`OrderID`
""");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.WhereSubqueryReferencesOuterQuery))]
        public override async Task Subquery_with_navigation_inside_inline_collection(bool async)
        {
            await base.Subquery_with_navigation_inside_inline_collection(async);

            AssertSql("");
        }

        [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
        public override Task DefaultIfEmpty_Sum_over_collection_navigation(bool async)
        {
            return base.DefaultIfEmpty_Sum_over_collection_navigation(async);
        }

    public override async Task Projecting_collection_split(bool async)
    {
        await base.Projecting_collection_split(async);

        AssertSql();
    }

    public override async Task Projecting_collection_then_include_split(bool async)
    {
        await base.Projecting_collection_then_include_split(async);

        AssertSql();
    }

    public override async Task Multiple_context_instances(bool async)
    {
        await base.Multiple_context_instances(async);

        AssertSql();
    }

    public override async Task Multiple_context_instances_2(bool async)
    {
        await base.Multiple_context_instances_2(async);

        AssertSql();
    }

    public override async Task Multiple_context_instances_set(bool async)
    {
        await base.Multiple_context_instances_set(async);

        AssertSql();
    }

    public override async Task Multiple_context_instances_parameter(bool async)
    {
        await base.Multiple_context_instances_parameter(async);

        AssertSql();
    }

    public override async Task Query_when_evaluatable_queryable_method_call_with_repository(bool async)
    {
        await base.Query_when_evaluatable_queryable_method_call_with_repository(async);

        AssertSql();
    }

    public override async Task Lifting_when_subquery_nested_order_by_simple(bool async)
    {
        await base.Lifting_when_subquery_nested_order_by_simple(async);

        AssertSql();
    }

    public override async Task Lifting_when_subquery_nested_order_by_anonymous(bool async)
    {
        await base.Lifting_when_subquery_nested_order_by_anonymous(async);

        AssertSql();
    }

    public override async Task Local_dictionary(bool async)
    {
        await base.Local_dictionary(async);

        AssertSql();
    }

    public override async Task Shaper_command_caching_when_parameter_names_different(bool async)
    {
        await base.Shaper_command_caching_when_parameter_names_different(async);

        AssertSql();
    }

    public override async Task Can_convert_manually_build_expression_with_default(bool async)
    {
        await base.Can_convert_manually_build_expression_with_default(async);

        AssertSql();
    }

    public override async Task Entity_equality_self(bool async)
    {
        await base.Entity_equality_self(async);

        AssertSql();
    }

    public override async Task Entity_equality_local(bool async)
    {
        await base.Entity_equality_local(async);

        AssertSql();
    }

    public override async Task Entity_equality_local_composite_key(bool async)
    {
        await base.Entity_equality_local_composite_key(async);

        AssertSql();
    }

    public override async Task Entity_equality_local_double_check(bool async)
    {
        await base.Entity_equality_local_double_check(async);

        AssertSql();
    }

    public override async Task Join_with_entity_equality_local_on_both_sources(bool async)
    {
        await base.Join_with_entity_equality_local_on_both_sources(async);

        AssertSql();
    }

    public override async Task Entity_equality_local_inline(bool async)
    {
        await base.Entity_equality_local_inline(async);

        AssertSql();
    }

    public override async Task Entity_equality_local_inline_composite_key(bool async)
    {
        await base.Entity_equality_local_inline_composite_key(async);

        AssertSql();
    }

    public override async Task Entity_equality_null(bool async)
    {
        await base.Entity_equality_null(async);

        AssertSql();
    }

    public override async Task Entity_equality_null_composite_key(bool async)
    {
        await base.Entity_equality_null_composite_key(async);

        AssertSql();
    }

    public override async Task Entity_equality_not_null(bool async)
    {
        await base.Entity_equality_not_null(async);

        AssertSql();
    }

    public override async Task Entity_equality_not_null_composite_key(bool async)
    {
        await base.Entity_equality_not_null_composite_key(async);

        AssertSql();
    }

    public override async Task Entity_equality_through_nested_anonymous_type_projection(bool async)
    {
        await base.Entity_equality_through_nested_anonymous_type_projection(async);

        AssertSql();
    }

    public override async Task Entity_equality_through_DTO_projection(bool async)
    {
        await base.Entity_equality_through_DTO_projection(async);

        AssertSql();
    }

    public override async Task Entity_equality_through_subquery(bool async)
    {
        await base.Entity_equality_through_subquery(async);

        AssertSql();
    }

    public override async Task Entity_equality_through_include(bool async)
    {
        await base.Entity_equality_through_include(async);

        AssertSql();
    }

    public override async Task Entity_equality_orderby(bool async)
    {
        await base.Entity_equality_orderby(async);

        AssertSql();
    }

    public override async Task Entity_equality_orderby_descending_composite_key(bool async)
    {
        await base.Entity_equality_orderby_descending_composite_key(async);

        AssertSql();
    }

    public override async Task Entity_equality_orderby_descending_subquery_composite_key(bool async)
    {
        await base.Entity_equality_orderby_descending_subquery_composite_key(async);

        AssertSql();
    }

    public override async Task Queryable_simple(bool async)
    {
        await base.Queryable_simple(async);

        AssertSql();
    }

    public override async Task Queryable_simple_anonymous(bool async)
    {
        await base.Queryable_simple_anonymous(async);

        AssertSql();
    }

    public override async Task Queryable_simple_anonymous_projection_subquery(bool async)
    {
        await base.Queryable_simple_anonymous_projection_subquery(async);

        AssertSql();
    }

    public override async Task Queryable_simple_anonymous_subquery(bool async)
    {
        await base.Queryable_simple_anonymous_subquery(async);

        AssertSql();
    }

    public override async Task Queryable_reprojection(bool async)
    {
        await base.Queryable_reprojection(async);

        AssertSql();
    }

    public override async Task Queryable_nested_simple(bool async)
    {
        await base.Queryable_nested_simple(async);

        AssertSql();
    }

    public override async Task Take_simple(bool async)
    {
        await base.Take_simple(async);

        AssertSql();
    }

    public override async Task Take_simple_parameterized(bool async)
    {
        await base.Take_simple_parameterized(async);

        AssertSql();
    }

    public override async Task Take_simple_projection(bool async)
    {
        await base.Take_simple_projection(async);

        AssertSql();
    }

    public override async Task Take_subquery_projection(bool async)
    {
        await base.Take_subquery_projection(async);

        AssertSql();
    }

    public override async Task Skip(bool async)
    {
        await base.Skip(async);

        AssertSql();
    }

    public override async Task Skip_no_orderby(bool async)
    {
        await base.Skip_no_orderby(async);

        AssertSql();
    }

    public override async Task Skip_orderby_const(bool async)
    {
        await base.Skip_orderby_const(async);

        AssertSql();
    }

    public override async Task Distinct_Skip(bool async)
    {
        await base.Distinct_Skip(async);

        AssertSql();
    }

    public override async Task Skip_Take(bool async)
    {
        await base.Skip_Take(async);

        AssertSql();
    }

    public override async Task Join_Customers_Orders_Skip_Take(bool async)
    {
        await base.Join_Customers_Orders_Skip_Take(async);

        AssertSql();
    }

    public override async Task Join_Customers_Orders_Skip_Take_followed_by_constant_projection(bool async)
    {
        await base.Join_Customers_Orders_Skip_Take_followed_by_constant_projection(async);

        AssertSql();
    }

    public override async Task Join_Customers_Orders_Projection_With_String_Concat_Skip_Take(bool async)
    {
        await base.Join_Customers_Orders_Projection_With_String_Concat_Skip_Take(async);

        AssertSql();
    }

    public override async Task Join_Customers_Orders_Orders_Skip_Take_Same_Properties(bool async)
    {
        await base.Join_Customers_Orders_Orders_Skip_Take_Same_Properties(async);

        AssertSql();
    }

    public override async Task Ternary_should_not_evaluate_both_sides(bool async)
    {
        await base.Ternary_should_not_evaluate_both_sides(async);

        AssertSql();
    }

    public override async Task Ternary_should_not_evaluate_both_sides_with_parameter(bool async)
    {
        await base.Ternary_should_not_evaluate_both_sides_with_parameter(async);

        AssertSql();
    }

    public override async Task Coalesce_Correct_Multiple_Same_TypeMapping(bool async)
    {
        await base.Coalesce_Correct_Multiple_Same_TypeMapping(async);

        AssertSql();
    }

    public override async Task Coalesce_Correct_TypeMapping_Double(bool async)
    {
        await base.Coalesce_Correct_TypeMapping_Double(async);

        AssertSql();
    }

    public override async Task Coalesce_Correct_TypeMapping_String(bool async)
    {
        await base.Coalesce_Correct_TypeMapping_String(async);

        AssertSql();
    }

    public override async Task Null_Coalesce_Short_Circuit(bool async)
    {
        await base.Null_Coalesce_Short_Circuit(async);

        AssertSql();
    }

    public override async Task Null_Coalesce_Short_Circuit_with_server_correlated_leftover(bool async)
    {
        await base.Null_Coalesce_Short_Circuit_with_server_correlated_leftover(async);

        AssertSql();
    }

    public override async Task Distinct_Skip_Take(bool async)
    {
        await base.Distinct_Skip_Take(async);

        AssertSql();
    }

    public override async Task Skip_Distinct(bool async)
    {
        await base.Skip_Distinct(async);

        AssertSql();
    }

    public override async Task Skip_Take_Distinct(bool async)
    {
        await base.Skip_Take_Distinct(async);

        AssertSql();
    }

    public override async Task Skip_Take_Any(bool async)
    {
        await base.Skip_Take_Any(async);

        AssertSql();
    }

    public override async Task Skip_Take_All(bool async)
    {
        await base.Skip_Take_All(async);

        AssertSql();
    }

    public override async Task Take_All(bool async)
    {
        await base.Take_All(async);

        AssertSql();
    }

    public override async Task Skip_Take_Any_with_predicate(bool async)
    {
        await base.Skip_Take_Any_with_predicate(async);

        AssertSql();
    }

    public override async Task Take_Any_with_predicate(bool async)
    {
        await base.Take_Any_with_predicate(async);

        AssertSql();
    }

    public override async Task Take_Skip_Distinct(bool async)
    {
        await base.Take_Skip_Distinct(async);

        AssertSql();
    }

    public override async Task Take_Skip_Distinct_Caching(bool async)
    {
        await base.Take_Skip_Distinct_Caching(async);

        AssertSql();
    }

    public override async Task Take_Distinct(bool async)
    {
        await base.Take_Distinct(async);

        AssertSql();
    }

    public override async Task Distinct_Take(bool async)
    {
        await base.Distinct_Take(async);

        AssertSql();
    }

    public override async Task Distinct_Take_Count(bool async)
    {
        await base.Distinct_Take_Count(async);

        AssertSql();
    }

    public override async Task Take_Distinct_Count(bool async)
    {
        await base.Take_Distinct_Count(async);

        AssertSql();
    }

    public override async Task Take_Where_Distinct_Count(bool async)
    {
        await base.Take_Where_Distinct_Count(async);

        AssertSql();
    }

    public override async Task Any_simple(bool async)
    {
        await base.Any_simple(async);

        AssertSql();
    }

    public override async Task OrderBy_Take_Count(bool async)
    {
        await base.OrderBy_Take_Count(async);

        AssertSql();
    }

    public override async Task Take_OrderBy_Count(bool async)
    {
        await base.Take_OrderBy_Count(async);

        AssertSql();
    }

    public override async Task Any_predicate(bool async)
    {
        await base.Any_predicate(async);

        AssertSql();
    }

    public override async Task Any_nested_negated(bool async)
    {
        await base.Any_nested_negated(async);

        AssertSql();
    }

    public override async Task Any_nested_negated2(bool async)
    {
        await base.Any_nested_negated2(async);

        AssertSql();
    }

    public override async Task Any_nested_negated3(bool async)
    {
        await base.Any_nested_negated3(async);

        AssertSql();
    }

    public override async Task Any_nested(bool async)
    {
        await base.Any_nested(async);

        AssertSql();
    }

    public override async Task Any_nested2(bool async)
    {
        await base.Any_nested2(async);

        AssertSql();
    }

    public override async Task Any_nested3(bool async)
    {
        await base.Any_nested3(async);

        AssertSql();
    }

    public override async Task Any_with_multiple_conditions_still_uses_exists(bool async)
    {
        await base.Any_with_multiple_conditions_still_uses_exists(async);

        AssertSql();
    }

    public override async Task Any_on_distinct(bool async)
    {
        await base.Any_on_distinct(async);

        AssertSql();
    }

    public override async Task Contains_on_distinct(bool async)
    {
        await base.Contains_on_distinct(async);

        AssertSql();
    }

    public override async Task All_on_distinct(bool async)
    {
        await base.All_on_distinct(async);

        AssertSql();
    }

    public override async Task All_top_level(bool async)
    {
        await base.All_top_level(async);

        AssertSql();
    }

    public override async Task All_top_level_column(bool async)
    {
        await base.All_top_level_column(async);

        AssertSql();
    }

    public override async Task All_top_level_subquery(bool async)
    {
        await base.All_top_level_subquery(async);

        AssertSql();
    }

    public override async Task All_top_level_subquery_ef_property(bool async)
    {
        await base.All_top_level_subquery_ef_property(async);

        AssertSql();
    }

    public override async Task All_client(bool async)
    {
        await base.All_client(async);

        AssertSql();
    }

    public override async Task All_client_and_server_top_level(bool async)
    {
        await base.All_client_and_server_top_level(async);

        AssertSql();
    }

    public override async Task All_client_or_server_top_level(bool async)
    {
        await base.All_client_or_server_top_level(async);

        AssertSql();
    }

    public override async Task Take_with_single(bool async)
    {
        await base.Take_with_single(async);

        AssertSql();
    }

    public override async Task Take_with_single_select_many(bool async)
    {
        await base.Take_with_single_select_many(async);

        AssertSql();
    }

    public override async Task Cast_results_to_object(bool async)
    {
        await base.Cast_results_to_object(async);

        AssertSql();
    }

    public override async Task First_client_predicate(bool async)
    {
        await base.First_client_predicate(async);

        AssertSql();
    }

    public override async Task Where_select_many_or(bool async)
    {
        await base.Where_select_many_or(async);

        AssertSql();
    }

    public override async Task Where_select_many_or2(bool async)
    {
        await base.Where_select_many_or2(async);

        AssertSql();
    }

    public override async Task Where_select_many_or3(bool async)
    {
        await base.Where_select_many_or3(async);

        AssertSql();
    }

    public override async Task Where_select_many_or4(bool async)
    {
        await base.Where_select_many_or4(async);

        AssertSql();
    }

    public override async Task Where_select_many_or_with_parameter(bool async)
    {
        await base.Where_select_many_or_with_parameter(async);

        AssertSql();
    }

    public override async Task Where_subquery_anon(bool async)
    {
        await base.Where_subquery_anon(async);

        AssertSql();
    }

    public override async Task Where_subquery_anon_nested(bool async)
    {
        await base.Where_subquery_anon_nested(async);

        AssertSql();
    }

    public override async Task Where_subquery_expression(bool async)
    {
        await base.Where_subquery_expression(async);

        AssertSql();
    }

    public override async Task Where_subquery_expression_same_parametername(bool async)
    {
        await base.Where_subquery_expression_same_parametername(async);

        AssertSql();
    }

    public override async Task Select_DTO_distinct_translated_to_server(bool async)
    {
        await base.Select_DTO_distinct_translated_to_server(async);

        AssertSql();
    }

    public override async Task Select_DTO_constructor_distinct_translated_to_server(bool async)
    {
        await base.Select_DTO_constructor_distinct_translated_to_server(async);

        AssertSql();
    }

    public override async Task Select_DTO_constructor_distinct_with_navigation_translated_to_server(bool async)
    {
        await base.Select_DTO_constructor_distinct_with_navigation_translated_to_server(async);

        AssertSql();
    }

    public override async Task Select_DTO_constructor_distinct_with_collection_projection_translated_to_server(bool async)
    {
        await base.Select_DTO_constructor_distinct_with_collection_projection_translated_to_server(async);

        AssertSql();
    }

    public override async Task Select_DTO_with_member_init_distinct_translated_to_server(bool async)
    {
        await base.Select_DTO_with_member_init_distinct_translated_to_server(async);

        AssertSql();
    }

    public override async Task Select_nested_collection_count_using_DTO(bool async)
    {
        await base.Select_nested_collection_count_using_DTO(async);

        AssertSql();
    }

    public override async Task Select_DTO_with_member_init_distinct_in_subquery_translated_to_server(bool async)
    {
        await base.Select_DTO_with_member_init_distinct_in_subquery_translated_to_server(async);

        AssertSql();
    }

    public override async Task Select_DTO_with_member_init_distinct_in_subquery_translated_to_server_2(bool async)
    {
        await base.Select_DTO_with_member_init_distinct_in_subquery_translated_to_server_2(async);

        AssertSql();
    }

    public override async Task Select_DTO_with_member_init_distinct_in_subquery_used_in_projection_translated_to_server(bool async)
    {
        await base.Select_DTO_with_member_init_distinct_in_subquery_used_in_projection_translated_to_server(async);

        AssertSql();
    }

    public override async Task Select_correlated_subquery_filtered_returning_queryable_throws(bool async)
    {
        await base.Select_correlated_subquery_filtered_returning_queryable_throws(async);

        AssertSql();
    }

    public override async Task Select_correlated_subquery_ordered_returning_queryable_throws(bool async)
    {
        await base.Select_correlated_subquery_ordered_returning_queryable_throws(async);

        AssertSql();
    }

    public override async Task Select_correlated_subquery_ordered_returning_queryable_in_DTO_throws(bool async)
    {
        await base.Select_correlated_subquery_ordered_returning_queryable_in_DTO_throws(async);

        AssertSql();
    }

    public override async Task Select_correlated_subquery_filtered(bool async)
    {
        await base.Select_correlated_subquery_filtered(async);

        AssertSql();
    }

    public override async Task Select_correlated_subquery_ordered(bool async)
    {
        await base.Select_correlated_subquery_ordered(async);

        AssertSql();
    }

    public override async Task Select_nested_collection_in_anonymous_type_returning_ordered_queryable(bool async)
    {
        await base.Select_nested_collection_in_anonymous_type_returning_ordered_queryable(async);

        AssertSql();
    }

    public override async Task Select_subquery_recursive_trivial_returning_queryable(bool async)
    {
        await base.Select_subquery_recursive_trivial_returning_queryable(async);

        AssertSql();
    }

    public override async Task Select_nested_collection_in_anonymous_type(bool async)
    {
        await base.Select_nested_collection_in_anonymous_type(async);

        AssertSql();
    }

    public override async Task Select_subquery_recursive_trivial(bool async)
    {
        await base.Select_subquery_recursive_trivial(async);

        AssertSql();
    }

    public override async Task Where_subquery_on_bool(bool async)
    {
        await base.Where_subquery_on_bool(async);

        AssertSql();
    }

    public override async Task Where_subquery_on_collection(bool async)
    {
        await base.Where_subquery_on_collection(async);

        AssertSql();
    }

    public override async Task Where_query_composition(bool async)
    {
        await base.Where_query_composition(async);

        AssertSql();
    }

    public override async Task Where_query_composition_is_null(bool async)
    {
        await base.Where_query_composition_is_null(async);

        AssertSql();
    }

    public override async Task Where_query_composition_is_not_null(bool async)
    {
        await base.Where_query_composition_is_not_null(async);

        AssertSql();
    }

    public override async Task Where_query_composition_entity_equality_one_element_SingleOrDefault(bool async)
    {
        await base.Where_query_composition_entity_equality_one_element_SingleOrDefault(async);

        AssertSql();
    }

    public override async Task Where_query_composition_entity_equality_one_element_Single(bool async)
    {
        await base.Where_query_composition_entity_equality_one_element_Single(async);

        AssertSql();
    }

    public override async Task Where_query_composition_entity_equality_one_element_FirstOrDefault(bool async)
    {
        await base.Where_query_composition_entity_equality_one_element_FirstOrDefault(async);

        AssertSql();
    }

    public override async Task Where_query_composition_entity_equality_one_element_First(bool async)
    {
        await base.Where_query_composition_entity_equality_one_element_First(async);

        AssertSql();
    }

    public override async Task Where_query_composition_entity_equality_no_elements_SingleOrDefault(bool async)
    {
        await base.Where_query_composition_entity_equality_no_elements_SingleOrDefault(async);

        AssertSql();
    }

    public override async Task Where_query_composition_entity_equality_no_elements_Single(bool async)
    {
        await base.Where_query_composition_entity_equality_no_elements_Single(async);

        AssertSql();
    }

    public override async Task Where_query_composition_entity_equality_no_elements_FirstOrDefault(bool async)
    {
        await base.Where_query_composition_entity_equality_no_elements_FirstOrDefault(async);

        AssertSql();
    }

    public override async Task Where_query_composition_entity_equality_no_elements_First(bool async)
    {
        await base.Where_query_composition_entity_equality_no_elements_First(async);

        AssertSql();
    }

    public override async Task Where_query_composition_entity_equality_multiple_elements_SingleOrDefault(bool async)
    {
        await base.Where_query_composition_entity_equality_multiple_elements_SingleOrDefault(async);

        AssertSql();
    }

    public override async Task Where_query_composition_entity_equality_multiple_elements_Single(bool async)
    {
        await base.Where_query_composition_entity_equality_multiple_elements_Single(async);

        AssertSql();
    }

    public override async Task Where_query_composition_entity_equality_multiple_elements_FirstOrDefault(bool async)
    {
        await base.Where_query_composition_entity_equality_multiple_elements_FirstOrDefault(async);

        AssertSql();
    }

    public override async Task Where_query_composition_entity_equality_multiple_elements_First(bool async)
    {
        await base.Where_query_composition_entity_equality_multiple_elements_First(async);

        AssertSql();
    }

    public override async Task Where_query_composition2(bool async)
    {
        await base.Where_query_composition2(async);

        AssertSql();
    }

    public override async Task Where_query_composition2_FirstOrDefault(bool async)
    {
        await base.Where_query_composition2_FirstOrDefault(async);

        AssertSql();
    }

    public override async Task Where_query_composition2_FirstOrDefault_with_anonymous(bool async)
    {
        await base.Where_query_composition2_FirstOrDefault_with_anonymous(async);

        AssertSql();
    }

    public override async Task Where_query_composition3(bool async)
    {
        await base.Where_query_composition3(async);

        AssertSql();
    }

    public override async Task Where_query_composition4(bool async)
    {
        await base.Where_query_composition4(async);

        AssertSql();
    }

    public override async Task Where_query_composition5(bool async)
    {
        await base.Where_query_composition5(async);

        AssertSql();
    }

    public override async Task Where_query_composition6(bool async)
    {
        await base.Where_query_composition6(async);

        AssertSql();
    }

    public override async Task Where_subquery_recursive_trivial(bool async)
    {
        await base.Where_subquery_recursive_trivial(async);

        AssertSql();
    }

    public override async Task OrderBy_scalar_primitive(bool async)
    {
        await base.OrderBy_scalar_primitive(async);

        AssertSql();
    }

    public override async Task SelectMany_mixed(bool async)
    {
        await base.SelectMany_mixed(async);

        AssertSql();
    }

    public override async Task SelectMany_simple1(bool async)
    {
        await base.SelectMany_simple1(async);

        AssertSql();
    }

    public override async Task SelectMany_simple_subquery(bool async)
    {
        await base.SelectMany_simple_subquery(async);

        AssertSql();
    }

    public override async Task SelectMany_simple2(bool async)
    {
        await base.SelectMany_simple2(async);

        AssertSql();
    }

    public override async Task SelectMany_entity_deep(bool async)
    {
        await base.SelectMany_entity_deep(async);

        AssertSql();
    }

    public override async Task SelectMany_projection1(bool async)
    {
        await base.SelectMany_projection1(async);

        AssertSql();
    }

    public override async Task SelectMany_projection2(bool async)
    {
        await base.SelectMany_projection2(async);

        AssertSql();
    }

    public override async Task SelectMany_nested_simple(bool async)
    {
        await base.SelectMany_nested_simple(async);

        AssertSql();
    }

    public override async Task SelectMany_correlated_simple(bool async)
    {
        await base.SelectMany_correlated_simple(async);

        AssertSql();
    }

    public override async Task SelectMany_correlated_subquery_simple(bool async)
    {
        await base.SelectMany_correlated_subquery_simple(async);

        AssertSql();
    }

    public override async Task SelectMany_correlated_with_DefaultIfEmpty_and_Select_value_type_in_selector_throws(bool async)
    {
        await base.SelectMany_correlated_with_DefaultIfEmpty_and_Select_value_type_in_selector_throws(async);

        AssertSql();
    }

    public override async Task SelectMany_correlated_with_Select_value_type_and_DefaultIfEmpty_in_selector(bool async)
    {
        await base.SelectMany_correlated_with_Select_value_type_and_DefaultIfEmpty_in_selector(async);

        AssertSql();
    }

    public override async Task SelectMany_correlated_subquery_hard(bool async)
    {
        await base.SelectMany_correlated_subquery_hard(async);

        AssertSql();
    }

    public override async Task SelectMany_cartesian_product_with_ordering(bool async)
    {
        await base.SelectMany_cartesian_product_with_ordering(async);

        AssertSql();
    }

    public override async Task SelectMany_primitive(bool async)
    {
        await base.SelectMany_primitive(async);

        AssertSql();
    }

    public override async Task SelectMany_primitive_select_subquery(bool async)
    {
        await base.SelectMany_primitive_select_subquery(async);

        AssertSql();
    }

    public override async Task Join_Where_Count(bool async)
    {
        await base.Join_Where_Count(async);

        AssertSql();
    }

    public override async Task Where_Join_Any(bool async)
    {
        await base.Where_Join_Any(async);

        AssertSql();
    }

    public override async Task Where_Join_Exists(bool async)
    {
        await base.Where_Join_Exists(async);

        AssertSql();
    }

    public override async Task Where_Join_Exists_Inequality(bool async)
    {
        await base.Where_Join_Exists_Inequality(async);

        AssertSql();
    }

    public override async Task Where_Join_Exists_Constant(bool async)
    {
        await base.Where_Join_Exists_Constant(async);

        AssertSql();
    }

    public override async Task Where_Join_Not_Exists(bool async)
    {
        await base.Where_Join_Not_Exists(async);

        AssertSql();
    }

    public override async Task Multiple_joins_Where_Order_Any(bool async)
    {
        await base.Multiple_joins_Where_Order_Any(async);

        AssertSql();
    }

    public override async Task Join_OrderBy_Count(bool async)
    {
        await base.Join_OrderBy_Count(async);

        AssertSql();
    }

    public override async Task Where_join_select(bool async)
    {
        await base.Where_join_select(async);

        AssertSql();
    }

    public override async Task Where_orderby_join_select(bool async)
    {
        await base.Where_orderby_join_select(async);

        AssertSql();
    }

    public override async Task Where_join_orderby_join_select(bool async)
    {
        await base.Where_join_orderby_join_select(async);

        AssertSql();
    }

    public override async Task Where_select_many(bool async)
    {
        await base.Where_select_many(async);

        AssertSql();
    }

    public override async Task Where_orderby_select_many(bool async)
    {
        await base.Where_orderby_select_many(async);

        AssertSql();
    }

    public override async Task DefaultIfEmpty_top_level(bool async)
    {
        await base.DefaultIfEmpty_top_level(async);

        AssertSql();
    }

    public override async Task Join_with_DefaultIfEmpty_on_both_sources(bool async)
    {
        await base.Join_with_DefaultIfEmpty_on_both_sources(async);

        AssertSql();
    }

    public override async Task DefaultIfEmpty_top_level_followed_by_constant_Select(bool async)
    {
        await base.DefaultIfEmpty_top_level_followed_by_constant_Select(async);

        AssertSql();
    }

    public override async Task DefaultIfEmpty_top_level_preceded_by_constant_Select(bool async)
    {
        await base.DefaultIfEmpty_top_level_preceded_by_constant_Select(async);

        AssertSql();
    }

    public override async Task DefaultIfEmpty_top_level_arg(bool async)
    {
        await base.DefaultIfEmpty_top_level_arg(async);

        AssertSql();
    }

    public override async Task DefaultIfEmpty_top_level_arg_followed_by_projecting_constant(bool async)
    {
        await base.DefaultIfEmpty_top_level_arg_followed_by_projecting_constant(async);

        AssertSql();
    }

    public override async Task DefaultIfEmpty_top_level_positive(bool async)
    {
        await base.DefaultIfEmpty_top_level_positive(async);

        AssertSql();
    }

    public override async Task DefaultIfEmpty_top_level_projection(bool async)
    {
        await base.DefaultIfEmpty_top_level_projection(async);

        AssertSql();
    }

    public override async Task SelectMany_customer_orders(bool async)
    {
        await base.SelectMany_customer_orders(async);

        AssertSql();
    }

    public override async Task SelectMany_Count(bool async)
    {
        await base.SelectMany_Count(async);

        AssertSql();
    }

    public override async Task SelectMany_LongCount(bool async)
    {
        await base.SelectMany_LongCount(async);

        AssertSql();
    }

    public override async Task SelectMany_OrderBy_ThenBy_Any(bool async)
    {
        await base.SelectMany_OrderBy_ThenBy_Any(async);

        AssertSql();
    }

    public override async Task OrderBy(bool async)
    {
        await base.OrderBy(async);

        AssertSql();
    }

    public override async Task OrderBy_true(bool async)
    {
        await base.OrderBy_true(async);

        AssertSql();
    }

    public override async Task OrderBy_integer(bool async)
    {
        await base.OrderBy_integer(async);

        AssertSql();
    }

    public override async Task OrderBy_parameter(bool async)
    {
        await base.OrderBy_parameter(async);

        AssertSql();
    }

    public override async Task OrderBy_anon(bool async)
    {
        await base.OrderBy_anon(async);

        AssertSql();
    }

    public override async Task OrderBy_anon2(bool async)
    {
        await base.OrderBy_anon2(async);

        AssertSql();
    }

    public override async Task OrderBy_client_mixed(bool async)
    {
        await base.OrderBy_client_mixed(async);

        AssertSql();
    }

    public override async Task OrderBy_multiple_queries(bool async)
    {
        await base.OrderBy_multiple_queries(async);

        AssertSql();
    }

    public override async Task OrderBy_shadow(bool async)
    {
        await base.OrderBy_shadow(async);

        AssertSql();
    }

    public override async Task OrderBy_ThenBy_predicate(bool async)
    {
        await base.OrderBy_ThenBy_predicate(async);

        AssertSql();
    }

    public override async Task OrderBy_correlated_subquery1(bool async)
    {
        await base.OrderBy_correlated_subquery1(async);

        AssertSql();
    }

    public override async Task OrderBy_correlated_subquery2(bool async)
    {
        await base.OrderBy_correlated_subquery2(async);

        AssertSql();
    }

    public override async Task OrderBy_Select(bool async)
    {
        await base.OrderBy_Select(async);

        AssertSql();
    }

    public override async Task OrderBy_multiple(bool async)
    {
        await base.OrderBy_multiple(async);

        AssertSql();
    }

    public override async Task OrderBy_ThenBy(bool async)
    {
        await base.OrderBy_ThenBy(async);

        AssertSql();
    }

    public override async Task OrderByDescending(bool async)
    {
        await base.OrderByDescending(async);

        AssertSql();
    }

    public override async Task OrderByDescending_ThenBy(bool async)
    {
        await base.OrderByDescending_ThenBy(async);

        AssertSql();
    }

    public override async Task OrderByDescending_ThenByDescending(bool async)
    {
        await base.OrderByDescending_ThenByDescending(async);

        AssertSql();
    }

    public override async Task Select_Order(bool async)
    {
        await base.Select_Order(async);

        AssertSql();
    }

    public override async Task Select_OrderDescending(bool async)
    {
        await base.Select_OrderDescending(async);

        AssertSql();
    }

    public override async Task Where_Order_First(bool async)
    {
        await base.Where_Order_First(async);

        AssertSql();
    }

    public override async Task OrderBy_ThenBy_Any(bool async)
    {
        await base.OrderBy_ThenBy_Any(async);

        AssertSql();
    }

    public override async Task OrderBy_Join(bool async)
    {
        await base.OrderBy_Join(async);

        AssertSql();
    }

    public override async Task OrderBy_SelectMany(bool async)
    {
        await base.OrderBy_SelectMany(async);

        AssertSql();
    }

    public override async Task Let_any_subquery_anonymous(bool async)
    {
        await base.Let_any_subquery_anonymous(async);

        AssertSql();
    }

    public override async Task OrderBy_arithmetic(bool async)
    {
        await base.OrderBy_arithmetic(async);

        AssertSql();
    }

    public override async Task OrderBy_condition_comparison(bool async)
    {
        await base.OrderBy_condition_comparison(async);

        AssertSql();
    }

    public override async Task OrderBy_ternary_conditions(bool async)
    {
        await base.OrderBy_ternary_conditions(async);

        AssertSql();
    }

    public override async Task OrderBy_any(bool async)
    {
        await base.OrderBy_any(async);

        AssertSql();
    }

    public override async Task SelectMany_Joined(bool async)
    {
        await base.SelectMany_Joined(async);

        AssertSql();
    }

    public override async Task SelectMany_Joined_DefaultIfEmpty(bool async)
    {
        await base.SelectMany_Joined_DefaultIfEmpty(async);

        AssertSql();
    }

    public override async Task SelectMany_Joined_Take(bool async)
    {
        await base.SelectMany_Joined_Take(async);

        AssertSql();
    }

    public override async Task SelectMany_Joined_DefaultIfEmpty2(bool async)
    {
        await base.SelectMany_Joined_DefaultIfEmpty2(async);

        AssertSql();
    }

    public override async Task SelectMany_Joined_DefaultIfEmpty3(bool async)
    {
        await base.SelectMany_Joined_DefaultIfEmpty3(async);

        AssertSql();
    }

    public override async Task Select_many_cross_join_same_collection(bool async)
    {
        await base.Select_many_cross_join_same_collection(async);

        AssertSql();
    }

    public override async Task OrderBy_null_coalesce_operator(bool async)
    {
        await base.OrderBy_null_coalesce_operator(async);

        AssertSql();
    }

    public override async Task Select_null_coalesce_operator(bool async)
    {
        await base.Select_null_coalesce_operator(async);

        AssertSql();
    }

    public override async Task OrderBy_conditional_operator(bool async)
    {
        await base.OrderBy_conditional_operator(async);

        AssertSql();
    }

    public override async Task OrderBy_conditional_operator_where_condition_false(bool async)
    {
        await base.OrderBy_conditional_operator_where_condition_false(async);

        AssertSql();
    }

    public override async Task OrderBy_comparison_operator(bool async)
    {
        await base.OrderBy_comparison_operator(async);

        AssertSql();
    }

    public override async Task Projection_null_coalesce_operator(bool async)
    {
        await base.Projection_null_coalesce_operator(async);

        AssertSql();
    }

    public override async Task Filter_coalesce_operator(bool async)
    {
        await base.Filter_coalesce_operator(async);

        AssertSql();
    }

    public override async Task Take_skip_null_coalesce_operator(bool async)
    {
        await base.Take_skip_null_coalesce_operator(async);

        AssertSql();
    }

    public override async Task Select_take_null_coalesce_operator(bool async)
    {
        await base.Select_take_null_coalesce_operator(async);

        AssertSql();
    }

    public override async Task Select_take_skip_null_coalesce_operator(bool async)
    {
        await base.Select_take_skip_null_coalesce_operator(async);

        AssertSql();
    }

    public override async Task Select_take_skip_null_coalesce_operator2(bool async)
    {
        await base.Select_take_skip_null_coalesce_operator2(async);

        AssertSql();
    }

    public override async Task Select_take_skip_null_coalesce_operator3(bool async)
    {
        await base.Select_take_skip_null_coalesce_operator3(async);

        AssertSql();
    }

    public override async Task Select_Property_when_non_shadow(bool async)
    {
        await base.Select_Property_when_non_shadow(async);

        AssertSql();
    }

    public override async Task Where_Property_when_non_shadow(bool async)
    {
        await base.Where_Property_when_non_shadow(async);

        AssertSql();
    }

    public override async Task Select_Property_when_shadow(bool async)
    {
        await base.Select_Property_when_shadow(async);

        AssertSql();
    }

    public override async Task Where_Property_when_shadow(bool async)
    {
        await base.Where_Property_when_shadow(async);

        AssertSql();
    }

    public override async Task Select_Property_when_shadow_unconstrained_generic_method(bool async)
    {
        await base.Select_Property_when_shadow_unconstrained_generic_method(async);

        AssertSql();
    }

    public override async Task Where_Property_when_shadow_unconstrained_generic_method(bool async)
    {
        await base.Where_Property_when_shadow_unconstrained_generic_method(async);

        AssertSql();
    }

    public override async Task Where_Property_shadow_closure(bool async)
    {
        await base.Where_Property_shadow_closure(async);

        AssertSql();
    }

    public override async Task Selected_column_can_coalesce(bool async)
    {
        await base.Selected_column_can_coalesce(async);

        AssertSql();
    }

    public override async Task IQueryable_captured_variable()
    {
        await base.IQueryable_captured_variable();

        AssertSql();
    }

    public override async Task Select_Subquery_Single(bool async)
    {
        await base.Select_Subquery_Single(async);

        AssertSql();
    }

    public override async Task Select_Where_Subquery_Deep_Single(bool async)
    {
        await base.Select_Where_Subquery_Deep_Single(async);

        AssertSql();
    }

    public override async Task Select_Where_Subquery_Deep_First(bool async)
    {
        await base.Select_Where_Subquery_Deep_First(async);

        AssertSql();
    }

    public override async Task Select_Where_Subquery_Equality(bool async)
    {
        await base.Select_Where_Subquery_Equality(async);

        AssertSql();
    }

    public override async Task Throws_on_concurrent_query_list(bool async)
    {
        await base.Throws_on_concurrent_query_list(async);

        AssertSql();
    }

    public override async Task Throws_on_concurrent_query_first(bool async)
    {
        await base.Throws_on_concurrent_query_first(async);

        AssertSql();
    }

    public override async Task Environment_newline_is_funcletized(bool async)
    {
        await base.Environment_newline_is_funcletized(async);

        AssertSql();
    }

    public override async Task Concat_string_int(bool async)
    {
        await base.Concat_string_int(async);

        AssertSql();
    }

    public override async Task Concat_int_string(bool async)
    {
        await base.Concat_int_string(async);

        AssertSql();
    }

    public override async Task Concat_parameter_string_int(bool async)
    {
        await base.Concat_parameter_string_int(async);

        AssertSql();
    }

    public override async Task Concat_constant_string_int(bool async)
    {
        await base.Concat_constant_string_int(async);

        AssertSql();
    }

    public override async Task String_concat_with_navigation1(bool async)
    {
        await base.String_concat_with_navigation1(async);

        AssertSql();
    }

    public override async Task String_concat_with_navigation2(bool async)
    {
        await base.String_concat_with_navigation2(async);

        AssertSql();
    }

    public override async Task Handle_materialization_properly_when_more_than_two_query_sources_are_involved(bool async)
    {
        await base.Handle_materialization_properly_when_more_than_two_query_sources_are_involved(async);

        AssertSql();
    }

    public override async Task Parameter_extraction_short_circuits_1(bool async)
    {
        await base.Parameter_extraction_short_circuits_1(async);

        AssertSql();
    }

    public override async Task Parameter_extraction_short_circuits_2(bool async)
    {
        await base.Parameter_extraction_short_circuits_2(async);

        AssertSql();
    }

    public override async Task Parameter_extraction_short_circuits_3(bool async)
    {
        await base.Parameter_extraction_short_circuits_3(async);

        AssertSql();
    }

    public override async Task Parameter_extraction_can_throw_exception_from_user_code(bool async)
    {
        await base.Parameter_extraction_can_throw_exception_from_user_code(async);

        AssertSql();
    }

    public override async Task Parameter_extraction_can_throw_exception_from_user_code_2(bool async)
    {
        await base.Parameter_extraction_can_throw_exception_from_user_code_2(async);

        AssertSql();
    }

    public override async Task Subquery_member_pushdown_does_not_change_original_subquery_model(bool async)
    {
        await base.Subquery_member_pushdown_does_not_change_original_subquery_model(async);

        AssertSql();
    }

    public override async Task Subquery_member_pushdown_does_not_change_original_subquery_model2(bool async)
    {
        await base.Subquery_member_pushdown_does_not_change_original_subquery_model2(async);

        AssertSql();
    }

    public override async Task Query_expression_with_to_string_and_contains(bool async)
    {
        await base.Query_expression_with_to_string_and_contains(async);

        AssertSql();
    }

    public override async Task Select_expression_other_to_string(bool async)
    {
        await base.Select_expression_other_to_string(async);

        AssertSql();
    }

    public override async Task Select_expression_long_to_string(bool async)
    {
        await base.Select_expression_long_to_string(async);

        AssertSql();
    }

    public override async Task Select_expression_int_to_string(bool async)
    {
        await base.Select_expression_int_to_string(async);

        AssertSql();
    }

    public override async Task ToString_with_formatter_is_evaluated_on_the_client(bool async)
    {
        await base.ToString_with_formatter_is_evaluated_on_the_client(async);

        AssertSql();
    }

    public override async Task Select_expression_date_add_year(bool async)
    {
        await base.Select_expression_date_add_year(async);

        AssertSql();
    }

    public override async Task Select_expression_datetime_add_month(bool async)
    {
        await base.Select_expression_datetime_add_month(async);

        AssertSql();
    }

    public override async Task Select_expression_datetime_add_hour(bool async)
    {
        await base.Select_expression_datetime_add_hour(async);

        AssertSql();
    }

    public override async Task Select_expression_datetime_add_minute(bool async)
    {
        await base.Select_expression_datetime_add_minute(async);

        AssertSql();
    }

    public override async Task Select_expression_datetime_add_second(bool async)
    {
        await base.Select_expression_datetime_add_second(async);

        AssertSql();
    }

    public override async Task Select_expression_datetime_add_ticks(bool async)
    {
        await base.Select_expression_datetime_add_ticks(async);

        AssertSql();
    }

    public override async Task Select_expression_date_add_milliseconds_above_the_range(bool async)
    {
        await base.Select_expression_date_add_milliseconds_above_the_range(async);

        AssertSql();
    }

    public override async Task Select_expression_date_add_milliseconds_below_the_range(bool async)
    {
        await base.Select_expression_date_add_milliseconds_below_the_range(async);

        AssertSql();
    }

    public override async Task Select_expression_date_add_milliseconds_large_number_divided(bool async)
    {
        await base.Select_expression_date_add_milliseconds_large_number_divided(async);

        AssertSql();
    }

    public override async Task Add_minutes_on_constant_value(bool async)
    {
        await base.Add_minutes_on_constant_value(async);

        AssertSql();
    }

    public override async Task DefaultIfEmpty_without_group_join(bool async)
    {
        await base.DefaultIfEmpty_without_group_join(async);

        AssertSql();
    }

    public override async Task DefaultIfEmpty_in_subquery(bool async)
    {
        await base.DefaultIfEmpty_in_subquery(async);

        AssertSql();
    }

    public override async Task DefaultIfEmpty_in_subquery_not_correlated(bool async)
    {
        await base.DefaultIfEmpty_in_subquery_not_correlated(async);

        AssertSql();
    }

    public override async Task DefaultIfEmpty_in_subquery_nested(bool async)
    {
        await base.DefaultIfEmpty_in_subquery_nested(async);

        AssertSql();
    }

    public override async Task DefaultIfEmpty_in_subquery_nested_filter_order_comparison(bool async)
    {
        await base.DefaultIfEmpty_in_subquery_nested_filter_order_comparison(async);

        AssertSql();
    }

    public override async Task OrderBy_skip_take(bool async)
    {
        await base.OrderBy_skip_take(async);

        AssertSql();
    }

    public override async Task OrderBy_skip_skip_take(bool async)
    {
        await base.OrderBy_skip_skip_take(async);

        AssertSql();
    }

    public override async Task OrderBy_skip_take_take(bool async)
    {
        await base.OrderBy_skip_take_take(async);

        AssertSql();
    }

    public override async Task OrderBy_skip_take_take_take_take(bool async)
    {
        await base.OrderBy_skip_take_take_take_take(async);

        AssertSql();
    }

    public override async Task OrderBy_skip_take_skip_take_skip(bool async)
    {
        await base.OrderBy_skip_take_skip_take_skip(async);

        AssertSql();
    }

    public override async Task OrderBy_skip_take_distinct(bool async)
    {
        await base.OrderBy_skip_take_distinct(async);

        AssertSql();
    }

    public override async Task OrderBy_coalesce_take_distinct(bool async)
    {
        await base.OrderBy_coalesce_take_distinct(async);

        AssertSql();
    }

    public override async Task OrderBy_coalesce_skip_take_distinct(bool async)
    {
        await base.OrderBy_coalesce_skip_take_distinct(async);

        AssertSql();
    }

    public override async Task OrderBy_coalesce_skip_take_distinct_take(bool async)
    {
        await base.OrderBy_coalesce_skip_take_distinct_take(async);

        AssertSql();
    }

    public override async Task OrderBy_skip_take_distinct_orderby_take(bool async)
    {
        await base.OrderBy_skip_take_distinct_orderby_take(async);

        AssertSql();
    }

    public override async Task No_orderby_added_for_fully_translated_manually_constructed_LOJ(bool async)
    {
        await base.No_orderby_added_for_fully_translated_manually_constructed_LOJ(async);

        AssertSql();
    }

    public override async Task No_orderby_added_for_client_side_GroupJoin_dependent_to_principal_LOJ(bool async)
    {
        await base.No_orderby_added_for_client_side_GroupJoin_dependent_to_principal_LOJ(async);

        AssertSql();
    }

    public override async Task No_orderby_added_for_client_side_GroupJoin_dependent_to_principal_LOJ_with_additional_join_condition1(bool async)
    {
        await base.No_orderby_added_for_client_side_GroupJoin_dependent_to_principal_LOJ_with_additional_join_condition1(async);

        AssertSql();
    }

    public override async Task No_orderby_added_for_client_side_GroupJoin_dependent_to_principal_LOJ_with_additional_join_condition2(bool async)
    {
        await base.No_orderby_added_for_client_side_GroupJoin_dependent_to_principal_LOJ_with_additional_join_condition2(async);

        AssertSql();
    }

    public override async Task Orderby_added_for_client_side_GroupJoin_principal_to_dependent_LOJ(bool async)
    {
        await base.Orderby_added_for_client_side_GroupJoin_principal_to_dependent_LOJ(async);

        AssertSql();
    }

    public override async Task Contains_with_DateTime_Date(bool async)
    {
        await base.Contains_with_DateTime_Date(async);

        AssertSql();
    }

    public override async Task Contains_with_subquery_involving_join_binds_to_correct_table(bool async)
    {
        await base.Contains_with_subquery_involving_join_binds_to_correct_table(async);

        AssertSql();
    }

    public override async Task Anonymous_member_distinct_where(bool async)
    {
        await base.Anonymous_member_distinct_where(async);

        AssertSql();
    }

    public override async Task Anonymous_member_distinct_orderby(bool async)
    {
        await base.Anonymous_member_distinct_orderby(async);

        AssertSql();
    }

    public override async Task Anonymous_member_distinct_result(bool async)
    {
        await base.Anonymous_member_distinct_result(async);

        AssertSql();
    }

    public override async Task Anonymous_complex_distinct_where(bool async)
    {
        await base.Anonymous_complex_distinct_where(async);

        AssertSql();
    }

    public override async Task Anonymous_complex_distinct_orderby(bool async)
    {
        await base.Anonymous_complex_distinct_orderby(async);

        AssertSql();
    }

    public override async Task Anonymous_complex_distinct_result(bool async)
    {
        await base.Anonymous_complex_distinct_result(async);

        AssertSql();
    }

    public override async Task Anonymous_complex_orderby(bool async)
    {
        await base.Anonymous_complex_orderby(async);

        AssertSql();
    }

    public override async Task Anonymous_subquery_orderby(bool async)
    {
        await base.Anonymous_subquery_orderby(async);

        AssertSql();
    }

    public override async Task DTO_member_distinct_where(bool async)
    {
        await base.DTO_member_distinct_where(async);

        AssertSql();
    }

    public override async Task DTO_member_distinct_orderby(bool async)
    {
        await base.DTO_member_distinct_orderby(async);

        AssertSql();
    }

    public override async Task DTO_member_distinct_result(bool async)
    {
        await base.DTO_member_distinct_result(async);

        AssertSql();
    }

    public override async Task DTO_complex_distinct_where(bool async)
    {
        await base.DTO_complex_distinct_where(async);

        AssertSql();
    }

    public override async Task DTO_complex_distinct_orderby(bool async)
    {
        await base.DTO_complex_distinct_orderby(async);

        AssertSql();
    }

    public override async Task DTO_complex_distinct_result(bool async)
    {
        await base.DTO_complex_distinct_result(async);

        AssertSql();
    }

    public override async Task DTO_complex_orderby(bool async)
    {
        await base.DTO_complex_orderby(async);

        AssertSql();
    }

    public override async Task DTO_subquery_orderby(bool async)
    {
        await base.DTO_subquery_orderby(async);

        AssertSql();
    }

    public override async Task Include_with_orderby_skip_preserves_ordering(bool async)
    {
        await base.Include_with_orderby_skip_preserves_ordering(async);

        AssertSql();
    }

    public override async Task Complex_query_with_repeated_query_model_compiles_correctly(bool async)
    {
        await base.Complex_query_with_repeated_query_model_compiles_correctly(async);

        AssertSql();
    }

    public override async Task Complex_query_with_repeated_nested_query_model_compiles_correctly(bool async)
    {
        await base.Complex_query_with_repeated_nested_query_model_compiles_correctly(async);

        AssertSql();
    }

    public override async Task Int16_parameter_can_be_used_for_int_column(bool async)
    {
        await base.Int16_parameter_can_be_used_for_int_column(async);

        AssertSql();
    }

    public override async Task Subquery_is_null_translated_correctly(bool async)
    {
        await base.Subquery_is_null_translated_correctly(async);

        AssertSql();
    }

    public override async Task Subquery_is_not_null_translated_correctly(bool async)
    {
        await base.Subquery_is_not_null_translated_correctly(async);

        AssertSql();
    }

    public override async Task Select_take_average(bool async)
    {
        await base.Select_take_average(async);

        AssertSql();
    }

    public override async Task Select_take_count(bool async)
    {
        await base.Select_take_count(async);

        AssertSql();
    }

    public override async Task Select_orderBy_take_count(bool async)
    {
        await base.Select_orderBy_take_count(async);

        AssertSql();
    }

    public override async Task Select_take_long_count(bool async)
    {
        await base.Select_take_long_count(async);

        AssertSql();
    }

    public override async Task Select_orderBy_take_long_count(bool async)
    {
        await base.Select_orderBy_take_long_count(async);

        AssertSql();
    }

    public override async Task Select_take_max(bool async)
    {
        await base.Select_take_max(async);

        AssertSql();
    }

    public override async Task Select_take_min(bool async)
    {
        await base.Select_take_min(async);

        AssertSql();
    }

    public override async Task Select_take_sum(bool async)
    {
        await base.Select_take_sum(async);

        AssertSql();
    }

    public override async Task Select_skip_average(bool async)
    {
        await base.Select_skip_average(async);

        AssertSql();
    }

    public override async Task Select_skip_count(bool async)
    {
        await base.Select_skip_count(async);

        AssertSql();
    }

    public override async Task Select_orderBy_skip_count(bool async)
    {
        await base.Select_orderBy_skip_count(async);

        AssertSql();
    }

    public override async Task Select_skip_long_count(bool async)
    {
        await base.Select_skip_long_count(async);

        AssertSql();
    }

    public override async Task Select_orderBy_skip_long_count(bool async)
    {
        await base.Select_orderBy_skip_long_count(async);

        AssertSql();
    }

    public override async Task Select_skip_max(bool async)
    {
        await base.Select_skip_max(async);

        AssertSql();
    }

    public override async Task Select_skip_min(bool async)
    {
        await base.Select_skip_min(async);

        AssertSql();
    }

    public override async Task Select_skip_sum(bool async)
    {
        await base.Select_skip_sum(async);

        AssertSql();
    }

    public override async Task Select_distinct_average(bool async)
    {
        await base.Select_distinct_average(async);

        AssertSql();
    }

    public override async Task Select_distinct_count(bool async)
    {
        await base.Select_distinct_count(async);

        AssertSql();
    }

    public override async Task Select_distinct_long_count(bool async)
    {
        await base.Select_distinct_long_count(async);

        AssertSql();
    }

    public override async Task Select_distinct_max(bool async)
    {
        await base.Select_distinct_max(async);

        AssertSql();
    }

    public override async Task Select_distinct_min(bool async)
    {
        await base.Select_distinct_min(async);

        AssertSql();
    }

    public override async Task Select_distinct_sum(bool async)
    {
        await base.Select_distinct_sum(async);

        AssertSql();
    }

    public override async Task Comparing_to_fixed_string_parameter(bool async)
    {
        await base.Comparing_to_fixed_string_parameter(async);

        AssertSql();
    }

    public override async Task Comparing_entities_using_Equals(bool async)
    {
        await base.Comparing_entities_using_Equals(async);

        AssertSql();
    }

    public override async Task Comparing_different_entity_types_using_Equals(bool async)
    {
        await base.Comparing_different_entity_types_using_Equals(async);

        AssertSql();
    }

    public override async Task Comparing_entity_to_null_using_Equals(bool async)
    {
        await base.Comparing_entity_to_null_using_Equals(async);

        AssertSql();
    }

    public override async Task Comparing_navigations_using_Equals(bool async)
    {
        await base.Comparing_navigations_using_Equals(async);

        AssertSql();
    }

    public override async Task Comparing_navigations_using_static_Equals(bool async)
    {
        await base.Comparing_navigations_using_static_Equals(async);

        AssertSql();
    }

    public override async Task Comparing_non_matching_entities_using_Equals(bool async)
    {
        await base.Comparing_non_matching_entities_using_Equals(async);

        AssertSql();
    }

    public override async Task Comparing_non_matching_collection_navigations_using_Equals(bool async)
    {
        await base.Comparing_non_matching_collection_navigations_using_Equals(async);

        AssertSql();
    }

    public override async Task Comparing_collection_navigation_to_null(bool async)
    {
        await base.Comparing_collection_navigation_to_null(async);

        AssertSql();
    }

    public override async Task Comparing_collection_navigation_to_null_complex(bool async)
    {
        await base.Comparing_collection_navigation_to_null_complex(async);

        AssertSql();
    }

    public override async Task Compare_collection_navigation_with_itself(bool async)
    {
        await base.Compare_collection_navigation_with_itself(async);

        AssertSql();
    }

    public override async Task Compare_two_collection_navigations_with_different_query_sources(bool async)
    {
        await base.Compare_two_collection_navigations_with_different_query_sources(async);

        AssertSql();
    }

    public override async Task Compare_two_collection_navigations_using_equals(bool async)
    {
        await base.Compare_two_collection_navigations_using_equals(async);

        AssertSql();
    }

    public override async Task Compare_two_collection_navigations_with_different_property_chains(bool async)
    {
        await base.Compare_two_collection_navigations_with_different_property_chains(async);

        AssertSql();
    }

    public override async Task OrderBy_ThenBy_same_column_different_direction(bool async)
    {
        await base.OrderBy_ThenBy_same_column_different_direction(async);

        AssertSql();
    }

    public override async Task OrderBy_OrderBy_same_column_different_direction(bool async)
    {
        await base.OrderBy_OrderBy_same_column_different_direction(async);

        AssertSql();
    }

    public override async Task Complex_nested_query_properly_binds_to_grandparent_when_parent_returns_scalar_result(bool async)
    {
        await base.Complex_nested_query_properly_binds_to_grandparent_when_parent_returns_scalar_result(async);

        AssertSql();
    }

    public override async Task OrderBy_Dto_projection_skip_take(bool async)
    {
        await base.OrderBy_Dto_projection_skip_take(async);

        AssertSql();
    }

    public override async Task Join_take_count_works(bool async)
    {
        await base.Join_take_count_works(async);

        AssertSql();
    }

    public override async Task OrderBy_empty_list_contains(bool async)
    {
        await base.OrderBy_empty_list_contains(async);

        AssertSql();
    }

    public override async Task OrderBy_empty_list_does_not_contains(bool async)
    {
        await base.OrderBy_empty_list_does_not_contains(async);

        AssertSql();
    }

    public override async Task Manual_expression_tree_typed_null_equality(bool async)
    {
        await base.Manual_expression_tree_typed_null_equality(async);

        AssertSql();
    }

    public override async Task Let_subquery_with_multiple_occurrences(bool async)
    {
        await base.Let_subquery_with_multiple_occurrences(async);

        AssertSql();
    }

    public override async Task Let_entity_equality_to_null(bool async)
    {
        await base.Let_entity_equality_to_null(async);

        AssertSql();
    }

    public override async Task Let_entity_equality_to_other_entity(bool async)
    {
        await base.Let_entity_equality_to_other_entity(async);

        AssertSql();
    }

    public override async Task SelectMany_after_client_method(bool async)
    {
        await base.SelectMany_after_client_method(async);

        AssertSql();
    }

    public override async Task Client_OrderBy_GroupBy_Group_ordering_works(bool async)
    {
        await base.Client_OrderBy_GroupBy_Group_ordering_works(async);

        AssertSql();
    }

    public override async Task Collection_navigation_equal_to_null_for_subquery(bool async)
    {
        await base.Collection_navigation_equal_to_null_for_subquery(async);

        AssertSql();
    }

    public override async Task Dependent_to_principal_navigation_equal_to_null_for_subquery(bool async)
    {
        await base.Dependent_to_principal_navigation_equal_to_null_for_subquery(async);

        AssertSql();
    }

    public override async Task Collection_navigation_equality_rewrite_for_subquery(bool async)
    {
        await base.Collection_navigation_equality_rewrite_for_subquery(async);

        AssertSql();
    }

    public override async Task Inner_parameter_in_nested_lambdas_gets_preserved(bool async)
    {
        await base.Inner_parameter_in_nested_lambdas_gets_preserved(async);

        AssertSql();
    }

    public override async Task Convert_to_nullable_on_nullable_value_is_ignored(bool async)
    {
        await base.Convert_to_nullable_on_nullable_value_is_ignored(async);

        AssertSql();
    }

    public override async Task Navigation_inside_interpolated_string_is_expanded(bool async)
    {
        await base.Navigation_inside_interpolated_string_is_expanded(async);

        AssertSql();
    }

    public override async Task Context_based_client_method(bool async)
    {
        await base.Context_based_client_method(async);

        AssertSql();
    }

    public override async Task OrderBy_object_type_server_evals(bool async)
    {
        await base.OrderBy_object_type_server_evals(async);

        AssertSql();
    }

    public override async Task AsQueryable_in_query_server_evals(bool async)
    {
        await base.AsQueryable_in_query_server_evals(async);

        AssertSql();
    }

    public override async Task Subquery_DefaultIfEmpty_Any(bool async)
    {
        await base.Subquery_DefaultIfEmpty_Any(async);

        AssertSql();
    }

    public override async Task Projection_skip_projection(bool async)
    {
        await base.Projection_skip_projection(async);

        AssertSql();
    }

    public override async Task Projection_take_projection(bool async)
    {
        await base.Projection_take_projection(async);

        AssertSql();
    }

    public override async Task Projection_skip_take_projection(bool async)
    {
        await base.Projection_skip_take_projection(async);

        AssertSql();
    }

    public override async Task Collection_projection_skip(bool async)
    {
        await base.Collection_projection_skip(async);

        AssertSql();
    }

    public override async Task Collection_projection_take(bool async)
    {
        await base.Collection_projection_take(async);

        AssertSql();
    }

    public override async Task Collection_projection_skip_take(bool async)
    {
        await base.Collection_projection_skip_take(async);

        AssertSql();
    }

    public override async Task Anonymous_projection_skip_empty_collection_FirstOrDefault(bool async)
    {
        await base.Anonymous_projection_skip_empty_collection_FirstOrDefault(async);

        AssertSql();
    }

    public override async Task Anonymous_projection_take_empty_collection_FirstOrDefault(bool async)
    {
        await base.Anonymous_projection_take_empty_collection_FirstOrDefault(async);

        AssertSql();
    }

    public override async Task Anonymous_projection_skip_take_empty_collection_FirstOrDefault(bool async)
    {
        await base.Anonymous_projection_skip_take_empty_collection_FirstOrDefault(async);

        AssertSql();
    }

    public override async Task Checked_context_with_arithmetic_does_not_fail(bool async)
    {
        await base.Checked_context_with_arithmetic_does_not_fail(async);

        AssertSql();
    }

    public override async Task Checked_context_with_case_to_same_nullable_type_does_not_fail(bool async)
    {
        await base.Checked_context_with_case_to_same_nullable_type_does_not_fail(async);

        AssertSql();
    }

    public override async Task Entity_equality_with_null_coalesce_client_side(bool async)
    {
        await base.Entity_equality_with_null_coalesce_client_side(async);

        AssertSql();
    }

    public override async Task Entity_equality_contains_with_list_of_null(bool async)
    {
        await base.Entity_equality_contains_with_list_of_null(async);

        AssertSql();
    }

    public override async Task MemberInitExpression_NewExpression_is_funcletized_even_when_bindings_are_not_evaluatable(bool async)
    {
        await base.MemberInitExpression_NewExpression_is_funcletized_even_when_bindings_are_not_evaluatable(async);

        AssertSql();
    }

    public override async Task Funcletize_conditional_with_evaluatable_test(bool async)
    {
        await base.Funcletize_conditional_with_evaluatable_test(async);

        AssertSql();
    }

    public override async Task Null_parameter_name_works(bool async)
    {
        await base.Null_parameter_name_works(async);

        AssertSql();
    }

    public override async Task String_include_on_incorrect_property_throws(bool async)
    {
        await base.String_include_on_incorrect_property_throws(async);

        AssertSql();
    }

    public override async Task EF_Property_include_on_incorrect_property_throws(bool async)
    {
        await base.EF_Property_include_on_incorrect_property_throws(async);

        AssertSql();
    }

    public override async Task Single_non_scalar_projection_after_skip_uses_join(bool async)
    {
        await base.Single_non_scalar_projection_after_skip_uses_join(async);

        AssertSql();
    }

    public override async Task Select_distinct_Select_with_client_bindings(bool async)
    {
        await base.Select_distinct_Select_with_client_bindings(async);

        AssertSql();
    }

    public override async Task ToList_over_string(bool async)
    {
        await base.ToList_over_string(async);

        AssertSql();
    }

    public override async Task ToArray_over_string(bool async)
    {
        await base.ToArray_over_string(async);

        AssertSql();
    }

    public override async Task AsEnumerable_over_string(bool async)
    {
        await base.AsEnumerable_over_string(async);

        AssertSql();
    }

    public override async Task Non_nullable_property_through_optional_navigation(bool async)
    {
        await base.Non_nullable_property_through_optional_navigation(async);

        AssertSql();
    }

    public override async Task Pending_selector_in_cardinality_reducing_method_is_applied_before_expanding_collection_navigation_member(bool async)
    {
        await base.Pending_selector_in_cardinality_reducing_method_is_applied_before_expanding_collection_navigation_member(async);

        AssertSql();
    }

    public override async Task Distinct_followed_by_ordering_on_condition(bool async)
    {
        await base.Distinct_followed_by_ordering_on_condition(async);

        AssertSql();
    }

    public override async Task Entity_equality_on_subquery_with_null_check(bool async)
    {
        await base.Entity_equality_on_subquery_with_null_check(async);

        AssertSql();
    }

    public override async Task DefaultIfEmpty_over_empty_collection_followed_by_projecting_constant(bool async)
    {
        await base.DefaultIfEmpty_over_empty_collection_followed_by_projecting_constant(async);

        AssertSql();
    }

    public override async Task FirstOrDefault_with_predicate_nested(bool async)
    {
        await base.FirstOrDefault_with_predicate_nested(async);

        AssertSql();
    }

    public override async Task First_on_collection_in_projection(bool async)
    {
        await base.First_on_collection_in_projection(async);

        AssertSql();
    }

    public override async Task SkipWhile_throws_meaningful_exception(bool async)
    {
        await base.SkipWhile_throws_meaningful_exception(async);

        AssertSql();
    }

    public override async Task Skip_0_Take_0_works_when_parameter(bool async)
    {
        await base.Skip_0_Take_0_works_when_parameter(async);

        AssertSql();
    }

    public override async Task Skip_0_Take_0_works_when_constant(bool async)
    {
        await base.Skip_0_Take_0_works_when_constant(async);

        AssertSql();
    }

    public override async Task Skip_1_Take_0_works_when_constant(bool async)
    {
        await base.Skip_1_Take_0_works_when_constant(async);

        AssertSql();
    }

    public override async Task Take_0_works_when_constant(bool async)
    {
        await base.Take_0_works_when_constant(async);

        AssertSql();
    }

    public override async Task ToListAsync_can_be_canceled()
    {
        await base.ToListAsync_can_be_canceled();

        AssertSql();
    }

    public override async Task ToListAsync_with_canceled_token()
    {
        await base.ToListAsync_with_canceled_token();

        AssertSql();
    }

    public override async Task Mixed_sync_async_query()
    {
        await base.Mixed_sync_async_query();

        AssertSql();
    }

    public override async Task Mixed_sync_async_in_query_cache()
    {
        await base.Mixed_sync_async_in_query_cache();

        AssertSql();
    }

    public override async Task Load_should_track_results(bool async)
    {
        await base.Load_should_track_results(async);

        AssertSql();
    }

    public override async Task Correlated_collection_with_distinct_without_default_identifiers_projecting_columns(bool async)
    {
        await base.Correlated_collection_with_distinct_without_default_identifiers_projecting_columns(async);

        AssertSql();
    }

    public override async Task Correlated_collection_with_distinct_without_default_identifiers_projecting_columns_with_navigation(bool async)
    {
        await base.Correlated_collection_with_distinct_without_default_identifiers_projecting_columns_with_navigation(async);

        AssertSql();
    }

    public override async Task Select_nested_collection_with_distinct(bool async)
    {
        await base.Select_nested_collection_with_distinct(async);

        AssertSql();
    }

    public override async Task Collection_projection_after_DefaultIfEmpty(bool async)
    {
        await base.Collection_projection_after_DefaultIfEmpty(async);

        AssertSql();
    }

    public override async Task Collection_navigation_equal_to_null_for_subquery_using_ElementAtOrDefault_constant_zero(bool async)
    {
        await base.Collection_navigation_equal_to_null_for_subquery_using_ElementAtOrDefault_constant_zero(async);

        AssertSql();
    }

    public override async Task Collection_navigation_equal_to_null_for_subquery_using_ElementAtOrDefault_constant_one(bool async)
    {
        await base.Collection_navigation_equal_to_null_for_subquery_using_ElementAtOrDefault_constant_one(async);

        AssertSql();
    }

    public override async Task Collection_navigation_equal_to_null_for_subquery_using_ElementAtOrDefault_parameter(bool async)
    {
        await base.Collection_navigation_equal_to_null_for_subquery_using_ElementAtOrDefault_parameter(async);

        AssertSql();
    }

    public override async Task Parameter_collection_Contains_with_projection_and_ordering(bool async)
    {
        await base.Parameter_collection_Contains_with_projection_and_ordering(async);

        AssertSql();
    }

    public override async Task Contains_over_concatenated_columns_with_different_sizes(bool async)
    {
        await base.Contains_over_concatenated_columns_with_different_sizes(async);

        AssertSql();
    }

    public override async Task Contains_over_concatenated_column_and_constant(bool async)
    {
        await base.Contains_over_concatenated_column_and_constant(async);

        AssertSql();
    }

    public override async Task Contains_over_concatenated_column_and_parameter(bool async)
    {
        await base.Contains_over_concatenated_column_and_parameter(async);

        AssertSql();
    }

    public override async Task Contains_over_concatenated_parameter_and_constant(bool async)
    {
        await base.Contains_over_concatenated_parameter_and_constant(async);

        AssertSql();
    }

    public override async Task Contains_over_concatenated_columns_both_fixed_length(bool async)
    {
        await base.Contains_over_concatenated_columns_both_fixed_length(async);

        AssertSql();
    }

    public override async Task Compiler_generated_local_closure_produces_valid_parameter_name(bool async)
    {
        await base.Compiler_generated_local_closure_produces_valid_parameter_name(async);

        AssertSql();
    }

    public override async Task Static_member_access_gets_parameterized_within_larger_evaluatable(bool async)
    {
        await base.Static_member_access_gets_parameterized_within_larger_evaluatable(async);

        AssertSql();
    }

    public override async Task Where_nanosecond_and_microsecond_component(bool async)
    {
        await base.Where_nanosecond_and_microsecond_component(async);

        AssertSql();
    }

    public override async Task Ternary_Not_Null_Contains(bool async)
    {
        await base.Ternary_Not_Null_Contains(async);

        AssertSql();
    }

    public override async Task Ternary_Not_Null_endsWith_Non_Numeric_First_Part(bool async)
    {
        await base.Ternary_Not_Null_endsWith_Non_Numeric_First_Part(async);

        AssertSql();
    }

    public override async Task Ternary_Null_Equals_Non_Numeric_First_Part(bool async)
    {
        await base.Ternary_Null_Equals_Non_Numeric_First_Part(async);

        AssertSql();
    }

    public override async Task Ternary_Null_StartsWith(bool async)
    {
        await base.Ternary_Null_StartsWith(async);

        AssertSql();
    }

    public override async Task Column_access_inside_subquery_predicate(bool async)
    {
        await base.Column_access_inside_subquery_predicate(async);

        AssertSql();
    }

    public override async Task Cast_to_object_over_parameter_directly_in_lambda(bool async)
    {
        await base.Cast_to_object_over_parameter_directly_in_lambda(async);

        AssertSql();
    }

    public override async Task Late_subquery_pushdown(bool async)
    {
        await base.Late_subquery_pushdown(async);

        AssertSql();
    }

    public override void Query_composition_against_ienumerable_set()
    {
        base.Query_composition_against_ienumerable_set();

        AssertSql();
    }

    public override void Can_cast_CreateQuery_result_to_IQueryable_T_bug_1730()
    {
        base.Can_cast_CreateQuery_result_to_IQueryable_T_bug_1730();

        AssertSql();
    }

        [ConditionalFact]
        public virtual void Check_all_tests_overridden()
            => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

        private void AssertSql(params string[] expected)
            => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

        protected override void ClearLog()
            => Fixture.TestSqlLoggerFactory.Clear();

        private class OrderCountDTO
        {
            public string Id { get; set; }
            public int Count { get; set; }

            public OrderCountDTO()
            {
            }

            public OrderCountDTO(string id)
            {
                Id = id;
                Count = 0;
            }

            public override bool Equals(object obj)
            {
                if (obj is null)
                {
                    return false;
                }

                return ReferenceEquals(this, obj) ? true : obj.GetType() == GetType() && Equals((OrderCountDTO)obj);
            }

            private bool Equals(OrderCountDTO other)
                => string.Equals(Id, other.Id) && Count == other.Count;

            public override int GetHashCode()
                => HashCode.Combine(Id, Count);
        }
    }
}
