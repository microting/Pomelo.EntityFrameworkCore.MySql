using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.BulkUpdates;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestUtilities;
using MySqlConnector;
using Microting.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Microting.EntityFrameworkCore.MySql.Tests;
using Xunit;

namespace Microting.EntityFrameworkCore.MySql.FunctionalTests.BulkUpdates;

public class NonSharedModelBulkUpdatesMySqlTest : NonSharedModelBulkUpdatesRelationalTestBase
{
    public NonSharedModelBulkUpdatesMySqlTest(NonSharedFixture fixture)
        : base(fixture)
    {
    }

    protected override ITestStoreFactory TestStoreFactory
        => MySqlTestStoreFactory.Instance;

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

    public override async Task Delete_aggregate_root_when_eager_loaded_owned_collection(bool async)
    {
        await base.Delete_aggregate_root_when_eager_loaded_owned_collection(async);

        AssertSql(
"""
DELETE `o`
FROM `Owner` AS `o`
""");
    }

    public override async Task Delete_aggregate_root_when_table_sharing_with_owned(bool async)
    {
        await base.Delete_aggregate_root_when_table_sharing_with_owned(async);

        AssertSql(
"""
DELETE `o`
FROM `Owner` AS `o`
""");
    }

    public override async Task Delete_aggregate_root_when_table_sharing_with_non_owned_throws(bool async)
    {
        await base.Delete_aggregate_root_when_table_sharing_with_non_owned_throws(async);

        AssertSql();
    }

    public override async Task Delete_predicate_based_on_optional_navigation(bool async)
    {
        await base.Delete_predicate_based_on_optional_navigation(async);

        AssertSql(
"""
DELETE `p`
FROM `Posts` AS `p`
LEFT JOIN `Blogs` AS `b` ON `p`.`BlogId` = `b`.`Id`
WHERE `b`.`Title` LIKE 'Arthur%'
""");
    }

    public override async Task Update_non_owned_property_on_entity_with_owned(bool async)
    {
        await base.Update_non_owned_property_on_entity_with_owned(async);

        AssertSql(
"""
@p='SomeValue' (Size = 4000)

UPDATE `Owner` AS `o`
SET `o`.`Title` = @p
""");
    }

    public override async Task Update_non_owned_property_on_entity_with_owned2(bool async)
    {
        await base.Update_non_owned_property_on_entity_with_owned2(async);

        AssertSql(
"""
UPDATE `Owner` AS `o`
SET `o`.`Title` = CONCAT(COALESCE(`o`.`Title`, ''), '_Suffix')
""");
    }

    public override async Task Update_owned_and_non_owned_properties_with_table_sharing(bool async)
    {
        await base.Update_owned_and_non_owned_properties_with_table_sharing(async);

        AssertSql(
"""
UPDATE `Owner` AS `o`
SET `o`.`Title` = COALESCE(CAST(`o`.`OwnedReference_Number` AS char), ''),
    `o`.`OwnedReference_Number` = CHAR_LENGTH(`o`.`Title`)
""");
    }

    public override async Task Update_main_table_in_entity_with_entity_splitting(bool async)
    {
        await base.Update_main_table_in_entity_with_entity_splitting(async);

        AssertSql(
"""
UPDATE `Blogs` AS `b`
SET `b`.`CreationTimestamp` = TIMESTAMP '2020-01-01 00:00:00'
""");
    }

    public override async Task Update_non_main_table_in_entity_with_entity_splitting(bool async)
    {
        await base.Update_non_main_table_in_entity_with_entity_splitting(async);

        AssertSql(
"""
UPDATE `Blogs` AS `b`
INNER JOIN `BlogsPart1` AS `b0` ON `b`.`Id` = `b0`.`Id`
SET `b0`.`Title` = CAST(`b0`.`Rating` AS char),
    `b0`.`Rating` = CHAR_LENGTH(`b0`.`Title`)
""");
    }

    public override async Task Delete_entity_with_auto_include(bool async)
    {
        await base.Delete_entity_with_auto_include(async);

        AssertSql(
"""
DELETE `c`
FROM `Context30572_Principal` AS `c`
LEFT JOIN `Context30572_Dependent` AS `c0` ON `c`.`DependentId` = `c0`.`Id`
""");
    }

