using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Xunit;
using Microsoft.EntityFrameworkCore.Query.Inheritance;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class TPTInheritanceQueryMySqlTest : TPTInheritanceQueryTestBase<TPTInheritanceQueryMySqlFixture>
    {
        public TPTInheritanceQueryMySqlTest(
            TPTInheritanceQueryMySqlFixture fixture,
            ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
            Fixture.TestSqlLoggerFactory.Clear();
            //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
        }
    }
}
