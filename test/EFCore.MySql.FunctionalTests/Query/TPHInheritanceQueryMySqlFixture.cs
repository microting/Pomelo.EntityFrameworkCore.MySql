using Pomelo.EntityFrameworkCore.MySql.Tests;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microsoft.EntityFrameworkCore.Query.Inheritance;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class TPHInheritanceQueryMySqlFixture : TPHInheritanceQueryFixture
{
    protected override ITestStoreFactory TestStoreFactory =>  MySqlTestStoreFactory.Instance;

    public override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
    {
        var optionsBuilder = base.AddOptions(builder);

        // `Primitive_collection_on_subtype` needs JSON_TABLE(), which the provider only emits when primitive collections
        // support has been explicitly enabled (and the server supports JSON_TABLE() at all).
        if (AppConfig.ServerVersion.Supports.JsonTable)
        {
            new MySqlDbContextOptionsBuilder(optionsBuilder).EnablePrimitiveCollectionsSupport();
        }

        return optionsBuilder;
    }
}
