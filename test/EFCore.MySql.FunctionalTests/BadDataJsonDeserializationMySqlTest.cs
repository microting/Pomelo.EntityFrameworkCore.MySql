using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests;

// Re-enabled to test JSON deserialization with bad data
// Skip on MariaDB as it uses JSON emulation (LONGTEXT) which has different behavior
[SupportedServerVersionLessThanCondition(nameof(ServerVersionSupport.JsonDataTypeEmulation))]
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
