using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query.Inheritance;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class TPHInheritanceQueryMySqlFixture : TPHInheritanceQueryFixture
{
    protected override ITestStoreFactory TestStoreFactory =>  MySqlTestStoreFactory.Instance;
}
