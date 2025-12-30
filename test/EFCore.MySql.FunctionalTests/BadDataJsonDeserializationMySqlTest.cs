using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests;

// Re-enabled to test JSON deserialization with bad data
// Now testing on both MySQL and MariaDB (MariaDB uses JSON alias for LONGTEXT with validation)
public class BadDataJsonDeserializationMySqlTest : BadDataJsonDeserializationTestBase
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder.UseMySql(AppConfig.ServerVersion, b =>
        {
            b.UseNetTopologySuite();
            b.EnablePrimitiveCollectionsSupport();
        }));
    }
}
