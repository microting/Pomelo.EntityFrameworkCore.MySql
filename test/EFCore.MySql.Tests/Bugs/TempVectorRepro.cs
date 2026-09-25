using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace Microting.EntityFrameworkCore.MySql.Bugs
{
    public class TempVectorRepro
    {
        private readonly ITestOutputHelper _output;

        public TempVectorRepro(ITestOutputHelper output) => _output = output;

        [Fact]
        public void Repro()
        {
            var services = MySqlTestHelpers.Instance.CreateContextServices(b => { });
            var modelBuilder = MySqlTestHelpers.Instance.CreateConventionBuilder(services);
            modelBuilder.Entity<Doc>(
                e =>
                {
                    e.Property(p => p.Embedding).HasColumnType("vector(312)");
                    e.Property(p => p.EmbeddingBytes).HasColumnType("vector(312)");
                });

            var model = services.GetService<IModelRuntimeInitializer>()
                .Initialize(modelBuilder.FinalizeModel(), designTime: true, validationLogger: null);

            var table = model.GetRelationalModel().Tables.Single();
            foreach (var column in table.Columns)
            {
                _output.WriteLine($"{column.Name}: {column.StoreType} => {string.Join(", ", column.GetAnnotations().Select(a => a.Name + "=" + a.Value))}");
            }
        }

        public class Doc
        {
            public int Id { get; set; }
            public string Embedding { get; set; }
            public byte[] EmbeddingBytes { get; set; }
        }
    }
}
