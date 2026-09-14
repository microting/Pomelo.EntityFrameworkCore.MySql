using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Microsoft.EntityFrameworkCore.Query.Inheritance;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class TPHInheritanceQueryMySqlTest : TPHInheritanceQueryTestBase<TPHInheritanceQueryMySqlFixture>
{
    public TPHInheritanceQueryMySqlTest(TPHInheritanceQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }

    // Requires JSON_TABLE(), which is only available on MySQL 8+ and MariaDB 10.6+.
    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    [SupportedServerVersionCondition(nameof(ServerVersionSupport.JsonTable))]
    public override Task Primitive_collection_on_subtype(bool async)
        => base.Primitive_collection_on_subtype(async);
}
