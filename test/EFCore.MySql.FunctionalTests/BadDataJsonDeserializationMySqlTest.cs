using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests;

// Re-enabled to test JSON deserialization with bad data
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
