using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests;

public class MaterializationInterceptionMySqlTest : MaterializationInterceptionTestBase<MaterializationInterceptionMySqlTest.MySqlLibraryContext>
{
    public MaterializationInterceptionMySqlTest()
        : base(new MySqlMaterializationInterceptionFixture())
    {
    }

    public class MySqlLibraryContext : LibraryContext
    {
        public MySqlLibraryContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TestEntity30244>().OwnsMany(e => e.Settings);

            // TODO: https://github.com/npgsql/efcore.pg/issues/2548
            // modelBuilder.Entity<TestEntity30244>().OwnsMany(e => e.Settings, b => b.ToJson());
        }
    }

    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;

    public class MySqlMaterializationInterceptionFixture : SingletonInterceptorsFixtureBase
    {
        protected override string StoreName => "MaterializationInterception";
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

        protected override IServiceCollection InjectInterceptors(
            IServiceCollection serviceCollection,
            IEnumerable<IInterceptor> injectedInterceptors)
            => base.InjectInterceptors(serviceCollection.AddEntityFrameworkMySql(), injectedInterceptors);
    }
}
