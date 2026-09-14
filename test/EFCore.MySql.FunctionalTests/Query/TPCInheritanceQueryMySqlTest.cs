using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;
using Microsoft.EntityFrameworkCore.Query.Inheritance;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class TPCInheritanceQueryMySqlTest : TPCInheritanceQueryTestBase<TPCInheritanceQueryMySqlFixture>
{
    public TPCInheritanceQueryMySqlTest(
        TPCInheritanceQueryMySqlFixture fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    protected override void UseTransaction(DatabaseFacade facade, IDbContextTransaction transaction)
        => facade.UseTransaction(transaction.GetDbTransaction());

    // Requires JSON_TABLE(), which is only available on MySQL 8+ and MariaDB 10.6+.
    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTable))]
    public override Task Primitive_collection_on_subtype(bool async)
        => base.Primitive_collection_on_subtype(async);
}
