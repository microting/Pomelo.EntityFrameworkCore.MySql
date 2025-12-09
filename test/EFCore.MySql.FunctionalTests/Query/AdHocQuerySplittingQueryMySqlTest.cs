using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class AdHocQuerySplittingQueryMySqlTest : AdHocQuerySplittingQueryTestBase
{
    public AdHocQuerySplittingQueryMySqlTest(NonSharedFixture fixture)
        : base(fixture)
    {
    }

    protected override DbContextOptionsBuilder SetQuerySplittingBehavior(
        DbContextOptionsBuilder optionsBuilder,
        QuerySplittingBehavior splittingBehavior)
    {
        new MySqlDbContextOptionsBuilder(optionsBuilder).UseQuerySplittingBehavior(splittingBehavior);

        return optionsBuilder;
    }

    protected override DbContextOptionsBuilder ClearQuerySplittingBehavior(DbContextOptionsBuilder optionsBuilder)
    {
        // Create a new options builder to ensure a new Options instance
        var newOptionsBuilder = new DbContextOptionsBuilder(optionsBuilder.Options);
        
        var extension = newOptionsBuilder.Options.FindExtension<MySqlOptionsExtension>();
        
        // Create a new extension instance to avoid modifying the existing one
        var newExtension = extension != null 
            ? new MySqlOptionsExtension(extension) 
            : new MySqlOptionsExtension();
        
        _querySplittingBehaviorFieldInfo.SetValue(newExtension, null);

        ((IDbContextOptionsBuilderInfrastructure)newOptionsBuilder).AddOrUpdateExtension(newExtension);

        return newOptionsBuilder;
    }

    private static readonly FieldInfo _querySplittingBehaviorFieldInfo =
        typeof(RelationalOptionsExtension).GetField("_querySplittingBehavior", BindingFlags.NonPublic | BindingFlags.Instance);

    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;
}