    public override async Task Update_with_alias_uniquification_in_setter_subquery(bool async)
    {
        await base.Update_with_alias_uniquification_in_setter_subquery(async);

        AssertSql(
"""
UPDATE `Orders` AS `o`
SET `o`.`Total` = (
    SELECT COALESCE(SUM(`o0`.`Amount`), 0)
    FROM `OrderProduct` AS `o0`
    WHERE `o`.`Id` = `o0`.`OrderId`)
WHERE `o`.`Id` = 1
""");
    }

    public override Task Delete_with_owned_collection_and_non_natively_translatable_query(bool async)
        => Assert.ThrowsAsync<MySqlException>(() =>base.Delete_with_owned_collection_and_non_natively_translatable_query(async));

    public override async Task Update_non_owned_property_on_entity_with_owned_in_join(bool async)
    {
        await base.Update_non_owned_property_on_entity_with_owned_in_join(async);

        AssertSql(
"""
@p='NewValue' (Size = 4000)

UPDATE `Owner` AS `o`
INNER JOIN `Owner` AS `o0` ON `o`.`Id` = `o0`.`Id`
SET `o`.`Title` = @p
""");
    }

    public override async Task Replace_ColumnExpression_in_column_setter(bool async)
    {
        await base.Replace_ColumnExpression_in_column_setter(async);

        AssertSql(
"""
@p='SomeValue' (Size = 4000)

UPDATE `Owner` AS `o`
INNER JOIN `OwnedCollection` AS `o0` ON `o`.`Id` = `o0`.`OwnerId`
SET `o0`.`Value` = @p
""");
    }

    public override async Task Delete_with_view_mapping(bool async)
    {
        await base.Delete_with_view_mapping(async);

        AssertSql(
"""
DELETE `b`
FROM `Blogs` AS `b`
""");
    }

    public override async Task Update_with_view_mapping(bool async)
    {
        await base.Update_with_view_mapping(async);

        AssertSql(
"""
@p='Updated' (Size = 4000)

UPDATE `Blogs` AS `b`
SET `b`.`Data` = @p
""");
    }

    public override async Task Update_complex_type_with_view_mapping(bool async)
    {
        await base.Update_complex_type_with_view_mapping(async);

        AssertSql();
    }

    // Regression tests for ExecuteDelete queries that end up as a single-table `DELETE ... [ORDER BY ...] [LIMIT ...]`.
    // For that shape the table alias is dropped, so a column of the deleted table that is referenced from inside a subquery
    // has to stay qualified. If it is emitted unqualified, MySQL/MariaDB bind it to the innermost table that has a column
    // of that name (`s`.`GroupId` = `GroupId` becomes `s`.`GroupId` = `s`.`GroupId`) and the predicate is no longer correlated.
    // Every query below is expected to delete exactly the logs of group 1 (3 of 5 rows), or the logs of groups 2 and 3 for `!Any`.

