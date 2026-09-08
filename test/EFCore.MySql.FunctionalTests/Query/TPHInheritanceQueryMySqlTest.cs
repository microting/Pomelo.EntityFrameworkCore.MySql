using Microsoft.EntityFrameworkCore.Query;
using Xunit;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class TPHInheritanceQueryMySqlTest : TPHInheritanceQueryTestBase<TPHInheritanceQueryMySqlFixture>
{
    public TPHInheritanceQueryMySqlTest(TPHInheritanceQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
    }
}
