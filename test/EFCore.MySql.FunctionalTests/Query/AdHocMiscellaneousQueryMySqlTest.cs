using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using NameSpace1;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class AdHocMiscellaneousQueryMySqlTest : AdHocMiscellaneousQueryRelationalTestBase
{
    public AdHocMiscellaneousQueryMySqlTest(NonSharedFixture fixture)
        : base(fixture)
    {
    }

    protected override ITestStoreFactory NonSharedTestStoreFactory
        => MySqlTestStoreFactory.Instance;

    protected override DbContextOptionsBuilder SetParameterizedCollectionMode(DbContextOptionsBuilder optionsBuilder, ParameterTranslationMode parameterizedCollectionMode)
    {
        new MySqlDbContextOptionsBuilder(optionsBuilder).UseParameterizedCollectionMode(parameterizedCollectionMode);

        return optionsBuilder;
    }

    protected override Task Seed2951(Context2951 context)
        => context.Database.ExecuteSqlRawAsync(
            """
CREATE TABLE `ZeroKey` (`Id` int);
INSERT INTO `ZeroKey` VALUES (NULL)
""");

    protected override async Task Seed30915(Context30915 context)
    {
        context.Statuses.AddRange(
            new Context30915.PickupStatus30915 { PickupStatusId = 1, Name = "Active" },
            new Context30915.PickupStatus30915 { PickupStatusId = 2, Name = "NoRequests" },
            new Context30915.PickupStatus30915 { PickupStatusId = 3, Name = "Busy" });

        context.Requests.AddRange(
            new Context30915.PickupRequest30915 { PickupStatusId = 1, Priority = 5 },
            new Context30915.PickupRequest30915 { PickupStatusId = 1, Priority = null },
            new Context30915.PickupRequest30915 { PickupStatusId = 3, Priority = 7 });

        await context.SaveChangesAsync();
    }

    public override async Task Multiple_different_entity_type_from_different_namespaces(bool async)
    {
        // The only change is the FromSqlRaw SQL string:
        //     Original: SELECT cast(null as int) AS MyValue
        //     Changed:  SELECT cast(null as signed) AS MyValue
        // The other comments are part of the base implementation.

        var contextFactory = await InitializeAsync<Context23981>();
        using var context = contextFactory.CreateContext();
        //var good1 = context.Set<NameSpace1.TestQuery>().FromSqlRaw(@"SELECT 1 AS MyValue").ToList(); // OK
        //var good2 = context.Set<NameSpace2.TestQuery>().FromSqlRaw(@"SELECT 1 AS MyValue").ToList(); // OK
        var bad = context.Set<TestQuery>().FromSqlRaw(@"SELECT cast(null as signed) AS MyValue").ToList(); // Exception
    }
}