    [ConditionalTheory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Delete_Where_Any_correlated_subquery_OrderBy_Take(bool async)
    {
        await AssertDeleteWithCorrelatedSubquery(
            async,
            context => context.Logs
                .Where(l => context.Summaries.Any(s => s.GroupId == l.GroupId))
                .OrderBy(l => l.Id)
                .Take(5000),
            expectedDeleted: 3,
            expectedRemainingGroups: [2, 3],
            """
@p='5000'

DELETE
FROM `Logs`
WHERE EXISTS (
    SELECT 1
    FROM `Summaries` AS `s`
    WHERE `s`.`GroupId` = `Logs`.`GroupId`)
ORDER BY `Id`
LIMIT @p
""");
    }

    [ConditionalTheory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Delete_Where_Any_correlated_subquery_Take(bool async)
    {
        await AssertDeleteWithCorrelatedSubquery(
            async,
            context => context.Logs
                .Where(l => context.Summaries.Any(s => s.GroupId == l.GroupId))
                .Take(5000),
            expectedDeleted: 3,
            expectedRemainingGroups: [2, 3],
            """
@p='5000'

DELETE
FROM `Logs`
WHERE EXISTS (
    SELECT 1
    FROM `Summaries` AS `s`
    WHERE `s`.`GroupId` = `Logs`.`GroupId`)
LIMIT @p
""");
    }

    [ConditionalTheory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Delete_Where_Any_correlated_subquery_OrderBy(bool async)
    {
        await AssertDeleteWithCorrelatedSubquery(
            async,
            context => context.Logs
                .Where(l => context.Summaries.Any(s => s.GroupId == l.GroupId))
                .OrderBy(l => l.Id),
            expectedDeleted: 3,
            expectedRemainingGroups: [2, 3],
            """
DELETE
FROM `Logs`
WHERE EXISTS (
    SELECT 1
    FROM `Summaries` AS `s`
    WHERE `s`.`GroupId` = `Logs`.`GroupId`)
ORDER BY `Id`
""");
    }

    [ConditionalTheory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Delete_Where_not_Any_correlated_subquery_OrderBy_Take(bool async)
    {
        await AssertDeleteWithCorrelatedSubquery(
            async,
            context => context.Logs
                .Where(l => !context.Summaries.Any(s => s.GroupId == l.GroupId))
                .OrderBy(l => l.Id)
                .Take(5000),
            expectedDeleted: 2,
            expectedRemainingGroups: [1, 1, 1],
            """
@p='5000'

DELETE
FROM `Logs`
WHERE NOT EXISTS (
    SELECT 1
    FROM `Summaries` AS `s`
    WHERE `s`.`GroupId` = `Logs`.`GroupId`)
ORDER BY `Id`
LIMIT @p
""");
    }

    [ConditionalTheory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Delete_Where_nested_Any_correlated_subquery_OrderBy_Take(bool async)
    {
        // The outer column is referenced from the second level of nesting.
        await AssertDeleteWithCorrelatedSubquery(
            async,
            context => context.Logs
                .Where(l => context.Summaries.Any(s => context.Summaries.Any(s2 => s2.Id == s.Id && s2.GroupId == l.GroupId)))
                .OrderBy(l => l.Id)
                .Take(5000),
            expectedDeleted: 3,
            expectedRemainingGroups: [2, 3],
            """
@p='5000'

DELETE
FROM `Logs`
WHERE EXISTS (
    SELECT 1
    FROM `Summaries` AS `s`
    WHERE EXISTS (
        SELECT 1
        FROM `Summaries` AS `s0`
        WHERE (`s0`.`Id` = `s`.`Id`) AND (`s0`.`GroupId` = `Logs`.`GroupId`)))
ORDER BY `Id`
LIMIT @p
""");
    }

    [ConditionalTheory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Delete_Where_Any_correlated_subquery_with_distinct_column_name_OrderBy_Take(bool async)
    {
        // `Ref` does not exist in the inner table, so the unqualified column used to be bound to the right table by accident.
        await AssertDeleteWithCorrelatedSubquery(
            async,
            context => context.Logs
                .Where(l => context.Summaries.Any(s => s.GroupId == l.Ref))
                .OrderBy(l => l.Id)
                .Take(5000),
            expectedDeleted: 3,
            expectedRemainingGroups: [2, 3],
            """
@p='5000'

DELETE
FROM `Logs`
WHERE EXISTS (
    SELECT 1
    FROM `Summaries` AS `s`
    WHERE `s`.`GroupId` = `Logs`.`Ref`)
ORDER BY `Id`
LIMIT @p
""");
    }

    [ConditionalTheory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Delete_Where_Contains_subquery_OrderBy_Take(bool async)
    {
        // The outer column is not inside the subquery here, so it stays unqualified.
        await AssertDeleteWithCorrelatedSubquery(
            async,
            context => context.Logs
                .Where(l => context.Summaries.Select(s => s.GroupId).Contains(l.GroupId))
                .OrderBy(l => l.Id)
                .Take(5000),
            expectedDeleted: 3,
            expectedRemainingGroups: [2, 3],
            """
@p='5000'

DELETE
FROM `Logs`
WHERE `GroupId` IN (
    SELECT `s`.`GroupId`
    FROM `Summaries` AS `s`
)
ORDER BY `Id`
LIMIT @p
""");
    }

    [ConditionalTheory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Delete_Where_Any_correlated_subquery(bool async)
    {
        // Without ORDER BY/LIMIT the multi-table form is used, which keeps the alias.
        await AssertDeleteWithCorrelatedSubquery(
            async,
            context => context.Logs.Where(l => context.Summaries.Any(s => s.GroupId == l.GroupId)),
            expectedDeleted: 3,
            expectedRemainingGroups: [2, 3],
            """
DELETE `l`
FROM `Logs` AS `l`
WHERE EXISTS (
    SELECT 1
    FROM `Summaries` AS `s`
    WHERE `s`.`GroupId` = `l`.`GroupId`)
""");
    }

    [ConditionalTheory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Update_Where_Any_correlated_subquery(bool async)
    {
        var contextFactory = await InitializeDeleteWithCorrelatedSubqueryAsync();
        await using var context = contextFactory.CreateContext();

        TestSqlLoggerFactory.Clear();

        var query = context.Logs.Where(l => context.Summaries.Any(s => s.GroupId == l.GroupId));
        var updated = async
            ? await query.ExecuteUpdateAsync(u => u.SetProperty(l => l.Note, "touched"))
            : query.ExecuteUpdate(u => u.SetProperty(l => l.Note, "touched"));

        Assert.Equal(3, updated);

        AssertSql(
"""
@p='touched' (Size = 4000)

UPDATE `Logs` AS `l`
SET `l`.`Note` = @p
WHERE EXISTS (
    SELECT 1
    FROM `Summaries` AS `s`
    WHERE `s`.`GroupId` = `l`.`GroupId`)
""");

        Assert.Equal(3, await context.Logs.CountAsync(l => l.Note == "touched"));
    }

    private async Task AssertDeleteWithCorrelatedSubquery(
        bool async,
        Func<DeleteWithCorrelatedSubqueryContext, IQueryable<DeleteWithCorrelatedSubqueryLog>> query,
        int expectedDeleted,
        int[] expectedRemainingGroups,
        string expectedSql)
    {
        var contextFactory = await InitializeDeleteWithCorrelatedSubqueryAsync();
        await using var context = contextFactory.CreateContext();

        TestSqlLoggerFactory.Clear();

        var logs = query(context);
        var deleted = async
            ? await logs.ExecuteDeleteAsync()
            : logs.ExecuteDelete();

        Assert.Equal(expectedDeleted, deleted);

        // The SQL is asserted before the verification query below, which would otherwise be part of the logged statements.
        AssertSql(expectedSql);

        Assert.Equal(
            expectedRemainingGroups.Order(),
            (await context.Logs.Select(l => l.GroupId).ToListAsync()).Order());
    }

    private Task<ContextFactory<DeleteWithCorrelatedSubqueryContext>> InitializeDeleteWithCorrelatedSubqueryAsync()
        => InitializeAsync<DeleteWithCorrelatedSubqueryContext>(
            // One of the tests takes without ordering on purpose: it is the shape that drops the alias without an ORDER BY.
            onConfiguring: options => options.ConfigureWarnings(w => w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning)),
            seed: async context =>
            {
                // 5 logs: 3 belong to group 1, which has a summary; groups 2 and 3 have none.
                context.Summaries.Add(new DeleteWithCorrelatedSubquerySummary { GroupId = 1 });
                context.Logs.AddRange(
                    new DeleteWithCorrelatedSubqueryLog { GroupId = 1, Ref = 1 },
                    new DeleteWithCorrelatedSubqueryLog { GroupId = 1, Ref = 1 },
                    new DeleteWithCorrelatedSubqueryLog { GroupId = 1, Ref = 1 },
                    new DeleteWithCorrelatedSubqueryLog { GroupId = 2, Ref = 2 },
                    new DeleteWithCorrelatedSubqueryLog { GroupId = 3, Ref = 3 });
                await context.SaveChangesAsync();
            });

    private class DeleteWithCorrelatedSubqueryContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<DeleteWithCorrelatedSubqueryLog> Logs => Set<DeleteWithCorrelatedSubqueryLog>();
        public DbSet<DeleteWithCorrelatedSubquerySummary> Summaries => Set<DeleteWithCorrelatedSubquerySummary>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeleteWithCorrelatedSubqueryLog>().ToTable("Logs");
            modelBuilder.Entity<DeleteWithCorrelatedSubquerySummary>().ToTable("Summaries");
        }
    }

    private class DeleteWithCorrelatedSubqueryLog
    {
        public long Id { get; set; }
        public int GroupId { get; set; }
        public int Ref { get; set; }
        public string Note { get; set; }
    }

    private class DeleteWithCorrelatedSubquerySummary
    {
        public long Id { get; set; }
        public int GroupId { get; set; }
    }

    private void AssertSql(params string[] expected)
        => TestSqlLoggerFactory.AssertBaseline(expected);

    private void AssertExecuteUpdateSql(params string[] expected)
        => TestSqlLoggerFactory.AssertBaseline(expected, forUpdate: true);
}
