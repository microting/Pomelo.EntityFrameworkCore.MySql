using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestModels.GearsOfWarModel;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Tests;
using Pomelo.EntityFrameworkCore.MySql.Tests.TestUtilities.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query;

public class TPCGearsOfWarQueryMySqlTest : TPCGearsOfWarQueryRelationalTestBase<TPCGearsOfWarQueryMySqlFixture>
{
    public TPCGearsOfWarQueryMySqlTest(TPCGearsOfWarQueryMySqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
        //Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => MySqlTestHelpers.AssertAllMethodsOverridden(GetType());

    // TODO: Create a test for this
    public override async Task Coalesce_with_non_root_evaluatable_Convert(bool async)
    {
        await base.Coalesce_with_non_root_evaluatable_Convert(async);

        AssertSql(
            """
            @__rank_0='1' (Nullable = true)

            SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
            FROM (
                SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
                FROM `Gears` AS `g`
                UNION ALL
                SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
                FROM `Officers` AS `o`
            ) AS `u`
            WHERE @__rank_0 = `u`.`Rank`
            """);
    }

    // TODO: Create a test for this
    public override async Task Project_equality_with_value_converted_property(bool async)
    {
        await base.Project_equality_with_value_converted_property(async);

        AssertSql(
            """
            SELECT `m`.`Difficulty` = 'Unknown'
            FROM `Missions` AS `m`
            """);
    }

    public override async Task Entity_equality_empty(bool async)
    {
        await base.Entity_equality_empty(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE FALSE
""");
    }

    public override async Task Include_multiple_one_to_one_and_one_to_many(bool async)
    {
        await base.Include_multiple_one_to_one_and_one_to_many(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`, `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
ORDER BY `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Include_multiple_one_to_one_optional_and_one_to_one_required(bool async)
    {
        await base.Include_multiple_one_to_one_optional_and_one_to_one_required(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`, `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
LEFT JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
""");
    }

    public override async Task Include_multiple_circular(bool async)
    {
        await base.Include_multiple_circular(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `c`.`Name`, `c`.`Location`, `c`.`Nation`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Cities` AS `c` ON `u`.`CityOfBirthName` = `c`.`Name`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `c`.`Name` = `u0`.`AssignedCityName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `c`.`Name`, `u0`.`Nickname`
""");
    }

    public override async Task Include_multiple_circular_with_filter(bool async)
    {
        await base.Include_multiple_circular_with_filter(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `c`.`Name`, `c`.`Location`, `c`.`Nation`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Cities` AS `c` ON `u`.`CityOfBirthName` = `c`.`Name`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `c`.`Name` = `u0`.`AssignedCityName`
WHERE `u`.`Nickname` = 'Marcus'
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `c`.`Name`, `u0`.`Nickname`
""");
    }

    public override async Task Include_using_alternate_key(bool async)
    {
        await base.Include_using_alternate_key(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
WHERE `u`.`Nickname` = 'Marcus'
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Include_navigation_on_derived_type(bool async)
    {
        await base.Include_navigation_on_derived_type(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task String_based_Include_navigation_on_derived_type(bool async)
    {
        await base.String_based_Include_navigation_on_derived_type(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task Select_Where_Navigation_Included(bool async)
    {
        await base.Select_Where_Navigation_Included(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`, `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE `u`.`Nickname` = 'Marcus'
""");
    }

    public override async Task Include_with_join_reference1(bool async)
    {
        await base.Include_with_join_reference1(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Tags` AS `t` ON (`u`.`SquadId` = `t`.`GearSquadId`) AND (`u`.`Nickname` = `t`.`GearNickName`)
INNER JOIN `Cities` AS `c` ON `u`.`CityOfBirthName` = `c`.`Name`
""");
    }

    public override async Task Include_with_join_reference2(bool async)
    {
        await base.Include_with_join_reference2(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM `Tags` AS `t`
INNER JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearSquadId` = `u`.`SquadId`) AND (`t`.`GearNickName` = `u`.`Nickname`)
INNER JOIN `Cities` AS `c` ON `u`.`CityOfBirthName` = `c`.`Name`
""");
    }

    public override async Task Include_with_join_collection1(bool async)
    {
        await base.Include_with_join_collection1(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `t`.`Id`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Tags` AS `t` ON (`u`.`SquadId` = `t`.`GearSquadId`) AND (`u`.`Nickname` = `t`.`GearNickName`)
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `t`.`Id`
""");
    }

    public override async Task Include_with_join_collection2(bool async)
    {
        await base.Include_with_join_collection2(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `t`.`Id`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Tags` AS `t`
INNER JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearSquadId` = `u`.`SquadId`) AND (`t`.`GearNickName` = `u`.`Nickname`)
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
ORDER BY `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Include_where_list_contains_navigation(bool async)
    {
        await base.Include_where_list_contains_navigation(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `t`.`Id`
FROM `Tags` AS `t`
""",
                //
                """
SELECT `t`.`Nickname`, `t`.`SquadId`, `t`.`AssignedCityName`, `t`.`CityOfBirthName`, `t`.`FullName`, `t`.`HasSoulPatch`, `t`.`LeaderNickname`, `t`.`LeaderSquadId`, `t`.`Rank`, `t`.`Discriminator`, `t0`.`Id`, `t0`.`GearNickName`, `t0`.`GearSquadId`, `t0`.`IssueDate`, `t0`.`Note`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `t`
LEFT JOIN `Tags` AS `t0` ON (`t`.`Nickname` = `t0`.`GearNickName`) AND (`t`.`SquadId` = `t0`.`GearSquadId`)
WHERE `t0`.`Id` IS NOT NULL AND EXISTS (
    SELECT 1
    FROM JSON_TABLE('["b39a6fba-9026-4d69-828e-fd7068673e57","70534e05-782c-4052-8720-c2c54481ce5f","a8ad98f9-e023-4e2a-9a70-c2728455bd34","df36f493-463f-4123-83f9-6b135deeb7ba","34c8d86e-a4ac-4be5-827f-584dda348a07","a7be028a-0cf2-448f-ab55-ce8bc5d8cf69"]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` char(36) PATH '$[0]'
    )) AS `t1`
    WHERE (`t1`.`value` = `t0`.`Id`) OR (`t1`.`value` IS NULL AND (`t0`.`Id` IS NULL)))
""");
        }
        else
        {
        AssertSql(
"""
SELECT `t`.`Id`
FROM `Tags` AS `t`
""",
                //
                """
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
WHERE `t`.`Id` IS NOT NULL AND `t`.`Id` IN ('b39a6fba-9026-4d69-828e-fd7068673e57', '70534e05-782c-4052-8720-c2c54481ce5f', 'a8ad98f9-e023-4e2a-9a70-c2728455bd34', 'df36f493-463f-4123-83f9-6b135deeb7ba', '34c8d86e-a4ac-4be5-827f-584dda348a07', 'a7be028a-0cf2-448f-ab55-ce8bc5d8cf69')
""");
        }
    }

    public override async Task Include_where_list_contains_navigation2(bool async)
    {
        await base.Include_where_list_contains_navigation2(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `t`.`Id`
FROM `Tags` AS `t`
""",
                //
                """
SELECT `t`.`Nickname`, `t`.`SquadId`, `t`.`AssignedCityName`, `t`.`CityOfBirthName`, `t`.`FullName`, `t`.`HasSoulPatch`, `t`.`LeaderNickname`, `t`.`LeaderSquadId`, `t`.`Rank`, `t`.`Discriminator`, `t0`.`Id`, `t0`.`GearNickName`, `t0`.`GearSquadId`, `t0`.`IssueDate`, `t0`.`Note`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `t`
INNER JOIN `Cities` AS `c` ON `t`.`CityOfBirthName` = `c`.`Name`
LEFT JOIN `Tags` AS `t0` ON (`t`.`Nickname` = `t0`.`GearNickName`) AND (`t`.`SquadId` = `t0`.`GearSquadId`)
WHERE `c`.`Location` IS NOT NULL AND EXISTS (
    SELECT 1
    FROM JSON_TABLE('["b39a6fba-9026-4d69-828e-fd7068673e57","70534e05-782c-4052-8720-c2c54481ce5f","a8ad98f9-e023-4e2a-9a70-c2728455bd34","df36f493-463f-4123-83f9-6b135deeb7ba","34c8d86e-a4ac-4be5-827f-584dda348a07","a7be028a-0cf2-448f-ab55-ce8bc5d8cf69"]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` char(36) PATH '$[0]'
    )) AS `t1`
    WHERE (`t1`.`value` = `t0`.`Id`) OR (`t1`.`value` IS NULL AND (`t0`.`Id` IS NULL)))
""");
        }
        else
        {
        AssertSql(
"""
SELECT `t`.`Id`
FROM `Tags` AS `t`
""",
                //
                """
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Cities` AS `c` ON `u`.`CityOfBirthName` = `c`.`Name`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
WHERE `c`.`Location` IS NOT NULL AND `t`.`Id` IN ('b39a6fba-9026-4d69-828e-fd7068673e57', '70534e05-782c-4052-8720-c2c54481ce5f', 'a8ad98f9-e023-4e2a-9a70-c2728455bd34', 'df36f493-463f-4123-83f9-6b135deeb7ba', '34c8d86e-a4ac-4be5-827f-584dda348a07', 'a7be028a-0cf2-448f-ab55-ce8bc5d8cf69')
""");
        }
    }

    public override async Task Navigation_accessed_twice_outside_and_inside_subquery(bool async)
    {
        await base.Navigation_accessed_twice_outside_and_inside_subquery(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `t`.`Id`
FROM `Tags` AS `t`
""",
                //
                """
SELECT `t`.`Nickname`, `t`.`SquadId`, `t`.`AssignedCityName`, `t`.`CityOfBirthName`, `t`.`FullName`, `t`.`HasSoulPatch`, `t`.`LeaderNickname`, `t`.`LeaderSquadId`, `t`.`Rank`, `t`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `t`
LEFT JOIN `Tags` AS `t0` ON (`t`.`Nickname` = `t0`.`GearNickName`) AND (`t`.`SquadId` = `t0`.`GearSquadId`)
WHERE `t0`.`Id` IS NOT NULL AND EXISTS (
    SELECT 1
    FROM JSON_TABLE('["b39a6fba-9026-4d69-828e-fd7068673e57","70534e05-782c-4052-8720-c2c54481ce5f","a8ad98f9-e023-4e2a-9a70-c2728455bd34","df36f493-463f-4123-83f9-6b135deeb7ba","34c8d86e-a4ac-4be5-827f-584dda348a07","a7be028a-0cf2-448f-ab55-ce8bc5d8cf69"]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` char(36) PATH '$[0]'
    )) AS `t1`
    WHERE (`t1`.`value` = `t0`.`Id`) OR (`t1`.`value` IS NULL AND (`t0`.`Id` IS NULL)))
""");
        }
        else
        {
        AssertSql(
"""
SELECT `t`.`Id`
FROM `Tags` AS `t`
""",
                //
                """
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
WHERE `t`.`Id` IS NOT NULL AND `t`.`Id` IN ('b39a6fba-9026-4d69-828e-fd7068673e57', '70534e05-782c-4052-8720-c2c54481ce5f', 'a8ad98f9-e023-4e2a-9a70-c2728455bd34', 'df36f493-463f-4123-83f9-6b135deeb7ba', '34c8d86e-a4ac-4be5-827f-584dda348a07', 'a7be028a-0cf2-448f-ab55-ce8bc5d8cf69')
""");
        }
    }

    public override async Task Include_with_join_multi_level(bool async)
    {
        await base.Include_with_join_multi_level(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `c`.`Name`, `c`.`Location`, `c`.`Nation`, `t`.`Id`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Tags` AS `t` ON (`u`.`SquadId` = `t`.`GearSquadId`) AND (`u`.`Nickname` = `t`.`GearNickName`)
INNER JOIN `Cities` AS `c` ON `u`.`CityOfBirthName` = `c`.`Name`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `c`.`Name` = `u0`.`AssignedCityName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `t`.`Id`, `c`.`Name`, `u0`.`Nickname`
""");
    }

    public override async Task Include_with_join_and_inheritance1(bool async)
    {
        await base.Include_with_join_and_inheritance1(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM `Tags` AS `t`
INNER JOIN (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearSquadId` = `u`.`SquadId`) AND (`t`.`GearNickName` = `u`.`Nickname`)
INNER JOIN `Cities` AS `c` ON `u`.`CityOfBirthName` = `c`.`Name`
""");
    }

    public override async Task Include_with_join_and_inheritance_with_orderby_before_and_after_include(bool async)
    {
        await base.Include_with_join_and_inheritance_with_orderby_before_and_after_include(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `t`.`Id`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM `Tags` AS `t`
INNER JOIN (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearSquadId` = `u`.`SquadId`) AND (`t`.`GearNickName` = `u`.`Nickname`)
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY `u`.`HasSoulPatch`, `u`.`Nickname` DESC, `t`.`Id`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task Include_with_join_and_inheritance2(bool async)
    {
        await base.Include_with_join_and_inheritance2(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `t`.`Id`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Tags` AS `t` ON (`u`.`SquadId` = `t`.`GearSquadId`) AND (`u`.`Nickname` = `t`.`GearNickName`)
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `t`.`Id`
""");
    }

    public override async Task Include_with_join_and_inheritance3(bool async)
    {
        await base.Include_with_join_and_inheritance3(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `t`.`Id`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM `Tags` AS `t`
INNER JOIN (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearSquadId` = `u`.`SquadId`) AND (`t`.`GearNickName` = `u`.`Nickname`)
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task Include_with_nested_navigation_in_order_by(bool async)
    {
        await base.Include_with_nested_navigation_in_order_by(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM `Weapons` AS `w`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON `w`.`OwnerFullName` = `u`.`FullName`
LEFT JOIN `Cities` AS `c` ON `u`.`CityOfBirthName` = `c`.`Name`
WHERE (`u`.`Nickname` <> 'Paduk') OR `u`.`Nickname` IS NULL
ORDER BY `c`.`Name`, `w`.`Id`
""");
    }

    public override async Task Where_enum(bool async)
    {
        await base.Where_enum(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`Rank` = 4
""");
    }

    public override async Task Where_nullable_enum_with_constant(bool async)
    {
        await base.Where_nullable_enum_with_constant(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE `w`.`AmmunitionType` = 1
""");
    }

    public override async Task Where_nullable_enum_with_null_constant(bool async)
    {
        await base.Where_nullable_enum_with_null_constant(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE `w`.`AmmunitionType` IS NULL
""");
    }

    public override async Task Where_nullable_enum_with_non_nullable_parameter(bool async)
    {
        await base.Where_nullable_enum_with_non_nullable_parameter(async);

        AssertSql(
"""
@__ammunitionType_0='1'

SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE `w`.`AmmunitionType` = @__ammunitionType_0
""");
    }

    public override async Task Where_nullable_enum_with_nullable_parameter(bool async)
    {
        await base.Where_nullable_enum_with_nullable_parameter(async);

        AssertSql(
"""
@__ammunitionType_0='1' (Nullable = true)

SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE `w`.`AmmunitionType` = @__ammunitionType_0
""",
                //
                """
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE `w`.`AmmunitionType` IS NULL
""");
    }

    public override async Task Where_bitwise_and_enum(bool async)
    {
        await base.Where_bitwise_and_enum(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(`u`.`Rank` & 2 AS signed) > 0
""",
                //
                """
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(`u`.`Rank` & 2 AS signed) = 2
""");
    }

    public override async Task Where_bitwise_and_integral(bool async)
    {
        await base.Where_bitwise_and_integral(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(`u`.`Rank` & 1 AS signed) = 1
""",
                //
                """
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(CAST(`u`.`Rank` AS signed) & 1 AS signed) = 1
""",
                //
                """
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(CAST(`u`.`Rank` AS signed) & 1 AS signed) = 1
""");
    }

    public override async Task Where_bitwise_and_nullable_enum_with_constant(bool async)
    {
        await base.Where_bitwise_and_nullable_enum_with_constant(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE CAST(`w`.`AmmunitionType` & 1 AS signed) > 0
""");
    }

    public override async Task Where_bitwise_and_nullable_enum_with_null_constant(bool async)
    {
        await base.Where_bitwise_and_nullable_enum_with_null_constant(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE CAST(`w`.`AmmunitionType` & NULL AS signed) > 0
""");
    }

    public override async Task Where_bitwise_and_nullable_enum_with_non_nullable_parameter(bool async)
    {
        await base.Where_bitwise_and_nullable_enum_with_non_nullable_parameter(async);

        AssertSql(
"""
@__ammunitionType_0='1'

SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE CAST(`w`.`AmmunitionType` & @__ammunitionType_0 AS signed) > 0
""");
    }

    public override async Task Where_bitwise_and_nullable_enum_with_nullable_parameter(bool async)
    {
        await base.Where_bitwise_and_nullable_enum_with_nullable_parameter(async);

        AssertSql(
"""
@__ammunitionType_0='1' (Nullable = true)

SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE CAST(`w`.`AmmunitionType` & @__ammunitionType_0 AS signed) > 0
""",
                //
                """
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE CAST(`w`.`AmmunitionType` & NULL AS signed) > 0
""");
    }

    public override async Task Where_bitwise_or_enum(bool async)
    {
        await base.Where_bitwise_or_enum(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(`u`.`Rank` | 2 AS signed) > 0
""");
    }

    public override async Task Bitwise_projects_values_in_select(bool async)
    {
        await base.Bitwise_projects_values_in_select(async);

        AssertSql(
"""
SELECT CAST(`u`.`Rank` & 2 AS signed) = 2 AS `BitwiseTrue`, CAST(`u`.`Rank` & 2 AS signed) = 4 AS `BitwiseFalse`, CAST(`u`.`Rank` & 2 AS signed) AS `BitwiseValue`
FROM (
    SELECT `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(`u`.`Rank` & 2 AS signed) = 2
LIMIT 1
""");
    }

    public override async Task Where_enum_has_flag(bool async)
    {
        await base.Where_enum_has_flag(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(`u`.`Rank` & 2 AS signed) = 2
""",
                //
                """
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(`u`.`Rank` & 18 AS signed) = 18
""",
                //
                """
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(`u`.`Rank` & 1 AS signed) = 1
""",
                //
                """
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(`u`.`Rank` & 1 AS signed) = 1
""",
                //
                """
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(2 & `u`.`Rank` AS signed) = `u`.`Rank`
""");
    }

    public override async Task Where_enum_has_flag_subquery(bool async)
    {
        await base.Where_enum_has_flag_subquery(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(`u`.`Rank` & COALESCE((
    SELECT `u0`.`Rank`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`Rank`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`Rank`
        FROM `Officers` AS `o0`
    ) AS `u0`
    ORDER BY `u0`.`Nickname`, `u0`.`SquadId`
    LIMIT 1), 0) AS signed) = COALESCE((
    SELECT `u0`.`Rank`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`Rank`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`Rank`
        FROM `Officers` AS `o0`
    ) AS `u0`
    ORDER BY `u0`.`Nickname`, `u0`.`SquadId`
    LIMIT 1), 0)
""",
                //
                """
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(2 & COALESCE((
    SELECT `u0`.`Rank`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`Rank`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`Rank`
        FROM `Officers` AS `o0`
    ) AS `u0`
    ORDER BY `u0`.`Nickname`, `u0`.`SquadId`
    LIMIT 1), 0) AS signed) = COALESCE((
    SELECT `u0`.`Rank`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`Rank`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`Rank`
        FROM `Officers` AS `o0`
    ) AS `u0`
    ORDER BY `u0`.`Nickname`, `u0`.`SquadId`
    LIMIT 1), 0)
""");
    }

    public override async Task Where_enum_has_flag_subquery_with_pushdown(bool async)
    {
        await base.Where_enum_has_flag_subquery_with_pushdown(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (CAST(`u`.`Rank` & (
    SELECT `u0`.`Rank`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`Rank`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`Rank`
        FROM `Officers` AS `o0`
    ) AS `u0`
    ORDER BY `u0`.`Nickname`, `u0`.`SquadId`
    LIMIT 1) AS signed) = (
    SELECT `u0`.`Rank`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`Rank`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`Rank`
        FROM `Officers` AS `o0`
    ) AS `u0`
    ORDER BY `u0`.`Nickname`, `u0`.`SquadId`
    LIMIT 1)) OR (
    SELECT `u0`.`Rank`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`Rank`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`Rank`
        FROM `Officers` AS `o0`
    ) AS `u0`
    ORDER BY `u0`.`Nickname`, `u0`.`SquadId`
    LIMIT 1) IS NULL
""",
                //
                """
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (CAST(2 & (
    SELECT `u0`.`Rank`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`Rank`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`Rank`
        FROM `Officers` AS `o0`
    ) AS `u0`
    ORDER BY `u0`.`Nickname`, `u0`.`SquadId`
    LIMIT 1) AS signed) = (
    SELECT `u0`.`Rank`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`Rank`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`Rank`
        FROM `Officers` AS `o0`
    ) AS `u0`
    ORDER BY `u0`.`Nickname`, `u0`.`SquadId`
    LIMIT 1)) OR (
    SELECT `u0`.`Rank`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`Rank`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`Rank`
        FROM `Officers` AS `o0`
    ) AS `u0`
    ORDER BY `u0`.`Nickname`, `u0`.`SquadId`
    LIMIT 1) IS NULL
""");
    }

    public override async Task Where_enum_has_flag_subquery_client_eval(bool async)
    {
        await base.Where_enum_has_flag_subquery_client_eval(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (CAST(`u`.`Rank` & (
    SELECT `u0`.`Rank`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`Rank`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`Rank`
        FROM `Officers` AS `o0`
    ) AS `u0`
    ORDER BY `u0`.`Nickname`, `u0`.`SquadId`
    LIMIT 1) AS signed) = (
    SELECT `u0`.`Rank`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`Rank`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`Rank`
        FROM `Officers` AS `o0`
    ) AS `u0`
    ORDER BY `u0`.`Nickname`, `u0`.`SquadId`
    LIMIT 1)) OR (
    SELECT `u0`.`Rank`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`Rank`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`Rank`
        FROM `Officers` AS `o0`
    ) AS `u0`
    ORDER BY `u0`.`Nickname`, `u0`.`SquadId`
    LIMIT 1) IS NULL
""");
    }

    public override async Task Where_enum_has_flag_with_non_nullable_parameter(bool async)
    {
        await base.Where_enum_has_flag_with_non_nullable_parameter(async);

        AssertSql(
"""
@__parameter_0='2'

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(`u`.`Rank` & @__parameter_0 AS signed) = @__parameter_0
""");
    }

    public override async Task Where_has_flag_with_nullable_parameter(bool async)
    {
        await base.Where_has_flag_with_nullable_parameter(async);

        AssertSql(
"""
@__parameter_0='2' (Nullable = true)

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(`u`.`Rank` & @__parameter_0 AS signed) = @__parameter_0
""");
    }

    public override async Task Select_enum_has_flag(bool async)
    {
        await base.Select_enum_has_flag(async);

        AssertSql(
"""
SELECT CAST(`u`.`Rank` & 2 AS signed) = 2 AS `hasFlagTrue`, CAST(`u`.`Rank` & 4 AS signed) = 4 AS `hasFlagFalse`
FROM (
    SELECT `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(`u`.`Rank` & 2 AS signed) = 2
LIMIT 1
""");
    }

    public override async Task Where_count_subquery_without_collision(bool async)
    {
        await base.Where_count_subquery_without_collision(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (
    SELECT COUNT(*)
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`) = 2
""");
    }

    public override async Task Where_any_subquery_without_collision(bool async)
    {
        await base.Where_any_subquery_without_collision(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE EXISTS (
    SELECT 1
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`)
""");
    }

    public override async Task Select_inverted_boolean(bool async)
    {
        await base.Select_inverted_boolean(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`IsAutomatic` = FALSE AS `Manual`
FROM `Weapons` AS `w`
WHERE `w`.`IsAutomatic` = TRUE
""");
    }

    public override async Task Select_comparison_with_null(bool async)
    {
        await base.Select_comparison_with_null(async);

        AssertSql(
"""
@__ammunitionType_0='1' (Nullable = true)

SELECT `w`.`Id`, (`w`.`AmmunitionType` = @__ammunitionType_0) AND `w`.`AmmunitionType` IS NOT NULL AS `Cartridge`
FROM `Weapons` AS `w`
WHERE `w`.`AmmunitionType` = @__ammunitionType_0
""",
                //
                """
SELECT `w`.`Id`, `w`.`AmmunitionType` IS NULL AS `Cartridge`
FROM `Weapons` AS `w`
WHERE `w`.`AmmunitionType` IS NULL
""");
    }

    public override async Task Select_null_parameter(bool async)
    {
        await base.Select_null_parameter(async);

        AssertSql(
"""
@__ammunitionType_0='1' (Nullable = true)

SELECT `w`.`Id`, @__ammunitionType_0 AS `AmmoType`
FROM `Weapons` AS `w`
""",
                //
                """
SELECT `w`.`Id`, NULL AS `AmmoType`
FROM `Weapons` AS `w`
""",
                //
                """
@__ammunitionType_0='2' (Nullable = true)

SELECT `w`.`Id`, @__ammunitionType_0 AS `AmmoType`
FROM `Weapons` AS `w`
""",
                //
                """
SELECT `w`.`Id`, NULL AS `AmmoType`
FROM `Weapons` AS `w`
""");
    }

    public override async Task Select_ternary_operation_with_boolean(bool async)
    {
        await base.Select_ternary_operation_with_boolean(async);

        AssertSql(
"""
SELECT `w`.`Id`, CASE
    WHEN `w`.`IsAutomatic` = TRUE THEN 1
    ELSE 0
END AS `Num`
FROM `Weapons` AS `w`
""");
    }

    public override async Task Select_ternary_operation_with_inverted_boolean(bool async)
    {
        await base.Select_ternary_operation_with_inverted_boolean(async);

        AssertSql(
"""
SELECT `w`.`Id`, CASE
    WHEN `w`.`IsAutomatic` = FALSE THEN 1
    ELSE 0
END AS `Num`
FROM `Weapons` AS `w`
""");
    }

    public override async Task Select_ternary_operation_with_has_value_not_null(bool async)
    {
        await base.Select_ternary_operation_with_has_value_not_null(async);

        AssertSql(
"""
SELECT `w`.`Id`, CASE
    WHEN `w`.`AmmunitionType` IS NOT NULL AND (`w`.`AmmunitionType` = 1) THEN 'Yes'
    ELSE 'No'
END AS `IsCartridge`
FROM `Weapons` AS `w`
WHERE `w`.`AmmunitionType` IS NOT NULL AND (`w`.`AmmunitionType` = 1)
""");
    }

    public override async Task Select_ternary_operation_multiple_conditions(bool async)
    {
        await base.Select_ternary_operation_multiple_conditions(async);

        AssertSql(
"""
SELECT `w`.`Id`, CASE
    WHEN (`w`.`AmmunitionType` = 2) AND (`w`.`SynergyWithId` = 1) THEN 'Yes'
    ELSE 'No'
END AS `IsCartridge`
FROM `Weapons` AS `w`
""");
    }

    public override async Task Select_ternary_operation_multiple_conditions_2(bool async)
    {
        await base.Select_ternary_operation_multiple_conditions_2(async);

        AssertSql(
"""
SELECT `w`.`Id`, CASE
    WHEN (`w`.`IsAutomatic` = FALSE) AND (`w`.`SynergyWithId` = 1) THEN 'Yes'
    ELSE 'No'
END AS `IsCartridge`
FROM `Weapons` AS `w`
""");
    }

    public override async Task Select_multiple_conditions(bool async)
    {
        await base.Select_multiple_conditions(async);

        AssertSql(
"""
SELECT `w`.`Id`, (`w`.`IsAutomatic` = FALSE) AND ((`w`.`SynergyWithId` = 1) AND `w`.`SynergyWithId` IS NOT NULL) AS `IsCartridge`
FROM `Weapons` AS `w`
""");
    }

    public override async Task Select_nested_ternary_operations(bool async)
    {
        await base.Select_nested_ternary_operations(async);

        AssertSql(
"""
SELECT `w`.`Id`, CASE
    WHEN `w`.`IsAutomatic` = FALSE THEN CASE
        WHEN `w`.`AmmunitionType` = 1 THEN 'ManualCartridge'
        ELSE 'Manual'
    END
    ELSE 'Auto'
END AS `IsManualCartridge`
FROM `Weapons` AS `w`
""");
    }

    public override async Task Null_propagation_optimization1(bool async)
    {
        await base.Null_propagation_optimization1(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`LeaderNickname` = 'Marcus'
""");
    }

    public override async Task Null_propagation_optimization2(bool async)
    {
        await base.Null_propagation_optimization2(async);

        // issue #16050
        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CASE
    WHEN `u`.`LeaderNickname` IS NULL THEN NULL
    ELSE `u`.`LeaderNickname` LIKE '%us'
END
""");
    }

    public override async Task Null_propagation_optimization3(bool async)
    {
        await base.Null_propagation_optimization3(async);

        // issue #16050
        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CASE
    WHEN `u`.`LeaderNickname` IS NOT NULL THEN `u`.`LeaderNickname` LIKE '%us'
END
""");
    }

    public override async Task Null_propagation_optimization4(bool async)
    {
        await base.Null_propagation_optimization4(async);

        // issue #16050
        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CASE
    WHEN `u`.`LeaderNickname` IS NULL THEN NULL
    ELSE CHAR_LENGTH(`u`.`LeaderNickname`)
END = 5
""");
    }

    public override async Task Null_propagation_optimization5(bool async)
    {
        await base.Null_propagation_optimization5(async);

        // issue #16050
        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CASE
    WHEN `u`.`LeaderNickname` IS NOT NULL THEN CHAR_LENGTH(`u`.`LeaderNickname`)
END = 5
""");
    }

    public override async Task Null_propagation_optimization6(bool async)
    {
        await base.Null_propagation_optimization6(async);

        // issue #16050
        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CASE
    WHEN `u`.`LeaderNickname` IS NOT NULL THEN CHAR_LENGTH(`u`.`LeaderNickname`)
END = 5
""");
    }

    public override async Task Select_null_propagation_optimization7(bool async)
    {
        await base.Select_null_propagation_optimization7(async);

        // issue #16050
        AssertSql(
"""
SELECT CASE
    WHEN `u`.`LeaderNickname` IS NOT NULL THEN CONCAT(`u`.`LeaderNickname`, `u`.`LeaderNickname`)
END
FROM (
    SELECT `g`.`LeaderNickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Select_null_propagation_optimization8(bool async)
    {
        await base.Select_null_propagation_optimization8(async);

        AssertSql(
"""
SELECT CONCAT(COALESCE(`u`.`LeaderNickname`, ''), COALESCE(`u`.`LeaderNickname`, ''))
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`LeaderNickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Select_null_propagation_optimization9(bool async)
    {
        await base.Select_null_propagation_optimization9(async);

        AssertSql(
"""
SELECT CHAR_LENGTH(`u`.`FullName`)
FROM (
    SELECT `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Select_null_propagation_negative1(bool async)
    {
        await base.Select_null_propagation_negative1(async);

        AssertSql(
"""
SELECT CASE
    WHEN `u`.`LeaderNickname` IS NOT NULL THEN CHAR_LENGTH(`u`.`Nickname`) = 5
END
FROM (
    SELECT `g`.`Nickname`, `g`.`LeaderNickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Select_null_propagation_negative2(bool async)
    {
        await base.Select_null_propagation_negative2(async);

        AssertSql(
"""
SELECT CASE
    WHEN `u`.`LeaderNickname` IS NOT NULL THEN `u0`.`LeaderNickname`
END
FROM (
    SELECT `g`.`LeaderNickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
CROSS JOIN (
    SELECT `g0`.`LeaderNickname`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`LeaderNickname`
    FROM `Officers` AS `o0`
) AS `u0`
""");
    }

    public override async Task Select_null_propagation_negative3(bool async)
    {
        await base.Select_null_propagation_negative3(async);

        AssertSql(
"""
SELECT `u0`.`Nickname`, CASE
    WHEN `u0`.`Nickname` IS NOT NULL AND (`u0`.`SquadId` IS NOT NULL) THEN `u0`.`LeaderNickname` IS NOT NULL
END AS `Condition`
FROM (
    SELECT `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`LeaderNickname`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`LeaderNickname`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`HasSoulPatch` = TRUE
ORDER BY `u0`.`Nickname`
""");
    }

    public override async Task Select_null_propagation_negative4(bool async)
    {
        await base.Select_null_propagation_negative4(async);

        AssertSql(
"""
SELECT `u0`.`Nickname` IS NOT NULL AND (`u0`.`SquadId` IS NOT NULL), `u0`.`Nickname`
FROM (
    SELECT `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`HasSoulPatch` = TRUE
ORDER BY `u0`.`Nickname`
""");
    }

    public override async Task Select_null_propagation_negative5(bool async)
    {
        await base.Select_null_propagation_negative5(async);

        AssertSql(
"""
SELECT `u0`.`Nickname` IS NOT NULL AND (`u0`.`SquadId` IS NOT NULL), `u0`.`Nickname`
FROM (
    SELECT `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`HasSoulPatch` = TRUE
ORDER BY `u0`.`Nickname`
""");
    }

    public override async Task Select_null_propagation_negative6(bool async)
    {
        await base.Select_null_propagation_negative6(async);

        AssertSql(
"""
SELECT CASE
    WHEN `u`.`LeaderNickname` IS NOT NULL THEN FALSE
END
FROM (
    SELECT `g`.`LeaderNickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Select_null_propagation_negative7(bool async)
    {
        await base.Select_null_propagation_negative7(async);

        AssertSql(
"""
SELECT CASE
    WHEN `u`.`LeaderNickname` IS NOT NULL THEN TRUE
END
FROM (
    SELECT `g`.`LeaderNickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Select_null_propagation_negative8(bool async)
    {
        await base.Select_null_propagation_negative8(async);

        AssertSql(
"""
SELECT CASE
    WHEN `s`.`Id` IS NOT NULL THEN `c`.`Name`
END
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
LEFT JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
LEFT JOIN `Cities` AS `c` ON `u`.`AssignedCityName` = `c`.`Name`
""");
    }

    public override async Task Select_null_propagation_negative9(bool async)
    {
        await base.Select_null_propagation_negative9(async);

        AssertSql(
"""
SELECT CASE
    WHEN `u`.`LeaderNickname` IS NOT NULL THEN CHAR_LENGTH(`u`.`Nickname`) = 5
END
FROM (
    SELECT `g`.`Nickname`, `g`.`LeaderNickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Select_null_propagation_works_for_navigations_with_composite_keys(bool async)
    {
        await base.Select_null_propagation_works_for_navigations_with_composite_keys(async);

        AssertSql(
"""
SELECT `u`.`Nickname`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
""");
    }

    public override async Task Select_null_propagation_works_for_multiple_navigations_with_composite_keys(bool async)
    {
        await base.Select_null_propagation_works_for_multiple_navigations_with_composite_keys(async);

        AssertSql(
"""
SELECT CASE
    WHEN `c`.`Name` IS NOT NULL THEN `c`.`Name`
END
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
LEFT JOIN `Tags` AS `t0` ON ((`u`.`Nickname` = `t0`.`GearNickName`) OR (`u`.`Nickname` IS NULL AND (`t0`.`GearNickName` IS NULL))) AND ((`u`.`SquadId` = `t0`.`GearSquadId`) OR (`u`.`SquadId` IS NULL AND (`t0`.`GearSquadId` IS NULL)))
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`
    FROM `Officers` AS `o0`
) AS `u0` ON (`t0`.`GearNickName` = `u0`.`Nickname`) AND (`t0`.`GearSquadId` = `u0`.`SquadId`)
LEFT JOIN `Cities` AS `c` ON `u0`.`AssignedCityName` = `c`.`Name`
""");
    }

    public override async Task Select_conditional_with_anonymous_type_and_null_constant(bool async)
    {
        await base.Select_conditional_with_anonymous_type_and_null_constant(async);

        AssertSql(
"""
SELECT `u`.`LeaderNickname` IS NOT NULL, `u`.`HasSoulPatch`
FROM (
    SELECT `g`.`Nickname`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
ORDER BY `u`.`Nickname`
""");
    }

    public override async Task Select_conditional_with_anonymous_types(bool async)
    {
        await base.Select_conditional_with_anonymous_types(async);

        AssertSql(
"""
SELECT `u`.`LeaderNickname` IS NOT NULL, `u`.`Nickname`, `u`.`FullName`
FROM (
    SELECT `g`.`Nickname`, `g`.`FullName`, `g`.`LeaderNickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`FullName`, `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
ORDER BY `u`.`Nickname`
""");
    }

    public override async Task Where_conditional_equality_1(bool async)
    {
        await base.Where_conditional_equality_1(async);

        AssertSql(
"""
SELECT `u`.`Nickname`
FROM (
    SELECT `g`.`Nickname`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`LeaderNickname` IS NULL
ORDER BY `u`.`Nickname`
""");
    }

    public override async Task Where_conditional_equality_2(bool async)
    {
        await base.Where_conditional_equality_2(async);

        AssertSql(
"""
SELECT `u`.`Nickname`
FROM (
    SELECT `g`.`Nickname`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`LeaderNickname` IS NULL
ORDER BY `u`.`Nickname`
""");
    }

    public override async Task Where_conditional_equality_3(bool async)
    {
        await base.Where_conditional_equality_3(async);

        AssertSql(
"""
SELECT `u`.`Nickname`
FROM (
    SELECT `g`.`Nickname`, `g`.`LeaderNickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
ORDER BY `u`.`Nickname`
""");
    }

    public override async Task Select_coalesce_with_anonymous_types(bool async)
    {
        await base.Select_coalesce_with_anonymous_types(async);

        AssertSql(
"""
SELECT `u`.`LeaderNickname`, `u`.`FullName`
FROM (
    SELECT `g`.`Nickname`, `g`.`FullName`, `g`.`LeaderNickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`FullName`, `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
ORDER BY `u`.`Nickname`
""");
    }

    public override async Task Where_compare_anonymous_types(bool async)
    {
        await base.Where_compare_anonymous_types(async);

        AssertSql();
    }

    public override async Task Where_member_access_on_anonymous_type(bool async)
    {
        await base.Where_member_access_on_anonymous_type(async);

        AssertSql(
"""
SELECT `u`.`Nickname`
FROM (
    SELECT `g`.`Nickname`, `g`.`LeaderNickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`LeaderNickname` = 'Marcus'
""");
    }

    public override async Task Where_compare_anonymous_types_with_uncorrelated_members(bool async)
    {
        await base.Where_compare_anonymous_types_with_uncorrelated_members(async);
        AssertSql(
"""
SELECT `u`.`Nickname`
FROM (
    SELECT `g`.`Nickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`
    FROM `Officers` AS `o`
) AS `u`
WHERE FALSE
""");
    }

    public override async Task Select_Where_Navigation_Scalar_Equals_Navigation_Scalar(bool async)
    {
        await base.Select_Where_Navigation_Scalar_Equals_Navigation_Scalar(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`, `t0`.`Id`, `t0`.`GearNickName`, `t0`.`GearSquadId`, `t0`.`IssueDate`, `t0`.`Note`
FROM `Tags` AS `t`
CROSS JOIN `Tags` AS `t0`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`
    FROM `Officers` AS `o0`
) AS `u0` ON (`t0`.`GearNickName` = `u0`.`Nickname`) AND (`t0`.`GearSquadId` = `u0`.`SquadId`)
WHERE (`u`.`Nickname` = `u0`.`Nickname`) OR (`u`.`Nickname` IS NULL AND (`u0`.`Nickname` IS NULL))
""");
    }

    public override async Task Select_Singleton_Navigation_With_Member_Access(bool async)
    {
        await base.Select_Singleton_Navigation_With_Member_Access(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE (`u`.`Nickname` = 'Marcus') AND ((`u`.`CityOfBirthName` <> 'Ephyra') OR `u`.`CityOfBirthName` IS NULL)
""");
    }

    public override async Task Select_Where_Navigation(bool async)
    {
        await base.Select_Where_Navigation(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE `u`.`Nickname` = 'Marcus'
""");
    }

    public override async Task Select_Where_Navigation_Equals_Navigation(bool async)
    {
        await base.Select_Where_Navigation_Equals_Navigation(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`, `t0`.`Id`, `t0`.`GearNickName`, `t0`.`GearSquadId`, `t0`.`IssueDate`, `t0`.`Note`
FROM `Tags` AS `t`
CROSS JOIN `Tags` AS `t0`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`
    FROM `Officers` AS `o0`
) AS `u0` ON (`t0`.`GearNickName` = `u0`.`Nickname`) AND (`t0`.`GearSquadId` = `u0`.`SquadId`)
WHERE ((`u`.`Nickname` = `u0`.`Nickname`) OR (`u`.`Nickname` IS NULL AND (`u0`.`Nickname` IS NULL))) AND ((`u`.`SquadId` = `u0`.`SquadId`) OR (`u`.`SquadId` IS NULL AND (`u0`.`SquadId` IS NULL)))
""");
    }

    public override async Task Select_Where_Navigation_Null(bool async)
    {
        await base.Select_Where_Navigation_Null(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE `u`.`Nickname` IS NULL OR (`u`.`SquadId` IS NULL)
""");
    }

    public override async Task Select_Where_Navigation_Null_Reverse(bool async)
    {
        await base.Select_Where_Navigation_Null_Reverse(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE `u`.`Nickname` IS NULL OR (`u`.`SquadId` IS NULL)
""");
    }

    public override async Task Select_Where_Navigation_Scalar_Equals_Navigation_Scalar_Projected(bool async)
    {
        await base.Select_Where_Navigation_Scalar_Equals_Navigation_Scalar_Projected(async);

        AssertSql(
"""
SELECT `t`.`Id` AS `Id1`, `t0`.`Id` AS `Id2`
FROM `Tags` AS `t`
CROSS JOIN `Tags` AS `t0`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`
    FROM `Officers` AS `o0`
) AS `u0` ON (`t0`.`GearNickName` = `u0`.`Nickname`) AND (`t0`.`GearSquadId` = `u0`.`SquadId`)
WHERE (`u`.`Nickname` = `u0`.`Nickname`) OR (`u`.`Nickname` IS NULL AND (`u0`.`Nickname` IS NULL))
""");
    }

    public override async Task Optional_Navigation_Null_Coalesce_To_Clr_Type(bool async)
    {
        await base.Optional_Navigation_Null_Coalesce_To_Clr_Type(async);

        AssertSql(
"""
SELECT COALESCE(`w0`.`IsAutomatic`, FALSE) AS `IsAutomatic`
FROM `Weapons` AS `w`
LEFT JOIN `Weapons` AS `w0` ON `w`.`SynergyWithId` = `w0`.`Id`
ORDER BY `w`.`Id`
LIMIT 1
""");
    }

    public override async Task Where_subquery_boolean(bool async)
    {
        await base.Where_subquery_boolean(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE COALESCE((
    SELECT `w`.`IsAutomatic`
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ORDER BY `w`.`Id`
    LIMIT 1), FALSE)
""");
    }

    public override async Task Where_subquery_boolean_with_pushdown(bool async)
    {
        await base.Where_subquery_boolean_with_pushdown(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (
    SELECT `w`.`IsAutomatic`
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ORDER BY `w`.`Id`
    LIMIT 1)
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Where_subquery_distinct_firstordefault_boolean(bool async)
    {
        await base.Where_subquery_distinct_firstordefault_boolean(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (`u`.`HasSoulPatch` = TRUE) AND COALESCE((
    SELECT `w0`.`IsAutomatic`
    FROM (
        SELECT DISTINCT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ) AS `w0`
    ORDER BY `w0`.`Id`
    LIMIT 1), FALSE)
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Where_subquery_distinct_firstordefault_boolean_with_pushdown(bool async)
    {
        await base.Where_subquery_distinct_firstordefault_boolean_with_pushdown(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (`u`.`HasSoulPatch` = TRUE) AND (
    SELECT `w0`.`IsAutomatic`
    FROM (
        SELECT DISTINCT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ) AS `w0`
    ORDER BY `w0`.`Id`
    LIMIT 1)
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Where_subquery_distinct_first_boolean(bool async)
    {
        await base.Where_subquery_distinct_first_boolean(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (`u`.`HasSoulPatch` = TRUE) AND (
    SELECT `w0`.`IsAutomatic`
    FROM (
        SELECT DISTINCT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ) AS `w0`
    ORDER BY `w0`.`Id`
    LIMIT 1)
ORDER BY `u`.`Nickname`
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Where_subquery_distinct_singleordefault_boolean1(bool async)
    {
        await base.Where_subquery_distinct_singleordefault_boolean1(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (`u`.`HasSoulPatch` = TRUE) AND COALESCE((
    SELECT `w0`.`IsAutomatic`
    FROM (
        SELECT DISTINCT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE (`u`.`FullName` = `w`.`OwnerFullName`) AND (`w`.`Name` LIKE '%Lancer%')
    ) AS `w0`
    LIMIT 1), FALSE)
ORDER BY `u`.`Nickname`
""");
    }

    public override async Task Where_subquery_distinct_singleordefault_boolean2(bool async)
    {
        await base.Where_subquery_distinct_singleordefault_boolean2(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (`u`.`HasSoulPatch` = TRUE) AND COALESCE((
    SELECT DISTINCT `w`.`IsAutomatic`
    FROM `Weapons` AS `w`
    WHERE (`u`.`FullName` = `w`.`OwnerFullName`) AND (`w`.`Name` LIKE '%Lancer%')
    LIMIT 1), FALSE)
ORDER BY `u`.`Nickname`
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Where_subquery_distinct_singleordefault_boolean_with_pushdown(bool async)
    {
        await base.Where_subquery_distinct_singleordefault_boolean_with_pushdown(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (`u`.`HasSoulPatch` = TRUE) AND (
    SELECT `w0`.`IsAutomatic`
    FROM (
        SELECT DISTINCT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE (`u`.`FullName` = `w`.`OwnerFullName`) AND (`w`.`Name` LIKE '%Lancer%')
    ) AS `w0`
    LIMIT 1)
ORDER BY `u`.`Nickname`
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Where_subquery_distinct_lastordefault_boolean(bool async)
    {
        await base.Where_subquery_distinct_lastordefault_boolean(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE NOT ((
    SELECT `w0`.`IsAutomatic`
    FROM (
        SELECT DISTINCT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ) AS `w0`
    ORDER BY `w0`.`Id` DESC
    LIMIT 1))
ORDER BY `u`.`Nickname`
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Where_subquery_distinct_last_boolean(bool async)
    {
        await base.Where_subquery_distinct_last_boolean(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (`u`.`HasSoulPatch` = FALSE) AND (
    SELECT `w0`.`IsAutomatic`
    FROM (
        SELECT DISTINCT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ) AS `w0`
    ORDER BY `w0`.`Id` DESC
    LIMIT 1)
ORDER BY `u`.`Nickname`
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Where_subquery_distinct_orderby_firstordefault_boolean(bool async)
    {
        await base.Where_subquery_distinct_orderby_firstordefault_boolean(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (`u`.`HasSoulPatch` = TRUE) AND COALESCE((
    SELECT `w0`.`IsAutomatic`
    FROM (
        SELECT DISTINCT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ) AS `w0`
    ORDER BY `w0`.`Id`
    LIMIT 1), FALSE)
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Where_subquery_distinct_orderby_firstordefault_boolean_with_pushdown(bool async)
    {
        await base.Where_subquery_distinct_orderby_firstordefault_boolean_with_pushdown(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (`u`.`HasSoulPatch` = TRUE) AND (
    SELECT `w0`.`IsAutomatic`
    FROM (
        SELECT DISTINCT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ) AS `w0`
    ORDER BY `w0`.`Id`
    LIMIT 1)
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Where_subquery_union_firstordefault_boolean(bool async)
    {
        await base.Where_subquery_union_firstordefault_boolean(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (`u`.`HasSoulPatch` = TRUE) AND (
    SELECT `u0`.`IsAutomatic`
    FROM (
        SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE `u`.`FullName` = `w`.`OwnerFullName`
        UNION
        SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
        FROM `Weapons` AS `w0`
        WHERE `u`.`FullName` = `w0`.`OwnerFullName`
    ) AS `u0`
    ORDER BY `u0`.`Id`
    LIMIT 1)
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Where_subquery_join_firstordefault_boolean(bool async)
    {
        await base.Where_subquery_join_firstordefault_boolean(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (`u`.`HasSoulPatch` = TRUE) AND (
    SELECT `w`.`IsAutomatic`
    FROM `Weapons` AS `w`
    INNER JOIN (
        SELECT `w0`.`Id`
        FROM `Weapons` AS `w0`
        WHERE `u`.`FullName` = `w0`.`OwnerFullName`
    ) AS `w1` ON `w`.`Id` = `w1`.`Id`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ORDER BY `w`.`Id`
    LIMIT 1)
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Where_subquery_left_join_firstordefault_boolean(bool async)
    {
        await base.Where_subquery_left_join_firstordefault_boolean(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (`u`.`HasSoulPatch` = TRUE) AND (
    SELECT `w`.`IsAutomatic`
    FROM `Weapons` AS `w`
    LEFT JOIN (
        SELECT `w0`.`Id`
        FROM `Weapons` AS `w0`
        WHERE `u`.`FullName` = `w0`.`OwnerFullName`
    ) AS `w1` ON `w`.`Id` = `w1`.`Id`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ORDER BY `w`.`Id`
    LIMIT 1)
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Where_subquery_concat_firstordefault_boolean(bool async)
    {
        await base.Where_subquery_concat_firstordefault_boolean(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (`u`.`HasSoulPatch` = TRUE) AND (
    SELECT `u0`.`IsAutomatic`
    FROM (
        SELECT `w`.`Id`, `w`.`IsAutomatic`
        FROM `Weapons` AS `w`
        WHERE `u`.`FullName` = `w`.`OwnerFullName`
        UNION ALL
        SELECT `w0`.`Id`, `w0`.`IsAutomatic`
        FROM `Weapons` AS `w0`
        WHERE `u`.`FullName` = `w0`.`OwnerFullName`
    ) AS `u0`
    ORDER BY `u0`.`Id`
    LIMIT 1)
""");
    }

    public override async Task Concat_with_count(bool async)
    {
        await base.Concat_with_count(async);

        AssertSql(
"""
SELECT COUNT(*)
FROM (
    SELECT 1
    FROM `Gears` AS `g`
    UNION ALL
    SELECT 1
    FROM `Officers` AS `o`
    UNION ALL
    SELECT 1
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT 1
    FROM `Officers` AS `o0`
) AS `u1`
""");
    }

    public override async Task Concat_scalars_with_count(bool async)
    {
        await base.Concat_scalars_with_count(async);

        AssertSql(
"""
SELECT COUNT(*)
FROM (
    SELECT 1
    FROM `Gears` AS `g`
    UNION ALL
    SELECT 1
    FROM `Officers` AS `o`
    UNION ALL
    SELECT 1
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT 1
    FROM `Officers` AS `o0`
) AS `u1`
""");
    }

    public override async Task Concat_anonymous_with_count(bool async)
    {
        await base.Concat_anonymous_with_count(async);

        AssertSql(
"""
SELECT COUNT(*)
FROM (
    SELECT 1
    FROM (
        SELECT 1
        FROM `Gears` AS `g`
        UNION ALL
        SELECT 1
        FROM `Officers` AS `o`
    ) AS `u`
    UNION ALL
    SELECT 1
    FROM (
        SELECT 1
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT 1
        FROM `Officers` AS `o0`
    ) AS `u0`
) AS `u1`
""");
    }

    public override async Task Concat_with_scalar_projection(bool async)
    {
        await base.Concat_with_scalar_projection(async);

        AssertSql(
"""
SELECT `g`.`Nickname`
FROM `Gears` AS `g`
UNION ALL
SELECT `o`.`Nickname`
FROM `Officers` AS `o`
UNION ALL
SELECT `g0`.`Nickname`
FROM `Gears` AS `g0`
UNION ALL
SELECT `o0`.`Nickname`
FROM `Officers` AS `o0`
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Select_navigation_with_concat_and_count(bool async)
    {
        await base.Select_navigation_with_concat_and_count(async);

        AssertSql(
"""
SELECT (
    SELECT COUNT(*)
    FROM (
        SELECT 1
        FROM `Weapons` AS `w`
        WHERE `u`.`FullName` = `w`.`OwnerFullName`
        UNION ALL
        SELECT 1
        FROM `Weapons` AS `w0`
        WHERE `u`.`FullName` = `w0`.`OwnerFullName`
    ) AS `u0`)
FROM (
    SELECT `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`HasSoulPatch` = FALSE
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Concat_with_collection_navigations(bool async)
    {
        await base.Concat_with_collection_navigations(async);

        AssertSql(
"""
SELECT (
    SELECT COUNT(*)
    FROM (
        SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE `u`.`FullName` = `w`.`OwnerFullName`
        UNION
        SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
        FROM `Weapons` AS `w0`
        WHERE `u`.`FullName` = `w0`.`OwnerFullName`
    ) AS `u0`)
FROM (
    SELECT `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`HasSoulPatch` = TRUE
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Union_with_collection_navigations(bool async)
    {
        await base.Union_with_collection_navigations(async);

        AssertSql(
"""
SELECT (
    SELECT COUNT(*)
    FROM (
        SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
        FROM (
            SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
            FROM `Gears` AS `g`
            UNION ALL
            SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
            FROM `Officers` AS `o0`
        ) AS `u0`
        WHERE (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
        UNION
        SELECT `u1`.`Nickname`, `u1`.`SquadId`, `u1`.`AssignedCityName`, `u1`.`CityOfBirthName`, `u1`.`FullName`, `u1`.`HasSoulPatch`, `u1`.`LeaderNickname`, `u1`.`LeaderSquadId`, `u1`.`Rank`, `u1`.`Discriminator`
        FROM (
            SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
            FROM `Gears` AS `g0`
            UNION ALL
            SELECT `o1`.`Nickname`, `o1`.`SquadId`, `o1`.`AssignedCityName`, `o1`.`CityOfBirthName`, `o1`.`FullName`, `o1`.`HasSoulPatch`, `o1`.`LeaderNickname`, `o1`.`LeaderSquadId`, `o1`.`Rank`, 'Officer' AS `Discriminator`
            FROM `Officers` AS `o1`
        ) AS `u1`
        WHERE (`u`.`Nickname` = `u1`.`LeaderNickname`) AND (`u`.`SquadId` = `u1`.`LeaderSquadId`)
    ) AS `u2`)
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Select_subquery_distinct_firstordefault(bool async)
    {
        await base.Select_subquery_distinct_firstordefault(async);

        AssertSql(
"""
SELECT (
    SELECT `w0`.`Name`
    FROM (
        SELECT DISTINCT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ) AS `w0`
    ORDER BY `w0`.`Id`
    LIMIT 1)
FROM (
    SELECT `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`HasSoulPatch` = TRUE
""");
    }

    public override async Task Singleton_Navigation_With_Member_Access(bool async)
    {
        await base.Singleton_Navigation_With_Member_Access(async);

        AssertSql(
"""
SELECT `u`.`CityOfBirthName` AS `B`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`CityOfBirthName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`CityOfBirthName`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE (`u`.`Nickname` = 'Marcus') AND ((`u`.`CityOfBirthName` <> 'Ephyra') OR `u`.`CityOfBirthName` IS NULL)
""");
    }

    public override async Task GroupJoin_Composite_Key(bool async)
    {
        await base.GroupJoin_Composite_Key(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM `Tags` AS `t`
INNER JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
""");
    }

    public override async Task Join_navigation_translated_to_subquery_composite_key(bool async)
    {
        await base.Join_navigation_translated_to_subquery_composite_key(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `s`.`Note`
FROM (
    SELECT `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN (
    SELECT `t`.`Note`, `u0`.`FullName`
    FROM `Tags` AS `t`
    LEFT JOIN (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`FullName`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`
        FROM `Officers` AS `o0`
    ) AS `u0` ON (`t`.`GearNickName` = `u0`.`Nickname`) AND (`t`.`GearSquadId` = `u0`.`SquadId`)
) AS `s` ON `u`.`FullName` = `s`.`FullName`
""");
    }

    public override async Task Join_with_order_by_on_inner_sequence_navigation_translated_to_subquery_composite_key(bool async)
    {
        await base.Join_with_order_by_on_inner_sequence_navigation_translated_to_subquery_composite_key(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `s`.`Note`
FROM (
    SELECT `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN (
    SELECT `t`.`Note`, `u0`.`FullName`
    FROM `Tags` AS `t`
    LEFT JOIN (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`FullName`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`
        FROM `Officers` AS `o0`
    ) AS `u0` ON (`t`.`GearNickName` = `u0`.`Nickname`) AND (`t`.`GearSquadId` = `u0`.`SquadId`)
) AS `s` ON `u`.`FullName` = `s`.`FullName`
""");
    }

    public override async Task Join_with_order_by_without_skip_or_take(bool async)
    {
        await base.Join_with_order_by_without_skip_or_take(async);

        AssertSql(
"""
SELECT `w`.`Name`, `u`.`FullName`
FROM (
    SELECT `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
""");
    }

    public override async Task Join_with_order_by_without_skip_or_take_nested(bool async)
    {
        await base.Join_with_order_by_without_skip_or_take_nested(async);

        AssertSql(
"""
SELECT `w`.`Name`, `u`.`FullName`
FROM `Squads` AS `s`
INNER JOIN (
    SELECT `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u` ON `s`.`Id` = `u`.`SquadId`
INNER JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
""");
    }

    public override async Task Collection_with_inheritance_and_join_include_joined(bool async)
    {
        await base.Collection_with_inheritance_and_join_include_joined(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `t0`.`Id`, `t0`.`GearNickName`, `t0`.`GearSquadId`, `t0`.`IssueDate`, `t0`.`Note`
FROM `Tags` AS `t`
INNER JOIN (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearSquadId` = `u`.`SquadId`) AND (`t`.`GearNickName` = `u`.`Nickname`)
LEFT JOIN `Tags` AS `t0` ON (`u`.`Nickname` = `t0`.`GearNickName`) AND (`u`.`SquadId` = `t0`.`GearSquadId`)
""");
    }

    public override async Task Collection_with_inheritance_and_join_include_source(bool async)
    {
        await base.Collection_with_inheritance_and_join_include_source(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `t0`.`Id`, `t0`.`GearNickName`, `t0`.`GearSquadId`, `t0`.`IssueDate`, `t0`.`Note`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Tags` AS `t` ON (`u`.`SquadId` = `t`.`GearSquadId`) AND (`u`.`Nickname` = `t`.`GearNickName`)
LEFT JOIN `Tags` AS `t0` ON (`u`.`Nickname` = `t0`.`GearNickName`) AND (`u`.`SquadId` = `t0`.`GearSquadId`)
""");
    }

    public override async Task Non_unicode_string_literal_is_used_for_non_unicode_column(bool async)
    {
        await base.Non_unicode_string_literal_is_used_for_non_unicode_column(async);

        AssertSql(
"""
SELECT `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM `Cities` AS `c`
WHERE `c`.`Location` = 'Unknown'
""");
    }

    public override async Task Non_unicode_string_literal_is_used_for_non_unicode_column_right(bool async)
    {
        await base.Non_unicode_string_literal_is_used_for_non_unicode_column_right(async);

        AssertSql(
"""
SELECT `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM `Cities` AS `c`
WHERE 'Unknown' = `c`.`Location`
""");
    }

    public override async Task Non_unicode_parameter_is_used_for_non_unicode_column(bool async)
    {
        await base.Non_unicode_parameter_is_used_for_non_unicode_column(async);

        AssertSql(
"""
@__value_0='Unknown' (Size = 4000)

SELECT `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM `Cities` AS `c`
WHERE `c`.`Location` = @__value_0
""");
    }

    public override async Task Non_unicode_string_literals_in_contains_is_used_for_non_unicode_column(bool async)
    {
        await base.Non_unicode_string_literals_in_contains_is_used_for_non_unicode_column(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM `Cities` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM JSON_TABLE('["Unknown","Jacinto\\u0027s location","Ephyra\\u0027s location"]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` longtext PATH '$[0]'
    )) AS `c0`
    WHERE (`c0`.`value` = `c`.`Location`) OR (`c0`.`value` IS NULL AND (`c`.`Location` IS NULL)))
""");
        }
        else
        {
            AssertSql(
"""
SELECT `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM `Cities` AS `c`
WHERE `c`.`Location` IN ('Unknown', 'Jacinto''s location', 'Ephyra''s location')
""");
        }
    }

    public override async Task Non_unicode_string_literals_is_used_for_non_unicode_column_with_subquery(bool async)
    {
        await base.Non_unicode_string_literals_is_used_for_non_unicode_column_with_subquery(async);

        AssertSql(
"""
SELECT `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM `Cities` AS `c`
WHERE (`c`.`Location` = 'Unknown') AND ((
    SELECT COUNT(*)
    FROM (
        SELECT `g`.`Nickname`, `g`.`CityOfBirthName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`CityOfBirthName`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE (`c`.`Name` = `u`.`CityOfBirthName`) AND (`u`.`Nickname` = 'Paduk')) = 1)
""");
    }

    public override async Task Non_unicode_string_literals_is_used_for_non_unicode_column_in_subquery(bool async)
    {
        await base.Non_unicode_string_literals_is_used_for_non_unicode_column_in_subquery(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Cities` AS `c` ON `u`.`CityOfBirthName` = `c`.`Name`
WHERE (`u`.`Nickname` = 'Marcus') AND (`c`.`Location` = 'Jacinto''s location')
""");
    }

    public override async Task Non_unicode_string_literals_is_used_for_non_unicode_column_with_contains(bool async)
    {
        await base.Non_unicode_string_literals_is_used_for_non_unicode_column_with_contains(async);

        AssertSql(
"""
SELECT `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM `Cities` AS `c`
WHERE `c`.`Location` LIKE '%Jacinto%'
""");
    }

    public override void Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result1()
    {
        base.Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result1();

        // Issue#16897
        AssertSql(
"""
SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`LeaderNickname` = `u0`.`Nickname`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`, `u0`.`SquadId`
""");
    }

    public override void Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result2()
    {
        base.Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result2();

        // Issue#16897
        AssertSql(
"""
SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `u`.`Nickname`, `u`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`LeaderNickname` = `u0`.`Nickname`
LEFT JOIN `Weapons` AS `w` ON `u0`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`, `u0`.`SquadId`
""");
    }

    public override async Task Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result3(bool async)
    {
        await base.Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result3(async);

        // Issue#16897
        AssertSql(
"""
SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `u`.`Nickname`, `u`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`LeaderNickname` = `u0`.`Nickname`
LEFT JOIN `Weapons` AS `w` ON `u0`.`FullName` = `w`.`OwnerFullName`
LEFT JOIN `Weapons` AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`, `u0`.`SquadId`, `w`.`Id`
""");
    }

    public override async Task Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result4(bool async)
    {
        await base.Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_coalesce_result4(async);

        // Issue#16897
        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`, `w1`.`Id`, `w1`.`AmmunitionType`, `w1`.`IsAutomatic`, `w1`.`Name`, `w1`.`OwnerFullName`, `w1`.`SynergyWithId`, `w2`.`Id`, `w2`.`AmmunitionType`, `w2`.`IsAutomatic`, `w2`.`Name`, `w2`.`OwnerFullName`, `w2`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`LeaderNickname` = `u0`.`Nickname`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
LEFT JOIN `Weapons` AS `w0` ON `u0`.`FullName` = `w0`.`OwnerFullName`
LEFT JOIN `Weapons` AS `w1` ON `u0`.`FullName` = `w1`.`OwnerFullName`
LEFT JOIN `Weapons` AS `w2` ON `u`.`FullName` = `w2`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`, `u0`.`SquadId`, `w`.`Id`, `w0`.`Id`, `w1`.`Id`
""");
    }

    public override async Task Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_inheritance_and_coalesce_result(bool async)
    {
        await base.Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_inheritance_and_coalesce_result(async);

        // Issue#16897
        AssertSql(
"""
SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `u`.`Nickname`, `u`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`LeaderNickname` = `u0`.`Nickname`
LEFT JOIN `Weapons` AS `w` ON `u0`.`FullName` = `w`.`OwnerFullName`
LEFT JOIN `Weapons` AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`, `u0`.`SquadId`, `w`.`Id`
""");
    }

    public override async Task Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_conditional_result(bool async)
    {
        await base.Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_conditional_result(async);

        // Issue#16897
        AssertSql(
"""
SELECT `u0`.`Nickname` IS NOT NULL AND (`u0`.`SquadId` IS NOT NULL), `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `u`.`Nickname`, `u`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`LeaderNickname` = `u0`.`Nickname`
LEFT JOIN `Weapons` AS `w` ON `u0`.`FullName` = `w`.`OwnerFullName`
LEFT JOIN `Weapons` AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`, `u0`.`SquadId`, `w`.`Id`
""");
    }

    public override async Task Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_complex_projection_result(bool async)
    {
        await base.Include_on_GroupJoin_SelectMany_DefaultIfEmpty_with_complex_projection_result(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`, `w1`.`Id`, `w1`.`AmmunitionType`, `w1`.`IsAutomatic`, `w1`.`Name`, `w1`.`OwnerFullName`, `w1`.`SynergyWithId`, `w2`.`Id`, `w2`.`AmmunitionType`, `w2`.`IsAutomatic`, `w2`.`Name`, `w2`.`OwnerFullName`, `w2`.`SynergyWithId`, `u0`.`Nickname` IS NOT NULL AND (`u0`.`SquadId` IS NOT NULL), `w3`.`Id`, `w3`.`AmmunitionType`, `w3`.`IsAutomatic`, `w3`.`Name`, `w3`.`OwnerFullName`, `w3`.`SynergyWithId`, `w4`.`Id`, `w4`.`AmmunitionType`, `w4`.`IsAutomatic`, `w4`.`Name`, `w4`.`OwnerFullName`, `w4`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`LeaderNickname` = `u0`.`Nickname`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
LEFT JOIN `Weapons` AS `w0` ON `u0`.`FullName` = `w0`.`OwnerFullName`
LEFT JOIN `Weapons` AS `w1` ON `u0`.`FullName` = `w1`.`OwnerFullName`
LEFT JOIN `Weapons` AS `w2` ON `u`.`FullName` = `w2`.`OwnerFullName`
LEFT JOIN `Weapons` AS `w3` ON `u0`.`FullName` = `w3`.`OwnerFullName`
LEFT JOIN `Weapons` AS `w4` ON `u`.`FullName` = `w4`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`, `u0`.`SquadId`, `w`.`Id`, `w0`.`Id`, `w1`.`Id`, `w2`.`Id`, `w3`.`Id`
""");
    }

    public override async Task Coalesce_operator_in_predicate(bool async)
    {
        await base.Coalesce_operator_in_predicate(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE COALESCE(`u`.`HasSoulPatch`, FALSE)
""");
    }

    public override async Task Coalesce_operator_in_predicate_with_other_conditions(bool async)
    {
        await base.Coalesce_operator_in_predicate_with_other_conditions(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE ((`t`.`Note` <> 'K.I.A.') OR `t`.`Note` IS NULL) AND COALESCE(`u`.`HasSoulPatch`, FALSE)
""");
    }

    public override async Task Coalesce_operator_in_projection_with_other_conditions(bool async)
    {
        await base.Coalesce_operator_in_projection_with_other_conditions(async);

        AssertSql(
"""
SELECT ((`t`.`Note` <> 'K.I.A.') OR `t`.`Note` IS NULL) AND COALESCE(`u`.`HasSoulPatch`, FALSE)
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_predicate(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_predicate(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE ((`t`.`Note` <> 'K.I.A.') OR `t`.`Note` IS NULL) AND (`u`.`HasSoulPatch` = TRUE)
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_predicate2(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_predicate2(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE `u`.`HasSoulPatch` = TRUE
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_predicate_negated(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_predicate_negated(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE `u`.`HasSoulPatch` = FALSE
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_predicate_negated_complex1(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_predicate_negated_complex1(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE NOT (CASE
    WHEN `u`.`HasSoulPatch` = TRUE THEN TRUE
    ELSE `u`.`HasSoulPatch`
END)
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_predicate_negated_complex2(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_predicate_negated_complex2(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE NOT (CASE
    WHEN `u`.`HasSoulPatch` = FALSE THEN FALSE
    ELSE `u`.`HasSoulPatch`
END)
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_conditional_expression(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_conditional_expression(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE CASE
    WHEN `u`.`HasSoulPatch` = TRUE THEN TRUE
    ELSE FALSE
END
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_binary_expression(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_binary_expression(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE (`u`.`HasSoulPatch` = TRUE) OR (`t`.`Note` LIKE '%Cole%')
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_binary_and_expression(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_binary_and_expression(async);

        AssertSql(
"""
SELECT (`u`.`HasSoulPatch` = TRUE) AND ((`t`.`Note` LIKE '%Cole%') AND `t`.`Note` IS NOT NULL)
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_projection(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_projection(async);

        AssertSql(
"""
SELECT `u`.`SquadId`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE (`t`.`Note` <> 'K.I.A.') OR `t`.`Note` IS NULL
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_projection_into_anonymous_type(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_projection_into_anonymous_type(async);

        AssertSql(
"""
SELECT `u`.`SquadId`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE (`t`.`Note` <> 'K.I.A.') OR `t`.`Note` IS NULL
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_DTOs(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_DTOs(async);

        AssertSql(
"""
SELECT `u`.`SquadId` AS `Id`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE (`t`.`Note` <> 'K.I.A.') OR `t`.`Note` IS NULL
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_list_initializers(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_list_initializers(async);

        AssertSql(
"""
SELECT `u`.`SquadId`, `u`.`SquadId` + 1
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE (`t`.`Note` <> 'K.I.A.') OR `t`.`Note` IS NULL
ORDER BY `t`.`Note`
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_array_initializers(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_array_initializers(async);

        AssertSql(
"""
SELECT `u`.`SquadId`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE (`t`.`Note` <> 'K.I.A.') OR `t`.`Note` IS NULL
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_orderby(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_orderby(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE (`t`.`Note` <> 'K.I.A.') OR `t`.`Note` IS NULL
ORDER BY `u`.`SquadId`
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_all(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_all(async);

        AssertSql(
"""
SELECT NOT EXISTS (
    SELECT 1
    FROM `Tags` AS `t`
    LEFT JOIN (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
        FROM `Officers` AS `o`
    ) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
    WHERE ((`t`.`Note` <> 'K.I.A.') OR `t`.`Note` IS NULL) AND (`u`.`HasSoulPatch` = FALSE))
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_negated_predicate(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_negated_predicate(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE ((`t`.`Note` <> 'K.I.A.') OR `t`.`Note` IS NULL) AND (`u`.`HasSoulPatch` = FALSE)
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_contains(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_contains(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE ((`t`.`Note` <> 'K.I.A.') OR `t`.`Note` IS NULL) AND `u`.`SquadId` IN (
    SELECT `g0`.`SquadId`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`SquadId`
    FROM `Officers` AS `o0`
)
""");
    }

    public override async Task Optional_navigation_type_compensation_works_with_skip(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_skip(async);

        AssertSql();
    }

    public override async Task Optional_navigation_type_compensation_works_with_take(bool async)
    {
        await base.Optional_navigation_type_compensation_works_with_take(async);

        AssertSql();
    }

    public override async Task Select_correlated_filtered_collection(bool async)
    {
        await base.Select_correlated_filtered_collection(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `c`.`Name`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`CityOfBirthName`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`CityOfBirthName`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Cities` AS `c` ON `u`.`CityOfBirthName` = `c`.`Name`
LEFT JOIN (
    SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
    FROM `Weapons` AS `w`
    WHERE (`w`.`Name` <> 'Lancer') OR `w`.`Name` IS NULL
) AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
WHERE `c`.`Name` IN ('Ephyra', 'Hanover')
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `c`.`Name`
""");
    }

    public override async Task Select_correlated_filtered_collection_with_composite_key(bool async)
    {
        await base.Select_correlated_filtered_collection_with_composite_key(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u1`.`Nickname`, `u1`.`SquadId`, `u1`.`AssignedCityName`, `u1`.`CityOfBirthName`, `u1`.`FullName`, `u1`.`HasSoulPatch`, `u1`.`LeaderNickname`, `u1`.`LeaderSquadId`, `u1`.`Rank`, `u1`.`Discriminator`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o0`
    ) AS `u0`
    WHERE `u0`.`Nickname` <> 'Dom'
) AS `u1` ON (`u`.`Nickname` = `u1`.`LeaderNickname`) AND (`u`.`SquadId` = `u1`.`LeaderSquadId`)
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u1`.`Nickname`
""");
    }

    public override async Task Select_correlated_filtered_collection_works_with_caching(bool async)
    {
        await base.Select_correlated_filtered_collection_works_with_caching(async);

        AssertSql(
"""
SELECT `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON `t`.`GearNickName` = `u`.`Nickname`
ORDER BY `t`.`Note`, `t`.`Id`, `u`.`Nickname`
""");
    }

    public override async Task Join_predicate_value_equals_condition(bool async)
    {
        await base.Join_predicate_value_equals_condition(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Weapons` AS `w` ON `w`.`SynergyWithId` IS NOT NULL
""");
    }

    public override async Task Join_predicate_value(bool async)
    {
        await base.Join_predicate_value(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Weapons` AS `w` ON `u`.`HasSoulPatch` = TRUE
""");
    }

    public override async Task Join_predicate_condition_equals_condition(bool async)
    {
        await base.Join_predicate_condition_equals_condition(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Weapons` AS `w` ON `w`.`SynergyWithId` IS NOT NULL
""");
    }

    public override async Task Left_join_predicate_value_equals_condition(bool async)
    {
        await base.Left_join_predicate_value_equals_condition(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w` ON `w`.`SynergyWithId` IS NOT NULL
""");
    }

    public override async Task Left_join_predicate_value(bool async)
    {
        await base.Left_join_predicate_value(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w` ON `u`.`HasSoulPatch` = TRUE
""");
    }

    public override async Task Left_join_predicate_condition_equals_condition(bool async)
    {
        await base.Left_join_predicate_condition_equals_condition(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w` ON `w`.`SynergyWithId` IS NOT NULL
""");
    }

    public override async Task Where_datetimeoffset_now(bool async)
    {
        await base.Where_datetimeoffset_now(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE `m`.`Timeline` <> UTC_TIMESTAMP(6)
""");
    }

    public override async Task Where_datetimeoffset_utcnow(bool async)
    {
        await base.Where_datetimeoffset_utcnow(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE `m`.`Timeline` <> UTC_TIMESTAMP(6)
""");
    }

    public override async Task Where_datetimeoffset_date_component(bool async)
    {
        await base.Where_datetimeoffset_date_component(async);

        AssertSql(
"""
@__Date_0='0001-01-01T00:00:00.0000000' (DbType = DateTime)

SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE CONVERT(`m`.`Timeline`, date) > @__Date_0
""");
    }

    public override async Task Where_datetimeoffset_year_component(bool async)
    {
        await base.Where_datetimeoffset_year_component(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE EXTRACT(year FROM `m`.`Timeline`) = 2
""");
    }

    public override async Task Where_datetimeoffset_month_component(bool async)
    {
        await base.Where_datetimeoffset_month_component(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE EXTRACT(month FROM `m`.`Timeline`) = 1
""");
    }

    public override async Task Where_datetimeoffset_dayofyear_component(bool async)
    {
        await base.Where_datetimeoffset_dayofyear_component(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE DAYOFYEAR(`m`.`Timeline`) = 2
""");
    }

    public override async Task Where_datetimeoffset_day_component(bool async)
    {
        await base.Where_datetimeoffset_day_component(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE EXTRACT(day FROM `m`.`Timeline`) = 2
""");
    }

    public override async Task Where_datetimeoffset_hour_component(bool async)
    {
        await AssertQuery(
            async,
            ss => from m in ss.Set<Mission>()
                where m.Timeline.Hour == /* 10 */ 8
                select m);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE EXTRACT(hour FROM `m`.`Timeline`) = 8
""");
    }

    public override async Task Where_datetimeoffset_minute_component(bool async)
    {
        await base.Where_datetimeoffset_minute_component(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE EXTRACT(minute FROM `m`.`Timeline`) = 0
""");
    }

    public override async Task Where_datetimeoffset_second_component(bool async)
    {
        await base.Where_datetimeoffset_second_component(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE EXTRACT(second FROM `m`.`Timeline`) = 0
""");
    }

    public override async Task Where_datetimeoffset_millisecond_component(bool async)
    {
        await base.Where_datetimeoffset_millisecond_component(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE (EXTRACT(microsecond FROM `m`.`Timeline`)) DIV (1000) = 0
""");
    }

    public override async Task DateTimeOffset_DateAdd_AddMonths(bool async)
    {
        await base.DateTimeOffset_DateAdd_AddMonths(async);

        AssertSql(
"""
SELECT DATE_ADD(`m`.`Timeline`, INTERVAL CAST(1 AS signed) month)
FROM `Missions` AS `m`
""");
    }

    public override async Task DateTimeOffset_DateAdd_AddDays(bool async)
    {
        await base.DateTimeOffset_DateAdd_AddDays(async);

        AssertSql(
"""
SELECT DATE_ADD(`m`.`Timeline`, INTERVAL CAST(1.0 AS signed) day)
FROM `Missions` AS `m`
""");
    }

    public override async Task DateTimeOffset_DateAdd_AddHours(bool async)
    {
        await base.DateTimeOffset_DateAdd_AddHours(async);

        AssertSql(
"""
SELECT DATE_ADD(`m`.`Timeline`, INTERVAL CAST(1.0 AS signed) hour)
FROM `Missions` AS `m`
""");
    }

    public override async Task DateTimeOffset_DateAdd_AddMinutes(bool async)
    {
        await base.DateTimeOffset_DateAdd_AddMinutes(async);

        AssertSql(
"""
SELECT DATE_ADD(`m`.`Timeline`, INTERVAL CAST(1.0 AS signed) minute)
FROM `Missions` AS `m`
""");
    }

    public override async Task DateTimeOffset_DateAdd_AddSeconds(bool async)
    {
        await base.DateTimeOffset_DateAdd_AddSeconds(async);

        AssertSql(
"""
SELECT DATE_ADD(`m`.`Timeline`, INTERVAL CAST(1.0 AS signed) second)
FROM `Missions` AS `m`
""");
    }

    public override async Task DateTimeOffset_DateAdd_AddMilliseconds(bool async)
    {
        await base.DateTimeOffset_DateAdd_AddMilliseconds(async);

        AssertSql(
"""
SELECT DATE_ADD(`m`.`Timeline`, INTERVAL 1000 * CAST(300.0 AS signed) microsecond)
FROM `Missions` AS `m`
""");
    }

    public override async Task Where_datetimeoffset_milliseconds_parameter_and_constant(bool async)
    {
        var dateTimeOffset = MySqlTestHelpers.GetExpectedValue(new DateTimeOffset(599898024001234567, new TimeSpan(1, 30, 0)));

        // Literal where clause
        var p = Expression.Parameter(typeof(Mission), "i");
        var dynamicWhere = Expression.Lambda<Func<Mission, bool>>(
            Expression.Equal(
                Expression.Property(p, "Timeline"),
                Expression.Constant(dateTimeOffset)
            ), p);

        await AssertCount(
            async,
            ss => ss.Set<Mission>().Where(dynamicWhere),
            ss => ss.Set<Mission>().Where(m => m.Timeline == dateTimeOffset));

        AssertSql(
"""
SELECT COUNT(*)
FROM `Missions` AS `m`
WHERE `m`.`Timeline` = TIMESTAMP '1902-01-02 08:30:00.123456'
""");
    }

    public override async Task Orderby_added_for_client_side_GroupJoin_composite_dependent_to_principal_LOJ_when_incomplete_key_is_used(
        bool async)
    {
        await base.Orderby_added_for_client_side_GroupJoin_composite_dependent_to_principal_LOJ_when_incomplete_key_is_used(async);

        AssertSql();
    }

    public override async Task Complex_predicate_with_AndAlso_and_nullable_bool_property(bool async)
    {
        await base.Complex_predicate_with_AndAlso_and_nullable_bool_property(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
LEFT JOIN (
    SELECT `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON `w`.`OwnerFullName` = `u`.`FullName`
WHERE (`w`.`Id` <> 50) AND (`u`.`HasSoulPatch` = FALSE)
""");
    }

    public override async Task Distinct_with_optional_navigation_is_translated_to_sql(bool async)
    {
        await base.Distinct_with_optional_navigation_is_translated_to_sql(async);

        AssertSql(
"""
SELECT DISTINCT `u`.`HasSoulPatch`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
WHERE (`t`.`Note` <> 'Foo') OR `t`.`Note` IS NULL
""");
    }

    public override async Task Sum_with_optional_navigation_is_translated_to_sql(bool async)
    {
        await base.Sum_with_optional_navigation_is_translated_to_sql(async);

        AssertSql(
"""
SELECT COALESCE(SUM(`u`.`SquadId`), 0)
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
WHERE (`t`.`Note` <> 'Foo') OR `t`.`Note` IS NULL
""");
    }

    public override async Task Count_with_optional_navigation_is_translated_to_sql(bool async)
    {
        await base.Count_with_optional_navigation_is_translated_to_sql(async);

        AssertSql(
"""
SELECT COUNT(*)
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
WHERE (`t`.`Note` <> 'Foo') OR `t`.`Note` IS NULL
""");
    }

    public override async Task FirstOrDefault_with_manually_created_groupjoin_is_translated_to_sql(bool async)
    {
        await base.FirstOrDefault_with_manually_created_groupjoin_is_translated_to_sql(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
LEFT JOIN (
    SELECT `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON `s`.`Id` = `u`.`SquadId`
WHERE `s`.`Name` = 'Kilo'
LIMIT 1
""");
    }

    public override async Task Any_with_optional_navigation_as_subquery_predicate_is_translated_to_sql(bool async)
    {
        await base.Any_with_optional_navigation_as_subquery_predicate_is_translated_to_sql(async);

        AssertSql(
"""
SELECT `s`.`Name`
FROM `Squads` AS `s`
WHERE NOT EXISTS (
    SELECT 1
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`
        FROM `Officers` AS `o`
    ) AS `u`
    LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
    WHERE (`s`.`Id` = `u`.`SquadId`) AND (`t`.`Note` = 'Dom''s Tag'))
""");
    }

    public override async Task All_with_optional_navigation_is_translated_to_sql(bool async)
    {
        await base.All_with_optional_navigation_is_translated_to_sql(async);

        AssertSql(
"""
SELECT NOT EXISTS (
    SELECT 1
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`
        FROM `Officers` AS `o`
    ) AS `u`
    LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
    WHERE `t`.`Note` = 'Foo')
""");
    }

    public override async Task Contains_with_local_nullable_guid_list_closure(bool async)
    {
        await base.Contains_with_local_nullable_guid_list_closure(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
WHERE `t`.`Id` IN (
    SELECT `i`.`value`
    FROM JSON_TABLE('["d2c26679-562b-44d1-ab96-23d1775e0926","23cbcf9b-ce14-45cf-aafa-2c2667ebfdd3","ab1b82d7-88db-42bd-a132-7eef9aa68af4"]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` char(36) PATH '$[0]'
    )) AS `i`
)
""");
        }
        else
        {
            AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
WHERE `t`.`Id` IN ('df36f493-463f-4123-83f9-6b135deeb7ba', '23cbcf9b-ce14-45cf-aafa-2c2667ebfdd3', 'ab1b82d7-88db-42bd-a132-7eef9aa68af4')
""");
        }
    }

    public override async Task Unnecessary_include_doesnt_get_added_complex_when_projecting_EF_Property(bool async)
    {
        await base.Unnecessary_include_doesnt_get_added_complex_when_projecting_EF_Property(async);

        AssertSql(
"""
SELECT `u`.`FullName`
FROM (
    SELECT `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`HasSoulPatch` = TRUE
ORDER BY `u`.`Rank`
""");
    }

    public override async Task Multiple_order_bys_are_properly_lifted_from_subquery_created_by_include(bool async)
    {
        await base.Multiple_order_bys_are_properly_lifted_from_subquery_created_by_include(async);

        AssertSql(
"""
SELECT `u`.`FullName`
FROM (
    SELECT `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`HasSoulPatch` = FALSE
ORDER BY `u`.`FullName`
""");
    }

    public override async Task Order_by_is_properly_lifted_from_subquery_with_same_order_by_in_the_outer_query(bool async)
    {
        await base.Order_by_is_properly_lifted_from_subquery_with_same_order_by_in_the_outer_query(async);

        AssertSql(
"""
SELECT `u`.`FullName`
FROM (
    SELECT `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`HasSoulPatch` = FALSE
ORDER BY `u`.`FullName`
""");
    }

    public override async Task Where_is_properly_lifted_from_subquery_created_by_include(bool async)
    {
        await base.Where_is_properly_lifted_from_subquery_created_by_include(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
WHERE (`u`.`FullName` <> 'Augustus Cole') AND (`u`.`HasSoulPatch` = FALSE)
ORDER BY `u`.`FullName`
""");
    }

    public override async Task Subquery_is_lifted_from_main_from_clause_of_SelectMany(bool async)
    {
        await base.Subquery_is_lifted_from_main_from_clause_of_SelectMany(async);

        AssertSql(
"""
SELECT `u`.`FullName` AS `Name1`, `u0`.`FullName` AS `Name2`
FROM (
    SELECT `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
CROSS JOIN (
    SELECT `g0`.`FullName`, `g0`.`HasSoulPatch`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`FullName`, `o0`.`HasSoulPatch`
    FROM `Officers` AS `o0`
) AS `u0`
WHERE (`u`.`HasSoulPatch` = TRUE) AND (`u0`.`HasSoulPatch` = FALSE)
ORDER BY `u`.`FullName`
""");
    }

    public override async Task Subquery_containing_SelectMany_projecting_main_from_clause_gets_lifted(bool async)
    {
        await base.Subquery_containing_SelectMany_projecting_main_from_clause_gets_lifted(async);

        AssertSql(
"""
SELECT `u`.`FullName`
FROM (
    SELECT `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
CROSS JOIN `Tags` AS `t`
WHERE `u`.`HasSoulPatch` = TRUE
ORDER BY `u`.`FullName`
""");
    }

    public override async Task Subquery_containing_join_projecting_main_from_clause_gets_lifted(bool async)
    {
        await base.Subquery_containing_join_projecting_main_from_clause_gets_lifted(async);

        AssertSql(
"""
SELECT `u`.`Nickname`
FROM (
    SELECT `g`.`Nickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Tags` AS `t` ON `u`.`Nickname` = `t`.`GearNickName`
ORDER BY `u`.`Nickname`
""");
    }

    public override async Task Subquery_containing_left_join_projecting_main_from_clause_gets_lifted(bool async)
    {
        await base.Subquery_containing_left_join_projecting_main_from_clause_gets_lifted(async);

        AssertSql(
"""
SELECT `u`.`Nickname`
FROM (
    SELECT `g`.`Nickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON `u`.`Nickname` = `t`.`GearNickName`
ORDER BY `u`.`Nickname`
""");
    }

    public override async Task Subquery_containing_join_gets_lifted_clashing_names(bool async)
    {
        await base.Subquery_containing_join_gets_lifted_clashing_names(async);

        AssertSql(
"""
SELECT `u`.`Nickname`
FROM (
    SELECT `g`.`Nickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Tags` AS `t` ON `u`.`Nickname` = `t`.`GearNickName`
INNER JOIN `Tags` AS `t0` ON `u`.`Nickname` = `t0`.`GearNickName`
WHERE (`t`.`GearNickName` <> 'Cole Train') OR `t`.`GearNickName` IS NULL
ORDER BY `u`.`Nickname`, `t0`.`Id`
""");
    }

    public override async Task Subquery_created_by_include_gets_lifted_nested(bool async)
    {
        await base.Subquery_created_by_include_gets_lifted_nested(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Cities` AS `c` ON `u`.`CityOfBirthName` = `c`.`Name`
WHERE EXISTS (
    SELECT 1
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`) AND (`u`.`HasSoulPatch` = FALSE)
ORDER BY `u`.`Nickname`
""");
    }

    public override async Task Subquery_is_lifted_from_additional_from_clause(bool async)
    {
        await base.Subquery_is_lifted_from_additional_from_clause(async);

        AssertSql(
"""
SELECT `u`.`FullName` AS `Name1`, `u0`.`FullName` AS `Name2`
FROM (
    SELECT `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
CROSS JOIN (
    SELECT `g0`.`FullName`, `g0`.`HasSoulPatch`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`FullName`, `o0`.`HasSoulPatch`
    FROM `Officers` AS `o0`
) AS `u0`
WHERE (`u`.`HasSoulPatch` = TRUE) AND (`u0`.`HasSoulPatch` = FALSE)
ORDER BY `u`.`FullName`
""");
    }

    public override async Task Subquery_with_result_operator_is_not_lifted(bool async)
    {
        await base.Subquery_with_result_operator_is_not_lifted(async);

        AssertSql(
"""
@__p_0='2'

SELECT `u0`.`FullName`
FROM (
    SELECT `u`.`FullName`, `u`.`Rank`
    FROM (
        SELECT `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`Rank`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`Rank`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE `u`.`HasSoulPatch` = FALSE
    ORDER BY `u`.`FullName`
    LIMIT @__p_0
) AS `u0`
ORDER BY `u0`.`Rank`
""");
    }

    public override async Task Skip_with_orderby_followed_by_orderBy_is_pushed_down(bool async)
    {
        await base.Skip_with_orderby_followed_by_orderBy_is_pushed_down(async);

        AssertSql(
"""
@__p_0='1'

SELECT `u0`.`FullName`
FROM (
    SELECT `u`.`FullName`, `u`.`Rank`
    FROM (
        SELECT `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`Rank`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`Rank`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE `u`.`HasSoulPatch` = FALSE
    ORDER BY `u`.`FullName`
    LIMIT 18446744073709551610 OFFSET @__p_0
) AS `u0`
ORDER BY `u0`.`Rank`
""");
    }

    [ConditionalTheory(Skip = "MySQL does not support LIMIT with a parameterized argument, unless the statement was prepared. The argument needs to be a numeric constant.")]
    public override async Task Take_without_orderby_followed_by_orderBy_is_pushed_down1(bool async)
    {
        await base.Take_without_orderby_followed_by_orderBy_is_pushed_down1(async);

        AssertSql(
"""
@__p_0='999'

SELECT `t0`.`FullName`
FROM (
    SELECT `t`.`FullName`, `t`.`Rank`
    FROM (
        SELECT `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`Rank`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`Rank`
        FROM `Officers` AS `o`
    ) AS `t`
    WHERE `t`.`HasSoulPatch` = FALSE
    LIMIT @__p_0
) AS `t0`
ORDER BY `t0`.`Rank`
""");
    }

    [ConditionalTheory(Skip = "MySQL does not support LIMIT with a parameterized argument, unless the statement was prepared. The argument needs to be a numeric constant.")]
    public override async Task Take_without_orderby_followed_by_orderBy_is_pushed_down2(bool async)
    {
        await base.Take_without_orderby_followed_by_orderBy_is_pushed_down2(async);

        AssertSql(
"""
@__p_0='999'

SELECT `t0`.`FullName`
FROM (
    SELECT `t`.`FullName`, `t`.`Rank`
    FROM (
        SELECT `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`Rank`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`Rank`
        FROM `Officers` AS `o`
    ) AS `t`
    WHERE `t`.`HasSoulPatch` = FALSE
    LIMIT @__p_0
) AS `t0`
ORDER BY `t0`.`Rank`
""");
    }

    [ConditionalTheory(Skip = "MySQL does not support LIMIT with a parameterized argument, unless the statement was prepared. The argument needs to be a numeric constant.")]
    public override async Task Take_without_orderby_followed_by_orderBy_is_pushed_down3(bool async)
    {
        await base.Take_without_orderby_followed_by_orderBy_is_pushed_down3(async);

        AssertSql(
"""
@__p_0='999'

SELECT `t0`.`FullName`
FROM (
    SELECT `t`.`FullName`, `t`.`Rank`
    FROM (
        SELECT `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`Rank`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`Rank`
        FROM `Officers` AS `o`
    ) AS `t`
    WHERE `t`.`HasSoulPatch` = FALSE
    LIMIT @__p_0
) AS `t0`
ORDER BY `t0`.`FullName`, `t0`.`Rank`
""");
    }

    public override async Task Select_length_of_string_property(bool async)
    {
        await base.Select_length_of_string_property(async);

        AssertSql(
"""
SELECT `w`.`Name`, CHAR_LENGTH(`w`.`Name`) AS `Length`
FROM `Weapons` AS `w`
""");
    }

    public override async Task Client_method_on_collection_navigation_in_outer_join_key(bool async)
    {
        await base.Client_method_on_collection_navigation_in_outer_join_key(async);

        AssertSql();
    }

    public override async Task Member_access_on_derived_entity_using_cast(bool async)
    {
        await base.Member_access_on_derived_entity_using_cast(async);

        AssertSql(
"""
SELECT `l`.`Name`, `l`.`Eradicated`
FROM `LocustHordes` AS `l`
ORDER BY `l`.`Name`
""");
    }

    public override async Task Member_access_on_derived_materialized_entity_using_cast(bool async)
    {
        await base.Member_access_on_derived_materialized_entity_using_cast(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`CapitalName`, `l`.`Name`, `l`.`ServerAddress`, `l`.`CommanderName`, `l`.`Eradicated`
FROM `LocustHordes` AS `l`
ORDER BY `l`.`Name`
""");
    }

    public override async Task Member_access_on_derived_entity_using_cast_and_let(bool async)
    {
        await base.Member_access_on_derived_entity_using_cast_and_let(async);

        AssertSql(
"""
SELECT `l`.`Name`, `l`.`Eradicated`
FROM `LocustHordes` AS `l`
ORDER BY `l`.`Name`
""");
    }

    public override async Task Property_access_on_derived_entity_using_cast(bool async)
    {
        await base.Property_access_on_derived_entity_using_cast(async);

        AssertSql(
"""
SELECT `l`.`Name`, `l`.`Eradicated`
FROM `LocustHordes` AS `l`
ORDER BY `l`.`Name`
""");
    }

    public override async Task Navigation_access_on_derived_entity_using_cast(bool async)
    {
        await base.Navigation_access_on_derived_entity_using_cast(async);

        AssertSql(
"""
SELECT `l`.`Name`, `l0`.`ThreatLevel` AS `Threat`
FROM `LocustHordes` AS `l`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
ORDER BY `l`.`Name`
""");
    }

    public override async Task Navigation_access_on_derived_materialized_entity_using_cast(bool async)
    {
        await base.Navigation_access_on_derived_materialized_entity_using_cast(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`CapitalName`, `l`.`Name`, `l`.`ServerAddress`, `l`.`CommanderName`, `l`.`Eradicated`, `l0`.`ThreatLevel` AS `Threat`
FROM `LocustHordes` AS `l`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
ORDER BY `l`.`Name`
""");
    }

    public override async Task Navigation_access_via_EFProperty_on_derived_entity_using_cast(bool async)
    {
        await base.Navigation_access_via_EFProperty_on_derived_entity_using_cast(async);

        AssertSql(
"""
SELECT `l`.`Name`, `l0`.`ThreatLevel` AS `Threat`
FROM `LocustHordes` AS `l`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
ORDER BY `l`.`Name`
""");
    }

    public override async Task Navigation_access_fk_on_derived_entity_using_cast(bool async)
    {
        await base.Navigation_access_fk_on_derived_entity_using_cast(async);

        AssertSql(
"""
SELECT `l`.`Name`, `l0`.`Name` AS `CommanderName`
FROM `LocustHordes` AS `l`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
ORDER BY `l`.`Name`
""");
    }

    public override async Task Collection_navigation_access_on_derived_entity_using_cast(bool async)
    {
        await base.Collection_navigation_access_on_derived_entity_using_cast(async);

        AssertSql(
"""
SELECT `l`.`Name`, (
    SELECT COUNT(*)
    FROM (
        SELECT `l0`.`LocustHordeId`
        FROM `LocustLeaders` AS `l0`
        UNION ALL
        SELECT `l1`.`LocustHordeId`
        FROM `LocustCommanders` AS `l1`
    ) AS `u`
    WHERE `l`.`Id` = `u`.`LocustHordeId`) AS `LeadersCount`
FROM `LocustHordes` AS `l`
ORDER BY `l`.`Name`
""");
    }

    public override async Task Collection_navigation_access_on_derived_entity_using_cast_in_SelectMany(bool async)
    {
        await base.Collection_navigation_access_on_derived_entity_using_cast_in_SelectMany(async);

        AssertSql(
"""
SELECT `l`.`Name`, `u`.`Name` AS `LeaderName`
FROM `LocustHordes` AS `l`
INNER JOIN (
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`
    FROM `LocustLeaders` AS `l0`
    UNION ALL
    SELECT `l1`.`Name`, `l1`.`LocustHordeId`
    FROM `LocustCommanders` AS `l1`
) AS `u` ON `l`.`Id` = `u`.`LocustHordeId`
ORDER BY `u`.`Name`
""");
    }

    public override async Task Include_on_derived_entity_using_OfType(bool async)
    {
        await base.Include_on_derived_entity_using_OfType(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`CapitalName`, `l`.`Name`, `l`.`ServerAddress`, `l`.`CommanderName`, `l`.`Eradicated`, `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM `LocustHordes` AS `l`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
LEFT JOIN (
    SELECT `l1`.`Name`, `l1`.`LocustHordeId`, `l1`.`ThreatLevel`, `l1`.`ThreatLevelByte`, `l1`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l1`
    UNION ALL
    SELECT `l2`.`Name`, `l2`.`LocustHordeId`, `l2`.`ThreatLevel`, `l2`.`ThreatLevelByte`, `l2`.`ThreatLevelNullableByte`, `l2`.`DefeatedByNickname`, `l2`.`DefeatedBySquadId`, `l2`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l2`
) AS `u` ON `l`.`Id` = `u`.`LocustHordeId`
ORDER BY `l`.`Name`, `l`.`Id`, `l0`.`Name`
""");
    }

    public override async Task Distinct_on_subquery_doesnt_get_lifted(bool async)
    {
        await base.Distinct_on_subquery_doesnt_get_lifted(async);

        AssertSql(
"""
SELECT `u0`.`HasSoulPatch`
FROM (
    SELECT DISTINCT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o`
    ) AS `u`
) AS `u0`
""");
    }

    public override async Task Cast_result_operator_on_subquery_is_properly_lifted_to_a_convert(bool async)
    {
        await base.Cast_result_operator_on_subquery_is_properly_lifted_to_a_convert(async);

        AssertSql(
"""
SELECT `l`.`Eradicated`
FROM `LocustHordes` AS `l`
""");
    }

    public override async Task Comparing_two_collection_navigations_composite_key(bool async)
    {
        await base.Comparing_two_collection_navigations_composite_key(async);

        AssertSql(
"""
SELECT `u`.`Nickname` AS `Nickname1`, `u0`.`Nickname` AS `Nickname2`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u`
CROSS JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`
    FROM `Officers` AS `o0`
) AS `u0`
WHERE (`u`.`Nickname` = `u0`.`Nickname`) AND (`u`.`SquadId` = `u0`.`SquadId`)
ORDER BY `u`.`Nickname`
""");
    }

    public override async Task Comparing_two_collection_navigations_inheritance(bool async)
    {
        await base.Comparing_two_collection_navigations_inheritance(async);

        AssertSql(
"""
SELECT `l`.`Name`, `u`.`Nickname`
FROM `LocustHordes` AS `l`
CROSS JOIN (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`
    FROM `Officers` AS `o0`
) AS `u0` ON (`l0`.`DefeatedByNickname` = `u0`.`Nickname`) AND (`l0`.`DefeatedBySquadId` = `u0`.`SquadId`)
WHERE (`u`.`HasSoulPatch` = TRUE) AND ((`u0`.`Nickname` = `u`.`Nickname`) AND (`u0`.`SquadId` = `u`.`SquadId`))
""");
    }

    public override async Task Comparing_entities_using_Equals_inheritance(bool async)
    {
        await base.Comparing_entities_using_Equals_inheritance(async);

        AssertSql(
"""
SELECT `u`.`Nickname` AS `Nickname1`, `u0`.`Nickname` AS `Nickname2`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u`
CROSS JOIN (
    SELECT `o0`.`Nickname`, `o0`.`SquadId`
    FROM `Officers` AS `o0`
) AS `u0`
WHERE (`u`.`Nickname` = `u0`.`Nickname`) AND (`u`.`SquadId` = `u0`.`SquadId`)
ORDER BY `u`.`Nickname`, `u0`.`Nickname`
""");
    }

    public override async Task Contains_on_nullable_array_produces_correct_sql(bool async)
    {
        await base.Contains_on_nullable_array_produces_correct_sql(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `t`.`Nickname`, `t`.`SquadId`, `t`.`AssignedCityName`, `t`.`CityOfBirthName`, `t`.`FullName`, `t`.`HasSoulPatch`, `t`.`LeaderNickname`, `t`.`LeaderSquadId`, `t`.`Rank`, `t`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `t`
LEFT JOIN `Cities` AS `c` ON `t`.`AssignedCityName` = `c`.`Name`
WHERE (`t`.`SquadId` < 2) AND EXISTS (
    SELECT 1
    FROM JSON_TABLE('["Ephyra",null]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` varchar(255) PATH '$[0]'
    )) AS `c0`
    WHERE (`c0`.`value` = `c`.`Name`) OR (`c0`.`value` IS NULL AND (`c`.`Name` IS NULL)))
""");
        }
        else
        {
        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Cities` AS `c` ON `u`.`AssignedCityName` = `c`.`Name`
WHERE (`u`.`SquadId` < 2) AND (`c`.`Name` IS NULL OR (`c`.`Name` = 'Ephyra'))
""");
        }
    }

    public override async Task Optional_navigation_with_collection_composite_key(bool async)
    {
        await base.Optional_navigation_with_collection_composite_key(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE (`u`.`Discriminator` = 'Officer') AND ((
    SELECT COUNT(*)
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`
        FROM `Officers` AS `o0`
    ) AS `u0`
    WHERE ((`u`.`Nickname` IS NOT NULL AND (`u`.`SquadId` IS NOT NULL)) AND ((`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`))) AND (`u0`.`Nickname` = 'Dom')) > 0)
""");
    }

    public override async Task Select_null_conditional_with_inheritance(bool async)
    {
        await base.Select_null_conditional_with_inheritance(async);

        AssertSql(
"""
SELECT CASE
    WHEN `l`.`CommanderName` IS NOT NULL THEN `l`.`CommanderName`
END
FROM `LocustHordes` AS `l`
""");
    }

    public override async Task Select_null_conditional_with_inheritance_negative(bool async)
    {
        await base.Select_null_conditional_with_inheritance_negative(async);

        AssertSql(
"""
SELECT CASE
    WHEN `l`.`CommanderName` IS NOT NULL THEN `l`.`Eradicated`
END
FROM `LocustHordes` AS `l`
""");
    }

    public override async Task Project_collection_navigation_with_inheritance1(bool async)
    {
        await base.Project_collection_navigation_with_inheritance1(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l0`.`Name`, `l1`.`Id`, `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM `LocustHordes` AS `l`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
LEFT JOIN `LocustHordes` AS `l1` ON `l0`.`Name` = `l1`.`CommanderName`
LEFT JOIN (
    SELECT `l2`.`Name`, `l2`.`LocustHordeId`, `l2`.`ThreatLevel`, `l2`.`ThreatLevelByte`, `l2`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l2`
    UNION ALL
    SELECT `l3`.`Name`, `l3`.`LocustHordeId`, `l3`.`ThreatLevel`, `l3`.`ThreatLevelByte`, `l3`.`ThreatLevelNullableByte`, `l3`.`DefeatedByNickname`, `l3`.`DefeatedBySquadId`, `l3`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l3`
) AS `u` ON `l1`.`Id` = `u`.`LocustHordeId`
ORDER BY `l`.`Id`, `l0`.`Name`, `l1`.`Id`
""");
    }

    public override async Task Project_collection_navigation_with_inheritance2(bool async)
    {
        await base.Project_collection_navigation_with_inheritance2(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l0`.`Name`, `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM `LocustHordes` AS `l`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`l0`.`DefeatedByNickname` = `u`.`Nickname`) AND (`l0`.`DefeatedBySquadId` = `u`.`SquadId`)
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON ((`u`.`Nickname` = `u0`.`LeaderNickname`) OR (`u`.`Nickname` IS NULL AND (`u0`.`LeaderNickname` IS NULL))) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY `l`.`Id`, `l0`.`Name`, `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task Project_collection_navigation_with_inheritance3(bool async)
    {
        await base.Project_collection_navigation_with_inheritance3(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l0`.`Name`, `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM `LocustHordes` AS `l`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`l0`.`DefeatedByNickname` = `u`.`Nickname`) AND (`l0`.`DefeatedBySquadId` = `u`.`SquadId`)
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON ((`u`.`Nickname` = `u0`.`LeaderNickname`) OR (`u`.`Nickname` IS NULL AND (`u0`.`LeaderNickname` IS NULL))) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY `l`.`Id`, `l0`.`Name`, `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task Include_reference_on_derived_type_using_string(bool async)
    {
        await base.Include_reference_on_derived_type_using_string(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u0` ON (`u`.`DefeatedByNickname` = `u0`.`Nickname`) AND (`u`.`DefeatedBySquadId` = `u0`.`SquadId`)
""");
    }

    public override async Task Include_reference_on_derived_type_using_string_nested1(bool async)
    {
        await base.Include_reference_on_derived_type_using_string_nested1(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u0` ON (`u`.`DefeatedByNickname` = `u0`.`Nickname`) AND (`u`.`DefeatedBySquadId` = `u0`.`SquadId`)
LEFT JOIN `Squads` AS `s` ON `u0`.`SquadId` = `s`.`Id`
""");
    }

    public override async Task Include_reference_on_derived_type_using_string_nested2(bool async)
    {
        await base.Include_reference_on_derived_type_using_string_nested2(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `s`.`Nickname`, `s`.`SquadId`, `s`.`AssignedCityName`, `s`.`CityOfBirthName`, `s`.`FullName`, `s`.`HasSoulPatch`, `s`.`LeaderNickname`, `s`.`LeaderSquadId`, `s`.`Rank`, `s`.`Discriminator`, `s`.`Name`, `s`.`Location`, `s`.`Nation`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u0` ON (`u`.`DefeatedByNickname` = `u0`.`Nickname`) AND (`u`.`DefeatedBySquadId` = `u0`.`SquadId`)
LEFT JOIN (
    SELECT `u1`.`Nickname`, `u1`.`SquadId`, `u1`.`AssignedCityName`, `u1`.`CityOfBirthName`, `u1`.`FullName`, `u1`.`HasSoulPatch`, `u1`.`LeaderNickname`, `u1`.`LeaderSquadId`, `u1`.`Rank`, `u1`.`Discriminator`, `c`.`Name`, `c`.`Location`, `c`.`Nation`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o0`
    ) AS `u1`
    INNER JOIN `Cities` AS `c` ON `u1`.`CityOfBirthName` = `c`.`Name`
) AS `s` ON ((`u0`.`Nickname` = `s`.`LeaderNickname`) OR (`u0`.`Nickname` IS NULL AND (`s`.`LeaderNickname` IS NULL))) AND (`u0`.`SquadId` = `s`.`LeaderSquadId`)
ORDER BY `u`.`Name`, `u0`.`Nickname`, `u0`.`SquadId`, `s`.`Nickname`, `s`.`SquadId`
""");
    }

    public override async Task Include_reference_on_derived_type_using_lambda(bool async)
    {
        await base.Include_reference_on_derived_type_using_lambda(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u0` ON (`u`.`DefeatedByNickname` = `u0`.`Nickname`) AND (`u`.`DefeatedBySquadId` = `u0`.`SquadId`)
""");
    }

    public override async Task Include_reference_on_derived_type_using_lambda_with_soft_cast(bool async)
    {
        await base.Include_reference_on_derived_type_using_lambda_with_soft_cast(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u0` ON (`u`.`DefeatedByNickname` = `u0`.`Nickname`) AND (`u`.`DefeatedBySquadId` = `u0`.`SquadId`)
""");
    }

    public override async Task Include_reference_on_derived_type_using_lambda_with_tracking(bool async)
    {
        await base.Include_reference_on_derived_type_using_lambda_with_tracking(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u0` ON (`u`.`DefeatedByNickname` = `u0`.`Nickname`) AND (`u`.`DefeatedBySquadId` = `u0`.`SquadId`)
""");
    }

    public override async Task Include_collection_on_derived_type_using_string(bool async)
    {
        await base.Include_collection_on_derived_type_using_string(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task Include_collection_on_derived_type_using_lambda(bool async)
    {
        await base.Include_collection_on_derived_type_using_lambda(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task Include_collection_on_derived_type_using_lambda_with_soft_cast(bool async)
    {
        await base.Include_collection_on_derived_type_using_lambda_with_soft_cast(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task Include_base_navigation_on_derived_entity(bool async)
    {
        await base.Include_base_navigation_on_derived_entity(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `t`.`Id`
""");
    }

    public override async Task ThenInclude_collection_on_derived_after_base_reference(bool async)
    {
        await base.ThenInclude_collection_on_derived_after_base_reference(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`, `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
ORDER BY `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task ThenInclude_collection_on_derived_after_derived_reference(bool async)
    {
        await base.ThenInclude_collection_on_derived_after_derived_reference(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`CapitalName`, `l`.`Name`, `l`.`ServerAddress`, `l`.`CommanderName`, `l`.`Eradicated`, `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM `LocustHordes` AS `l`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`l0`.`DefeatedByNickname` = `u`.`Nickname`) AND (`l0`.`DefeatedBySquadId` = `u`.`SquadId`)
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON ((`u`.`Nickname` = `u0`.`LeaderNickname`) OR (`u`.`Nickname` IS NULL AND (`u0`.`LeaderNickname` IS NULL))) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY `l`.`Id`, `l0`.`Name`, `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task ThenInclude_collection_on_derived_after_derived_collection(bool async)
    {
        await base.ThenInclude_collection_on_derived_after_derived_collection(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `s`.`Nickname`, `s`.`SquadId`, `s`.`AssignedCityName`, `s`.`CityOfBirthName`, `s`.`FullName`, `s`.`HasSoulPatch`, `s`.`LeaderNickname`, `s`.`LeaderSquadId`, `s`.`Rank`, `s`.`Discriminator`, `s`.`Nickname0`, `s`.`SquadId0`, `s`.`AssignedCityName0`, `s`.`CityOfBirthName0`, `s`.`FullName0`, `s`.`HasSoulPatch0`, `s`.`LeaderNickname0`, `s`.`LeaderSquadId0`, `s`.`Rank0`, `s`.`Discriminator0`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `u1`.`Nickname` AS `Nickname0`, `u1`.`SquadId` AS `SquadId0`, `u1`.`AssignedCityName` AS `AssignedCityName0`, `u1`.`CityOfBirthName` AS `CityOfBirthName0`, `u1`.`FullName` AS `FullName0`, `u1`.`HasSoulPatch` AS `HasSoulPatch0`, `u1`.`LeaderNickname` AS `LeaderNickname0`, `u1`.`LeaderSquadId` AS `LeaderSquadId0`, `u1`.`Rank` AS `Rank0`, `u1`.`Discriminator` AS `Discriminator0`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o0`
    ) AS `u0`
    LEFT JOIN (
        SELECT `g1`.`Nickname`, `g1`.`SquadId`, `g1`.`AssignedCityName`, `g1`.`CityOfBirthName`, `g1`.`FullName`, `g1`.`HasSoulPatch`, `g1`.`LeaderNickname`, `g1`.`LeaderSquadId`, `g1`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g1`
        UNION ALL
        SELECT `o1`.`Nickname`, `o1`.`SquadId`, `o1`.`AssignedCityName`, `o1`.`CityOfBirthName`, `o1`.`FullName`, `o1`.`HasSoulPatch`, `o1`.`LeaderNickname`, `o1`.`LeaderSquadId`, `o1`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o1`
    ) AS `u1` ON (`u0`.`Nickname` = `u1`.`LeaderNickname`) AND (`u0`.`SquadId` = `u1`.`LeaderSquadId`)
) AS `s` ON (`u`.`Nickname` = `s`.`LeaderNickname`) AND (`u`.`SquadId` = `s`.`LeaderSquadId`)
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `s`.`Nickname`, `s`.`SquadId`, `s`.`Nickname0`
""");
    }

    public override async Task ThenInclude_reference_on_derived_after_derived_collection(bool async)
    {
        await base.ThenInclude_reference_on_derived_after_derived_collection(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`CapitalName`, `l`.`Name`, `l`.`ServerAddress`, `l`.`CommanderName`, `l`.`Eradicated`, `s`.`Name`, `s`.`LocustHordeId`, `s`.`ThreatLevel`, `s`.`ThreatLevelByte`, `s`.`ThreatLevelNullableByte`, `s`.`DefeatedByNickname`, `s`.`DefeatedBySquadId`, `s`.`HighCommandId`, `s`.`Discriminator`, `s`.`Nickname`, `s`.`SquadId`, `s`.`AssignedCityName`, `s`.`CityOfBirthName`, `s`.`FullName`, `s`.`HasSoulPatch`, `s`.`LeaderNickname`, `s`.`LeaderSquadId`, `s`.`Rank`, `s`.`Discriminator0`
FROM `LocustHordes` AS `l`
LEFT JOIN (
    SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator` AS `Discriminator0`
    FROM (
        SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
        FROM `LocustLeaders` AS `l0`
        UNION ALL
        SELECT `l1`.`Name`, `l1`.`LocustHordeId`, `l1`.`ThreatLevel`, `l1`.`ThreatLevelByte`, `l1`.`ThreatLevelNullableByte`, `l1`.`DefeatedByNickname`, `l1`.`DefeatedBySquadId`, `l1`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
        FROM `LocustCommanders` AS `l1`
    ) AS `u`
    LEFT JOIN (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o`
    ) AS `u0` ON (`u`.`DefeatedByNickname` = `u0`.`Nickname`) AND (`u`.`DefeatedBySquadId` = `u0`.`SquadId`)
) AS `s` ON `l`.`Id` = `s`.`LocustHordeId`
ORDER BY `l`.`Id`, `s`.`Name`, `s`.`Nickname`
""");
    }

    public override async Task Multiple_derived_included_on_one_method(bool async)
    {
        await base.Multiple_derived_included_on_one_method(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`CapitalName`, `l`.`Name`, `l`.`ServerAddress`, `l`.`CommanderName`, `l`.`Eradicated`, `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM `LocustHordes` AS `l`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`l0`.`DefeatedByNickname` = `u`.`Nickname`) AND (`l0`.`DefeatedBySquadId` = `u`.`SquadId`)
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON ((`u`.`Nickname` = `u0`.`LeaderNickname`) OR (`u`.`Nickname` IS NULL AND (`u0`.`LeaderNickname` IS NULL))) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY `l`.`Id`, `l0`.`Name`, `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task Include_on_derived_multi_level(bool async)
    {
        await base.Include_on_derived_multi_level(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `s1`.`Nickname`, `s1`.`SquadId`, `s1`.`AssignedCityName`, `s1`.`CityOfBirthName`, `s1`.`FullName`, `s1`.`HasSoulPatch`, `s1`.`LeaderNickname`, `s1`.`LeaderSquadId`, `s1`.`Rank`, `s1`.`Discriminator`, `s1`.`Id`, `s1`.`Banner`, `s1`.`Banner5`, `s1`.`InternalNumber`, `s1`.`Name`, `s1`.`SquadId0`, `s1`.`MissionId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`, `s0`.`SquadId` AS `SquadId0`, `s0`.`MissionId`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o0`
    ) AS `u0`
    INNER JOIN `Squads` AS `s` ON `u0`.`SquadId` = `s`.`Id`
    LEFT JOIN `SquadMissions` AS `s0` ON `s`.`Id` = `s0`.`SquadId`
) AS `s1` ON (`u`.`Nickname` = `s1`.`LeaderNickname`) AND (`u`.`SquadId` = `s1`.`LeaderSquadId`)
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `s1`.`Nickname`, `s1`.`SquadId`, `s1`.`Id`, `s1`.`SquadId0`
""");
    }

    public override async Task Projecting_nullable_bool_in_conditional_works(bool async)
    {
        await base.Projecting_nullable_bool_in_conditional_works(async);

        AssertSql(
"""
SELECT CASE
    WHEN `u`.`Nickname` IS NOT NULL AND (`u`.`SquadId` IS NOT NULL) THEN `u`.`HasSoulPatch`
    ELSE FALSE
END AS `Prop`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
""");
    }

    public override async Task ToString_enum_property_projection(bool async)
    {
        await base.ToString_enum_property_projection(async);

        AssertSql(
"""
SELECT CASE `u`.`Rank`
    WHEN 0 THEN 'None'
    WHEN 1 THEN 'Private'
    WHEN 2 THEN 'Corporal'
    WHEN 4 THEN 'Sergeant'
    WHEN 8 THEN 'Lieutenant'
    WHEN 16 THEN 'Captain'
    WHEN 32 THEN 'Major'
    WHEN 64 THEN 'Colonel'
    WHEN 128 THEN 'General'
    ELSE CAST(`u`.`Rank` AS char)
END
FROM (
    SELECT `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task ToString_nullable_enum_property_projection(bool async)
    {
        await base.ToString_nullable_enum_property_projection(async);

        AssertSql(
"""
SELECT CASE `w`.`AmmunitionType`
    WHEN 1 THEN 'Cartridge'
    WHEN 2 THEN 'Shell'
    ELSE COALESCE(CAST(`w`.`AmmunitionType` AS char), '')
END
FROM `Weapons` AS `w`
""");
    }

    public override async Task ToString_enum_contains(bool async)
    {
        await base.ToString_enum_contains(async);

        AssertSql(
"""
SELECT `m`.`CodeName`
FROM `Missions` AS `m`
WHERE CAST(`m`.`Difficulty` AS char) LIKE '%Med%'
""");
    }

    public override async Task ToString_nullable_enum_contains(bool async)
    {
        await base.ToString_nullable_enum_contains(async);

        AssertSql(
"""
SELECT `w`.`Name`
FROM `Weapons` AS `w`
WHERE CASE `w`.`AmmunitionType`
    WHEN 1 THEN 'Cartridge'
    WHEN 2 THEN 'Shell'
    ELSE COALESCE(CAST(`w`.`AmmunitionType` AS char), '')
END LIKE '%Cart%'
""");
    }

    public override async Task Correlated_collections_naked_navigation_with_ToList(bool async)
    {
        await base.Correlated_collections_naked_navigation_with_ToList(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
WHERE `u`.`Nickname` <> 'Marcus'
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Correlated_collections_naked_navigation_with_ToList_followed_by_projecting_count(bool async)
    {
        await base.Correlated_collections_naked_navigation_with_ToList_followed_by_projecting_count(async);

        AssertSql(
"""
SELECT (
    SELECT COUNT(*)
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`)
FROM (
    SELECT `g`.`Nickname`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`Nickname` <> 'Marcus'
ORDER BY `u`.`Nickname`
""");
    }

    public override async Task Correlated_collections_naked_navigation_with_ToArray(bool async)
    {
        await base.Correlated_collections_naked_navigation_with_ToArray(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
WHERE `u`.`Nickname` <> 'Marcus'
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Correlated_collections_basic_projection(bool async)
    {
        await base.Correlated_collections_basic_projection(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
    FROM `Weapons` AS `w`
    WHERE (`w`.`IsAutomatic` = TRUE) OR ((`w`.`Name` <> 'foo') OR `w`.`Name` IS NULL)
) AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
WHERE `u`.`Nickname` <> 'Marcus'
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Correlated_collections_basic_projection_explicit_to_list(bool async)
    {
        await base.Correlated_collections_basic_projection_explicit_to_list(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
    FROM `Weapons` AS `w`
    WHERE (`w`.`IsAutomatic` = TRUE) OR ((`w`.`Name` <> 'foo') OR `w`.`Name` IS NULL)
) AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
WHERE `u`.`Nickname` <> 'Marcus'
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Correlated_collections_basic_projection_explicit_to_array(bool async)
    {
        await base.Correlated_collections_basic_projection_explicit_to_array(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
    FROM `Weapons` AS `w`
    WHERE (`w`.`IsAutomatic` = TRUE) OR ((`w`.`Name` <> 'foo') OR `w`.`Name` IS NULL)
) AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
WHERE `u`.`Nickname` <> 'Marcus'
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Correlated_collections_basic_projection_ordered(bool async)
    {
        await base.Correlated_collections_basic_projection_ordered(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
    FROM `Weapons` AS `w`
    WHERE (`w`.`IsAutomatic` = TRUE) OR ((`w`.`Name` <> 'foo') OR `w`.`Name` IS NULL)
) AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
WHERE `u`.`Nickname` <> 'Marcus'
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `w0`.`Name` DESC
""");
    }

    public override async Task Correlated_collections_basic_projection_composite_key(bool async)
    {
        await base.Correlated_collections_basic_projection_composite_key(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u1`.`Nickname`, `u1`.`FullName`, `u1`.`SquadId`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `u0`.`Nickname`, `u0`.`FullName`, `u0`.`SquadId`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`
        FROM `Officers` AS `o0`
    ) AS `u0`
    WHERE `u0`.`HasSoulPatch` = FALSE
) AS `u1` ON (`u`.`Nickname` = `u1`.`LeaderNickname`) AND (`u`.`SquadId` = `u1`.`LeaderSquadId`)
WHERE `u`.`Nickname` <> 'Foo'
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u1`.`Nickname`
""");
    }

    public override async Task Correlated_collections_basic_projecting_single_property(bool async)
    {
        await base.Correlated_collections_basic_projecting_single_property(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w0`.`Name`, `w0`.`Id`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `w`.`Name`, `w`.`Id`, `w`.`OwnerFullName`
    FROM `Weapons` AS `w`
    WHERE (`w`.`IsAutomatic` = TRUE) OR ((`w`.`Name` <> 'foo') OR `w`.`Name` IS NULL)
) AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
WHERE `u`.`Nickname` <> 'Marcus'
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Correlated_collections_basic_projecting_constant(bool async)
    {
        await base.Correlated_collections_basic_projecting_constant(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w0`.`c`, `w0`.`Id`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT 'BFG' AS `c`, `w`.`Id`, `w`.`OwnerFullName`
    FROM `Weapons` AS `w`
    WHERE (`w`.`IsAutomatic` = TRUE) OR ((`w`.`Name` <> 'foo') OR `w`.`Name` IS NULL)
) AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
WHERE `u`.`Nickname` <> 'Marcus'
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Correlated_collections_basic_projecting_constant_bool(bool async)
    {
        await base.Correlated_collections_basic_projecting_constant_bool(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w0`.`c`, `w0`.`Id`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT TRUE AS `c`, `w`.`Id`, `w`.`OwnerFullName`
    FROM `Weapons` AS `w`
    WHERE (`w`.`IsAutomatic` = TRUE) OR ((`w`.`Name` <> 'foo') OR `w`.`Name` IS NULL)
) AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
WHERE `u`.`Nickname` <> 'Marcus'
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Correlated_collections_projection_of_collection_thru_navigation(bool async)
    {
        await base.Correlated_collections_projection_of_collection_thru_navigation(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `s`.`Id`, `s1`.`SquadId`, `s1`.`MissionId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
LEFT JOIN (
    SELECT `s0`.`SquadId`, `s0`.`MissionId`
    FROM `SquadMissions` AS `s0`
    WHERE `s0`.`MissionId` <> 17
) AS `s1` ON `s`.`Id` = `s1`.`SquadId`
WHERE `u`.`Nickname` <> 'Marcus'
ORDER BY `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `s`.`Id`, `s1`.`SquadId`
""");
    }

    public override async Task Correlated_collections_project_anonymous_collection_result(bool async)
    {
        await base.Correlated_collections_project_anonymous_collection_result(async);

        AssertSql(
"""
SELECT `s`.`Name`, `s`.`Id`, `u`.`FullName`, `u`.`Rank`, `u`.`Nickname`, `u`.`SquadId`
FROM `Squads` AS `s`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`, `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`, `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u` ON `s`.`Id` = `u`.`SquadId`
WHERE `s`.`Id` < 20
ORDER BY `s`.`Id`, `u`.`Nickname`
""");
    }

    public override async Task Correlated_collections_nested(bool async)
    {
        await base.Correlated_collections_nested(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s3`.`SquadId`, `s3`.`MissionId`, `s3`.`Id`, `s3`.`SquadId0`, `s3`.`MissionId0`
FROM `Squads` AS `s`
LEFT JOIN (
    SELECT `s0`.`SquadId`, `s0`.`MissionId`, `m`.`Id`, `s2`.`SquadId` AS `SquadId0`, `s2`.`MissionId` AS `MissionId0`
    FROM `SquadMissions` AS `s0`
    INNER JOIN `Missions` AS `m` ON `s0`.`MissionId` = `m`.`Id`
    LEFT JOIN (
        SELECT `s1`.`SquadId`, `s1`.`MissionId`
        FROM `SquadMissions` AS `s1`
        WHERE `s1`.`SquadId` < 7
    ) AS `s2` ON `m`.`Id` = `s2`.`MissionId`
    WHERE `s0`.`MissionId` < 42
) AS `s3` ON `s`.`Id` = `s3`.`SquadId`
ORDER BY `s`.`Id`, `s3`.`SquadId`, `s3`.`MissionId`, `s3`.`Id`, `s3`.`SquadId0`
""");
    }

    public override async Task Correlated_collections_nested_mixed_streaming_with_buffer1(bool async)
    {
        await base.Correlated_collections_nested_mixed_streaming_with_buffer1(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s3`.`SquadId`, `s3`.`MissionId`, `s3`.`Id`, `s3`.`SquadId0`, `s3`.`MissionId0`
FROM `Squads` AS `s`
LEFT JOIN (
    SELECT `s0`.`SquadId`, `s0`.`MissionId`, `m`.`Id`, `s2`.`SquadId` AS `SquadId0`, `s2`.`MissionId` AS `MissionId0`
    FROM `SquadMissions` AS `s0`
    INNER JOIN `Missions` AS `m` ON `s0`.`MissionId` = `m`.`Id`
    LEFT JOIN (
        SELECT `s1`.`SquadId`, `s1`.`MissionId`
        FROM `SquadMissions` AS `s1`
        WHERE `s1`.`SquadId` < 2
    ) AS `s2` ON `m`.`Id` = `s2`.`MissionId`
    WHERE `s0`.`MissionId` < 3
) AS `s3` ON `s`.`Id` = `s3`.`SquadId`
ORDER BY `s`.`Id`, `s3`.`SquadId`, `s3`.`MissionId`, `s3`.`Id`, `s3`.`SquadId0`
""");
    }

    public override async Task Correlated_collections_nested_mixed_streaming_with_buffer2(bool async)
    {
        await base.Correlated_collections_nested_mixed_streaming_with_buffer2(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s3`.`SquadId`, `s3`.`MissionId`, `s3`.`Id`, `s3`.`SquadId0`, `s3`.`MissionId0`
FROM `Squads` AS `s`
LEFT JOIN (
    SELECT `s0`.`SquadId`, `s0`.`MissionId`, `m`.`Id`, `s2`.`SquadId` AS `SquadId0`, `s2`.`MissionId` AS `MissionId0`
    FROM `SquadMissions` AS `s0`
    INNER JOIN `Missions` AS `m` ON `s0`.`MissionId` = `m`.`Id`
    LEFT JOIN (
        SELECT `s1`.`SquadId`, `s1`.`MissionId`
        FROM `SquadMissions` AS `s1`
        WHERE `s1`.`SquadId` < 7
    ) AS `s2` ON `m`.`Id` = `s2`.`MissionId`
    WHERE `s0`.`MissionId` < 42
) AS `s3` ON `s`.`Id` = `s3`.`SquadId`
ORDER BY `s`.`Id`, `s3`.`SquadId`, `s3`.`MissionId`, `s3`.`Id`, `s3`.`SquadId0`
""");
    }

    public override async Task Correlated_collections_nested_with_custom_ordering(bool async)
    {
        await base.Correlated_collections_nested_with_custom_ordering(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `s`.`FullName`, `s`.`Nickname`, `s`.`SquadId`, `s`.`Id`, `s`.`AmmunitionType`, `s`.`IsAutomatic`, `s`.`Name`, `s`.`OwnerFullName`, `s`.`SynergyWithId`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `u0`.`FullName`, `u0`.`Nickname`, `u0`.`SquadId`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`, `u0`.`Rank`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`
        FROM `Officers` AS `o0`
    ) AS `u0`
    LEFT JOIN (
        SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE (`w`.`Name` <> 'Bar') OR `w`.`Name` IS NULL
    ) AS `w0` ON `u0`.`FullName` = `w0`.`OwnerFullName`
    WHERE `u0`.`FullName` <> 'Foo'
) AS `s` ON (`u`.`Nickname` = `s`.`LeaderNickname`) AND (`u`.`SquadId` = `s`.`LeaderSquadId`)
ORDER BY `u`.`HasSoulPatch` DESC, `u`.`Nickname`, `u`.`SquadId`, `s`.`Rank`, `s`.`Nickname`, `s`.`SquadId`, `s`.`IsAutomatic`
""");
    }

    public override async Task Correlated_collections_same_collection_projected_multiple_times(bool async)
    {
        await base.Correlated_collections_same_collection_projected_multiple_times(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `w1`.`Id`, `w1`.`AmmunitionType`, `w1`.`IsAutomatic`, `w1`.`Name`, `w1`.`OwnerFullName`, `w1`.`SynergyWithId`, `w2`.`Id`, `w2`.`AmmunitionType`, `w2`.`IsAutomatic`, `w2`.`Name`, `w2`.`OwnerFullName`, `w2`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
    FROM `Weapons` AS `w`
    WHERE `w`.`IsAutomatic` = TRUE
) AS `w1` ON `u`.`FullName` = `w1`.`OwnerFullName`
LEFT JOIN (
    SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
    FROM `Weapons` AS `w0`
    WHERE `w0`.`IsAutomatic` = TRUE
) AS `w2` ON `u`.`FullName` = `w2`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `w1`.`Id`
""");
    }

    public override async Task Correlated_collections_similar_collection_projected_multiple_times(bool async)
    {
        await base.Correlated_collections_similar_collection_projected_multiple_times(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `w1`.`Id`, `w1`.`AmmunitionType`, `w1`.`IsAutomatic`, `w1`.`Name`, `w1`.`OwnerFullName`, `w1`.`SynergyWithId`, `w2`.`Id`, `w2`.`AmmunitionType`, `w2`.`IsAutomatic`, `w2`.`Name`, `w2`.`OwnerFullName`, `w2`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`, `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`, `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
    FROM `Weapons` AS `w`
    WHERE `w`.`IsAutomatic` = TRUE
) AS `w1` ON `u`.`FullName` = `w1`.`OwnerFullName`
LEFT JOIN (
    SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
    FROM `Weapons` AS `w0`
    WHERE `w0`.`IsAutomatic` = FALSE
) AS `w2` ON `u`.`FullName` = `w2`.`OwnerFullName`
ORDER BY `u`.`Rank`, `u`.`Nickname`, `u`.`SquadId`, `w1`.`OwnerFullName`, `w1`.`Id`, `w2`.`IsAutomatic`
""");
    }

    public override async Task Correlated_collections_different_collections_projected(bool async)
    {
        await base.Correlated_collections_different_collections_projected(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w0`.`Name`, `w0`.`IsAutomatic`, `w0`.`Id`, `u0`.`Nickname`, `u0`.`Rank`, `u0`.`SquadId`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `w`.`Name`, `w`.`IsAutomatic`, `w`.`Id`, `w`.`OwnerFullName`
    FROM `Weapons` AS `w`
    WHERE `w`.`IsAutomatic` = TRUE
) AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`
    FROM `Officers` AS `o0`
) AS `u0` ON (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `w0`.`Id`, `u0`.`FullName`, `u0`.`Nickname`
""");
    }

    public override async Task Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys(bool async)
    {
        await base.Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys(async);

        AssertSql(
"""
SELECT `u`.`FullName`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `g`.`LeaderNickname`, `g`.`LeaderSquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`
        FROM `Officers` AS `o0`
    ) AS `u0`
    WHERE (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`))
ORDER BY `u`.`HasSoulPatch` DESC, `t`.`Note`
""");
    }

    public override async Task Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys_inside_subquery(bool async)
    {
        await base.Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys_inside_subquery(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `t`.`Id`, `u1`.`Nickname`, `u1`.`SquadId`, `s`.`Id`, `s`.`AmmunitionType`, `s`.`IsAutomatic`, `s`.`Name`, `s`.`OwnerFullName`, `s`.`SynergyWithId`, `s`.`Nickname`, `s`.`SquadId`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`FullName`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o1`.`Nickname`, `o1`.`SquadId`, `o1`.`FullName`
    FROM `Officers` AS `o1`
) AS `u1` ON (`t`.`GearNickName` = `u1`.`Nickname`) AND (`t`.`GearSquadId` = `u1`.`SquadId`)
LEFT JOIN (
    SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `u2`.`Nickname`, `u2`.`SquadId`
    FROM `Weapons` AS `w`
    LEFT JOIN (
        SELECT `g1`.`Nickname`, `g1`.`SquadId`, `g1`.`FullName`
        FROM `Gears` AS `g1`
        UNION ALL
        SELECT `o2`.`Nickname`, `o2`.`SquadId`, `o2`.`FullName`
        FROM `Officers` AS `o2`
    ) AS `u2` ON `w`.`OwnerFullName` = `u2`.`FullName`
) AS `s` ON `u1`.`FullName` = `s`.`OwnerFullName`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `g`.`LeaderNickname`, `g`.`LeaderSquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`
        FROM `Officers` AS `o0`
    ) AS `u0`
    WHERE (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`))
ORDER BY `u`.`HasSoulPatch` DESC, `t`.`Note`, `u`.`Nickname`, `u`.`SquadId`, `t`.`Id`, `u1`.`Nickname`, `u1`.`SquadId`, `s`.`IsAutomatic`, `s`.`Nickname` DESC, `s`.`Id`
""");
    }

    public override async Task Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys_inside_subquery_duplicated_orderings(
        bool async)
    {
        await base.Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys_inside_subquery_duplicated_orderings(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `t`.`Id`, `u1`.`Nickname`, `u1`.`SquadId`, `s`.`Id`, `s`.`AmmunitionType`, `s`.`IsAutomatic`, `s`.`Name`, `s`.`OwnerFullName`, `s`.`SynergyWithId`, `s`.`Nickname`, `s`.`SquadId`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`FullName`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o1`.`Nickname`, `o1`.`SquadId`, `o1`.`FullName`
    FROM `Officers` AS `o1`
) AS `u1` ON (`t`.`GearNickName` = `u1`.`Nickname`) AND (`t`.`GearSquadId` = `u1`.`SquadId`)
LEFT JOIN (
    SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `u2`.`Nickname`, `u2`.`SquadId`
    FROM `Weapons` AS `w`
    LEFT JOIN (
        SELECT `g1`.`Nickname`, `g1`.`SquadId`, `g1`.`FullName`
        FROM `Gears` AS `g1`
        UNION ALL
        SELECT `o2`.`Nickname`, `o2`.`SquadId`, `o2`.`FullName`
        FROM `Officers` AS `o2`
    ) AS `u2` ON `w`.`OwnerFullName` = `u2`.`FullName`
) AS `s` ON `u1`.`FullName` = `s`.`OwnerFullName`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `g`.`LeaderNickname`, `g`.`LeaderSquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`
        FROM `Officers` AS `o0`
    ) AS `u0`
    WHERE (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`))
ORDER BY `u`.`HasSoulPatch` DESC, `t`.`Note`, `u`.`Nickname`, `u`.`SquadId`, `t`.`Id`, `u1`.`Nickname`, `u1`.`SquadId`, `s`.`IsAutomatic`, `s`.`Nickname` DESC, `s`.`Id`
""");
    }

    public override async Task Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys_inside_subquery_complex_orderings(
        bool async)
    {
        await base.Multiple_orderby_with_navigation_expansion_on_one_of_the_order_bys_inside_subquery_complex_orderings(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `t`.`Id`, `u1`.`Nickname`, `u1`.`SquadId`, `s`.`Id`, `s`.`AmmunitionType`, `s`.`IsAutomatic`, `s`.`Name`, `s`.`OwnerFullName`, `s`.`SynergyWithId`, `s`.`Nickname`, `s`.`SquadId`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`FullName`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o1`.`Nickname`, `o1`.`SquadId`, `o1`.`FullName`
    FROM `Officers` AS `o1`
) AS `u1` ON (`t`.`GearNickName` = `u1`.`Nickname`) AND (`t`.`GearSquadId` = `u1`.`SquadId`)
LEFT JOIN (
    SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `u2`.`Nickname`, `u2`.`SquadId`, (
        SELECT COUNT(*)
        FROM `Weapons` AS `w0`
        WHERE `u2`.`FullName` IS NOT NULL AND (`u2`.`FullName` = `w0`.`OwnerFullName`)) AS `c`
    FROM `Weapons` AS `w`
    LEFT JOIN (
        SELECT `g1`.`Nickname`, `g1`.`SquadId`, `g1`.`FullName`
        FROM `Gears` AS `g1`
        UNION ALL
        SELECT `o2`.`Nickname`, `o2`.`SquadId`, `o2`.`FullName`
        FROM `Officers` AS `o2`
    ) AS `u2` ON `w`.`OwnerFullName` = `u2`.`FullName`
) AS `s` ON `u1`.`FullName` = `s`.`OwnerFullName`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `g`.`LeaderNickname`, `g`.`LeaderSquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`
        FROM `Officers` AS `o0`
    ) AS `u0`
    WHERE (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`))
ORDER BY `u`.`HasSoulPatch` DESC, `t`.`Note`, `u`.`Nickname`, `u`.`SquadId`, `t`.`Id`, `u1`.`Nickname`, `u1`.`SquadId`, `s`.`Id` DESC, `s`.`c`, `s`.`Nickname`
""");
    }

    public override async Task Correlated_collections_multiple_nested_complex_collections(bool async)
    {
        await base.Correlated_collections_multiple_nested_complex_collections(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `t`.`Id`, `u1`.`Nickname`, `u1`.`SquadId`, `s1`.`FullName`, `s1`.`Nickname`, `s1`.`SquadId`, `s1`.`Id`, `s1`.`Nickname0`, `s1`.`SquadId0`, `s1`.`Id0`, `s1`.`Name`, `s1`.`IsAutomatic`, `s1`.`Id1`, `s1`.`Nickname00`, `s1`.`HasSoulPatch`, `s1`.`SquadId00`, `s2`.`Id`, `s2`.`AmmunitionType`, `s2`.`IsAutomatic`, `s2`.`Name`, `s2`.`OwnerFullName`, `s2`.`SynergyWithId`, `s2`.`Nickname`, `s2`.`SquadId`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`FullName`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o1`.`Nickname`, `o1`.`SquadId`, `o1`.`FullName`
    FROM `Officers` AS `o1`
) AS `u1` ON (`t`.`GearNickName` = `u1`.`Nickname`) AND (`t`.`GearSquadId` = `u1`.`SquadId`)
LEFT JOIN (
    SELECT `u2`.`FullName`, `u2`.`Nickname`, `u2`.`SquadId`, `s0`.`Id`, `s0`.`Nickname` AS `Nickname0`, `s0`.`SquadId` AS `SquadId0`, `s0`.`Id0`, `s0`.`Name`, `s0`.`IsAutomatic`, `s0`.`Id1`, `s0`.`Nickname0` AS `Nickname00`, `s0`.`HasSoulPatch`, `s0`.`SquadId0` AS `SquadId00`, `u2`.`Rank`, `s0`.`IsAutomatic0`, `u2`.`LeaderNickname`, `u2`.`LeaderSquadId`
    FROM (
        SELECT `g1`.`Nickname`, `g1`.`SquadId`, `g1`.`FullName`, `g1`.`LeaderNickname`, `g1`.`LeaderSquadId`, `g1`.`Rank`
        FROM `Gears` AS `g1`
        UNION ALL
        SELECT `o2`.`Nickname`, `o2`.`SquadId`, `o2`.`FullName`, `o2`.`LeaderNickname`, `o2`.`LeaderSquadId`, `o2`.`Rank`
        FROM `Officers` AS `o2`
    ) AS `u2`
    LEFT JOIN (
        SELECT `w`.`Id`, `u3`.`Nickname`, `u3`.`SquadId`, `s`.`Id` AS `Id0`, `w0`.`Name`, `w0`.`IsAutomatic`, `w0`.`Id` AS `Id1`, `u4`.`Nickname` AS `Nickname0`, `u4`.`HasSoulPatch`, `u4`.`SquadId` AS `SquadId0`, `w`.`IsAutomatic` AS `IsAutomatic0`, `w`.`OwnerFullName`
        FROM `Weapons` AS `w`
        LEFT JOIN (
            SELECT `g2`.`Nickname`, `g2`.`SquadId`, `g2`.`FullName`
            FROM `Gears` AS `g2`
            UNION ALL
            SELECT `o3`.`Nickname`, `o3`.`SquadId`, `o3`.`FullName`
            FROM `Officers` AS `o3`
        ) AS `u3` ON `w`.`OwnerFullName` = `u3`.`FullName`
        LEFT JOIN `Squads` AS `s` ON `u3`.`SquadId` = `s`.`Id`
        LEFT JOIN `Weapons` AS `w0` ON `u3`.`FullName` = `w0`.`OwnerFullName`
        LEFT JOIN (
            SELECT `g3`.`Nickname`, `g3`.`SquadId`, `g3`.`HasSoulPatch`
            FROM `Gears` AS `g3`
            UNION ALL
            SELECT `o4`.`Nickname`, `o4`.`SquadId`, `o4`.`HasSoulPatch`
            FROM `Officers` AS `o4`
        ) AS `u4` ON `s`.`Id` = `u4`.`SquadId`
        WHERE (`w`.`Name` <> 'Bar') OR `w`.`Name` IS NULL
    ) AS `s0` ON `u2`.`FullName` = `s0`.`OwnerFullName`
    WHERE `u2`.`FullName` <> 'Foo'
) AS `s1` ON (`u`.`Nickname` = `s1`.`LeaderNickname`) AND (`u`.`SquadId` = `s1`.`LeaderSquadId`)
LEFT JOIN (
    SELECT `w1`.`Id`, `w1`.`AmmunitionType`, `w1`.`IsAutomatic`, `w1`.`Name`, `w1`.`OwnerFullName`, `w1`.`SynergyWithId`, `u5`.`Nickname`, `u5`.`SquadId`
    FROM `Weapons` AS `w1`
    LEFT JOIN (
        SELECT `g4`.`Nickname`, `g4`.`SquadId`, `g4`.`FullName`
        FROM `Gears` AS `g4`
        UNION ALL
        SELECT `o5`.`Nickname`, `o5`.`SquadId`, `o5`.`FullName`
        FROM `Officers` AS `o5`
    ) AS `u5` ON `w1`.`OwnerFullName` = `u5`.`FullName`
) AS `s2` ON `u1`.`FullName` = `s2`.`OwnerFullName`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `g`.`LeaderNickname`, `g`.`LeaderSquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`
        FROM `Officers` AS `o0`
    ) AS `u0`
    WHERE (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`))
ORDER BY `u`.`HasSoulPatch` DESC, `t`.`Note`, `u`.`Nickname`, `u`.`SquadId`, `t`.`Id`, `u1`.`Nickname`, `u1`.`SquadId`, `s1`.`Rank`, `s1`.`Nickname`, `s1`.`SquadId`, `s1`.`IsAutomatic0`, `s1`.`Id`, `s1`.`Nickname0`, `s1`.`SquadId0`, `s1`.`Id0`, `s1`.`Id1`, `s1`.`Nickname00`, `s1`.`SquadId00`, `s2`.`IsAutomatic`, `s2`.`Nickname` DESC, `s2`.`Id`
""");
    }

    public override async Task Correlated_collections_inner_subquery_selector_references_outer_qsre(bool async)
    {
        await base.Correlated_collections_inner_subquery_selector_references_outer_qsre(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `u1`.`ReportName`, `u1`.`OfficerName`, `u1`.`Nickname`, `u1`.`SquadId`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN LATERAL (
    SELECT `u0`.`FullName` AS `ReportName`, `u`.`FullName` AS `OfficerName`, `u0`.`Nickname`, `u0`.`SquadId`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`
        FROM `Officers` AS `o0`
    ) AS `u0`
    WHERE (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
) AS `u1` ON TRUE
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u1`.`Nickname`
""");
    }

    public override async Task Correlated_collections_inner_subquery_predicate_references_outer_qsre(bool async)
    {
        await base.Correlated_collections_inner_subquery_predicate_references_outer_qsre(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `u1`.`ReportName`, `u1`.`Nickname`, `u1`.`SquadId`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN LATERAL (
    SELECT `u0`.`FullName` AS `ReportName`, `u0`.`Nickname`, `u0`.`SquadId`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`
        FROM `Officers` AS `o0`
    ) AS `u0`
    WHERE ((`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)) AND (`u`.`FullName` <> 'Foo')
) AS `u1` ON TRUE
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u1`.`Nickname`
""");
    }

    public override async Task Correlated_collections_nested_inner_subquery_references_outer_qsre_one_level_up(bool async)
    {
        await base.Correlated_collections_nested_inner_subquery_references_outer_qsre_one_level_up(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `s`.`FullName`, `s`.`Nickname`, `s`.`SquadId`, `s`.`Name`, `s`.`Nickname0`, `s`.`Id`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `u0`.`FullName`, `u0`.`Nickname`, `u0`.`SquadId`, `w0`.`Name`, `w0`.`Nickname` AS `Nickname0`, `w0`.`Id`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`
        FROM `Officers` AS `o0`
    ) AS `u0`
    LEFT JOIN LATERAL (
        SELECT `w`.`Name`, `u0`.`Nickname`, `w`.`Id`
        FROM `Weapons` AS `w`
        WHERE (`u0`.`FullName` = `w`.`OwnerFullName`) AND ((`w`.`Name` <> 'Bar') OR `w`.`Name` IS NULL)
    ) AS `w0` ON TRUE
    WHERE `u0`.`FullName` <> 'Foo'
) AS `s` ON (`u`.`Nickname` = `s`.`LeaderNickname`) AND (`u`.`SquadId` = `s`.`LeaderSquadId`)
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `s`.`Nickname`, `s`.`SquadId`
""");
    }

    public override async Task Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(bool async)
    {
        await base.Correlated_collections_nested_inner_subquery_references_outer_qsre_two_levels_up(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `s`.`FullName`, `s`.`Nickname`, `s`.`SquadId`, `s`.`Name`, `s`.`Nickname0`, `s`.`Id`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN LATERAL (
    SELECT `u0`.`FullName`, `u0`.`Nickname`, `u0`.`SquadId`, `w0`.`Name`, `w0`.`Nickname` AS `Nickname0`, `w0`.`Id`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`
        FROM `Officers` AS `o0`
    ) AS `u0`
    LEFT JOIN (
        SELECT `w`.`Name`, `u`.`Nickname`, `w`.`Id`, `w`.`OwnerFullName`
        FROM `Weapons` AS `w`
        WHERE (`w`.`Name` <> 'Bar') OR `w`.`Name` IS NULL
    ) AS `w0` ON `u0`.`FullName` = `w0`.`OwnerFullName`
    WHERE ((`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)) AND (`u0`.`FullName` <> 'Foo')
) AS `s` ON TRUE
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `s`.`Nickname`, `s`.`SquadId`
""");
    }

    public override async Task Correlated_collections_on_select_many(bool async)
    {
        await base.Correlated_collections_on_select_many(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `s`.`Name`, `u`.`SquadId`, `s`.`Id`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`, `u1`.`Nickname`, `u1`.`SquadId`, `u1`.`AssignedCityName`, `u1`.`CityOfBirthName`, `u1`.`FullName`, `u1`.`HasSoulPatch`, `u1`.`LeaderNickname`, `u1`.`LeaderSquadId`, `u1`.`Rank`, `u1`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
CROSS JOIN `Squads` AS `s`
LEFT JOIN (
    SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
    FROM `Weapons` AS `w`
    WHERE (`w`.`IsAutomatic` = TRUE) OR ((`w`.`Name` <> 'foo') OR `w`.`Name` IS NULL)
) AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
LEFT JOIN (
    SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o0`
    ) AS `u0`
    WHERE `u0`.`HasSoulPatch` = FALSE
) AS `u1` ON `s`.`Id` = `u1`.`SquadId`
WHERE `u`.`HasSoulPatch` = TRUE
ORDER BY `u`.`Nickname`, `s`.`Id` DESC, `u`.`SquadId`, `w0`.`Id`, `u1`.`Nickname`
""");
    }

    public override async Task Correlated_collections_with_Skip(bool async)
    {
        await base.Correlated_collections_with_Skip(async);

        AssertSql(
"""
SELECT `s`.`Id`, `u1`.`Nickname`, `u1`.`SquadId`, `u1`.`AssignedCityName`, `u1`.`CityOfBirthName`, `u1`.`FullName`, `u1`.`HasSoulPatch`, `u1`.`LeaderNickname`, `u1`.`LeaderSquadId`, `u1`.`Rank`, `u1`.`Discriminator`
FROM `Squads` AS `s`
LEFT JOIN (
    SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
    FROM (
        SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, ROW_NUMBER() OVER(PARTITION BY `u`.`SquadId` ORDER BY `u`.`Nickname`) AS `row`
        FROM (
            SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
            FROM `Gears` AS `g`
            UNION ALL
            SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
            FROM `Officers` AS `o`
        ) AS `u`
    ) AS `u0`
    WHERE 1 < `u0`.`row`
) AS `u1` ON `s`.`Id` = `u1`.`SquadId`
ORDER BY `s`.`Name`, `s`.`Id`, `u1`.`SquadId`, `u1`.`Nickname`
""");
    }

    public override async Task Correlated_collections_with_Take(bool async)
    {
        await base.Correlated_collections_with_Take(async);

        AssertSql(
"""
SELECT `s`.`Id`, `u1`.`Nickname`, `u1`.`SquadId`, `u1`.`AssignedCityName`, `u1`.`CityOfBirthName`, `u1`.`FullName`, `u1`.`HasSoulPatch`, `u1`.`LeaderNickname`, `u1`.`LeaderSquadId`, `u1`.`Rank`, `u1`.`Discriminator`
FROM `Squads` AS `s`
LEFT JOIN (
    SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
    FROM (
        SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, ROW_NUMBER() OVER(PARTITION BY `u`.`SquadId` ORDER BY `u`.`Nickname`) AS `row`
        FROM (
            SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
            FROM `Gears` AS `g`
            UNION ALL
            SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
            FROM `Officers` AS `o`
        ) AS `u`
    ) AS `u0`
    WHERE `u0`.`row` <= 2
) AS `u1` ON `s`.`Id` = `u1`.`SquadId`
ORDER BY `s`.`Name`, `s`.`Id`, `u1`.`SquadId`, `u1`.`Nickname`
""");
    }

    public override async Task Correlated_collections_with_Distinct(bool async)
    {
        await base.Correlated_collections_with_Distinct(async);

        AssertSql(
"""
SELECT `s`.`Id`, `u1`.`Nickname`, `u1`.`SquadId`, `u1`.`AssignedCityName`, `u1`.`CityOfBirthName`, `u1`.`FullName`, `u1`.`HasSoulPatch`, `u1`.`LeaderNickname`, `u1`.`LeaderSquadId`, `u1`.`Rank`, `u1`.`Discriminator`
FROM `Squads` AS `s`
LEFT JOIN LATERAL (
    SELECT DISTINCT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
    FROM (
        SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
        FROM (
            SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
            FROM `Gears` AS `g`
            UNION ALL
            SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
            FROM `Officers` AS `o`
        ) AS `u`
        WHERE `s`.`Id` = `u`.`SquadId`
        ORDER BY `u`.`Nickname`
        LIMIT 18446744073709551610 OFFSET 0
    ) AS `u0`
) AS `u1` ON TRUE
ORDER BY `s`.`Name`, `s`.`Id`, `u1`.`Nickname`
""");
    }

    public override async Task Correlated_collections_with_FirstOrDefault(bool async)
    {
        await base.Correlated_collections_with_FirstOrDefault(async);

        AssertSql(
"""
SELECT (
    SELECT `u`.`FullName`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE `s`.`Id` = `u`.`SquadId`
    ORDER BY `u`.`Nickname`
    LIMIT 1)
FROM `Squads` AS `s`
ORDER BY `s`.`Name`
""");
    }

    public override async Task Correlated_collections_on_left_join_with_predicate(bool async)
    {
        await base.Correlated_collections_on_left_join_with_predicate(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `t`.`Id`, `u`.`SquadId`, `w`.`Name`, `w`.`Id`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON `t`.`GearNickName` = `u`.`Nickname`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
WHERE `u`.`HasSoulPatch` = FALSE
ORDER BY `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Correlated_collections_on_left_join_with_null_value(bool async)
    {
        await base.Correlated_collections_on_left_join_with_null_value(async);

        AssertSql(
"""
SELECT `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`, `w`.`Name`, `w`.`Id`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u` ON `t`.`GearNickName` = `u`.`Nickname`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
ORDER BY `t`.`Note`, `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Correlated_collections_left_join_with_self_reference(bool async)
    {
        await base.Correlated_collections_left_join_with_self_reference(async);

        AssertSql(
"""
SELECT `t`.`Note`, `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`, `u0`.`FullName`, `u0`.`Nickname`, `u0`.`SquadId`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON `t`.`GearNickName` = `u`.`Nickname`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`
    FROM `Officers` AS `o0`
) AS `u0` ON ((`u`.`Nickname` = `u0`.`LeaderNickname`) OR (`u`.`Nickname` IS NULL AND (`u0`.`LeaderNickname` IS NULL))) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task Correlated_collections_deeply_nested_left_join(bool async)
    {
        await base.Correlated_collections_deeply_nested_left_join(async);

        AssertSql(
"""
SELECT `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`, `s`.`Id`, `s0`.`Nickname`, `s0`.`SquadId`, `s0`.`Id`, `s0`.`AmmunitionType`, `s0`.`IsAutomatic`, `s0`.`Name`, `s0`.`OwnerFullName`, `s0`.`SynergyWithId`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON `t`.`GearNickName` = `u`.`Nickname`
LEFT JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
LEFT JOIN (
    SELECT `u0`.`Nickname`, `u0`.`SquadId`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`FullName`, `g0`.`HasSoulPatch`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`, `o0`.`HasSoulPatch`
        FROM `Officers` AS `o0`
    ) AS `u0`
    LEFT JOIN (
        SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE `w`.`IsAutomatic` = TRUE
    ) AS `w0` ON `u0`.`FullName` = `w0`.`OwnerFullName`
    WHERE `u0`.`HasSoulPatch` = TRUE
) AS `s0` ON `s`.`Id` = `s0`.`SquadId`
ORDER BY `t`.`Note`, `u`.`Nickname` DESC, `t`.`Id`, `u`.`SquadId`, `s`.`Id`, `s0`.`Nickname`, `s0`.`SquadId`
""");
    }

    public override async Task Correlated_collections_from_left_join_with_additional_elements_projected_of_that_join(bool async)
    {
        await base.Correlated_collections_from_left_join_with_additional_elements_projected_of_that_join(async);

        AssertSql(
"""
SELECT `w`.`Id`, `u`.`Nickname`, `u`.`SquadId`, `s`.`Id`, `s0`.`Nickname`, `s0`.`SquadId`, `s0`.`Id`, `s0`.`AmmunitionType`, `s0`.`IsAutomatic`, `s0`.`Name`, `s0`.`OwnerFullName`, `s0`.`SynergyWithId`, `s0`.`Rank`
FROM `Weapons` AS `w`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u` ON `w`.`OwnerFullName` = `u`.`FullName`
LEFT JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
LEFT JOIN (
    SELECT `u0`.`Nickname`, `u0`.`SquadId`, `w1`.`Id`, `w1`.`AmmunitionType`, `w1`.`IsAutomatic`, `w1`.`Name`, `w1`.`OwnerFullName`, `w1`.`SynergyWithId`, `u0`.`Rank`, `u0`.`FullName`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`FullName`, `g0`.`Rank`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`, `o0`.`Rank`
        FROM `Officers` AS `o0`
    ) AS `u0`
    LEFT JOIN (
        SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
        FROM `Weapons` AS `w0`
        WHERE `w0`.`IsAutomatic` = FALSE
    ) AS `w1` ON `u0`.`FullName` = `w1`.`OwnerFullName`
) AS `s0` ON `s`.`Id` = `s0`.`SquadId`
ORDER BY `w`.`Name`, `w`.`Id`, `u`.`Nickname`, `u`.`SquadId`, `s`.`Id`, `s0`.`FullName` DESC, `s0`.`Nickname`, `s0`.`SquadId`, `s0`.`Id`
""");
    }

    public override async Task Correlated_collections_complex_scenario1(bool async)
    {
        await base.Correlated_collections_complex_scenario1(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `s0`.`Id`, `s0`.`Nickname`, `s0`.`SquadId`, `s0`.`Id0`, `s0`.`Nickname0`, `s0`.`HasSoulPatch`, `s0`.`SquadId0`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `w`.`Id`, `u0`.`Nickname`, `u0`.`SquadId`, `s`.`Id` AS `Id0`, `u1`.`Nickname` AS `Nickname0`, `u1`.`HasSoulPatch`, `u1`.`SquadId` AS `SquadId0`, `w`.`OwnerFullName`
    FROM `Weapons` AS `w`
    LEFT JOIN (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`FullName`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`
        FROM `Officers` AS `o0`
    ) AS `u0` ON `w`.`OwnerFullName` = `u0`.`FullName`
    LEFT JOIN `Squads` AS `s` ON `u0`.`SquadId` = `s`.`Id`
    LEFT JOIN (
        SELECT `g1`.`Nickname`, `g1`.`SquadId`, `g1`.`HasSoulPatch`
        FROM `Gears` AS `g1`
        UNION ALL
        SELECT `o1`.`Nickname`, `o1`.`SquadId`, `o1`.`HasSoulPatch`
        FROM `Officers` AS `o1`
    ) AS `u1` ON `s`.`Id` = `u1`.`SquadId`
) AS `s0` ON `u`.`FullName` = `s0`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `s0`.`Id`, `s0`.`Nickname`, `s0`.`SquadId`, `s0`.`Id0`, `s0`.`Nickname0`
""");
    }

    public override async Task Correlated_collections_complex_scenario2(bool async)
    {
        await base.Correlated_collections_complex_scenario2(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `s1`.`FullName`, `s1`.`Nickname`, `s1`.`SquadId`, `s1`.`Id`, `s1`.`Nickname0`, `s1`.`SquadId0`, `s1`.`Id0`, `s1`.`Nickname00`, `s1`.`HasSoulPatch`, `s1`.`SquadId00`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `u0`.`FullName`, `u0`.`Nickname`, `u0`.`SquadId`, `s0`.`Id`, `s0`.`Nickname` AS `Nickname0`, `s0`.`SquadId` AS `SquadId0`, `s0`.`Id0`, `s0`.`Nickname0` AS `Nickname00`, `s0`.`HasSoulPatch`, `s0`.`SquadId0` AS `SquadId00`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`
        FROM `Officers` AS `o0`
    ) AS `u0`
    LEFT JOIN (
        SELECT `w`.`Id`, `u1`.`Nickname`, `u1`.`SquadId`, `s`.`Id` AS `Id0`, `u2`.`Nickname` AS `Nickname0`, `u2`.`HasSoulPatch`, `u2`.`SquadId` AS `SquadId0`, `w`.`OwnerFullName`
        FROM `Weapons` AS `w`
        LEFT JOIN (
            SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`FullName`
            FROM `Gears` AS `g0`
            UNION ALL
            SELECT `o1`.`Nickname`, `o1`.`SquadId`, `o1`.`FullName`
            FROM `Officers` AS `o1`
        ) AS `u1` ON `w`.`OwnerFullName` = `u1`.`FullName`
        LEFT JOIN `Squads` AS `s` ON `u1`.`SquadId` = `s`.`Id`
        LEFT JOIN (
            SELECT `g1`.`Nickname`, `g1`.`SquadId`, `g1`.`HasSoulPatch`
            FROM `Gears` AS `g1`
            UNION ALL
            SELECT `o2`.`Nickname`, `o2`.`SquadId`, `o2`.`HasSoulPatch`
            FROM `Officers` AS `o2`
        ) AS `u2` ON `s`.`Id` = `u2`.`SquadId`
    ) AS `s0` ON `u0`.`FullName` = `s0`.`OwnerFullName`
) AS `s1` ON (`u`.`Nickname` = `s1`.`LeaderNickname`) AND (`u`.`SquadId` = `s1`.`LeaderSquadId`)
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `s1`.`Nickname`, `s1`.`SquadId`, `s1`.`Id`, `s1`.`Nickname0`, `s1`.`SquadId0`, `s1`.`Id0`, `s1`.`Nickname00`
""");
    }

    public override async Task Correlated_collections_with_funky_orderby_complex_scenario1(bool async)
    {
        await base.Correlated_collections_with_funky_orderby_complex_scenario1(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `s0`.`Id`, `s0`.`Nickname`, `s0`.`SquadId`, `s0`.`Id0`, `s0`.`Nickname0`, `s0`.`HasSoulPatch`, `s0`.`SquadId0`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `w`.`Id`, `u0`.`Nickname`, `u0`.`SquadId`, `s`.`Id` AS `Id0`, `u1`.`Nickname` AS `Nickname0`, `u1`.`HasSoulPatch`, `u1`.`SquadId` AS `SquadId0`, `w`.`OwnerFullName`
    FROM `Weapons` AS `w`
    LEFT JOIN (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`FullName`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`
        FROM `Officers` AS `o0`
    ) AS `u0` ON `w`.`OwnerFullName` = `u0`.`FullName`
    LEFT JOIN `Squads` AS `s` ON `u0`.`SquadId` = `s`.`Id`
    LEFT JOIN (
        SELECT `g1`.`Nickname`, `g1`.`SquadId`, `g1`.`HasSoulPatch`
        FROM `Gears` AS `g1`
        UNION ALL
        SELECT `o1`.`Nickname`, `o1`.`SquadId`, `o1`.`HasSoulPatch`
        FROM `Officers` AS `o1`
    ) AS `u1` ON `s`.`Id` = `u1`.`SquadId`
) AS `s0` ON `u`.`FullName` = `s0`.`OwnerFullName`
ORDER BY `u`.`FullName`, `u`.`Nickname` DESC, `u`.`SquadId`, `s0`.`Id`, `s0`.`Nickname`, `s0`.`SquadId`, `s0`.`Id0`, `s0`.`Nickname0`
""");
    }

    public override async Task Correlated_collections_with_funky_orderby_complex_scenario2(bool async)
    {
        await base.Correlated_collections_with_funky_orderby_complex_scenario2(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `s1`.`FullName`, `s1`.`Nickname`, `s1`.`SquadId`, `s1`.`Id`, `s1`.`Nickname0`, `s1`.`SquadId0`, `s1`.`Id0`, `s1`.`Nickname00`, `s1`.`HasSoulPatch`, `s1`.`SquadId00`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `u0`.`FullName`, `u0`.`Nickname`, `u0`.`SquadId`, `s0`.`Id`, `s0`.`Nickname` AS `Nickname0`, `s0`.`SquadId` AS `SquadId0`, `s0`.`Id0`, `s0`.`Nickname0` AS `Nickname00`, `s0`.`HasSoulPatch`, `s0`.`SquadId0` AS `SquadId00`, `u0`.`HasSoulPatch` AS `HasSoulPatch0`, `s0`.`IsAutomatic`, `s0`.`Name`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`
        FROM `Officers` AS `o0`
    ) AS `u0`
    LEFT JOIN (
        SELECT `w`.`Id`, `u1`.`Nickname`, `u1`.`SquadId`, `s`.`Id` AS `Id0`, `u2`.`Nickname` AS `Nickname0`, `u2`.`HasSoulPatch`, `u2`.`SquadId` AS `SquadId0`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`
        FROM `Weapons` AS `w`
        LEFT JOIN (
            SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`FullName`
            FROM `Gears` AS `g0`
            UNION ALL
            SELECT `o1`.`Nickname`, `o1`.`SquadId`, `o1`.`FullName`
            FROM `Officers` AS `o1`
        ) AS `u1` ON `w`.`OwnerFullName` = `u1`.`FullName`
        LEFT JOIN `Squads` AS `s` ON `u1`.`SquadId` = `s`.`Id`
        LEFT JOIN (
            SELECT `g1`.`Nickname`, `g1`.`SquadId`, `g1`.`HasSoulPatch`
            FROM `Gears` AS `g1`
            UNION ALL
            SELECT `o2`.`Nickname`, `o2`.`SquadId`, `o2`.`HasSoulPatch`
            FROM `Officers` AS `o2`
        ) AS `u2` ON `s`.`Id` = `u2`.`SquadId`
    ) AS `s0` ON `u0`.`FullName` = `s0`.`OwnerFullName`
) AS `s1` ON (`u`.`Nickname` = `s1`.`LeaderNickname`) AND (`u`.`SquadId` = `s1`.`LeaderSquadId`)
ORDER BY `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `s1`.`FullName`, `s1`.`HasSoulPatch0` DESC, `s1`.`Nickname`, `s1`.`SquadId`, `s1`.`IsAutomatic`, `s1`.`Name` DESC, `s1`.`Id`, `s1`.`Nickname0`, `s1`.`SquadId0`, `s1`.`Id0`, `s1`.`Nickname00`
""");
    }

    public override async Task Correlated_collection_with_top_level_FirstOrDefault(bool async)
    {
        await base.Correlated_collection_with_top_level_FirstOrDefault(async);

        AssertSql(
"""
SELECT `u0`.`Nickname`, `u0`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`FullName`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
        FROM `Officers` AS `o`
    ) AS `u`
    ORDER BY `u`.`Nickname`
    LIMIT 1
) AS `u0`
LEFT JOIN `Weapons` AS `w` ON `u0`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u0`.`Nickname`, `u0`.`SquadId`
""");
    }

    public override async Task Correlated_collection_with_top_level_Count(bool async)
    {
        await base.Correlated_collection_with_top_level_Count(async);

        AssertSql(
"""
SELECT COUNT(*)
FROM (
    SELECT 1
    FROM `Gears` AS `g`
    UNION ALL
    SELECT 1
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Correlated_collection_with_top_level_Last_with_orderby_on_outer(bool async)
    {
        await base.Correlated_collection_with_top_level_Last_with_orderby_on_outer(async);

        AssertSql(
"""
SELECT `u0`.`Nickname`, `u0`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`FullName`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
        FROM `Officers` AS `o`
    ) AS `u`
    ORDER BY `u`.`FullName`
    LIMIT 1
) AS `u0`
LEFT JOIN `Weapons` AS `w` ON `u0`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u0`.`FullName`, `u0`.`Nickname`, `u0`.`SquadId`
""");
    }

    public override async Task Correlated_collection_with_top_level_Last_with_order_by_on_inner(bool async)
    {
        await base.Correlated_collection_with_top_level_Last_with_order_by_on_inner(async);

        AssertSql(
"""
SELECT `u0`.`Nickname`, `u0`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`FullName`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
        FROM `Officers` AS `o`
    ) AS `u`
    ORDER BY `u`.`FullName` DESC
    LIMIT 1
) AS `u0`
LEFT JOIN `Weapons` AS `w` ON `u0`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u0`.`FullName` DESC, `u0`.`Nickname`, `u0`.`SquadId`, `w`.`Name`
""");
    }

    public override async Task Null_semantics_on_nullable_bool_from_inner_join_subquery_is_fully_applied(bool async)
    {
        await base.Null_semantics_on_nullable_bool_from_inner_join_subquery_is_fully_applied(async);

        AssertSql(
"""
SELECT `l2`.`Id`, `l2`.`CapitalName`, `l2`.`Name`, `l2`.`ServerAddress`, `l2`.`CommanderName`, `l2`.`Eradicated`
FROM (
    SELECT `l`.`Name`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`
    FROM `LocustCommanders` AS `l0`
) AS `u`
INNER JOIN (
    SELECT `l1`.`Id`, `l1`.`CapitalName`, `l1`.`Name`, `l1`.`ServerAddress`, `l1`.`CommanderName`, `l1`.`Eradicated`
    FROM `LocustHordes` AS `l1`
    WHERE `l1`.`Name` = 'Swarm'
) AS `l2` ON `u`.`Name` = `l2`.`CommanderName`
WHERE (`l2`.`Eradicated` <> TRUE) OR (`l2`.`Eradicated` IS NULL)
""");
    }

    public override async Task Null_semantics_on_nullable_bool_from_left_join_subquery_is_fully_applied(bool async)
    {
        await base.Null_semantics_on_nullable_bool_from_left_join_subquery_is_fully_applied(async);

        AssertSql(
"""
SELECT `l2`.`Id`, `l2`.`CapitalName`, `l2`.`Name`, `l2`.`ServerAddress`, `l2`.`CommanderName`, `l2`.`Eradicated`
FROM (
    SELECT `l`.`Name`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN (
    SELECT `l1`.`Id`, `l1`.`CapitalName`, `l1`.`Name`, `l1`.`ServerAddress`, `l1`.`CommanderName`, `l1`.`Eradicated`
    FROM `LocustHordes` AS `l1`
    WHERE `l1`.`Name` = 'Swarm'
) AS `l2` ON `u`.`Name` = `l2`.`CommanderName`
WHERE (`l2`.`Eradicated` <> TRUE) OR (`l2`.`Eradicated` IS NULL)
""");
    }

    public override async Task Include_on_derived_type_with_order_by_and_paging(bool async)
    {
        await base.Include_on_derived_type_with_order_by_and_paging(async);

        AssertSql(
"""
@__p_0='10'

SELECT `s`.`Name`, `s`.`LocustHordeId`, `s`.`ThreatLevel`, `s`.`ThreatLevelByte`, `s`.`ThreatLevelNullableByte`, `s`.`DefeatedByNickname`, `s`.`DefeatedBySquadId`, `s`.`HighCommandId`, `s`.`Discriminator`, `s`.`Nickname`, `s`.`SquadId`, `s`.`AssignedCityName`, `s`.`CityOfBirthName`, `s`.`FullName`, `s`.`HasSoulPatch`, `s`.`LeaderNickname`, `s`.`LeaderSquadId`, `s`.`Rank`, `s`.`Discriminator0` AS `Discriminator`, `s`.`Id`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator` AS `Discriminator0`, `t`.`Id`, `t`.`Note`
    FROM (
        SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
        FROM `LocustLeaders` AS `l`
        UNION ALL
        SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
        FROM `LocustCommanders` AS `l0`
    ) AS `u`
    LEFT JOIN (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o`
    ) AS `u0` ON (`u`.`DefeatedByNickname` = `u0`.`Nickname`) AND (`u`.`DefeatedBySquadId` = `u0`.`SquadId`)
    LEFT JOIN `Tags` AS `t` ON ((`u0`.`Nickname` = `t`.`GearNickName`) OR (`u0`.`Nickname` IS NULL AND (`t`.`GearNickName` IS NULL))) AND ((`u0`.`SquadId` = `t`.`GearSquadId`) OR (`u0`.`SquadId` IS NULL AND (`t`.`GearSquadId` IS NULL)))
    ORDER BY `t`.`Note`
    LIMIT @__p_0
) AS `s`
LEFT JOIN `Weapons` AS `w` ON `s`.`FullName` = `w`.`OwnerFullName`
ORDER BY `s`.`Note`, `s`.`Name`, `s`.`Nickname`, `s`.`SquadId`, `s`.`Id`
""");
    }

    public override async Task Select_required_navigation_on_derived_type(bool async)
    {
        await base.Select_required_navigation_on_derived_type(async);

        AssertSql(
"""
SELECT `l1`.`Name`
FROM (
    SELECT NULL AS `HighCommandId`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`HighCommandId`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN `LocustHighCommands` AS `l1` ON `u`.`HighCommandId` = `l1`.`Id`
""");
    }

    public override async Task Select_required_navigation_on_the_same_type_with_cast(bool async)
    {
        await base.Select_required_navigation_on_the_same_type_with_cast(async);

        AssertSql(
"""
SELECT `c`.`Name`
FROM (
    SELECT `g`.`CityOfBirthName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`CityOfBirthName`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Cities` AS `c` ON `u`.`CityOfBirthName` = `c`.`Name`
""");
    }

    public override async Task Where_required_navigation_on_derived_type(bool async)
    {
        await base.Where_required_navigation_on_derived_type(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN `LocustHighCommands` AS `l1` ON `u`.`HighCommandId` = `l1`.`Id`
WHERE `l1`.`IsOperational` = TRUE
""");
    }

    public override async Task Outer_parameter_in_join_key(bool async)
    {
        await base.Outer_parameter_in_join_key(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `s`.`Note`, `s`.`Id`, `s`.`Nickname`, `s`.`SquadId`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN LATERAL (
    SELECT `t`.`Note`, `t`.`Id`, `u0`.`Nickname`, `u0`.`SquadId`
    FROM `Tags` AS `t`
    INNER JOIN (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`
        FROM `Officers` AS `o0`
    ) AS `u0` ON `u`.`FullName` = `u0`.`FullName`
) AS `s` ON TRUE
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `s`.`Id`, `s`.`Nickname`
""");
    }

    public override async Task Outer_parameter_in_join_key_inner_and_outer(bool async)
    {
        await base.Outer_parameter_in_join_key_inner_and_outer(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `s`.`Note`, `s`.`Id`, `s`.`Nickname`, `s`.`SquadId`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN LATERAL (
    SELECT `t`.`Note`, `t`.`Id`, `u0`.`Nickname`, `u0`.`SquadId`
    FROM `Tags` AS `t`
    INNER JOIN (
        SELECT `g`.`Nickname`, `g`.`SquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`
        FROM `Officers` AS `o0`
    ) AS `u0` ON `u`.`FullName` = `u`.`Nickname`
) AS `s` ON TRUE
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `s`.`Id`, `s`.`Nickname`
""");
    }

    public override async Task Outer_parameter_in_group_join_with_DefaultIfEmpty(bool async)
    {
        await base.Outer_parameter_in_group_join_with_DefaultIfEmpty(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `s`.`Note`, `s`.`Id`, `s`.`Nickname`, `s`.`SquadId`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN LATERAL (
    SELECT `t`.`Note`, `t`.`Id`, `u0`.`Nickname`, `u0`.`SquadId`
    FROM `Tags` AS `t`
    LEFT JOIN (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`
        FROM `Officers` AS `o0`
    ) AS `u0` ON `u`.`FullName` = `u0`.`FullName`
) AS `s` ON TRUE
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `s`.`Id`, `s`.`Nickname`
""");
    }

    public override async Task Negated_bool_ternary_inside_anonymous_type_in_projection(bool async)
    {
        await base.Negated_bool_ternary_inside_anonymous_type_in_projection(async);

        AssertSql(
"""
SELECT NOT (CASE
    WHEN `u`.`HasSoulPatch` = TRUE THEN TRUE
    ELSE COALESCE(`u`.`HasSoulPatch`, TRUE)
END) AS `c`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
""");
    }

    public override async Task Order_by_entity_qsre(bool async)
    {
        await base.Order_by_entity_qsre(async);

        AssertSql(
"""
SELECT `u`.`FullName`
FROM (
    SELECT `g`.`Nickname`, `g`.`AssignedCityName`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`AssignedCityName`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Cities` AS `c` ON `u`.`AssignedCityName` = `c`.`Name`
ORDER BY `c`.`Name`, `u`.`Nickname` DESC
""");
    }

    public override async Task Order_by_entity_qsre_with_inheritance(bool async)
    {
        await base.Order_by_entity_qsre_with_inheritance(async);

        AssertSql(
"""
SELECT `u`.`Name`
FROM (
    SELECT `l`.`Name`, `l`.`HighCommandId`
    FROM `LocustCommanders` AS `l`
) AS `u`
INNER JOIN `LocustHighCommands` AS `l0` ON `u`.`HighCommandId` = `l0`.`Id`
ORDER BY `l0`.`Id`, `u`.`Name`
""");
    }

    public override async Task Order_by_entity_qsre_composite_key(bool async)
    {
        await base.Order_by_entity_qsre_composite_key(async);

        AssertSql(
"""
SELECT `w`.`Name`
FROM `Weapons` AS `w`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u` ON `w`.`OwnerFullName` = `u`.`FullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `w`.`Id`
""");
    }

    public override async Task Order_by_entity_qsre_with_other_orderbys(bool async)
    {
        await base.Order_by_entity_qsre_with_other_orderbys(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u` ON `w`.`OwnerFullName` = `u`.`FullName`
LEFT JOIN `Weapons` AS `w0` ON `w`.`SynergyWithId` = `w0`.`Id`
ORDER BY `w`.`IsAutomatic`, `u`.`Nickname` DESC, `u`.`SquadId` DESC, `w0`.`Id`, `w`.`Name`
""");
    }

    public override async Task Join_on_entity_qsre_keys(bool async)
    {
        await base.Join_on_entity_qsre_keys(async);

        AssertSql(
"""
SELECT `w`.`Name` AS `Name1`, `w0`.`Name` AS `Name2`
FROM `Weapons` AS `w`
INNER JOIN `Weapons` AS `w0` ON `w`.`Id` = `w0`.`Id`
""");
    }

    public override async Task Join_on_entity_qsre_keys_composite_key(bool async)
    {
        await base.Join_on_entity_qsre_keys_composite_key(async);

        AssertSql(
"""
SELECT `u`.`FullName` AS `GearName1`, `u0`.`FullName` AS `GearName2`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`FullName`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`
    FROM `Officers` AS `o0`
) AS `u0` ON (`u`.`Nickname` = `u0`.`Nickname`) AND (`u`.`SquadId` = `u0`.`SquadId`)
""");
    }

    public override async Task Join_on_entity_qsre_keys_inheritance(bool async)
    {
        await base.Join_on_entity_qsre_keys_inheritance(async);

        AssertSql(
"""
SELECT `u`.`FullName` AS `GearName`, `u0`.`FullName` AS `OfficerName`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN (
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`
    FROM `Officers` AS `o0`
) AS `u0` ON (`u`.`Nickname` = `u0`.`Nickname`) AND (`u`.`SquadId` = `u0`.`SquadId`)
""");
    }

    public override async Task Join_on_entity_qsre_keys_outer_key_is_navigation(bool async)
    {
        await base.Join_on_entity_qsre_keys_outer_key_is_navigation(async);

        AssertSql(
"""
SELECT `w`.`Name` AS `Name1`, `w1`.`Name` AS `Name2`
FROM `Weapons` AS `w`
LEFT JOIN `Weapons` AS `w0` ON `w`.`SynergyWithId` = `w0`.`Id`
INNER JOIN `Weapons` AS `w1` ON `w0`.`Id` = `w1`.`Id`
""");
    }

    public override async Task Join_on_entity_qsre_keys_inner_key_is_navigation(bool async)
    {
        await base.Join_on_entity_qsre_keys_inner_key_is_navigation(async);

        AssertSql(
"""
SELECT `c`.`Name` AS `CityName`, `s`.`Nickname` AS `GearNickname`
FROM `Cities` AS `c`
INNER JOIN (
    SELECT `u`.`Nickname`, `c0`.`Name`
    FROM (
        SELECT `g`.`Nickname`, `g`.`AssignedCityName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`AssignedCityName`
        FROM `Officers` AS `o`
    ) AS `u`
    LEFT JOIN `Cities` AS `c0` ON `u`.`AssignedCityName` = `c0`.`Name`
) AS `s` ON `c`.`Name` = `s`.`Name`
""");
    }

    public override async Task Join_on_entity_qsre_keys_inner_key_is_navigation_composite_key(bool async)
    {
        await base.Join_on_entity_qsre_keys_inner_key_is_navigation_composite_key(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `s`.`Note`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN (
    SELECT `t`.`Note`, `u0`.`Nickname`, `u0`.`SquadId`
    FROM `Tags` AS `t`
    LEFT JOIN (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`
        FROM `Officers` AS `o0`
    ) AS `u0` ON (`t`.`GearNickName` = `u0`.`Nickname`) AND (`t`.`GearSquadId` = `u0`.`SquadId`)
    WHERE `t`.`Note` IN ('Cole''s Tag', 'Dom''s Tag')
) AS `s` ON (`u`.`Nickname` = `s`.`Nickname`) AND (`u`.`SquadId` = `s`.`SquadId`)
""");
    }

    public override async Task Join_on_entity_qsre_keys_inner_key_is_nested_navigation(bool async)
    {
        await base.Join_on_entity_qsre_keys_inner_key_is_nested_navigation(async);

        AssertSql(
"""
SELECT `s`.`Name` AS `SquadName`, `s1`.`Name` AS `WeaponName`
FROM `Squads` AS `s`
INNER JOIN (
    SELECT `w`.`Name`, `s0`.`Id` AS `Id0`
    FROM `Weapons` AS `w`
    LEFT JOIN (
        SELECT `g`.`SquadId`, `g`.`FullName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`SquadId`, `o`.`FullName`
        FROM `Officers` AS `o`
    ) AS `u` ON `w`.`OwnerFullName` = `u`.`FullName`
    LEFT JOIN `Squads` AS `s0` ON `u`.`SquadId` = `s0`.`Id`
    WHERE `w`.`IsAutomatic` = TRUE
) AS `s1` ON `s`.`Id` = `s1`.`Id0`
""");
    }

    public override async Task GroupJoin_on_entity_qsre_keys_inner_key_is_nested_navigation(bool async)
    {
        await base.GroupJoin_on_entity_qsre_keys_inner_key_is_nested_navigation(async);

        AssertSql(
"""
SELECT `s`.`Name` AS `SquadName`, `s1`.`Name` AS `WeaponName`
FROM `Squads` AS `s`
LEFT JOIN (
    SELECT `w`.`Name`, `s0`.`Id` AS `Id0`
    FROM `Weapons` AS `w`
    LEFT JOIN (
        SELECT `g`.`SquadId`, `g`.`FullName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`SquadId`, `o`.`FullName`
        FROM `Officers` AS `o`
    ) AS `u` ON `w`.`OwnerFullName` = `u`.`FullName`
    LEFT JOIN `Squads` AS `s0` ON `u`.`SquadId` = `s0`.`Id`
) AS `s1` ON `s`.`Id` = `s1`.`Id0`
""");
    }

    public override async Task Streaming_correlated_collection_issue_11403(bool async)
    {
        await base.Streaming_correlated_collection_issue_11403(async);

        AssertSql(
"""
SELECT `u0`.`Nickname`, `u0`.`SquadId`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM (
    SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`FullName`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
        FROM `Officers` AS `o`
    ) AS `u`
    ORDER BY `u`.`Nickname`
    LIMIT 1
) AS `u0`
LEFT JOIN (
    SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
    FROM `Weapons` AS `w`
    WHERE `w`.`IsAutomatic` = FALSE
) AS `w0` ON `u0`.`FullName` = `w0`.`OwnerFullName`
ORDER BY `u0`.`Nickname`, `u0`.`SquadId`, `w0`.`Id`
""");
    }

    public override async Task Project_one_value_type_from_empty_collection(bool async)
    {
        await base.Project_one_value_type_from_empty_collection(async);

        AssertSql(
"""
SELECT `s`.`Name`, COALESCE((
    SELECT `u`.`SquadId`
    FROM (
        SELECT `g`.`SquadId`, `g`.`HasSoulPatch`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`SquadId`, `o`.`HasSoulPatch`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE (`s`.`Id` = `u`.`SquadId`) AND (`u`.`HasSoulPatch` = TRUE)
    LIMIT 1), 0) AS `SquadId`
FROM `Squads` AS `s`
WHERE `s`.`Name` = 'Kilo'
""");
    }

    public override async Task Project_one_value_type_converted_to_nullable_from_empty_collection(bool async)
    {
        await base.Project_one_value_type_converted_to_nullable_from_empty_collection(async);

        AssertSql(
"""
SELECT `s`.`Name`, (
    SELECT `u`.`SquadId`
    FROM (
        SELECT `g`.`SquadId`, `g`.`HasSoulPatch`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`SquadId`, `o`.`HasSoulPatch`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE (`s`.`Id` = `u`.`SquadId`) AND (`u`.`HasSoulPatch` = TRUE)
    LIMIT 1) AS `SquadId`
FROM `Squads` AS `s`
WHERE `s`.`Name` = 'Kilo'
""");
    }

    public override async Task Project_one_value_type_with_client_projection_from_empty_collection(bool async)
    {
        await base.Project_one_value_type_with_client_projection_from_empty_collection(async);

        AssertSql(
"""
SELECT `s`.`Name`, `u1`.`SquadId`, `u1`.`LeaderSquadId`, `u1`.`c`
FROM `Squads` AS `s`
LEFT JOIN (
    SELECT `u0`.`SquadId`, `u0`.`LeaderSquadId`, `u0`.`c`
    FROM (
        SELECT `u`.`SquadId`, `u`.`LeaderSquadId`, 1 AS `c`, ROW_NUMBER() OVER(PARTITION BY `u`.`SquadId` ORDER BY `u`.`Nickname`, `u`.`SquadId`) AS `row`
        FROM (
            SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`, `g`.`LeaderSquadId`
            FROM `Gears` AS `g`
            UNION ALL
            SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`, `o`.`LeaderSquadId`
            FROM `Officers` AS `o`
        ) AS `u`
        WHERE `u`.`HasSoulPatch` = TRUE
    ) AS `u0`
    WHERE `u0`.`row` <= 1
) AS `u1` ON `s`.`Id` = `u1`.`SquadId`
WHERE `s`.`Name` = 'Kilo'
""");
    }

    public override async Task Filter_on_subquery_projecting_one_value_type_from_empty_collection(bool async)
    {
        await base.Filter_on_subquery_projecting_one_value_type_from_empty_collection(async);

        AssertSql(
"""
SELECT `s`.`Name`
FROM `Squads` AS `s`
WHERE (`s`.`Name` = 'Kilo') AND (COALESCE((
    SELECT `u`.`SquadId`
    FROM (
        SELECT `g`.`SquadId`, `g`.`HasSoulPatch`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`SquadId`, `o`.`HasSoulPatch`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE (`s`.`Id` = `u`.`SquadId`) AND (`u`.`HasSoulPatch` = TRUE)
    LIMIT 1), 0) <> 0)
""");
    }

    public override async Task Select_subquery_projecting_single_constant_int(bool async)
    {
        await base.Select_subquery_projecting_single_constant_int(async);

        AssertSql(
"""
SELECT `s`.`Name`, COALESCE((
    SELECT 42
    FROM (
        SELECT `g`.`SquadId`, `g`.`HasSoulPatch`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`SquadId`, `o`.`HasSoulPatch`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE (`s`.`Id` = `u`.`SquadId`) AND (`u`.`HasSoulPatch` = TRUE)
    LIMIT 1), 0) AS `Gear`
FROM `Squads` AS `s`
""");
    }

    public override async Task Select_subquery_projecting_single_constant_string(bool async)
    {
        await base.Select_subquery_projecting_single_constant_string(async);

        AssertSql(
"""
SELECT `s`.`Name`, (
    SELECT 'Foo'
    FROM (
        SELECT `g`.`SquadId`, `g`.`HasSoulPatch`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`SquadId`, `o`.`HasSoulPatch`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE (`s`.`Id` = `u`.`SquadId`) AND (`u`.`HasSoulPatch` = TRUE)
    LIMIT 1) AS `Gear`
FROM `Squads` AS `s`
""");
    }

    public override async Task Select_subquery_projecting_single_constant_bool(bool async)
    {
        await base.Select_subquery_projecting_single_constant_bool(async);

        AssertSql(
"""
SELECT `s`.`Name`, COALESCE((
    SELECT TRUE
    FROM (
        SELECT `g`.`SquadId`, `g`.`HasSoulPatch`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`SquadId`, `o`.`HasSoulPatch`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE (`s`.`Id` = `u`.`SquadId`) AND (`u`.`HasSoulPatch` = TRUE)
    LIMIT 1), FALSE) AS `Gear`
FROM `Squads` AS `s`
""");
    }

    public override async Task Select_subquery_projecting_single_constant_inside_anonymous(bool async)
    {
        await base.Select_subquery_projecting_single_constant_inside_anonymous(async);

        AssertSql(
"""
SELECT `s`.`Name`, `u1`.`One`
FROM `Squads` AS `s`
LEFT JOIN (
    SELECT `u0`.`One`, `u0`.`SquadId`
    FROM (
        SELECT 1 AS `One`, `u`.`SquadId`, ROW_NUMBER() OVER(PARTITION BY `u`.`SquadId` ORDER BY `u`.`Nickname`, `u`.`SquadId`) AS `row`
        FROM (
            SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
            FROM `Gears` AS `g`
            UNION ALL
            SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
            FROM `Officers` AS `o`
        ) AS `u`
        WHERE `u`.`HasSoulPatch` = TRUE
    ) AS `u0`
    WHERE `u0`.`row` <= 1
) AS `u1` ON `s`.`Id` = `u1`.`SquadId`
""");
    }

    public override async Task Select_subquery_projecting_multiple_constants_inside_anonymous(bool async)
    {
        await base.Select_subquery_projecting_multiple_constants_inside_anonymous(async);

        AssertSql(
"""
SELECT `s`.`Name`, `u1`.`True1`, `u1`.`False1`, `u1`.`c`
FROM `Squads` AS `s`
LEFT JOIN (
    SELECT `u0`.`True1`, `u0`.`False1`, `u0`.`c`, `u0`.`SquadId`
    FROM (
        SELECT TRUE AS `True1`, FALSE AS `False1`, 1 AS `c`, `u`.`SquadId`, ROW_NUMBER() OVER(PARTITION BY `u`.`SquadId` ORDER BY `u`.`Nickname`, `u`.`SquadId`) AS `row`
        FROM (
            SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
            FROM `Gears` AS `g`
            UNION ALL
            SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
            FROM `Officers` AS `o`
        ) AS `u`
        WHERE `u`.`HasSoulPatch` = TRUE
    ) AS `u0`
    WHERE `u0`.`row` <= 1
) AS `u1` ON `s`.`Id` = `u1`.`SquadId`
""");
    }

    public override async Task Include_with_order_by_constant(bool async)
    {
        await base.Include_with_order_by_constant(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`, `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM `Squads` AS `s`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON `s`.`Id` = `u`.`SquadId`
ORDER BY `s`.`Id`, `u`.`Nickname`
""");
    }

    public override async Task Correlated_collection_order_by_constant(bool async)
    {
        await base.Correlated_collection_order_by_constant(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w`.`Name`, `w`.`Id`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Select_subquery_projecting_single_constant_null_of_non_mapped_type(bool async)
    {
        await base.Select_subquery_projecting_single_constant_null_of_non_mapped_type(async);

        AssertSql(
"""
SELECT `s`.`Name`, `u1`.`c`
FROM `Squads` AS `s`
LEFT JOIN (
    SELECT `u0`.`c`, `u0`.`SquadId`
    FROM (
        SELECT 1 AS `c`, `u`.`SquadId`, ROW_NUMBER() OVER(PARTITION BY `u`.`SquadId` ORDER BY `u`.`Nickname`, `u`.`SquadId`) AS `row`
        FROM (
            SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
            FROM `Gears` AS `g`
            UNION ALL
            SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
            FROM `Officers` AS `o`
        ) AS `u`
        WHERE `u`.`HasSoulPatch` = TRUE
    ) AS `u0`
    WHERE `u0`.`row` <= 1
) AS `u1` ON `s`.`Id` = `u1`.`SquadId`
""");
    }

    public override async Task Select_subquery_projecting_single_constant_of_non_mapped_type(bool async)
    {
        await base.Select_subquery_projecting_single_constant_of_non_mapped_type(async);

        AssertSql(
"""
SELECT `s`.`Name`, `u1`.`c`
FROM `Squads` AS `s`
LEFT JOIN (
    SELECT `u0`.`c`, `u0`.`SquadId`
    FROM (
        SELECT 1 AS `c`, `u`.`SquadId`, ROW_NUMBER() OVER(PARTITION BY `u`.`SquadId` ORDER BY `u`.`Nickname`, `u`.`SquadId`) AS `row`
        FROM (
            SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
            FROM `Gears` AS `g`
            UNION ALL
            SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
            FROM `Officers` AS `o`
        ) AS `u`
        WHERE `u`.`HasSoulPatch` = TRUE
    ) AS `u0`
    WHERE `u0`.`row` <= 1
) AS `u1` ON `s`.`Id` = `u1`.`SquadId`
""");
    }

    public override async Task Include_collection_OrderBy_aggregate(bool async)
    {
        await base.Include_collection_OrderBy_aggregate(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY (
    SELECT COUNT(*)
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`), `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task Include_collection_with_complex_OrderBy2(bool async)
    {
        await base.Include_collection_with_complex_OrderBy2(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY (
    SELECT `w`.`IsAutomatic`
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ORDER BY `w`.`Id`
    LIMIT 1), `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task Include_collection_with_complex_OrderBy3(bool async)
    {
        await base.Include_collection_with_complex_OrderBy3(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY COALESCE((
    SELECT `w`.`IsAutomatic`
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ORDER BY `w`.`Id`
    LIMIT 1), FALSE), `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task Correlated_collection_with_complex_OrderBy(bool async)
    {
        await base.Correlated_collection_with_complex_OrderBy(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u1`.`Nickname`, `u1`.`SquadId`, `u1`.`AssignedCityName`, `u1`.`CityOfBirthName`, `u1`.`FullName`, `u1`.`HasSoulPatch`, `u1`.`LeaderNickname`, `u1`.`LeaderSquadId`, `u1`.`Rank`, `u1`.`Discriminator`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o0`
    ) AS `u0`
    WHERE `u0`.`HasSoulPatch` = FALSE
) AS `u1` ON (`u`.`Nickname` = `u1`.`LeaderNickname`) AND (`u`.`SquadId` = `u1`.`LeaderSquadId`)
ORDER BY (
    SELECT COUNT(*)
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`), `u`.`Nickname`, `u`.`SquadId`, `u1`.`Nickname`
""");
    }

    public override async Task Correlated_collection_with_very_complex_order_by(bool async)
    {
        await base.Correlated_collection_with_very_complex_order_by(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u2`.`Nickname`, `u2`.`SquadId`, `u2`.`AssignedCityName`, `u2`.`CityOfBirthName`, `u2`.`FullName`, `u2`.`HasSoulPatch`, `u2`.`LeaderNickname`, `u2`.`LeaderSquadId`, `u2`.`Rank`, `u2`.`Discriminator`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `u1`.`Nickname`, `u1`.`SquadId`, `u1`.`AssignedCityName`, `u1`.`CityOfBirthName`, `u1`.`FullName`, `u1`.`HasSoulPatch`, `u1`.`LeaderNickname`, `u1`.`LeaderSquadId`, `u1`.`Rank`, `u1`.`Discriminator`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o1`.`Nickname`, `o1`.`SquadId`, `o1`.`AssignedCityName`, `o1`.`CityOfBirthName`, `o1`.`FullName`, `o1`.`HasSoulPatch`, `o1`.`LeaderNickname`, `o1`.`LeaderSquadId`, `o1`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o1`
    ) AS `u1`
    WHERE `u1`.`HasSoulPatch` = FALSE
) AS `u2` ON (`u`.`Nickname` = `u2`.`LeaderNickname`) AND (`u`.`SquadId` = `u2`.`LeaderSquadId`)
ORDER BY (
    SELECT COUNT(*)
    FROM `Weapons` AS `w`
    WHERE (`u`.`FullName` = `w`.`OwnerFullName`) AND (`w`.`IsAutomatic` = COALESCE((
        SELECT `u0`.`HasSoulPatch`
        FROM (
            SELECT `g`.`Nickname`, `g`.`HasSoulPatch`
            FROM `Gears` AS `g`
            UNION ALL
            SELECT `o0`.`Nickname`, `o0`.`HasSoulPatch`
            FROM `Officers` AS `o0`
        ) AS `u0`
        WHERE `u0`.`Nickname` = 'Marcus'
        LIMIT 1), FALSE))), `u`.`Nickname`, `u`.`SquadId`, `u2`.`Nickname`
""");
    }

    public override async Task Cast_to_derived_type_after_OfType_works(bool async)
    {
        await base.Cast_to_derived_type_after_OfType_works(async);

        AssertSql(
"""
SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
FROM `Officers` AS `o`
""");
    }

    public override async Task Select_subquery_boolean(bool async)
    {
        await base.Select_subquery_boolean(async);

        AssertSql(
"""
SELECT COALESCE((
    SELECT `w`.`IsAutomatic`
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ORDER BY `w`.`Id`
    LIMIT 1), FALSE)
FROM (
    SELECT `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Select_subquery_boolean_with_pushdown(bool async)
    {
        await base.Select_subquery_boolean_with_pushdown(async);

        AssertSql(
"""
SELECT (
    SELECT `w`.`IsAutomatic`
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ORDER BY `w`.`Id`
    LIMIT 1)
FROM (
    SELECT `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Select_subquery_int_with_inside_cast_and_coalesce(bool async)
    {
        await base.Select_subquery_int_with_inside_cast_and_coalesce(async);

        AssertSql(
"""
SELECT COALESCE((
    SELECT `w`.`Id`
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ORDER BY `w`.`Id`
    LIMIT 1), 42)
FROM (
    SELECT `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Select_subquery_int_with_outside_cast_and_coalesce(bool async)
    {
        await base.Select_subquery_int_with_outside_cast_and_coalesce(async);

        AssertSql(
"""
SELECT COALESCE((
    SELECT `w`.`Id`
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ORDER BY `w`.`Id`
    LIMIT 1), 0)
FROM (
    SELECT `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Select_subquery_int_with_pushdown_and_coalesce(bool async)
    {
        await base.Select_subquery_int_with_pushdown_and_coalesce(async);

        AssertSql(
"""
SELECT COALESCE((
    SELECT `w`.`Id`
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ORDER BY `w`.`Id`
    LIMIT 1), 42)
FROM (
    SELECT `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Select_subquery_int_with_pushdown_and_coalesce2(bool async)
    {
        await base.Select_subquery_int_with_pushdown_and_coalesce2(async);

        AssertSql(
"""
SELECT COALESCE((
    SELECT `w`.`Id`
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
    ORDER BY `w`.`Id`
    LIMIT 1), (
    SELECT `w0`.`Id`
    FROM `Weapons` AS `w0`
    WHERE `u`.`FullName` = `w0`.`OwnerFullName`
    ORDER BY `w0`.`Id`
    LIMIT 1))
FROM (
    SELECT `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Select_subquery_boolean_empty(bool async)
    {
        await base.Select_subquery_boolean_empty(async);

        AssertSql(
"""
SELECT COALESCE((
    SELECT `w`.`IsAutomatic`
    FROM `Weapons` AS `w`
    WHERE (`u`.`FullName` = `w`.`OwnerFullName`) AND (`w`.`Name` = 'BFG')
    ORDER BY `w`.`Id`
    LIMIT 1), FALSE)
FROM (
    SELECT `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Select_subquery_boolean_empty_with_pushdown(bool async)
    {
        await base.Select_subquery_boolean_empty_with_pushdown(async);

        AssertSql(
"""
SELECT (
    SELECT `w`.`IsAutomatic`
    FROM `Weapons` AS `w`
    WHERE (`u`.`FullName` = `w`.`OwnerFullName`) AND (`w`.`Name` = 'BFG')
    ORDER BY `w`.`Id`
    LIMIT 1)
FROM (
    SELECT `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Select_subquery_distinct_singleordefault_boolean1(bool async)
    {
        await base.Select_subquery_distinct_singleordefault_boolean1(async);

        AssertSql(
"""
SELECT COALESCE((
    SELECT `w0`.`IsAutomatic`
    FROM (
        SELECT DISTINCT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE (`u`.`FullName` = `w`.`OwnerFullName`) AND (`w`.`Name` LIKE '%Lancer%')
    ) AS `w0`
    LIMIT 1), FALSE)
FROM (
    SELECT `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`HasSoulPatch` = TRUE
""");
    }

    public override async Task Select_subquery_distinct_singleordefault_boolean2(bool async)
    {
        await base.Select_subquery_distinct_singleordefault_boolean2(async);

        AssertSql(
"""
SELECT COALESCE((
    SELECT DISTINCT `w`.`IsAutomatic`
    FROM `Weapons` AS `w`
    WHERE (`u`.`FullName` = `w`.`OwnerFullName`) AND (`w`.`Name` LIKE '%Lancer%')
    LIMIT 1), FALSE)
FROM (
    SELECT `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`HasSoulPatch` = TRUE
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Select_subquery_distinct_singleordefault_boolean_with_pushdown(bool async)
    {
        await base.Select_subquery_distinct_singleordefault_boolean_with_pushdown(async);

        AssertSql(
"""
SELECT (
    SELECT `w0`.`IsAutomatic`
    FROM (
        SELECT DISTINCT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE (`u`.`FullName` = `w`.`OwnerFullName`) AND (`w`.`Name` LIKE '%Lancer%')
    ) AS `w0`
    LIMIT 1)
FROM (
    SELECT `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`HasSoulPatch` = TRUE
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Select_subquery_distinct_singleordefault_boolean_empty1(bool async)
    {
        await base.Select_subquery_distinct_singleordefault_boolean_empty1(async);

        AssertSql(
"""
SELECT COALESCE((
    SELECT `w0`.`IsAutomatic`
    FROM (
        SELECT DISTINCT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE (`u`.`FullName` = `w`.`OwnerFullName`) AND (`w`.`Name` = 'BFG')
    ) AS `w0`
    LIMIT 1), FALSE)
FROM (
    SELECT `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`HasSoulPatch` = TRUE
""");
    }

    public override async Task Select_subquery_distinct_singleordefault_boolean_empty2(bool async)
    {
        await base.Select_subquery_distinct_singleordefault_boolean_empty2(async);

        AssertSql(
"""
SELECT COALESCE((
    SELECT DISTINCT `w`.`IsAutomatic`
    FROM `Weapons` AS `w`
    WHERE (`u`.`FullName` = `w`.`OwnerFullName`) AND (`w`.`Name` = 'BFG')
    LIMIT 1), FALSE)
FROM (
    SELECT `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`HasSoulPatch` = TRUE
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.OuterReferenceInMultiLevelSubquery))]
    public override async Task Select_subquery_distinct_singleordefault_boolean_empty_with_pushdown(bool async)
    {
        await base.Select_subquery_distinct_singleordefault_boolean_empty_with_pushdown(async);

        AssertSql(
"""
SELECT (
    SELECT `w0`.`IsAutomatic`
    FROM (
        SELECT DISTINCT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
        FROM `Weapons` AS `w`
        WHERE (`u`.`FullName` = `w`.`OwnerFullName`) AND (`w`.`Name` = 'BFG')
    ) AS `w0`
    LIMIT 1)
FROM (
    SELECT `g`.`FullName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`HasSoulPatch` = TRUE
""");
    }

    public override async Task Cast_subquery_to_base_type_using_typed_ToList(bool async)
    {
        await base.Cast_subquery_to_base_type_using_typed_ToList(async);

        AssertSql(
"""
SELECT `c`.`Name`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Nickname`, `u`.`Rank`, `u`.`SquadId`
FROM `Cities` AS `c`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u` ON `c`.`Name` = `u`.`AssignedCityName`
WHERE `c`.`Name` = 'Ephyra'
ORDER BY `c`.`Name`, `u`.`Nickname`
""");
    }

    public override async Task Cast_ordered_subquery_to_base_type_using_typed_ToArray(bool async)
    {
        await base.Cast_ordered_subquery_to_base_type_using_typed_ToArray(async);

        AssertSql(
"""
SELECT `c`.`Name`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Nickname`, `u`.`Rank`, `u`.`SquadId`
FROM `Cities` AS `c`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u` ON `c`.`Name` = `u`.`AssignedCityName`
WHERE `c`.`Name` = 'Ephyra'
ORDER BY `c`.`Name`, `u`.`Nickname` DESC
""");
    }

    public override async Task Correlated_collection_with_complex_order_by_funcletized_to_constant_bool(bool async)
    {
        await base.Correlated_collection_with_complex_order_by_funcletized_to_constant_bool(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `t`.`Nickname`, `t`.`SquadId`, `w`.`Name`, `w`.`Id`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `t`
LEFT JOIN `Weapons` AS `w` ON `t`.`FullName` = `w`.`OwnerFullName`
ORDER BY COALESCE(`t`.`Nickname` IN (
    SELECT `n`.`value`
    FROM JSON_TABLE('[]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` varchar(255) PATH '$[0]'
    )) AS `n`
), FALSE) DESC, `t`.`Nickname`, `t`.`SquadId`
""");
        }
        else
        {
        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w`.`Name`, `w`.`Id`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
        }
    }

    public override async Task Double_order_by_on_nullable_bool_coming_from_optional_navigation(bool async)
    {
        await base.Double_order_by_on_nullable_bool_coming_from_optional_navigation(async);

        AssertSql(
"""
SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM `Weapons` AS `w`
LEFT JOIN `Weapons` AS `w0` ON `w`.`SynergyWithId` = `w0`.`Id`
ORDER BY `w0`.`IsAutomatic`, `w0`.`Id`
""");
    }

    public override async Task Double_order_by_on_Like(bool async)
    {
        await base.Double_order_by_on_Like(async);

        AssertSql(
"""
SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM `Weapons` AS `w`
LEFT JOIN `Weapons` AS `w0` ON `w`.`SynergyWithId` = `w0`.`Id`
ORDER BY (`w0`.`Name` LIKE '%Lancer') AND `w0`.`Name` IS NOT NULL
""");
    }

    public override async Task Double_order_by_on_is_null(bool async)
    {
        await base.Double_order_by_on_is_null(async);

        AssertSql(
"""
SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM `Weapons` AS `w`
LEFT JOIN `Weapons` AS `w0` ON `w`.`SynergyWithId` = `w0`.`Id`
ORDER BY `w0`.`Name` IS NULL
""");
    }

    public override async Task Double_order_by_on_string_compare(bool async)
    {
        await base.Double_order_by_on_string_compare(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
ORDER BY (`w`.`Name` = 'Marcus'' Lancer') AND `w`.`Name` IS NOT NULL, `w`.`Id`
""");
    }

    public override async Task Double_order_by_binary_expression(bool async)
    {
        await base.Double_order_by_binary_expression(async);

        AssertSql(
"""
SELECT `w`.`Id` + 2 AS `Binary`
FROM `Weapons` AS `w`
ORDER BY `w`.`Id` + 2
""");
    }

    public override async Task String_compare_with_null_conditional_argument(bool async)
    {
        await base.String_compare_with_null_conditional_argument(async);

        AssertSql(
"""
SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM `Weapons` AS `w`
LEFT JOIN `Weapons` AS `w0` ON `w`.`SynergyWithId` = `w0`.`Id`
ORDER BY (`w0`.`Name` = 'Marcus'' Lancer') AND `w0`.`Name` IS NOT NULL
""");
    }

    public override async Task String_compare_with_null_conditional_argument2(bool async)
    {
        await base.String_compare_with_null_conditional_argument2(async);

        AssertSql(
"""
SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM `Weapons` AS `w`
LEFT JOIN `Weapons` AS `w0` ON `w`.`SynergyWithId` = `w0`.`Id`
ORDER BY ('Marcus'' Lancer' = `w0`.`Name`) AND `w0`.`Name` IS NOT NULL
""");
    }

    public override async Task String_concat_with_null_conditional_argument(bool async)
    {
        await base.String_concat_with_null_conditional_argument(async);

        AssertSql(
"""
SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM `Weapons` AS `w`
LEFT JOIN `Weapons` AS `w0` ON `w`.`SynergyWithId` = `w0`.`Id`
ORDER BY CONCAT(COALESCE(`w0`.`Name`, ''), CAST(5 AS char))
""");
    }

    public override async Task String_concat_with_null_conditional_argument2(bool async)
    {
        await base.String_concat_with_null_conditional_argument2(async);

        AssertSql(
"""
SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM `Weapons` AS `w`
LEFT JOIN `Weapons` AS `w0` ON `w`.`SynergyWithId` = `w0`.`Id`
ORDER BY CONCAT(`w0`.`Name`, 'Marcus'' Lancer')
""");
    }

    public override async Task String_concat_on_various_types(bool async)
    {
        await base.String_concat_on_various_types(async);

        AssertSql(
"""
SELECT CONCAT(CONCAT('HasSoulPatch ', CAST(`u`.`HasSoulPatch` AS char)), ' HasSoulPatch') AS `HasSoulPatch`, CONCAT(CONCAT('Rank ', CAST(`u`.`Rank` AS char)), ' Rank') AS `Rank`, CONCAT(CONCAT('SquadId ', CAST(`u`.`SquadId` AS char)), ' SquadId') AS `SquadId`, CONCAT(CONCAT('Rating ', COALESCE(CAST(`m`.`Rating` AS char), '')), ' Rating') AS `Rating`, CONCAT(CONCAT('Timeline ', CAST(`m`.`Timeline` AS char)), ' Timeline') AS `Timeline`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`, `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`, `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u`
CROSS JOIN `Missions` AS `m`
ORDER BY `u`.`Nickname`, `m`.`Id`
""");
    }

    public override async Task Time_of_day_datetimeoffset(bool async)
    {
        await base.Time_of_day_datetimeoffset(async);

        AssertSql(
"""
SELECT CAST(`m`.`Timeline` AS time(6))
FROM `Missions` AS `m`
""");
    }

    public override async Task GroupBy_Property_Include_Select_Average(bool async)
    {
        await base.GroupBy_Property_Include_Select_Average(async);

        AssertSql(
"""
SELECT AVG(CAST(`u`.`SquadId` AS double))
FROM (
    SELECT `g`.`SquadId`, `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`SquadId`, `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u`
GROUP BY `u`.`Rank`
""");
    }

    public override async Task GroupBy_Property_Include_Select_Sum(bool async)
    {
        await base.GroupBy_Property_Include_Select_Sum(async);

        AssertSql(
"""
SELECT COALESCE(SUM(`u`.`SquadId`), 0)
FROM (
    SELECT `g`.`SquadId`, `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`SquadId`, `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u`
GROUP BY `u`.`Rank`
""");
    }

    public override async Task GroupBy_Property_Include_Select_Count(bool async)
    {
        await base.GroupBy_Property_Include_Select_Count(async);

        AssertSql(
"""
SELECT COUNT(*)
FROM (
    SELECT `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u`
GROUP BY `u`.`Rank`
""");
    }

    public override async Task GroupBy_Property_Include_Select_LongCount(bool async)
    {
        await base.GroupBy_Property_Include_Select_LongCount(async);

        AssertSql(
"""
SELECT COUNT(*)
FROM (
    SELECT `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u`
GROUP BY `u`.`Rank`
""");
    }

    public override async Task GroupBy_Property_Include_Select_Min(bool async)
    {
        await base.GroupBy_Property_Include_Select_Min(async);

        AssertSql(
"""
SELECT MIN(`u`.`SquadId`)
FROM (
    SELECT `g`.`SquadId`, `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`SquadId`, `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u`
GROUP BY `u`.`Rank`
""");
    }

    public override async Task GroupBy_Property_Include_Aggregate_with_anonymous_selector(bool async)
    {
        await base.GroupBy_Property_Include_Aggregate_with_anonymous_selector(async);

        AssertSql(
"""
SELECT `u`.`Nickname` AS `Key`, COUNT(*) AS `c`
FROM (
    SELECT `g`.`Nickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`
    FROM `Officers` AS `o`
) AS `u`
GROUP BY `u`.`Nickname`
ORDER BY `u`.`Nickname`
""");
    }

    public override async Task Group_by_with_include_with_entity_in_result_selector(bool async)
    {
        await base.Group_by_with_include_with_entity_in_result_selector(async);

        AssertSql(
"""
SELECT `u1`.`Rank`, `u1`.`c`, `s0`.`Nickname`, `s0`.`SquadId`, `s0`.`AssignedCityName`, `s0`.`CityOfBirthName`, `s0`.`FullName`, `s0`.`HasSoulPatch`, `s0`.`LeaderNickname`, `s0`.`LeaderSquadId`, `s0`.`Rank`, `s0`.`Discriminator`, `s0`.`Name`, `s0`.`Location`, `s0`.`Nation`
FROM (
    SELECT `u`.`Rank`, COUNT(*) AS `c`
    FROM (
        SELECT `g`.`Rank`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Rank`
        FROM `Officers` AS `o`
    ) AS `u`
    GROUP BY `u`.`Rank`
) AS `u1`
LEFT JOIN (
    SELECT `s`.`Nickname`, `s`.`SquadId`, `s`.`AssignedCityName`, `s`.`CityOfBirthName`, `s`.`FullName`, `s`.`HasSoulPatch`, `s`.`LeaderNickname`, `s`.`LeaderSquadId`, `s`.`Rank`, `s`.`Discriminator`, `s`.`Name`, `s`.`Location`, `s`.`Nation`
    FROM (
        SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `c`.`Name`, `c`.`Location`, `c`.`Nation`, ROW_NUMBER() OVER(PARTITION BY `u0`.`Rank` ORDER BY `u0`.`Nickname`) AS `row`
        FROM (
            SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
            FROM `Gears` AS `g0`
            UNION ALL
            SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
            FROM `Officers` AS `o0`
        ) AS `u0`
        INNER JOIN `Cities` AS `c` ON `u0`.`CityOfBirthName` = `c`.`Name`
    ) AS `s`
    WHERE `s`.`row` <= 1
) AS `s0` ON `u1`.`Rank` = `s0`.`Rank`
ORDER BY `u1`.`Rank`
""");
    }

    public override async Task GroupBy_Property_Include_Select_Max(bool async)
    {
        await base.GroupBy_Property_Include_Select_Max(async);

        AssertSql(
"""
SELECT MAX(`u`.`SquadId`)
FROM (
    SELECT `g`.`SquadId`, `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`SquadId`, `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u`
GROUP BY `u`.`Rank`
""");
    }

    public override async Task Include_with_group_by_and_FirstOrDefault_gets_properly_applied(bool async)
    {
        await base.Include_with_group_by_and_FirstOrDefault_gets_properly_applied(async);

        AssertSql(
"""
SELECT `s0`.`Nickname`, `s0`.`SquadId`, `s0`.`AssignedCityName`, `s0`.`CityOfBirthName`, `s0`.`FullName`, `s0`.`HasSoulPatch`, `s0`.`LeaderNickname`, `s0`.`LeaderSquadId`, `s0`.`Rank`, `s0`.`Discriminator`, `s0`.`Name`, `s0`.`Location`, `s0`.`Nation`
FROM (
    SELECT `u`.`Rank`
    FROM (
        SELECT `g`.`Rank`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Rank`
        FROM `Officers` AS `o`
    ) AS `u`
    GROUP BY `u`.`Rank`
) AS `u1`
LEFT JOIN (
    SELECT `s`.`Nickname`, `s`.`SquadId`, `s`.`AssignedCityName`, `s`.`CityOfBirthName`, `s`.`FullName`, `s`.`HasSoulPatch`, `s`.`LeaderNickname`, `s`.`LeaderSquadId`, `s`.`Rank`, `s`.`Discriminator`, `s`.`Name`, `s`.`Location`, `s`.`Nation`
    FROM (
        SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `c`.`Name`, `c`.`Location`, `c`.`Nation`, ROW_NUMBER() OVER(PARTITION BY `u0`.`Rank` ORDER BY `u0`.`Nickname`, `u0`.`SquadId`, `c`.`Name`) AS `row`
        FROM (
            SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
            FROM `Gears` AS `g0`
            UNION ALL
            SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
            FROM `Officers` AS `o0`
        ) AS `u0`
        INNER JOIN `Cities` AS `c` ON `u0`.`CityOfBirthName` = `c`.`Name`
        WHERE `u0`.`HasSoulPatch` = TRUE
    ) AS `s`
    WHERE `s`.`row` <= 1
) AS `s0` ON `u1`.`Rank` = `s0`.`Rank`
""");
    }

    public override async Task Include_collection_with_Cast_to_base(bool async)
    {
        await base.Include_collection_with_Cast_to_base(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Include_with_client_method_and_member_access_still_applies_includes(bool async)
    {
        await base.Include_with_client_method_and_member_access_still_applies_includes(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
""");
    }

    public override async Task Include_with_projection_of_unmapped_property_still_gets_applied(bool async)
    {
        await base.Include_with_projection_of_unmapped_property_still_gets_applied(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Multiple_includes_with_client_method_around_entity_and_also_projecting_included_collection()
    {
        await base.Multiple_includes_with_client_method_around_entity_and_also_projecting_included_collection();

        AssertSql(
"""
SELECT `s`.`Name`, `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s0`.`Nickname`, `s0`.`SquadId`, `s0`.`AssignedCityName`, `s0`.`CityOfBirthName`, `s0`.`FullName`, `s0`.`HasSoulPatch`, `s0`.`LeaderNickname`, `s0`.`LeaderSquadId`, `s0`.`Rank`, `s0`.`Discriminator`, `s0`.`Id`, `s0`.`AmmunitionType`, `s0`.`IsAutomatic`, `s0`.`Name`, `s0`.`OwnerFullName`, `s0`.`SynergyWithId`
FROM `Squads` AS `s`
LEFT JOIN (
    SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o`
    ) AS `u`
    LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
) AS `s0` ON `s`.`Id` = `s0`.`SquadId`
WHERE `s`.`Name` = 'Delta'
ORDER BY `s`.`Id`, `s0`.`Nickname`, `s0`.`SquadId`
""");
    }

    public override async Task OrderBy_same_expression_containing_IsNull_correctly_deduplicates_the_ordering(bool async)
    {
        await base.OrderBy_same_expression_containing_IsNull_correctly_deduplicates_the_ordering(async);

        AssertSql(
"""
SELECT CASE
    WHEN `u`.`LeaderNickname` IS NOT NULL THEN CHAR_LENGTH(`u`.`Nickname`) = 5
END
FROM (
    SELECT `g`.`Nickname`, `g`.`LeaderNickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
ORDER BY CASE
    WHEN `u`.`LeaderNickname` IS NOT NULL THEN CHAR_LENGTH(`u`.`Nickname`) = 5
END IS NOT NULL
""");
    }

    public override async Task GetValueOrDefault_in_projection(bool async)
    {
        await base.GetValueOrDefault_in_projection(async);

        AssertSql(
"""
SELECT COALESCE(`w`.`SynergyWithId`, 0)
FROM `Weapons` AS `w`
""");
    }

    public override async Task GetValueOrDefault_in_filter(bool async)
    {
        await base.GetValueOrDefault_in_filter(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE COALESCE(`w`.`SynergyWithId`, 0) = 0
""");
    }

    public override async Task GetValueOrDefault_in_filter_non_nullable_column(bool async)
    {
        await base.GetValueOrDefault_in_filter_non_nullable_column(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE `w`.`Id` = 0
""");
    }

    public override async Task GetValueOrDefault_in_order_by(bool async)
    {
        await base.GetValueOrDefault_in_order_by(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
ORDER BY COALESCE(`w`.`SynergyWithId`, 0), `w`.`Id`
""");
    }

    public override async Task GetValueOrDefault_with_argument(bool async)
    {
        await base.GetValueOrDefault_with_argument(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE COALESCE(`w`.`SynergyWithId`, `w`.`Id`) = 1
""");
    }

    public override async Task GetValueOrDefault_with_argument_complex(bool async)
    {
        await base.GetValueOrDefault_with_argument_complex(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE COALESCE(`w`.`SynergyWithId`, CHAR_LENGTH(`w`.`Name`) + 42) > 10
""");
    }

    public override async Task Filter_with_complex_predicate_containing_subquery(bool async)
    {
        await base.Filter_with_complex_predicate_containing_subquery(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (`u`.`FullName` <> 'Dom') AND EXISTS (
    SELECT 1
    FROM `Weapons` AS `w`
    WHERE (`u`.`FullName` = `w`.`OwnerFullName`) AND (`w`.`IsAutomatic` = TRUE))
""");
    }

    public override async Task Query_with_complex_let_containing_ordering_and_filter_projecting_firstOrDefault_element_of_let(
        bool async)
    {
        await base.Query_with_complex_let_containing_ordering_and_filter_projecting_firstOrDefault_element_of_let(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, (
    SELECT `w`.`Name`
    FROM `Weapons` AS `w`
    WHERE (`u`.`FullName` = `w`.`OwnerFullName`) AND (`w`.`IsAutomatic` = TRUE)
    ORDER BY `w`.`AmmunitionType` DESC
    LIMIT 1) AS `WeaponName`
FROM (
    SELECT `g`.`Nickname`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`Nickname` <> 'Dom'
""");
    }

    public override async Task
        Null_semantics_is_correctly_applied_for_function_comparisons_that_take_arguments_from_optional_navigation(bool async)
    {
        await base.Null_semantics_is_correctly_applied_for_function_comparisons_that_take_arguments_from_optional_navigation(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE (SUBSTRING(`t`.`Note`, 0 + 1, `u`.`SquadId`) = `t`.`GearNickName`) OR ((`t`.`Note` IS NULL OR (`u`.`SquadId` IS NULL)) AND `t`.`GearNickName` IS NULL)
""");
    }

    public override async Task
        Null_semantics_is_correctly_applied_for_function_comparisons_that_take_arguments_from_optional_navigation_complex(bool async)
    {
        await base.Null_semantics_is_correctly_applied_for_function_comparisons_that_take_arguments_from_optional_navigation_complex(
            async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
LEFT JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
WHERE (SUBSTRING(`t`.`Note`, 0 + 1, CHAR_LENGTH(`s`.`Name`)) = `t`.`GearNickName`) OR ((`t`.`Note` IS NULL OR (`s`.`Name` IS NULL)) AND `t`.`GearNickName` IS NULL)
""");
    }

    public override async Task Filter_with_new_Guid(bool async)
    {
        await base.Filter_with_new_Guid(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
WHERE `t`.`Id` = 'df36f493-463f-4123-83f9-6b135deeb7ba'
""");
    }

    public override async Task Filter_with_new_Guid_closure(bool async)
    {
        await base.Filter_with_new_Guid_closure(async);

        AssertSql();
    }

    public override async Task OfTypeNav1(bool async)
    {
        await base.OfTypeNav1(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
LEFT JOIN `Tags` AS `t0` ON (`u`.`Nickname` = `t0`.`GearNickName`) AND (`u`.`SquadId` = `t0`.`GearSquadId`)
WHERE ((`t`.`Note` <> 'Foo') OR `t`.`Note` IS NULL) AND ((`t0`.`Note` <> 'Bar') OR `t0`.`Note` IS NULL)
""");
    }

    public override async Task OfTypeNav2(bool async)
    {
        await base.OfTypeNav2(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
LEFT JOIN `Cities` AS `c` ON `u`.`AssignedCityName` = `c`.`Name`
WHERE ((`t`.`Note` <> 'Foo') OR `t`.`Note` IS NULL) AND ((`c`.`Location` <> 'Bar') OR `c`.`Location` IS NULL)
""");
    }

    public override async Task OfTypeNav3(bool async)
    {
        await base.OfTypeNav3(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
INNER JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
LEFT JOIN `Tags` AS `t0` ON (`u`.`Nickname` = `t0`.`GearNickName`) AND (`u`.`SquadId` = `t0`.`GearSquadId`)
WHERE ((`t`.`Note` <> 'Foo') OR `t`.`Note` IS NULL) AND ((`t0`.`Note` <> 'Bar') OR `t0`.`Note` IS NULL)
""");
    }

    public override async Task Nav_rewrite_Distinct_with_convert()
    {
        await base.Nav_rewrite_Distinct_with_convert();

        AssertSql();
    }

    public override async Task Nav_rewrite_Distinct_with_convert_anonymous()
    {
        await base.Nav_rewrite_Distinct_with_convert_anonymous();

        AssertSql();
    }

    public override async Task Nav_rewrite_with_convert1(bool async)
    {
        await base.Nav_rewrite_with_convert1(async);

        AssertSql(
"""
SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`
FROM `LocustHordes` AS `l`
LEFT JOIN `Cities` AS `c` ON `l`.`CapitalName` = `c`.`Name`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
WHERE (`c`.`Name` <> 'Foo') OR `c`.`Name` IS NULL
""");
    }

    public override async Task Nav_rewrite_with_convert2(bool async)
    {
        await base.Nav_rewrite_with_convert2(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`CapitalName`, `l`.`Name`, `l`.`ServerAddress`, `l`.`CommanderName`, `l`.`Eradicated`
FROM `LocustHordes` AS `l`
LEFT JOIN `Cities` AS `c` ON `l`.`CapitalName` = `c`.`Name`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
WHERE ((`c`.`Name` <> 'Foo') OR `c`.`Name` IS NULL) AND ((`l0`.`Name` <> 'Bar') OR `l0`.`Name` IS NULL)
""");
    }

    public override async Task Nav_rewrite_with_convert3(bool async)
    {
        await base.Nav_rewrite_with_convert3(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`CapitalName`, `l`.`Name`, `l`.`ServerAddress`, `l`.`CommanderName`, `l`.`Eradicated`
FROM `LocustHordes` AS `l`
LEFT JOIN `Cities` AS `c` ON `l`.`CapitalName` = `c`.`Name`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
WHERE ((`c`.`Name` <> 'Foo') OR `c`.`Name` IS NULL) AND ((`l0`.`Name` <> 'Bar') OR `l0`.`Name` IS NULL)
""");
    }

    public override async Task Where_contains_on_navigation_with_composite_keys(bool async)
    {
        await base.Where_contains_on_navigation_with_composite_keys(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE EXISTS (
    SELECT 1
    FROM `Cities` AS `c`
    WHERE EXISTS (
        SELECT 1
        FROM (
            SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`CityOfBirthName`
            FROM `Gears` AS `g0`
            UNION ALL
            SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`CityOfBirthName`
            FROM `Officers` AS `o0`
        ) AS `u0`
        WHERE (`c`.`Name` = `u0`.`CityOfBirthName`) AND ((`u0`.`Nickname` = `u`.`Nickname`) AND (`u0`.`SquadId` = `u`.`SquadId`))))
""");
    }

    public override async Task Include_with_complex_order_by(bool async)
    {
        await base.Include_with_complex_order_by(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
ORDER BY (
    SELECT `w`.`Name`
    FROM `Weapons` AS `w`
    WHERE (`u`.`FullName` = `w`.`OwnerFullName`) AND (`w`.`Name` LIKE '%Gnasher%')
    LIMIT 1), `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Anonymous_projection_take_followed_by_projecting_single_element_from_collection_navigation(bool async)
    {
        await base.Anonymous_projection_take_followed_by_projecting_single_element_from_collection_navigation(async);

        AssertSql(
"""
@__p_0='25'

SELECT `w1`.`Id`, `w1`.`AmmunitionType`, `w1`.`IsAutomatic`, `w1`.`Name`, `w1`.`OwnerFullName`, `w1`.`SynergyWithId`
FROM (
    SELECT `u`.`FullName`
    FROM (
        SELECT `g`.`FullName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`FullName`
        FROM `Officers` AS `o`
    ) AS `u`
    LIMIT @__p_0
) AS `u0`
LEFT JOIN (
    SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
    FROM (
        SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, ROW_NUMBER() OVER(PARTITION BY `w`.`OwnerFullName` ORDER BY `w`.`Id`) AS `row`
        FROM `Weapons` AS `w`
    ) AS `w0`
    WHERE `w0`.`row` <= 1
) AS `w1` ON `u0`.`FullName` = `w1`.`OwnerFullName`
""");
    }

    public override async Task Bool_projection_from_subquery_treated_appropriately_in_where(bool async)
    {
        await base.Bool_projection_from_subquery_treated_appropriately_in_where(async);

        AssertSql(
"""
SELECT `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM `Cities` AS `c`
WHERE (
    SELECT `u`.`HasSoulPatch`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
        FROM `Officers` AS `o`
    ) AS `u`
    ORDER BY `u`.`Nickname`, `u`.`SquadId`
    LIMIT 1)
""");
    }

    public override async Task DateTimeOffset_Contains_Less_than_Greater_than(bool async)
    {
        var dto = MySqlTestHelpers.GetExpectedValue(new DateTimeOffset(599898024001234567, new TimeSpan(1, 30, 0)));
        var start = dto.AddDays(-1);
        var end = dto.AddDays(1);
        var dates = new[] { dto };

        await AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(
                m => start <= m.Timeline.Date && m.Timeline < end && dates.Contains(m.Timeline)));

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
@__start_0='1902-01-01T10:00:00.1234567+01:30'
@__end_1='1902-01-03T10:00:00.1234567+01:30'

SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE ((@__start_0 <= CONVERT(`m`.`Timeline`, date)) AND (`m`.`Timeline` < @__end_1)) AND `m`.`Timeline` IN (
    SELECT `d`.`value`
    FROM JSON_TABLE('["1902-01-02T10:00:00.1234567+01:30"]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` datetime(6) PATH '$[0]'
    )) AS `d`
)
""");
        }
        else
        {
            AssertSql(
"""
@__start_0='1902-01-01T08:30:00.1234560+00:00'
@__end_1='1902-01-03T08:30:00.1234560+00:00'

SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE ((@__start_0 <= CONVERT(`m`.`Timeline`, date)) AND (`m`.`Timeline` < @__end_1)) AND (`m`.`Timeline` = TIMESTAMP '1902-01-02 08:30:00.123456')
""");
        }
    }

    [ConditionalTheory(Skip = "TODO: Does not work as expected, probably due to some test definition issues.")]
    public override async Task DateTimeOffsetNow_minus_timespan(bool async)
    {
        var timeSpan = new TimeSpan(10000); // <-- changed from 1000 to 10000 ticks

        await AssertQuery(
            async,
            ss => ss.Set<Mission>().Where(e => e.Timeline > DateTimeOffset.Now - timeSpan));

        AssertSql(
"""
@__timeSpan_0='00:00:00.0010000' (DbType = DateTimeOffset)

SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE `m`.`Timeline` > (UTC_TIMESTAMP() - @__timeSpan_0)
""");
    }

    public override async Task Navigation_inside_interpolated_string_expanded(bool async)
    {
        await base.Navigation_inside_interpolated_string_expanded(async);

        AssertSql(
"""
SELECT `w`.`SynergyWithId` IS NOT NULL, `w0`.`OwnerFullName`
FROM `Weapons` AS `w`
LEFT JOIN `Weapons` AS `w0` ON `w`.`SynergyWithId` = `w0`.`Id`
""");
    }

    public override async Task Left_join_projection_using_coalesce_tracking(bool async)
    {
        await base.Left_join_projection_using_coalesce_tracking(async);

        AssertSql(
"""
SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`LeaderNickname` = `u0`.`Nickname`
""");
    }

    public override async Task Left_join_projection_using_conditional_tracking(bool async)
    {
        await base.Left_join_projection_using_conditional_tracking(async);

        AssertSql(
"""
SELECT `u0`.`Nickname` IS NULL OR (`u0`.`SquadId` IS NULL), `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`LeaderNickname` = `u0`.`Nickname`
""");
    }

    public override async Task Project_collection_navigation_nested_with_take_composite_key(bool async)
    {
        await base.Project_collection_navigation_nested_with_take_composite_key(async);

        AssertSql(
"""
SELECT `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`, `u2`.`Nickname`, `u2`.`SquadId`, `u2`.`AssignedCityName`, `u2`.`CityOfBirthName`, `u2`.`FullName`, `u2`.`HasSoulPatch`, `u2`.`LeaderNickname`, `u2`.`LeaderSquadId`, `u2`.`Rank`, `u2`.`Discriminator`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
LEFT JOIN (
    SELECT `u1`.`Nickname`, `u1`.`SquadId`, `u1`.`AssignedCityName`, `u1`.`CityOfBirthName`, `u1`.`FullName`, `u1`.`HasSoulPatch`, `u1`.`LeaderNickname`, `u1`.`LeaderSquadId`, `u1`.`Rank`, `u1`.`Discriminator`
    FROM (
        SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, ROW_NUMBER() OVER(PARTITION BY `u0`.`LeaderNickname`, `u0`.`LeaderSquadId` ORDER BY `u0`.`Nickname`, `u0`.`SquadId`) AS `row`
        FROM (
            SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
            FROM `Gears` AS `g0`
            UNION ALL
            SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
            FROM `Officers` AS `o0`
        ) AS `u0`
    ) AS `u1`
    WHERE `u1`.`row` <= 50
) AS `u2` ON ((`u`.`Nickname` = `u2`.`LeaderNickname`) OR (`u`.`Nickname` IS NULL AND (`u2`.`LeaderNickname` IS NULL))) AND (`u`.`SquadId` = `u2`.`LeaderSquadId`)
WHERE `u`.`Discriminator` = 'Officer'
ORDER BY `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`, `u2`.`Nickname`
""");
    }

    public override async Task Project_collection_navigation_nested_composite_key(bool async)
    {
        await base.Project_collection_navigation_nested_composite_key(async);

        AssertSql(
"""
SELECT `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON ((`u`.`Nickname` = `u0`.`LeaderNickname`) OR (`u`.`Nickname` IS NULL AND (`u0`.`LeaderNickname` IS NULL))) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
WHERE `u`.`Discriminator` = 'Officer'
ORDER BY `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task Null_checks_in_correlated_predicate_are_correctly_translated(bool async)
    {
        await base.Null_checks_in_correlated_predicate_are_correctly_translated(async);

        AssertSql(
"""
SELECT `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON ((`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)) AND `t`.`Note` IS NOT NULL
ORDER BY `t`.`Id`, `u`.`Nickname`
""");
    }

    public override async Task SelectMany_Where_DefaultIfEmpty_with_navigation_in_the_collection_selector(bool async)
    {
        await base.SelectMany_Where_DefaultIfEmpty_with_navigation_in_the_collection_selector(async);

        AssertSql(
"""
@__isAutomatic_0='True'

SELECT `u`.`Nickname`, `u`.`FullName`, `w0`.`Id` IS NOT NULL AS `Collection`
FROM (
    SELECT `g`.`Nickname`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `w`.`Id`, `w`.`OwnerFullName`
    FROM `Weapons` AS `w`
    WHERE `w`.`IsAutomatic` = @__isAutomatic_0
) AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
""");
    }

    public override async Task Join_with_inner_being_a_subquery_projecting_single_property(bool async)
    {
        await base.Join_with_inner_being_a_subquery_projecting_single_property(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN (
    SELECT `g0`.`Nickname`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`Nickname` = `u0`.`Nickname`
""");
    }

    public override async Task Join_with_inner_being_a_subquery_projecting_anonymous_type_with_single_property(bool async)
    {
        await base.Join_with_inner_being_a_subquery_projecting_anonymous_type_with_single_property(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN (
    SELECT `g0`.`Nickname`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`Nickname` = `u0`.`Nickname`
""");
    }

    public override async Task Navigation_based_on_complex_expression1(bool async)
    {
        await base.Navigation_based_on_complex_expression1(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`CapitalName`, `l`.`Name`, `l`.`ServerAddress`, `l`.`CommanderName`, `l`.`Eradicated`
FROM `LocustHordes` AS `l`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
WHERE `l0`.`Name` IS NOT NULL
""");
    }

    public override async Task Navigation_based_on_complex_expression2(bool async)
    {
        await base.Navigation_based_on_complex_expression2(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`CapitalName`, `l`.`Name`, `l`.`ServerAddress`, `l`.`CommanderName`, `l`.`Eradicated`
FROM `LocustHordes` AS `l`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
WHERE `l0`.`Name` IS NOT NULL
""");
    }

    public override async Task Navigation_based_on_complex_expression3(bool async)
    {
        await base.Navigation_based_on_complex_expression3(async);

        AssertSql(
"""
SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`
FROM `LocustHordes` AS `l`
LEFT JOIN `LocustCommanders` AS `l0` ON `l`.`CommanderName` = `l0`.`Name`
""");
    }

    public override async Task Navigation_based_on_complex_expression4(bool async)
    {
        await base.Navigation_based_on_complex_expression4(async);

        AssertSql(
"""
SELECT TRUE, `l1`.`Name`, `l1`.`LocustHordeId`, `l1`.`ThreatLevel`, `l1`.`ThreatLevelByte`, `l1`.`ThreatLevelNullableByte`, `l1`.`DefeatedByNickname`, `l1`.`DefeatedBySquadId`, `l1`.`HighCommandId`, `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM `LocustHordes` AS `l`
CROSS JOIN (
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN `LocustCommanders` AS `l1` ON `l`.`CommanderName` = `l1`.`Name`
""");
    }

    public override async Task Navigation_based_on_complex_expression5(bool async)
    {
        await base.Navigation_based_on_complex_expression5(async);

        AssertSql(
"""
SELECT `l1`.`Name`, `l1`.`LocustHordeId`, `l1`.`ThreatLevel`, `l1`.`ThreatLevelByte`, `l1`.`ThreatLevelNullableByte`, `l1`.`DefeatedByNickname`, `l1`.`DefeatedBySquadId`, `l1`.`HighCommandId`, `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM `LocustHordes` AS `l`
CROSS JOIN (
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN `LocustCommanders` AS `l1` ON `l`.`CommanderName` = `l1`.`Name`
""");
    }

    public override async Task Navigation_based_on_complex_expression6(bool async)
    {
        await base.Navigation_based_on_complex_expression6(async);

        AssertSql(
"""
SELECT (`l1`.`Name` = 'Queen Myrrah') AND `l1`.`Name` IS NOT NULL, `l1`.`Name`, `l1`.`LocustHordeId`, `l1`.`ThreatLevel`, `l1`.`ThreatLevelByte`, `l1`.`ThreatLevelNullableByte`, `l1`.`DefeatedByNickname`, `l1`.`DefeatedBySquadId`, `l1`.`HighCommandId`, `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM `LocustHordes` AS `l`
CROSS JOIN (
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN `LocustCommanders` AS `l1` ON `l`.`CommanderName` = `l1`.`Name`
""");
    }

    public override async Task Select_as_operator(bool async)
    {
        await base.Select_as_operator(async);

        AssertSql(
"""
SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
FROM `LocustLeaders` AS `l`
UNION ALL
SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
FROM `LocustCommanders` AS `l0`
""");
    }

    public override async Task Select_datetimeoffset_comparison_in_projection(bool async)
    {
        await base.Select_datetimeoffset_comparison_in_projection(async);

        AssertSql(
"""
SELECT `m`.`Timeline` > UTC_TIMESTAMP(6)
FROM `Missions` AS `m`
""");
    }

    public override async Task OfType_in_subquery_works(bool async)
    {
        await base.OfType_in_subquery_works(async);

        AssertSql(
"""
SELECT `s`.`Name`, `s`.`Location`, `s`.`Nation`
FROM `Officers` AS `o`
INNER JOIN (
    SELECT `c`.`Name`, `c`.`Location`, `c`.`Nation`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`
    FROM (
        SELECT `o0`.`AssignedCityName`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`
        FROM `Officers` AS `o0`
    ) AS `u`
    LEFT JOIN `Cities` AS `c` ON `u`.`AssignedCityName` = `c`.`Name`
) AS `s` ON (`o`.`Nickname` = `s`.`LeaderNickname`) AND (`o`.`SquadId` = `s`.`LeaderSquadId`)
""");
    }

    public override async Task Nullable_bool_comparison_is_translated_to_server(bool async)
    {
        await base.Nullable_bool_comparison_is_translated_to_server(async);

        AssertSql(
"""
SELECT (`l`.`Eradicated` = TRUE) AND (`l`.`Eradicated` IS NOT NULL) AS `IsEradicated`
FROM `LocustHordes` AS `l`
""");
    }

    public override async Task Accessing_reference_navigation_collection_composition_generates_single_query(bool async)
    {
        await base.Accessing_reference_navigation_collection_composition_generates_single_query(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `s`.`Id`, `s`.`IsAutomatic`, `s`.`Name`, `s`.`Id0`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `w`.`Id`, `w`.`IsAutomatic`, `w0`.`Name`, `w0`.`Id` AS `Id0`, `w`.`OwnerFullName`
    FROM `Weapons` AS `w`
    LEFT JOIN `Weapons` AS `w0` ON `w`.`SynergyWithId` = `w0`.`Id`
) AS `s` ON `u`.`FullName` = `s`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `s`.`Id`
""");
    }

    public override async Task Reference_include_chain_loads_correctly_when_middle_is_null(bool async)
    {
        await base.Reference_include_chain_loads_correctly_when_middle_is_null(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`, `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
LEFT JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
ORDER BY `t`.`Note`
""");
    }

    public override async Task Accessing_property_of_optional_navigation_in_child_projection_works(bool async)
    {
        await base.Accessing_property_of_optional_navigation_in_child_projection_works(async);

        AssertSql(
"""
SELECT `u`.`Nickname` IS NOT NULL AND (`u`.`SquadId` IS NOT NULL), `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`, `s`.`Nickname`, `s`.`Id`, `s`.`SquadId`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
LEFT JOIN (
    SELECT `u0`.`Nickname`, `w`.`Id`, `u0`.`SquadId`, `w`.`OwnerFullName`
    FROM `Weapons` AS `w`
    LEFT JOIN (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`FullName`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`
        FROM `Officers` AS `o0`
    ) AS `u0` ON `w`.`OwnerFullName` = `u0`.`FullName`
) AS `s` ON `u`.`FullName` = `s`.`OwnerFullName`
ORDER BY `t`.`Note`, `t`.`Id`, `u`.`Nickname`, `u`.`SquadId`, `s`.`Id`, `s`.`Nickname`
""");
    }

    public override async Task Collection_navigation_ofType_filter_works(bool async)
    {
        await base.Collection_navigation_ofType_filter_works(async);

        AssertSql(
"""
SELECT `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM `Cities` AS `c`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `o`.`Nickname`, `o`.`CityOfBirthName`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE (`c`.`Name` = `u`.`CityOfBirthName`) AND (`u`.`Nickname` = 'Marcus'))
""");
    }

    public override async Task Query_reusing_parameter_doesnt_declare_duplicate_parameter(bool async)
    {
        await base.Query_reusing_parameter_doesnt_declare_duplicate_parameter(async);

        AssertSql(
"""
@__prm_Inner_Nickname_0='Marcus' (Size = 255)

SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT DISTINCT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE `u`.`Nickname` <> @__prm_Inner_Nickname_0
) AS `u0`
ORDER BY `u0`.`FullName`
""");
    }

    public override async Task Query_reusing_parameter_with_inner_query_doesnt_declare_duplicate_parameter(bool async)
    {
        await base.Query_reusing_parameter_with_inner_query_doesnt_declare_duplicate_parameter(async);

        AssertSql(
"""
@__squadId_0='1'

SELECT `u1`.`Nickname`, `u1`.`SquadId`, `u1`.`AssignedCityName`, `u1`.`CityOfBirthName`, `u1`.`FullName`, `u1`.`HasSoulPatch`, `u1`.`LeaderNickname`, `u1`.`LeaderSquadId`, `u1`.`Rank`, `u1`.`Discriminator`
FROM (
    SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o`
    ) AS `u`
    INNER JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
    WHERE `s`.`Id` IN (
        SELECT `s0`.`Id`
        FROM `Squads` AS `s0`
        WHERE `s0`.`Id` = @__squadId_0
    )
    UNION ALL
    SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o0`
    ) AS `u0`
    INNER JOIN `Squads` AS `s1` ON `u0`.`SquadId` = `s1`.`Id`
    WHERE `s1`.`Id` IN (
        SELECT `s2`.`Id`
        FROM `Squads` AS `s2`
        WHERE `s2`.`Id` = @__squadId_0
    )
) AS `u1`
ORDER BY `u1`.`FullName`
""");
    }

    public override async Task Query_reusing_parameter_with_inner_query_expression_doesnt_declare_duplicate_parameter(bool async)
    {
        await base.Query_reusing_parameter_with_inner_query_expression_doesnt_declare_duplicate_parameter(async);

        AssertSql(
"""
@__gearId_0='1'

SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `g`.`SquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`SquadId`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE ((`s`.`Id` = `u`.`SquadId`) AND (`u`.`SquadId` = @__gearId_0)) AND (`u`.`SquadId` = @__gearId_0))
""");
    }

    public override async Task Query_reusing_parameter_doesnt_declare_duplicate_parameter_complex(bool async)
    {
        await base.Query_reusing_parameter_doesnt_declare_duplicate_parameter_complex(async);

        AssertSql(
"""
@__entity_equality_prm_Inner_Squad_0_Id='1' (Nullable = true)

SELECT `s1`.`Nickname`, `s1`.`SquadId`, `s1`.`AssignedCityName`, `s1`.`CityOfBirthName`, `s1`.`FullName`, `s1`.`HasSoulPatch`, `s1`.`LeaderNickname`, `s1`.`LeaderSquadId`, `s1`.`Rank`, `s1`.`Discriminator`
FROM (
    SELECT DISTINCT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o`
    ) AS `u`
    INNER JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
    WHERE `s`.`Id` = @__entity_equality_prm_Inner_Squad_0_Id
) AS `s1`
INNER JOIN `Squads` AS `s0` ON `s1`.`SquadId` = `s0`.`Id`
WHERE `s0`.`Id` = @__entity_equality_prm_Inner_Squad_0_Id
ORDER BY `s1`.`FullName`
""");
    }

    public override async Task Complex_GroupBy_after_set_operator(bool async)
    {
        await base.Complex_GroupBy_after_set_operator(async);

        AssertSql(
"""
SELECT `u1`.`Name`, `u1`.`Count`, COALESCE(SUM(`u1`.`Count`), 0) AS `Sum`
FROM (
    SELECT `c`.`Name`, (
        SELECT COUNT(*)
        FROM `Weapons` AS `w`
        WHERE `u`.`FullName` = `w`.`OwnerFullName`) AS `Count`
    FROM (
        SELECT `g`.`AssignedCityName`, `g`.`FullName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`AssignedCityName`, `o`.`FullName`
        FROM `Officers` AS `o`
    ) AS `u`
    LEFT JOIN `Cities` AS `c` ON `u`.`AssignedCityName` = `c`.`Name`
    UNION ALL
    SELECT `c0`.`Name`, (
        SELECT COUNT(*)
        FROM `Weapons` AS `w0`
        WHERE `u0`.`FullName` = `w0`.`OwnerFullName`) AS `Count`
    FROM (
        SELECT `g0`.`CityOfBirthName`, `g0`.`FullName`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`CityOfBirthName`, `o0`.`FullName`
        FROM `Officers` AS `o0`
    ) AS `u0`
    INNER JOIN `Cities` AS `c0` ON `u0`.`CityOfBirthName` = `c0`.`Name`
) AS `u1`
GROUP BY `u1`.`Name`, `u1`.`Count`
""");
    }

    public override async Task Complex_GroupBy_after_set_operator_using_result_selector(bool async)
    {
        await base.Complex_GroupBy_after_set_operator_using_result_selector(async);

        AssertSql(
"""
SELECT `u1`.`Name`, `u1`.`Count`, COALESCE(SUM(`u1`.`Count`), 0) AS `Sum`
FROM (
    SELECT `c`.`Name`, (
        SELECT COUNT(*)
        FROM `Weapons` AS `w`
        WHERE `u`.`FullName` = `w`.`OwnerFullName`) AS `Count`
    FROM (
        SELECT `g`.`AssignedCityName`, `g`.`FullName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`AssignedCityName`, `o`.`FullName`
        FROM `Officers` AS `o`
    ) AS `u`
    LEFT JOIN `Cities` AS `c` ON `u`.`AssignedCityName` = `c`.`Name`
    UNION ALL
    SELECT `c0`.`Name`, (
        SELECT COUNT(*)
        FROM `Weapons` AS `w0`
        WHERE `u0`.`FullName` = `w0`.`OwnerFullName`) AS `Count`
    FROM (
        SELECT `g0`.`CityOfBirthName`, `g0`.`FullName`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`CityOfBirthName`, `o0`.`FullName`
        FROM `Officers` AS `o0`
    ) AS `u0`
    INNER JOIN `Cities` AS `c0` ON `u0`.`CityOfBirthName` = `c0`.`Name`
) AS `u1`
GROUP BY `u1`.`Name`, `u1`.`Count`
""");
    }

    public override async Task Left_join_with_GroupBy_with_composite_group_key(bool async)
    {
        await base.Left_join_with_GroupBy_with_composite_group_key(async);

        AssertSql(
"""
SELECT `u`.`CityOfBirthName`, `u`.`HasSoulPatch`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`CityOfBirthName`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`CityOfBirthName`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
LEFT JOIN `Tags` AS `t` ON `u`.`Nickname` = `t`.`GearNickName`
GROUP BY `u`.`CityOfBirthName`, `u`.`HasSoulPatch`
""");
    }

    public override async Task GroupBy_with_boolean_grouping_key(bool async)
    {
        await base.GroupBy_with_boolean_grouping_key(async);

        AssertSql(
"""
SELECT `u0`.`CityOfBirthName`, `u0`.`HasSoulPatch`, `u0`.`IsMarcus`, COUNT(*) AS `Count`
FROM (
    SELECT `u`.`CityOfBirthName`, `u`.`HasSoulPatch`, `u`.`Nickname` = 'Marcus' AS `IsMarcus`
    FROM (
        SELECT `g`.`Nickname`, `g`.`CityOfBirthName`, `g`.`HasSoulPatch`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`CityOfBirthName`, `o`.`HasSoulPatch`
        FROM `Officers` AS `o`
    ) AS `u`
) AS `u0`
GROUP BY `u0`.`CityOfBirthName`, `u0`.`HasSoulPatch`, `u0`.`IsMarcus`
""");
    }

    public override async Task GroupBy_with_boolean_groupin_key_thru_navigation_access(bool async)
    {
        await base.GroupBy_with_boolean_groupin_key_thru_navigation_access(async);

        AssertSql(
"""
SELECT `u`.`HasSoulPatch`, LOWER(`s`.`Name`) AS `Name`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
LEFT JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
GROUP BY `u`.`HasSoulPatch`, `s`.`Name`
""");
    }

    public override async Task Group_by_over_projection_with_multiple_properties_accessed_thru_navigation(bool async)
    {
        await base.Group_by_over_projection_with_multiple_properties_accessed_thru_navigation(async);

        AssertSql(
"""
SELECT `c`.`Name`
FROM (
    SELECT `g`.`CityOfBirthName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`CityOfBirthName`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Cities` AS `c` ON `u`.`CityOfBirthName` = `c`.`Name`
GROUP BY `c`.`Name`
""");
    }

    public override async Task Group_by_on_StartsWith_with_null_parameter_as_argument(bool async)
    {
        await base.Group_by_on_StartsWith_with_null_parameter_as_argument(async);

        AssertSql(
"""
SELECT `u0`.`Key`
FROM (
    SELECT FALSE AS `Key`
    FROM (
        SELECT `g`.`FullName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`FullName`
        FROM `Officers` AS `o`
    ) AS `u`
) AS `u0`
GROUP BY `u0`.`Key`
""");
    }

    public override async Task Group_by_with_having_StartsWith_with_null_parameter_as_argument(bool async)
    {
        await base.Group_by_with_having_StartsWith_with_null_parameter_as_argument(async);

        AssertSql(
"""
SELECT `u0`.`FullName`
FROM (
    SELECT `u`.`FullName`, FALSE AS `c`
    FROM (
        SELECT `g`.`FullName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`FullName`
        FROM `Officers` AS `o`
    ) AS `u`
    GROUP BY `u`.`FullName`, `c`
    HAVING `c`
) AS `u0`
""");
    }

    public override async Task Select_StartsWith_with_null_parameter_as_argument(bool async)
    {
        await base.Select_StartsWith_with_null_parameter_as_argument(async);

        AssertSql(
"""
SELECT FALSE
FROM (
    SELECT `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Select_null_parameter_is_not_null(bool async)
    {
        await base.Select_null_parameter_is_not_null(async);

        AssertSql(
"""
@__p_0='False'

SELECT @__p_0
FROM (
    SELECT 1
    FROM `Gears` AS `g`
    UNION ALL
    SELECT 1
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Where_null_parameter_is_not_null(bool async)
    {
        await base.Where_null_parameter_is_not_null(async);

        AssertSql(
"""
@__p_0='False'

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE @__p_0
""");
    }

    public override async Task OrderBy_StartsWith_with_null_parameter_as_argument(bool async)
    {
        await base.OrderBy_StartsWith_with_null_parameter_as_argument(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
ORDER BY `u`.`Nickname`
""");
    }

    public override async Task OrderBy_Contains_empty_list(bool async)
    {
        await base.OrderBy_Contains_empty_list(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `t`.`Nickname`, `t`.`SquadId`, `t`.`AssignedCityName`, `t`.`CityOfBirthName`, `t`.`FullName`, `t`.`HasSoulPatch`, `t`.`LeaderNickname`, `t`.`LeaderSquadId`, `t`.`Rank`, `t`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `t`
ORDER BY `t`.`SquadId` IN (
    SELECT `i`.`value`
    FROM JSON_TABLE('[]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `i`
)
""");
        }
        else
        {
        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
""");
        }
    }

    public override async Task Where_with_enum_flags_parameter(bool async)
    {
        await base.Where_with_enum_flags_parameter(async);

        AssertSql(
"""
@__rank_0='1' (Nullable = true)

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(`u`.`Rank` & @__rank_0 AS signed) = @__rank_0
""",
                //
                """
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
""",
                //
                """
@__rank_0='2' (Nullable = true)

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(`u`.`Rank` | @__rank_0 AS signed) <> @__rank_0
""",
                //
                """
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE FALSE
""");
    }

    public override async Task FirstOrDefault_navigation_access_entity_equality_in_where_predicate_apply_peneding_selector(bool async)
    {
        await base.FirstOrDefault_navigation_access_entity_equality_in_where_predicate_apply_peneding_selector(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Cities` AS `c` ON `u`.`AssignedCityName` = `c`.`Name`
WHERE (`c`.`Name` = (
    SELECT `c0`.`Name`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`CityOfBirthName`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`CityOfBirthName`
        FROM `Officers` AS `o0`
    ) AS `u0`
    INNER JOIN `Cities` AS `c0` ON `u0`.`CityOfBirthName` = `c0`.`Name`
    ORDER BY `u0`.`Nickname`
    LIMIT 1)) OR (`c`.`Name` IS NULL AND ((
    SELECT `c0`.`Name`
    FROM (
        SELECT `g0`.`Nickname`, `g0`.`CityOfBirthName`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`CityOfBirthName`
        FROM `Officers` AS `o0`
    ) AS `u0`
    INNER JOIN `Cities` AS `c0` ON `u0`.`CityOfBirthName` = `c0`.`Name`
    ORDER BY `u0`.`Nickname`
    LIMIT 1) IS NULL))
""");
    }

    public override async Task Bitwise_operation_with_non_null_parameter_optimizes_null_checks(bool async)
    {
        await base.Bitwise_operation_with_non_null_parameter_optimizes_null_checks(async);

        AssertSql(
"""
@__ranks_0='134'

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(`u`.`Rank` & @__ranks_0 AS signed) <> 0
""",
                //
                """
@__ranks_0='134'

SELECT CAST(`u`.`Rank` | @__ranks_0 AS signed) = @__ranks_0
FROM (
    SELECT `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u`
""",
                //
                """
@__ranks_0='134'

SELECT CAST(`u`.`Rank` | CAST(`u`.`Rank` | CAST(@__ranks_0 | CAST(`u`.`Rank` | @__ranks_0 AS signed) AS signed) AS signed) AS signed) = @__ranks_0
FROM (
    SELECT `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Bitwise_operation_with_null_arguments(bool async)
    {
        await base.Bitwise_operation_with_null_arguments(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE `w`.`AmmunitionType` IS NULL
""",
                //
                """
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE `w`.`AmmunitionType` IS NULL
""",
                //
                """
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE `w`.`AmmunitionType` IS NULL
""",
                //
                """
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
""",
                //
                """
@__prm_0='2' (Nullable = true)

SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE (CAST(`w`.`AmmunitionType` & @__prm_0 AS signed) <> 0) OR `w`.`AmmunitionType` IS NULL
""",
                //
                """
@__prm_0='1' (Nullable = true)

SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE CAST(`w`.`AmmunitionType` & @__prm_0 AS signed) = @__prm_0
""");
    }

    public override async Task Logical_operation_with_non_null_parameter_optimizes_null_checks(bool async)
    {
        await base.Logical_operation_with_non_null_parameter_optimizes_null_checks(async);

        AssertSql(
"""
@__prm_0='True'

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`HasSoulPatch` <> @__prm_0
""",
                //
                """
@__prm_0='False'

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE `u`.`HasSoulPatch` <> @__prm_0
""");
    }

    public override async Task Cast_OfType_works_correctly(bool async)
    {
        await base.Cast_OfType_works_correctly(async);

        AssertSql(
"""
SELECT `o`.`FullName`
FROM `Officers` AS `o`
""");
    }

    public override async Task Join_inner_source_custom_projection_followed_by_filter(bool async)
    {
        await base.Join_inner_source_custom_projection_followed_by_filter(async);

        AssertSql(
"""
SELECT CASE
    WHEN `l1`.`Name` = 'Locust' THEN TRUE
END AS `IsEradicated`, `l1`.`CommanderName`, `l1`.`Name`
FROM (
    SELECT `l`.`Name`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`
    FROM `LocustCommanders` AS `l0`
) AS `u`
INNER JOIN `LocustHordes` AS `l1` ON `u`.`Name` = `l1`.`CommanderName`
WHERE (CASE
    WHEN `l1`.`Name` = 'Locust' THEN TRUE
END <> TRUE) OR (CASE
    WHEN `l1`.`Name` = 'Locust' THEN TRUE
END IS NULL)
""");
    }

    public override async Task Byte_array_contains_literal(bool async)
    {
        await base.Byte_array_contains_literal(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE LOCATE(0x01, `s`.`Banner`) > 0
""");
    }

    public override async Task Byte_array_filter_by_length_literal(bool async)
    {
        await base.Byte_array_filter_by_length_literal(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE LENGTH(`s`.`Banner`) = 2
""");
    }

    public override async Task Byte_array_filter_by_length_parameter(bool async)
    {
        await base.Byte_array_filter_by_length_parameter(async);

        AssertSql(
"""
@__p_0='2'

SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE LENGTH(`s`.`Banner`) = @__p_0
""");
    }

    public override void Byte_array_filter_by_length_parameter_compiled()
    {
        base.Byte_array_filter_by_length_parameter_compiled();

        AssertSql(
"""
@__byteArrayParam='0x2A80' (Size = 8000)

SELECT COUNT(*)
FROM `Squads` AS `s`
WHERE LENGTH(`s`.`Banner`) = LENGTH(@__byteArrayParam)
""");
    }

    public override async Task Byte_array_contains_parameter(bool async)
    {
        await base.Byte_array_contains_parameter(async);

        AssertSql(
"""
@__someByte_0='1'

SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE LOCATE(UNHEX(HEX(@__someByte_0)), `s`.`Banner`) > 0
""");
    }

    public override async Task Byte_array_filter_by_length_literal_does_not_cast_on_varbinary_n(bool async)
    {
        await base.Byte_array_filter_by_length_literal_does_not_cast_on_varbinary_n(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE LENGTH(`s`.`Banner5`) = 5
""");
    }

    public override async Task Conditional_expression_with_test_being_simplified_to_constant_simple(bool isAsync)
    {
        await base.Conditional_expression_with_test_being_simplified_to_constant_simple(isAsync);

        AssertSql(
"""
@__prm_0='True'

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CASE
    WHEN `u`.`HasSoulPatch` = @__prm_0 THEN TRUE
    ELSE FALSE
END
""");
    }

    public override async Task Conditional_expression_with_test_being_simplified_to_constant_complex(bool isAsync)
    {
        await base.Conditional_expression_with_test_being_simplified_to_constant_complex(isAsync);

        AssertSql(
"""
@__prm_0='True'
@__prm2_1='Marcus' Lancer' (Size = 4000)

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CASE
    WHEN `u`.`HasSoulPatch` = @__prm_0 THEN (
        SELECT `w`.`Name`
        FROM `Weapons` AS `w`
        WHERE `w`.`Id` = `u`.`SquadId`
        LIMIT 1) = @__prm2_1
    ELSE FALSE
END
""");
    }

    public override async Task OrderBy_bool_coming_from_optional_navigation(bool async)
    {
        await base.OrderBy_bool_coming_from_optional_navigation(async);

        AssertSql(
"""
SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM `Weapons` AS `w`
LEFT JOIN `Weapons` AS `w0` ON `w`.`SynergyWithId` = `w0`.`Id`
ORDER BY `w0`.`IsAutomatic`
""");
    }

    public override async Task DateTimeOffset_Date_returns_datetime(bool async)
    {
        await base.DateTimeOffset_Date_returns_datetime(async);

        AssertSql(
"""
@__dateTimeOffset_Date_0='0002-03-01T00:00:00.0000000' (DbType = DateTime)

SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE CONVERT(`m`.`Timeline`, date) >= @__dateTimeOffset_Date_0
""");
    }

    public override async Task Conditional_with_conditions_evaluating_to_false_gets_optimized(bool async)
    {
        await base.Conditional_with_conditions_evaluating_to_false_gets_optimized(async);

        AssertSql(
"""
SELECT `u`.`FullName`
FROM (
    SELECT `g`.`Nickname`, `g`.`CityOfBirthName`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`CityOfBirthName`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Conditional_with_conditions_evaluating_to_true_gets_optimized(bool async)
    {
        await base.Conditional_with_conditions_evaluating_to_true_gets_optimized(async);

        AssertSql(
"""
SELECT `u`.`CityOfBirthName`
FROM (
    SELECT `g`.`Nickname`, `g`.`CityOfBirthName`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`CityOfBirthName`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Projecting_required_string_column_compared_to_null_parameter(bool async)
    {
        await base.Projecting_required_string_column_compared_to_null_parameter(async);

        AssertSql(
"""
SELECT FALSE
FROM (
    SELECT `g`.`Nickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Byte_array_filter_by_SequenceEqual(bool isAsync)
    {
        await base.Byte_array_filter_by_SequenceEqual(isAsync);

        AssertSql(
"""
@__byteArrayParam_0='0x0405060708' (Size = 5)

SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE `s`.`Banner5` = @__byteArrayParam_0
""");
    }

    public override async Task Group_by_nullable_property_HasValue_and_project_the_grouping_key(bool async)
    {
        await base.Group_by_nullable_property_HasValue_and_project_the_grouping_key(async);

        AssertSql(
"""
SELECT `w0`.`Key`
FROM (
    SELECT `w`.`SynergyWithId` IS NOT NULL AS `Key`
    FROM `Weapons` AS `w`
) AS `w0`
GROUP BY `w0`.`Key`
""");
    }

    public override async Task Group_by_nullable_property_and_project_the_grouping_key_HasValue(bool async)
    {
        await base.Group_by_nullable_property_and_project_the_grouping_key_HasValue(async);

        AssertSql(
"""
SELECT `w`.`SynergyWithId` IS NOT NULL
FROM `Weapons` AS `w`
GROUP BY `w`.`SynergyWithId`
""");
    }

    public override async Task Checked_context_with_cast_does_not_fail(bool isAsync)
    {
        await base.Checked_context_with_cast_does_not_fail(isAsync);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
WHERE CAST(`u`.`ThreatLevel` AS unsigned) >= 5
""");
    }

    public override async Task Checked_context_with_addition_does_not_fail(bool isAsync)
    {
        await base.Checked_context_with_addition_does_not_fail(isAsync);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
WHERE CAST(`u`.`ThreatLevel` AS signed) <= (5 + CAST(`u`.`ThreatLevel` AS signed))
""");
    }

    public override async Task TimeSpan_Hours(bool async)
    {
        await base.TimeSpan_Hours(async);

        AssertSql(
"""
SELECT EXTRACT(hour FROM `m`.`Duration`)
FROM `Missions` AS `m`
""");
    }

    public override async Task TimeSpan_Minutes(bool async)
    {
        await base.TimeSpan_Minutes(async);

        AssertSql(
"""
SELECT EXTRACT(minute FROM `m`.`Duration`)
FROM `Missions` AS `m`
""");
    }

    public override async Task TimeSpan_Seconds(bool async)
    {
        await base.TimeSpan_Seconds(async);

        AssertSql(
"""
SELECT EXTRACT(second FROM `m`.`Duration`)
FROM `Missions` AS `m`
""");
    }

    public override async Task TimeSpan_Milliseconds(bool async)
    {
        await base.TimeSpan_Milliseconds(async);

        AssertSql(
"""
SELECT (EXTRACT(microsecond FROM `m`.`Duration`)) DIV (1000)
FROM `Missions` AS `m`
""");
    }

    public override async Task Where_TimeSpan_Hours(bool async)
    {
        await base.Where_TimeSpan_Hours(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE EXTRACT(hour FROM `m`.`Duration`) = 1
""");
    }

    public override async Task Where_TimeSpan_Minutes(bool async)
    {
        await base.Where_TimeSpan_Minutes(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE EXTRACT(minute FROM `m`.`Duration`) = 2
""");
    }

    public override async Task Where_TimeSpan_Seconds(bool async)
    {
        await base.Where_TimeSpan_Seconds(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE EXTRACT(second FROM `m`.`Duration`) = 3
""");
    }

    public override async Task Where_TimeSpan_Milliseconds(bool async)
    {
        await base.Where_TimeSpan_Milliseconds(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE (EXTRACT(microsecond FROM `m`.`Duration`)) DIV (1000) = 456
""");
    }

    public override async Task Contains_on_collection_of_byte_subquery(bool async)
    {
        await base.Contains_on_collection_of_byte_subquery(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
WHERE `u`.`ThreatLevelByte` IN (
    SELECT `l1`.`ThreatLevelByte`
    FROM `LocustLeaders` AS `l1`
    UNION ALL
    SELECT `l2`.`ThreatLevelByte`
    FROM `LocustCommanders` AS `l2`
)
""");
    }

    public override async Task Contains_on_collection_of_nullable_byte_subquery(bool async)
    {
        await base.Contains_on_collection_of_nullable_byte_subquery(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `l1`.`ThreatLevelNullableByte`
        FROM `LocustLeaders` AS `l1`
        UNION ALL
        SELECT `l2`.`ThreatLevelNullableByte`
        FROM `LocustCommanders` AS `l2`
    ) AS `u0`
    WHERE (`u0`.`ThreatLevelNullableByte` = `u`.`ThreatLevelNullableByte`) OR (`u0`.`ThreatLevelNullableByte` IS NULL AND (`u`.`ThreatLevelNullableByte` IS NULL)))
""");
    }

    public override async Task Contains_on_collection_of_nullable_byte_subquery_null_constant(bool async)
    {
        await base.Contains_on_collection_of_nullable_byte_subquery_null_constant(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `l1`.`ThreatLevelNullableByte`
        FROM `LocustLeaders` AS `l1`
        UNION ALL
        SELECT `l2`.`ThreatLevelNullableByte`
        FROM `LocustCommanders` AS `l2`
    ) AS `u0`
    WHERE `u0`.`ThreatLevelNullableByte` IS NULL)
""");
    }

    public override async Task Contains_on_collection_of_nullable_byte_subquery_null_parameter(bool async)
    {
        await base.Contains_on_collection_of_nullable_byte_subquery_null_parameter(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
WHERE EXISTS (
    SELECT 1
    FROM (
        SELECT `l1`.`ThreatLevelNullableByte`
        FROM `LocustLeaders` AS `l1`
        UNION ALL
        SELECT `l2`.`ThreatLevelNullableByte`
        FROM `LocustCommanders` AS `l2`
    ) AS `u0`
    WHERE `u0`.`ThreatLevelNullableByte` IS NULL)
""");
    }

    public override async Task Contains_on_byte_array_property_using_byte_column(bool async)
    {
        await base.Contains_on_byte_array_property_using_byte_column(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`, `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM `Squads` AS `s`
CROSS JOIN (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
WHERE LOCATE(UNHEX(HEX(`u`.`ThreatLevelByte`)), `s`.`Banner`) > 0
""");
    }

    public override async Task Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion(
        bool async)
    {
        await base.Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion(async);

        AssertSql(
"""
SELECT `u2`.`Nickname`, `u2`.`SquadId`, `u2`.`AssignedCityName`, `u2`.`CityOfBirthName`, `u2`.`FullName`, `u2`.`HasSoulPatch`, `u2`.`LeaderNickname`, `u2`.`LeaderSquadId`, `u2`.`Rank`, `u2`.`Discriminator`
FROM (
    SELECT `l`.`ThreatLevelByte`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`ThreatLevelByte`
    FROM `LocustCommanders` AS `l0`
) AS `u`
JOIN LATERAL (
    SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o`
    ) AS `u0`
    WHERE `u`.`ThreatLevelByte` IN (
        SELECT `l1`.`ThreatLevelByte`
        FROM `LocustLeaders` AS `l1`
        UNION ALL
        SELECT `l2`.`ThreatLevelByte`
        FROM `LocustCommanders` AS `l2`
    )
) AS `u2` ON TRUE
""");
    }

    public override async Task Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion_negated(
        bool async)
    {
        await base.Subquery_projecting_non_nullable_scalar_contains_non_nullable_value_doesnt_need_null_expansion_negated(async);

        AssertSql(
"""
SELECT `u2`.`Nickname`, `u2`.`SquadId`, `u2`.`AssignedCityName`, `u2`.`CityOfBirthName`, `u2`.`FullName`, `u2`.`HasSoulPatch`, `u2`.`LeaderNickname`, `u2`.`LeaderSquadId`, `u2`.`Rank`, `u2`.`Discriminator`
FROM (
    SELECT `l`.`ThreatLevelByte`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`ThreatLevelByte`
    FROM `LocustCommanders` AS `l0`
) AS `u`
JOIN LATERAL (
    SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o`
    ) AS `u0`
    WHERE `u`.`ThreatLevelByte` NOT IN (
        SELECT `l1`.`ThreatLevelByte`
        FROM `LocustLeaders` AS `l1`
        UNION ALL
        SELECT `l2`.`ThreatLevelByte`
        FROM `LocustCommanders` AS `l2`
    )
) AS `u2` ON TRUE
""");
    }

    public override async Task Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion(bool async)
    {
        await base.Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion(async);

        AssertSql(
"""
SELECT `u2`.`Nickname`, `u2`.`SquadId`, `u2`.`AssignedCityName`, `u2`.`CityOfBirthName`, `u2`.`FullName`, `u2`.`HasSoulPatch`, `u2`.`LeaderNickname`, `u2`.`LeaderSquadId`, `u2`.`Rank`, `u2`.`Discriminator`
FROM (
    SELECT `l`.`ThreatLevelNullableByte`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`ThreatLevelNullableByte`
    FROM `LocustCommanders` AS `l0`
) AS `u`
JOIN LATERAL (
    SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o`
    ) AS `u0`
    WHERE EXISTS (
        SELECT 1
        FROM (
            SELECT `l1`.`ThreatLevelNullableByte`
            FROM `LocustLeaders` AS `l1`
            UNION ALL
            SELECT `l2`.`ThreatLevelNullableByte`
            FROM `LocustCommanders` AS `l2`
        ) AS `u1`
        WHERE (`u1`.`ThreatLevelNullableByte` = `u`.`ThreatLevelNullableByte`) OR (`u1`.`ThreatLevelNullableByte` IS NULL AND (`u`.`ThreatLevelNullableByte` IS NULL)))
) AS `u2` ON TRUE
""");
    }

    public override async Task Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion_negated(bool async)
    {
        await base.Subquery_projecting_nullable_scalar_contains_nullable_value_needs_null_expansion_negated(async);

        AssertSql(
"""
SELECT `u2`.`Nickname`, `u2`.`SquadId`, `u2`.`AssignedCityName`, `u2`.`CityOfBirthName`, `u2`.`FullName`, `u2`.`HasSoulPatch`, `u2`.`LeaderNickname`, `u2`.`LeaderSquadId`, `u2`.`Rank`, `u2`.`Discriminator`
FROM (
    SELECT `l`.`ThreatLevelNullableByte`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`ThreatLevelNullableByte`
    FROM `LocustCommanders` AS `l0`
) AS `u`
JOIN LATERAL (
    SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o`
    ) AS `u0`
    WHERE NOT EXISTS (
        SELECT 1
        FROM (
            SELECT `l1`.`ThreatLevelNullableByte`
            FROM `LocustLeaders` AS `l1`
            UNION ALL
            SELECT `l2`.`ThreatLevelNullableByte`
            FROM `LocustCommanders` AS `l2`
        ) AS `u1`
        WHERE (`u1`.`ThreatLevelNullableByte` = `u`.`ThreatLevelNullableByte`) OR (`u1`.`ThreatLevelNullableByte` IS NULL AND (`u`.`ThreatLevelNullableByte` IS NULL)))
) AS `u2` ON TRUE
""");
    }

    public override async Task Enum_closure_typed_as_underlying_type_generates_correct_parameter_type(bool async)
    {
        await base.Enum_closure_typed_as_underlying_type_generates_correct_parameter_type(async);

        AssertSql(
"""
@__prm_0='1' (Nullable = true)

SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE @__prm_0 = `w`.`AmmunitionType`
""");
    }

    public override async Task Enum_flags_closure_typed_as_underlying_type_generates_correct_parameter_type(bool async)
    {
        await base.Enum_flags_closure_typed_as_underlying_type_generates_correct_parameter_type(async);

        AssertSql(
"""
@__prm_0='133'

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(@__prm_0 & `u`.`Rank` AS signed) = `u`.`Rank`
""");
    }

    public override async Task Enum_flags_closure_typed_as_different_type_generates_correct_parameter_type(bool async)
    {
        await base.Enum_flags_closure_typed_as_different_type_generates_correct_parameter_type(async);

        AssertSql(
"""
@__prm_0='5'

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE CAST(@__prm_0 & CAST(`u`.`Rank` AS signed) AS signed) = CAST(`u`.`Rank` AS signed)
""");
    }

    public override async Task Constant_enum_with_same_underlying_value_as_previously_parameterized_int(bool async)
    {
        await base.Constant_enum_with_same_underlying_value_as_previously_parameterized_int(async);

        AssertSql(
"""
@__p_0='1'

SELECT CAST(`u`.`Rank` & 1 AS signed)
FROM (
    SELECT `g`.`Nickname`, `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u`
ORDER BY `u`.`Nickname`
LIMIT @__p_0
""");
    }

    public override async Task Enum_array_contains(bool async)
    {
        await base.Enum_array_contains(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
LEFT JOIN `Weapons` AS `w0` ON `w`.`SynergyWithId` = `w0`.`Id`
WHERE `w0`.`Id` IS NOT NULL AND EXISTS (
    SELECT 1
    FROM JSON_TABLE('[null,1]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` int PATH '$[0]'
    )) AS `t`
    WHERE (`t`.`value` = `w0`.`AmmunitionType`) OR (`t`.`value` IS NULL AND (`w0`.`AmmunitionType` IS NULL)))
""");
        }
        else
        {
            AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
LEFT JOIN `Weapons` AS `w0` ON `w`.`SynergyWithId` = `w0`.`Id`
WHERE `w0`.`Id` IS NOT NULL AND (`w0`.`AmmunitionType` IS NULL OR (`w0`.`AmmunitionType` = 1))
""");
        }
    }

    public override async Task CompareTo_used_with_non_unicode_string_column_and_constant(bool async)
    {
        await base.CompareTo_used_with_non_unicode_string_column_and_constant(async);

        AssertSql(
"""
SELECT `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM `Cities` AS `c`
WHERE `c`.`Location` = 'Unknown'
""");
    }

    public override async Task Coalesce_used_with_non_unicode_string_column_and_constant(bool async)
    {
        await base.Coalesce_used_with_non_unicode_string_column_and_constant(async);

        AssertSql(
"""
SELECT COALESCE(`c`.`Location`, 'Unknown')
FROM `Cities` AS `c`
""");
    }

    public override async Task Groupby_anonymous_type_with_navigations_followed_up_by_anonymous_projection_and_orderby(bool async)
    {
        await base.Groupby_anonymous_type_with_navigations_followed_up_by_anonymous_projection_and_orderby(async);

        AssertSql(
"""
SELECT `c`.`Name`, `c`.`Location`, COUNT(*) AS `Count`
FROM `Weapons` AS `w`
LEFT JOIN (
    SELECT `g`.`CityOfBirthName`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`CityOfBirthName`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u` ON `w`.`OwnerFullName` = `u`.`FullName`
LEFT JOIN `Cities` AS `c` ON `u`.`CityOfBirthName` = `c`.`Name`
GROUP BY `c`.`Name`, `c`.`Location`
ORDER BY `c`.`Location`
""");
    }

    public override async Task SelectMany_predicate_with_non_equality_comparison_converted_to_inner_join(bool async)
    {
        await base.SelectMany_predicate_with_non_equality_comparison_converted_to_inner_join(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Weapons` AS `w` ON (`u`.`FullName` <> `w`.`OwnerFullName`) OR `w`.`OwnerFullName` IS NULL
ORDER BY `u`.`Nickname`, `w`.`Id`
""");
    }

    public override async Task SelectMany_predicate_with_non_equality_comparison_DefaultIfEmpty_converted_to_left_join(bool async)
    {
        await base.SelectMany_predicate_with_non_equality_comparison_DefaultIfEmpty_converted_to_left_join(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w` ON (`u`.`FullName` <> `w`.`OwnerFullName`) OR `w`.`OwnerFullName` IS NULL
ORDER BY `u`.`Nickname`, `w`.`Id`
""");
    }

    public override async Task SelectMany_predicate_after_navigation_with_non_equality_comparison_DefaultIfEmpty_converted_to_left_join(
        bool async)
    {
        await base.SelectMany_predicate_after_navigation_with_non_equality_comparison_DefaultIfEmpty_converted_to_left_join(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `s`.`Id`, `s`.`AmmunitionType`, `s`.`IsAutomatic`, `s`.`Name`, `s`.`OwnerFullName`, `s`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
    FROM `Weapons` AS `w`
    LEFT JOIN `Weapons` AS `w0` ON `w`.`SynergyWithId` = `w0`.`Id`
) AS `s` ON (`u`.`FullName` <> `s`.`OwnerFullName`) OR `s`.`OwnerFullName` IS NULL
ORDER BY `u`.`Nickname`, `s`.`Id`
""");
    }

    public override async Task SelectMany_without_result_selector_and_non_equality_comparison_converted_to_join(bool async)
    {
        await base.SelectMany_without_result_selector_and_non_equality_comparison_converted_to_join(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w` ON (`u`.`FullName` <> `w`.`OwnerFullName`) OR `w`.`OwnerFullName` IS NULL
""");
    }

    public override async Task Filtered_collection_projection_with_order_comparison_predicate_converted_to_join(bool async)
    {
        await base.Filtered_collection_projection_with_order_comparison_predicate_converted_to_join(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w` ON (`u`.`FullName` = `w`.`OwnerFullName`) AND (`u`.`SquadId` < `w`.`Id`)
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Filtered_collection_projection_with_order_comparison_predicate_converted_to_join2(bool async)
    {
        await base.Filtered_collection_projection_with_order_comparison_predicate_converted_to_join2(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w` ON (`u`.`FullName` = `w`.`OwnerFullName`) AND (`u`.`SquadId` <= `w`.`Id`)
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Filtered_collection_projection_with_order_comparison_predicate_converted_to_join3(bool async)
    {
        await base.Filtered_collection_projection_with_order_comparison_predicate_converted_to_join3(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w` ON (`u`.`FullName` = `w`.`OwnerFullName`) AND (`u`.`SquadId` >= `w`.`Id`)
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task SelectMany_predicate_with_non_equality_comparison_with_Take_doesnt_convert_to_join(bool async)
    {
        await base.SelectMany_predicate_with_non_equality_comparison_with_Take_doesnt_convert_to_join(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
JOIN LATERAL (
    SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
    FROM `Weapons` AS `w`
    WHERE (`w`.`OwnerFullName` <> `u`.`FullName`) OR `w`.`OwnerFullName` IS NULL
    ORDER BY `w`.`Id`
    LIMIT 3
) AS `w0` ON TRUE
ORDER BY `u`.`Nickname`, `w0`.`Id`
""");
    }

    public override async Task FirstOrDefault_over_int_compared_to_zero(bool async)
    {
        await base.FirstOrDefault_over_int_compared_to_zero(async);

        AssertSql(
"""
SELECT `s`.`Name`
FROM `Squads` AS `s`
WHERE (`s`.`Name` = 'Delta') AND (COALESCE((
    SELECT `u`.`SquadId`
    FROM (
        SELECT `g`.`SquadId`, `g`.`FullName`, `g`.`HasSoulPatch`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`SquadId`, `o`.`FullName`, `o`.`HasSoulPatch`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE (`s`.`Id` = `u`.`SquadId`) AND (`u`.`HasSoulPatch` = TRUE)
    ORDER BY `u`.`FullName`
    LIMIT 1), 0) <> 0)
""");
    }

    public override async Task Correlated_collection_with_inner_collection_references_element_two_levels_up(bool async)
    {
        await base.Correlated_collection_with_inner_collection_references_element_two_levels_up(async);

        AssertSql(
"""
SELECT `u`.`FullName`, `u`.`Nickname`, `u`.`SquadId`, `u1`.`ReportName`, `u1`.`OfficerName`, `u1`.`Nickname`, `u1`.`SquadId`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN LATERAL (
    SELECT `u0`.`FullName` AS `ReportName`, `u`.`FullName` AS `OfficerName`, `u0`.`Nickname`, `u0`.`SquadId`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`FullName`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`
        FROM `Officers` AS `o0`
    ) AS `u0`
    WHERE (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
) AS `u1` ON TRUE
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u1`.`Nickname`
""");
    }

    public override async Task Accessing_derived_property_using_hard_and_soft_cast(bool async)
    {
        await base.Accessing_derived_property_using_hard_and_soft_cast(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
WHERE (`u`.`Discriminator` = 'LocustCommander') AND ((`u`.`HighCommandId` <> 0) OR `u`.`HighCommandId` IS NULL)
""",
                //
                """
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
WHERE (`u`.`Discriminator` = 'LocustCommander') AND ((`u`.`HighCommandId` <> 0) OR `u`.`HighCommandId` IS NULL)
""");
    }

    public override async Task Cast_to_derived_followed_by_include_and_FirstOrDefault(bool async)
    {
        await base.Cast_to_derived_followed_by_include_and_FirstOrDefault(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u0` ON (`u`.`DefeatedByNickname` = `u0`.`Nickname`) AND (`u`.`DefeatedBySquadId` = `u0`.`SquadId`)
WHERE `u`.`Name` LIKE '%Queen%'
LIMIT 1
""");
    }

    public override async Task Correlated_collection_take(bool async)
    {
        await base.Correlated_collection_take(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `c`.`Name`, `w1`.`Id`, `w1`.`AmmunitionType`, `w1`.`IsAutomatic`, `w1`.`Name`, `w1`.`OwnerFullName`, `w1`.`SynergyWithId`, `c`.`Location`, `c`.`Nation`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`CityOfBirthName`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`CityOfBirthName`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Cities` AS `c` ON `u`.`CityOfBirthName` = `c`.`Name`
LEFT JOIN (
    SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
    FROM (
        SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, ROW_NUMBER() OVER(PARTITION BY `w`.`OwnerFullName` ORDER BY `w`.`Id`) AS `row`
        FROM `Weapons` AS `w`
    ) AS `w0`
    WHERE `w0`.`row` <= 10
) AS `w1` ON `u`.`FullName` = `w1`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `c`.`Name`
""");
    }

    public override async Task First_on_byte_array(bool async)
    {
        await base.First_on_byte_array(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE ASCII(`s`.`Banner`) = 2
""");
    }

    public override async Task Array_access_on_byte_array(bool async)
    {
        await base.Array_access_on_byte_array(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE ASCII(SUBSTRING(`s`.`Banner5`, 2 + 1, 1)) = 6
""");
    }

    public override async Task Project_shadow_properties(bool async)
    {
        await base.Project_shadow_properties(async);

        AssertSql(
"""
SELECT `g`.`Nickname`, `g`.`AssignedCityName`
FROM `Gears` AS `g`
UNION ALL
SELECT `o`.`Nickname`, `o`.`AssignedCityName`
FROM `Officers` AS `o`
""");
    }

    public override async Task Composite_key_entity_equal(bool async)
    {
        await base.Composite_key_entity_equal(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
CROSS JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0`
WHERE (`u`.`Nickname` = `u0`.`Nickname`) AND (`u`.`SquadId` = `u0`.`SquadId`)
""");
    }

    public override async Task Composite_key_entity_not_equal(bool async)
    {
        await base.Composite_key_entity_not_equal(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
CROSS JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0`
WHERE (`u`.`Nickname` <> `u0`.`Nickname`) OR (`u`.`SquadId` <> `u0`.`SquadId`)
""");
    }

    public override async Task Composite_key_entity_equal_null(bool async)
    {
        await base.Composite_key_entity_equal_null(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, `l`.`DefeatedByNickname`, `l`.`DefeatedBySquadId`, `l`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u0` ON (`u`.`DefeatedByNickname` = `u0`.`Nickname`) AND (`u`.`DefeatedBySquadId` = `u0`.`SquadId`)
WHERE `u0`.`Nickname` IS NULL OR (`u0`.`SquadId` IS NULL)
""");
    }

    public override async Task Composite_key_entity_not_equal_null(bool async)
    {
        await base.Composite_key_entity_not_equal_null(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, `l`.`DefeatedByNickname`, `l`.`DefeatedBySquadId`, `l`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u0` ON (`u`.`DefeatedByNickname` = `u0`.`Nickname`) AND (`u`.`DefeatedBySquadId` = `u0`.`SquadId`)
WHERE `u0`.`Nickname` IS NOT NULL AND (`u0`.`SquadId` IS NOT NULL)
""");
    }

    public override async Task Projecting_property_converted_to_nullable_with_comparison(bool async)
    {
        await base.Projecting_property_converted_to_nullable_with_comparison(async);

        AssertSql(
"""
SELECT `t`.`Note`, `t`.`GearNickName` IS NOT NULL, `u`.`Nickname`, `u`.`SquadId`, `u`.`HasSoulPatch`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`SquadId`
END = 1
""");
    }

    public override async Task Projecting_property_converted_to_nullable_with_addition(bool async)
    {
        await base.Projecting_property_converted_to_nullable_with_addition(async);

        AssertSql(
"""
SELECT `t`.`Note`, `t`.`GearNickName` IS NOT NULL, `u`.`Nickname`, `u`.`SquadId`, `u`.`HasSoulPatch`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE (CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`SquadId`
END + 1) = 2
""");
    }

    public override async Task Projecting_property_converted_to_nullable_with_addition_and_final_projection(bool async)
    {
        await base.Projecting_property_converted_to_nullable_with_addition_and_final_projection(async);

        AssertSql(
"""
SELECT `t`.`Note`, CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`SquadId`
END + 1 AS `Value`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`Nickname`
END IS NOT NULL
""");
    }

    public override async Task Projecting_property_converted_to_nullable_with_conditional(bool async)
    {
        await base.Projecting_property_converted_to_nullable_with_conditional(async);

        AssertSql(
"""
SELECT CASE
    WHEN (`t`.`Note` <> 'K.I.A.') OR `t`.`Note` IS NULL THEN CASE
        WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`SquadId`
    END
    ELSE -1
END
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
""");
    }

    public override async Task Projecting_property_converted_to_nullable_with_function_call(bool async)
    {
        await base.Projecting_property_converted_to_nullable_with_function_call(async);

        AssertSql(
"""
SELECT SUBSTRING(CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`Nickname`
END, 0 + 1, 3)
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
""");
    }

    public override async Task Projecting_property_converted_to_nullable_with_function_call2(bool async)
    {
        await base.Projecting_property_converted_to_nullable_with_function_call2(async);

        AssertSql(
"""
SELECT `t`.`Note`, SUBSTRING(`t`.`Note`, 0 + 1, CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`SquadId`
END) AS `Function`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`Nickname`
END IS NOT NULL
""");
    }

    public override async Task Projecting_property_converted_to_nullable_into_element_init(bool async)
    {
        await base.Projecting_property_converted_to_nullable_into_element_init(async);

        AssertSql(
"""
SELECT CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN CHAR_LENGTH(`u`.`Nickname`)
END, CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`SquadId`
END, CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`SquadId`
END + 1
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`Nickname`
END IS NOT NULL
ORDER BY `t`.`Note`
""");
    }

    public override async Task Projecting_property_converted_to_nullable_into_member_assignment(bool async)
    {
        await base.Projecting_property_converted_to_nullable_into_member_assignment(async);

        AssertSql(
"""
SELECT CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`SquadId`
END AS `Id`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`Nickname`
END IS NOT NULL
ORDER BY `t`.`Note`
""");
    }

    public override async Task Projecting_property_converted_to_nullable_into_new_array(bool async)
    {
        await base.Projecting_property_converted_to_nullable_into_new_array(async);

        AssertSql(
"""
SELECT CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN CHAR_LENGTH(`u`.`Nickname`)
END, CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`SquadId`
END, CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`SquadId`
END + 1
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`Nickname`
END IS NOT NULL
ORDER BY `t`.`Note`
""");
    }

    public override async Task Projecting_property_converted_to_nullable_into_unary(bool async)
    {
        await base.Projecting_property_converted_to_nullable_into_unary(async);

        AssertSql(
"""
SELECT `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`Nickname`
END IS NOT NULL AND NOT (CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`HasSoulPatch`
END)
ORDER BY `t`.`Note`
""");
    }

    public override async Task Projecting_property_converted_to_nullable_into_member_access(bool async)
    {
        await base.Projecting_property_converted_to_nullable_into_member_access(async);

        AssertSql(
"""
SELECT `u`.`Nickname`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`CityOfBirthName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`CityOfBirthName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
WHERE (EXTRACT(month FROM `t`.`IssueDate`) <> 5) OR EXTRACT(month FROM `t`.`IssueDate`) IS NULL
ORDER BY `u`.`Nickname`
""");
    }

    public override async Task Projecting_property_converted_to_nullable_and_use_it_in_order_by(bool async)
    {
        await base.Projecting_property_converted_to_nullable_and_use_it_in_order_by(async);

        AssertSql(
"""
SELECT `t`.`Note`, `t`.`GearNickName` IS NOT NULL, `u`.`Nickname`, `u`.`SquadId`, `u`.`HasSoulPatch`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`Nickname`
END IS NOT NULL
ORDER BY CASE
    WHEN `t`.`GearNickName` IS NOT NULL THEN `u`.`SquadId`
END, `t`.`Note`
""");
    }

    public override async Task Where_DateOnly_Year(bool async)
    {
        await base.Where_DateOnly_Year(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE EXTRACT(year FROM `m`.`Date`) = 1990
""");
    }

    public override async Task Where_DateOnly_Month(bool async)
    {
        await base.Where_DateOnly_Month(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE EXTRACT(month FROM `m`.`Date`) = 11
""");
    }

    public override async Task Where_DateOnly_Day(bool async)
    {
        await base.Where_DateOnly_Day(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE EXTRACT(day FROM `m`.`Date`) = 10
""");
    }

    public override async Task Where_DateOnly_DayOfYear(bool async)
    {
        await base.Where_DateOnly_DayOfYear(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE DAYOFYEAR(`m`.`Date`) = 314
""");
    }

    public override async Task Where_DateOnly_DayOfWeek(bool async)
    {
        await base.Where_DateOnly_DayOfWeek(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE (DAYOFWEEK(`m`.`Date`) - 1) = 6
""");
    }

    public override async Task Where_DateOnly_AddYears(bool async)
    {
        await base.Where_DateOnly_AddYears(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE DATE_ADD(`m`.`Date`, INTERVAL CAST(3 AS signed) year) = DATE '1993-11-10'
""");
    }

    public override async Task Where_DateOnly_AddMonths(bool async)
    {
        await base.Where_DateOnly_AddMonths(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE DATE_ADD(`m`.`Date`, INTERVAL CAST(3 AS signed) month) = DATE '1991-02-10'
""");
    }

    public override async Task Where_DateOnly_AddDays(bool async)
    {
        await base.Where_DateOnly_AddDays(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE DATE_ADD(`m`.`Date`, INTERVAL CAST(3 AS signed) day) = DATE '1990-11-13'
""");
    }

    public override async Task Where_TimeOnly_Hour(bool async)
    {
        await base.Where_TimeOnly_Hour(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE EXTRACT(hour FROM `m`.`Time`) = 10
""");
    }

    public override async Task Where_TimeOnly_Minute(bool async)
    {
        await base.Where_TimeOnly_Minute(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE EXTRACT(minute FROM `m`.`Time`) = 15
""");
    }

    public override async Task Where_TimeOnly_Second(bool async)
    {
        await base.Where_TimeOnly_Second(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE EXTRACT(second FROM `m`.`Time`) = 50
""");
    }

    public override async Task Where_TimeOnly_Millisecond(bool async)
    {
        await base.Where_TimeOnly_Millisecond(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE (EXTRACT(microsecond FROM `m`.`Time`)) DIV (1000) = 500
""");
    }

    public override async Task Where_TimeOnly_AddHours(bool async)
    {
        await base.Where_TimeOnly_AddHours(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE DATE_ADD(`m`.`Time`, INTERVAL CAST(3.0 AS signed) hour) = TIME '13:15:50.5'
""");
    }

    public override async Task Where_TimeOnly_AddMinutes(bool async)
    {
        await base.Where_TimeOnly_AddMinutes(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE DATE_ADD(`m`.`Time`, INTERVAL CAST(3.0 AS signed) minute) = TIME '10:18:50.5'
""");
    }

    public override async Task Where_TimeOnly_Add_TimeSpan(bool async)
    {
        await base.Where_TimeOnly_Add_TimeSpan(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE (`m`.`Time` + TIME '03:00:00') = TIME '13:15:50.5'
""");
    }

    public override async Task Where_TimeOnly_IsBetween(bool async)
    {
        await base.Where_TimeOnly_IsBetween(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE (`m`.`Time` >= TIME '10:00:00') & (`m`.`Time` < TIME '11:00:00')
""");
    }

    public override async Task Where_TimeOnly_subtract_TimeOnly(bool async)
    {
        await base.Where_TimeOnly_subtract_TimeOnly(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE (`m`.`Time` - TIME '10:00:00') = TIME '00:15:50.5'
""");
    }

    public override async Task Project_navigation_defined_on_base_from_entity_with_inheritance_using_soft_cast(bool async)
    {
        await base.Project_navigation_defined_on_base_from_entity_with_inheritance_using_soft_cast(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`, `t`.`Id` IS NULL AS `IsNull`, `c`.`Name`, `c`.`Location`, `c`.`Nation`, `c`.`Name` IS NULL AS `IsNull`, `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`, `s`.`Id` IS NULL AS `IsNull`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
LEFT JOIN `Cities` AS `c` ON `u`.`CityOfBirthName` = `c`.`Name`
LEFT JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
""");
    }

    public override async Task Project_navigation_defined_on_derived_from_entity_with_inheritance_using_soft_cast(bool async)
    {
        await base.Project_navigation_defined_on_derived_from_entity_with_inheritance_using_soft_cast(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `u0`.`Nickname` IS NULL OR (`u0`.`SquadId` IS NULL) AS `IsNull`, `l1`.`Id`, `l1`.`CapitalName`, `l1`.`Name`, `l1`.`ServerAddress`, `l1`.`CommanderName`, `l1`.`Eradicated`, `l1`.`Id` IS NULL AS `IsNull`, `l2`.`Id`, `l2`.`IsOperational`, `l2`.`Name`, `l2`.`Id` IS NULL AS `IsNull`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u0` ON (`u`.`DefeatedByNickname` = `u0`.`Nickname`) AND (`u`.`DefeatedBySquadId` = `u0`.`SquadId`)
LEFT JOIN `LocustHordes` AS `l1` ON `u`.`Name` = `l1`.`CommanderName`
LEFT JOIN `LocustHighCommands` AS `l2` ON `u`.`HighCommandId` = `l2`.`Id`
""");
    }

    public override async Task Join_entity_with_itself_grouped_by_key_followed_by_include_skip_take(bool async)
    {
        await base.Join_entity_with_itself_grouped_by_key_followed_by_include_skip_take(async);

        AssertSql(
"""
@__p_1='10'
@__p_0='0'

SELECT `s`.`Nickname`, `s`.`SquadId`, `s`.`AssignedCityName`, `s`.`CityOfBirthName`, `s`.`FullName`, `s`.`HasSoulPatch`, `s`.`LeaderNickname`, `s`.`LeaderSquadId`, `s`.`Rank`, `s`.`Discriminator`, `s`.`HasSoulPatch0`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u1`.`HasSoulPatch` AS `HasSoulPatch0`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
        FROM `Officers` AS `o`
    ) AS `u`
    INNER JOIN (
        SELECT MIN(CHAR_LENGTH(`u0`.`Nickname`)) AS `c`, `u0`.`HasSoulPatch`
        FROM (
            SELECT `g0`.`Nickname`, `g0`.`HasSoulPatch`
            FROM `Gears` AS `g0`
            UNION ALL
            SELECT `o0`.`Nickname`, `o0`.`HasSoulPatch`
            FROM `Officers` AS `o0`
        ) AS `u0`
        WHERE `u0`.`Nickname` <> 'Dom'
        GROUP BY `u0`.`HasSoulPatch`
    ) AS `u1` ON CHAR_LENGTH(`u`.`Nickname`) = `u1`.`c`
    ORDER BY `u`.`Nickname`
    LIMIT @__p_1 OFFSET @__p_0
) AS `s`
LEFT JOIN `Weapons` AS `w` ON `s`.`FullName` = `w`.`OwnerFullName`
ORDER BY `s`.`Nickname`, `s`.`SquadId`, `s`.`HasSoulPatch0`
""");
    }

    public override async Task Where_bool_column_and_Contains(bool async)
    {
        await base.Where_bool_column_and_Contains(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `t`.`Nickname`, `t`.`SquadId`, `t`.`AssignedCityName`, `t`.`CityOfBirthName`, `t`.`FullName`, `t`.`HasSoulPatch`, `t`.`LeaderNickname`, `t`.`LeaderSquadId`, `t`.`Rank`, `t`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `t`
WHERE (`t`.`HasSoulPatch` = TRUE) AND `t`.`HasSoulPatch` IN (
    SELECT `v`.`value`
    FROM JSON_TABLE('[false,true]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` tinyint(1) PATH '$[0]'
    )) AS `v`
)
""");
        }
        else
        {
        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (`u`.`HasSoulPatch` = TRUE) AND `u`.`HasSoulPatch` IN (FALSE, TRUE)
""");
        }
    }

    public override async Task Where_bool_column_or_Contains(bool async)
    {
        await base.Where_bool_column_or_Contains(async);

        if (MySqlTestHelpers.HasPrimitiveCollectionsSupport(Fixture))
        {
            AssertSql(
"""
SELECT `t`.`Nickname`, `t`.`SquadId`, `t`.`AssignedCityName`, `t`.`CityOfBirthName`, `t`.`FullName`, `t`.`HasSoulPatch`, `t`.`LeaderNickname`, `t`.`LeaderSquadId`, `t`.`Rank`, `t`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `t`
WHERE (`t`.`HasSoulPatch` = TRUE) AND `t`.`HasSoulPatch` IN (
    SELECT `v`.`value`
    FROM JSON_TABLE('[false,true]', '$[*]' COLUMNS (
        `key` FOR ORDINALITY,
        `value` tinyint(1) PATH '$[0]'
    )) AS `v`
)
""");
        }
        else
        {
        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE (`u`.`HasSoulPatch` = TRUE) AND `u`.`HasSoulPatch` IN (FALSE, TRUE)
""");
        }
    }

    public override async Task Parameter_used_multiple_times_take_appropriate_inferred_type_mapping(bool async)
    {
        await base.Parameter_used_multiple_times_take_appropriate_inferred_type_mapping(async);

        AssertSql(
"""
@__place_0='Ephyra's location' (Size = 4000)

SELECT `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM `Cities` AS `c`
WHERE ((`c`.`Nation` = @__place_0) OR (`c`.`Location` = @__place_0)) OR (`c`.`Location` = @__place_0)
""");
    }

    public override async Task Enum_matching_take_value_gets_different_type_mapping(bool async)
    {
        await base.Enum_matching_take_value_gets_different_type_mapping(async);

        AssertSql(
"""
@__value_1='1'
@__p_0='1'

SELECT CAST(`u`.`Rank` & @__value_1 AS signed)
FROM (
    SELECT `g`.`Nickname`, `g`.`Rank`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`Rank`
    FROM `Officers` AS `o`
) AS `u`
ORDER BY `u`.`Nickname`
LIMIT @__p_0
""");
    }

    public override async Task SelectMany_Where_DefaultIfEmpty_with_navigation_in_the_collection_selector_order_comparison(bool async)
    {
        await base.SelectMany_Where_DefaultIfEmpty_with_navigation_in_the_collection_selector_order_comparison(async);

        AssertSql(
"""
@__prm_0='1'

SELECT `u`.`Nickname`, `u`.`FullName`, `w0`.`Id` IS NOT NULL AS `Collection`
FROM (
    SELECT `g`.`Nickname`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `w`.`Id`, `w`.`OwnerFullName`
    FROM `Weapons` AS `w`
    WHERE `w`.`Id` > @__prm_0
) AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
""");
    }

    public override async Task Project_entity_and_collection_element(bool async)
    {
        await base.Project_entity_and_collection_element(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `w2`.`Id`, `w2`.`AmmunitionType`, `w2`.`IsAutomatic`, `w2`.`Name`, `w2`.`OwnerFullName`, `w2`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
LEFT JOIN (
    SELECT `w1`.`Id`, `w1`.`AmmunitionType`, `w1`.`IsAutomatic`, `w1`.`Name`, `w1`.`OwnerFullName`, `w1`.`SynergyWithId`
    FROM (
        SELECT `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`, ROW_NUMBER() OVER(PARTITION BY `w0`.`OwnerFullName` ORDER BY `w0`.`Id`) AS `row`
        FROM `Weapons` AS `w0`
    ) AS `w1`
    WHERE `w1`.`row` <= 1
) AS `w2` ON `u`.`FullName` = `w2`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `s`.`Id`
""");
    }

    public override async Task DateTimeOffset_DateAdd_AddYears(bool async)
    {
        await base.DateTimeOffset_DateAdd_AddYears(async);

        AssertSql(
"""
SELECT DATE_ADD(`m`.`Timeline`, INTERVAL CAST(1 AS signed) year)
FROM `Missions` AS `m`
""");
    }

    public override async Task Correlated_collection_via_SelectMany_with_Distinct_missing_indentifying_columns_in_projection(bool async)
    {
        await base.Correlated_collection_via_SelectMany_with_Distinct_missing_indentifying_columns_in_projection(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `s`.`HasSoulPatch`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN LATERAL (
    SELECT DISTINCT `u1`.`HasSoulPatch`
    FROM `Weapons` AS `w`
    LEFT JOIN (
        SELECT `g0`.`AssignedCityName`, `g0`.`FullName`
        FROM `Gears` AS `g0`
        UNION ALL
        SELECT `o0`.`AssignedCityName`, `o0`.`FullName`
        FROM `Officers` AS `o0`
    ) AS `u0` ON `w`.`OwnerFullName` = `u0`.`FullName`
    LEFT JOIN `Cities` AS `c` ON `u0`.`AssignedCityName` = `c`.`Name`
    INNER JOIN (
        SELECT `g1`.`CityOfBirthName`, `g1`.`HasSoulPatch`
        FROM `Gears` AS `g1`
        UNION ALL
        SELECT `o1`.`CityOfBirthName`, `o1`.`HasSoulPatch`
        FROM `Officers` AS `o1`
    ) AS `u1` ON `c`.`Name` = `u1`.`CityOfBirthName`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
) AS `s` ON TRUE
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Basic_query_gears(bool async)
    {
        await base.Basic_query_gears(async);

        AssertSql(
"""
SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
FROM `Gears` AS `g`
UNION ALL
SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
FROM `Officers` AS `o`
""");
    }

    public override async Task Contains_on_readonly_enumerable(bool async)
    {
        await base.Contains_on_readonly_enumerable(async);

        AssertSql(
"""
SELECT `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM `Weapons` AS `w`
WHERE `w`.`AmmunitionType` = 1
""");
    }

    public override async Task SelectMany_Where_DefaultIfEmpty_with_navigation_in_the_collection_selector_not_equal(bool async)
    {
        await base.SelectMany_Where_DefaultIfEmpty_with_navigation_in_the_collection_selector_not_equal(async);

        AssertSql(
"""
@__isAutomatic_0='True'

SELECT `u`.`Nickname`, `u`.`FullName`, `w0`.`Id` IS NOT NULL AS `Collection`
FROM (
    SELECT `g`.`Nickname`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `w`.`Id`, `w`.`OwnerFullName`
    FROM `Weapons` AS `w`
    WHERE `w`.`IsAutomatic` <> @__isAutomatic_0
) AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
""");
    }

    public override async Task Trying_to_access_unmapped_property_in_projection(bool async)
    {
        await base.Trying_to_access_unmapped_property_in_projection(async);

        AssertSql(
"""
SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
FROM `Gears` AS `g`
UNION ALL
SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
FROM `Officers` AS `o`
""");
    }

    public override async Task Cast_to_derived_type_causes_client_eval(bool async)
    {
        await base.Cast_to_derived_type_causes_client_eval(async);

        AssertSql(
"""
SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
FROM `Gears` AS `g`
UNION ALL
SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
FROM `Officers` AS `o`
""");
    }

    public override async Task Comparison_with_value_converted_subclass(bool async)
    {
        await base.Comparison_with_value_converted_subclass(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`CapitalName`, `l`.`Name`, `l`.`ServerAddress`, `l`.`CommanderName`, `l`.`Eradicated`
FROM `LocustHordes` AS `l`
WHERE `l`.`ServerAddress` = CAST('127.0.0.1' AS char)
""");
    }

    public override async Task FirstOrDefault_on_empty_collection_of_DateTime_in_subquery(bool async)
    {
        await base.FirstOrDefault_on_empty_collection_of_DateTime_in_subquery(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, COALESCE((
    SELECT `t1`.`IssueDate`
    FROM `Tags` AS `t1`
    WHERE `t1`.`GearNickName` = `u`.`FullName`
    ORDER BY `t1`.`Id`
    LIMIT 1), TIMESTAMP '0001-01-01 00:00:00') AS `invalidTagIssueDate`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Tags` AS `t` ON (`u`.`Nickname` = `t`.`GearNickName`) AND (`u`.`SquadId` = `t`.`GearSquadId`)
WHERE `t`.`IssueDate` > COALESCE((
    SELECT `t0`.`IssueDate`
    FROM `Tags` AS `t0`
    WHERE `t0`.`GearNickName` = `u`.`FullName`
    ORDER BY `t0`.`Id`
    LIMIT 1), TIMESTAMP '0001-01-01 00:00:00')
""");
    }

    public override async Task
        Correlated_collection_with_groupby_not_projecting_identifier_column_with_group_aggregate_in_final_projection_multiple_grouping_keys(
            bool async)
    {
        await base
            .Correlated_collection_with_groupby_not_projecting_identifier_column_with_group_aggregate_in_final_projection_multiple_grouping_keys(
                async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`Count`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN LATERAL (
    SELECT `w`.`IsAutomatic`, `w`.`Name`, COUNT(*) AS `Count`
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
    GROUP BY `w`.`IsAutomatic`, `w`.`Name`
) AS `w0` ON TRUE
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `w0`.`IsAutomatic`
""");
    }

    public override async Task Sum_with_no_data_nullable_double(bool async)
    {
        await base.Sum_with_no_data_nullable_double(async);

        AssertSql(
"""
SELECT COALESCE(SUM(`m`.`Rating`), 0.0)
FROM `Missions` AS `m`
WHERE `m`.`CodeName` = 'Operation Foobar'
""");
    }

    public override async Task ToString_guid_property_projection(bool async)
    {
        await base.ToString_guid_property_projection(async);

        AssertSql(
"""
SELECT `t`.`GearNickName` AS `A`, CAST(`t`.`Id` AS char) AS `B`
FROM `Tags` AS `t`
""");
    }

    public override async Task Correlated_collection_with_distinct_not_projecting_identifier_column(bool async)
    {
        await base.Correlated_collection_with_distinct_not_projecting_identifier_column(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w0`.`Name`, `w0`.`IsAutomatic`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN LATERAL (
    SELECT DISTINCT `w`.`Name`, `w`.`IsAutomatic`
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
) AS `w0` ON TRUE
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `w0`.`Name`
""");
    }

    public override async Task Include_after_Select_throws(bool async)
    {
        await base.Include_after_Select_throws(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`CapitalName`, `l`.`Name`, `l`.`ServerAddress`, `l`.`CommanderName`, `l`.`Eradicated`, `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM `LocustHordes` AS `l`
LEFT JOIN `Cities` AS `c` ON `l`.`CapitalName` = `c`.`Name`
""");
    }

    public override async Task Cast_to_derived_followed_by_multiple_includes(bool async)
    {
        await base.Cast_to_derived_followed_by_multiple_includes(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u0` ON (`u`.`DefeatedByNickname` = `u0`.`Nickname`) AND (`u`.`DefeatedBySquadId` = `u0`.`SquadId`)
LEFT JOIN `Weapons` AS `w` ON `u0`.`FullName` = `w`.`OwnerFullName`
WHERE `u`.`Name` LIKE '%Queen%'
ORDER BY `u`.`Name`, `u0`.`Nickname`, `u0`.`SquadId`
""");
    }

    public override async Task Correlated_collection_with_distinct_projecting_identifier_column(bool async)
    {
        await base.Correlated_collection_with_distinct_projecting_identifier_column(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w0`.`Id`, `w0`.`Name`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN LATERAL (
    SELECT DISTINCT `w`.`Id`, `w`.`Name`
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
) AS `w0` ON TRUE
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Where_equals_method_on_nullable_with_object_overload(bool async)
    {
        await base.Where_equals_method_on_nullable_with_object_overload(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE `m`.`Rating` IS NULL
""");
    }

    public override async Task
        Correlated_collection_with_groupby_not_projecting_identifier_column_but_only_grouping_key_in_final_projection(bool async)
    {
        await base.Correlated_collection_with_groupby_not_projecting_identifier_column_but_only_grouping_key_in_final_projection(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w0`.`Key`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN LATERAL (
    SELECT `w`.`IsAutomatic` AS `Key`
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
    GROUP BY `w`.`IsAutomatic`
) AS `w0` ON TRUE
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Project_derivied_entity_with_convert_to_parent(bool async)
    {
        await base.Project_derivied_entity_with_convert_to_parent(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`CapitalName`, `l`.`Name`, `l`.`ServerAddress`, `l`.`CommanderName`, `l`.`Eradicated`
FROM `LocustHordes` AS `l`
""");
    }

    public override async Task Include_after_SelectMany_throws(bool async)
    {
        await base.Include_after_SelectMany_throws(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `LocustHordes` AS `l`
LEFT JOIN `Cities` AS `c` ON `l`.`CapitalName` = `c`.`Name`
INNER JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u` ON `c`.`Name` = `u`.`CityOfBirthName`
INNER JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
""");
    }

    public override async Task Correlated_collection_with_distinct_projecting_identifier_column_composite_key(bool async)
    {
        await base.Correlated_collection_with_distinct_projecting_identifier_column_composite_key(async);

        AssertSql(
"""
SELECT `s`.`Id`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`HasSoulPatch`
FROM `Squads` AS `s`
LEFT JOIN (
    SELECT DISTINCT `u`.`Nickname`, `u`.`SquadId`, `u`.`HasSoulPatch`
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`HasSoulPatch`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`HasSoulPatch`
        FROM `Officers` AS `o`
    ) AS `u`
) AS `u0` ON `s`.`Id` = `u0`.`SquadId`
ORDER BY `s`.`Id`, `u0`.`Nickname`
""");
    }

    public override async Task Include_on_entity_that_is_not_present_in_final_projection_but_uses_TypeIs_instead(bool async)
    {
        await base.Include_on_entity_that_is_not_present_in_final_projection_but_uses_TypeIs_instead(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`Discriminator` = 'Officer' AS `IsOfficer`
FROM (
    SELECT `g`.`Nickname`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task GroupBy_Select_sum(bool async)
    {
        await base.GroupBy_Select_sum(async);

        AssertSql(
"""
SELECT COALESCE(SUM(`m`.`Rating`), 0.0)
FROM `Missions` AS `m`
GROUP BY `m`.`CodeName`
""");
    }

    public override async Task ToString_boolean_property_nullable(bool async)
    {
        await base.ToString_boolean_property_nullable(async);

        AssertSql(
"""
SELECT CASE `l`.`Eradicated`
    WHEN FALSE THEN 'False'
    WHEN TRUE THEN 'True'
    ELSE ''
END
FROM `LocustHordes` AS `l`
""");
    }

    public override async Task Correlated_collection_after_distinct_3_levels(bool async)
    {
        await base.Correlated_collection_after_distinct_3_levels(async);

        AssertSql(
"""
SELECT `s0`.`Id`, `s0`.`Name`, `s1`.`Nickname`, `s1`.`FullName`, `s1`.`HasSoulPatch`, `s1`.`Id`, `s1`.`Name`, `s1`.`Nickname0`, `s1`.`FullName0`, `s1`.`HasSoulPatch0`, `s1`.`Id0`
FROM (
    SELECT DISTINCT `s`.`Id`, `s`.`Name`
    FROM `Squads` AS `s`
) AS `s0`
LEFT JOIN LATERAL (
    SELECT `u0`.`Nickname`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `w0`.`Id`, `w0`.`Name`, `w0`.`Nickname` AS `Nickname0`, `w0`.`FullName` AS `FullName0`, `w0`.`HasSoulPatch` AS `HasSoulPatch0`, `w0`.`Id0`
    FROM (
        SELECT DISTINCT `u`.`Nickname`, `u`.`FullName`, `u`.`HasSoulPatch`
        FROM (
            SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`, `g`.`HasSoulPatch`
            FROM `Gears` AS `g`
            UNION ALL
            SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`, `o`.`HasSoulPatch`
            FROM `Officers` AS `o`
        ) AS `u`
        WHERE `u`.`SquadId` = `s0`.`Id`
    ) AS `u0`
    LEFT JOIN LATERAL (
        SELECT `s0`.`Id`, `s0`.`Name`, `u0`.`Nickname`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `w`.`Id` AS `Id0`
        FROM `Weapons` AS `w`
        WHERE `w`.`OwnerFullName` = `u0`.`FullName`
    ) AS `w0` ON TRUE
) AS `s1` ON TRUE
ORDER BY `s0`.`Id`, `s1`.`Nickname`, `s1`.`FullName`, `s1`.`HasSoulPatch`
""");
    }

    public override async Task ToString_boolean_property_non_nullable(bool async)
    {
        await base.ToString_boolean_property_non_nullable(async);

        AssertSql(
"""
SELECT CASE
    WHEN `w`.`IsAutomatic` = TRUE THEN 'True'
    ELSE 'False'
END
FROM `Weapons` AS `w`
""");
    }

    public override async Task Include_on_derived_entity_with_cast(bool async)
    {
        await base.Include_on_derived_entity_with_cast(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`CapitalName`, `l`.`Name`, `l`.`ServerAddress`, `l`.`CommanderName`, `l`.`Eradicated`, `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM `LocustHordes` AS `l`
LEFT JOIN `Cities` AS `c` ON `l`.`CapitalName` = `c`.`Name`
ORDER BY `l`.`Id`
""");
    }

    public override async Task String_concat_nullable_expressions_are_coalesced(bool async)
    {
        await base.String_concat_nullable_expressions_are_coalesced(async);

        AssertSql(
"""
SELECT CONCAT(CONCAT(CONCAT(`u`.`FullName`, ''), COALESCE(`u`.`LeaderNickname`, '')), '')
FROM (
    SELECT `g`.`FullName`, `g`.`LeaderNickname`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`FullName`, `o`.`LeaderNickname`
    FROM `Officers` AS `o`
) AS `u`
""");
    }

    public override async Task Correlated_collection_with_distinct_projecting_identifier_column_and_correlation_key(bool async)
    {
        await base.Correlated_collection_with_distinct_projecting_identifier_column_and_correlation_key(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w0`.`Id`, `w0`.`Name`, `w0`.`OwnerFullName`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT DISTINCT `w`.`Id`, `w`.`Name`, `w`.`OwnerFullName`
    FROM `Weapons` AS `w`
) AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Correlated_collection_with_groupby_not_projecting_identifier_column_with_group_aggregate_in_final_projection(
        bool async)
    {
        await base.Correlated_collection_with_groupby_not_projecting_identifier_column_with_group_aggregate_in_final_projection(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `w0`.`Key`, `w0`.`Count`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN LATERAL (
    SELECT `w`.`IsAutomatic` AS `Key`, COUNT(*) AS `Count`
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`
    GROUP BY `w`.`IsAutomatic`
) AS `w0` ON TRUE
ORDER BY `u`.`Nickname`, `u`.`SquadId`
""");
    }

    public override async Task Project_discriminator_columns(bool async)
    {
        await base.Project_discriminator_columns(async);

        AssertSql();
    }

    [ConditionalTheory(Skip = "Another LATERAL JOIN bug in MySQL. Grouping leads to unexpected result set.")]
    public override async Task
        Correlated_collection_with_groupby_with_complex_grouping_key_not_projecting_identifier_column_with_group_aggregate_in_final_projection(
            bool async)
    {
        await base
            .Correlated_collection_with_groupby_with_complex_grouping_key_not_projecting_identifier_column_with_group_aggregate_in_final_projection(
                async);

        AssertSql("");
    }

    public override async Task Correlated_collection_with_distinct_not_projecting_identifier_column_also_projecting_complex_expressions(
        bool async)
    {
        await base.Correlated_collection_with_distinct_not_projecting_identifier_column_also_projecting_complex_expressions(async);

        AssertSql();
    }

    public override async Task Client_eval_followed_by_aggregate_operation(bool async)
    {
        await base.Client_eval_followed_by_aggregate_operation(async);

        AssertSql();
    }

    // TODO: Implement strategy as discussed with @roji (including emails) for EF Core 5.
    [ConditionalTheory(Skip = "#996")]
    public override async Task Client_member_and_unsupported_string_Equals_in_the_same_query(bool async)
    {
        await base.Client_member_and_unsupported_string_Equals_in_the_same_query(async);

        AssertSql();
    }

    public override async Task Client_side_equality_with_parameter_works_with_optional_navigations(bool async)
    {
        await base.Client_side_equality_with_parameter_works_with_optional_navigations(async);

        AssertSql();
    }

    public override async Task Correlated_collection_order_by_constant_null_of_non_mapped_type(bool async)
    {
        await base.Correlated_collection_order_by_constant_null_of_non_mapped_type(async);

        AssertSql();
    }

    public override async Task GetValueOrDefault_on_DateTimeOffset(bool async)
    {
        await base.GetValueOrDefault_on_DateTimeOffset(async);

        AssertSql();
    }

    public override async Task Where_coalesce_with_anonymous_types(bool async)
    {
        await base.Where_coalesce_with_anonymous_types(async);

        AssertSql();
    }

    public override async Task Projecting_correlated_collection_followed_by_Distinct(bool async)
    {
        await base.Projecting_correlated_collection_followed_by_Distinct(async);

        AssertSql();
    }

    public override async Task Projecting_some_properties_as_well_as_correlated_collection_followed_by_Distinct(bool async)
    {
        await base.Projecting_some_properties_as_well_as_correlated_collection_followed_by_Distinct(async);

        AssertSql();
    }

    public override async Task Projecting_entity_as_well_as_correlated_collection_followed_by_Distinct(bool async)
    {
        await base.Projecting_entity_as_well_as_correlated_collection_followed_by_Distinct(async);

        AssertSql();
    }

    public override async Task Projecting_entity_as_well_as_complex_correlated_collection_followed_by_Distinct(bool async)
    {
        await base.Projecting_entity_as_well_as_complex_correlated_collection_followed_by_Distinct(async);

        AssertSql();
    }

    public override async Task Projecting_entity_as_well_as_correlated_collection_of_scalars_followed_by_Distinct(bool async)
    {
        await base.Projecting_entity_as_well_as_correlated_collection_of_scalars_followed_by_Distinct(async);

        AssertSql();
    }

    public override async Task Correlated_collection_with_distinct_3_levels(bool async)
    {
        await base.Correlated_collection_with_distinct_3_levels(async);

        AssertSql();
    }

    public override async Task Correlated_collection_after_distinct_3_levels_without_original_identifiers(bool async)
    {
        await base.Correlated_collection_after_distinct_3_levels_without_original_identifiers(async);

        AssertSql();
    }

    public override async Task Checked_context_throws_on_client_evaluation(bool async)
    {
        await base.Checked_context_throws_on_client_evaluation(async);

        AssertSql();
    }

    public override async Task Trying_to_access_unmapped_property_throws_informative_error(bool async)
    {
        await base.Trying_to_access_unmapped_property_throws_informative_error(async);

        AssertSql();
    }

    public override async Task Trying_to_access_unmapped_property_inside_aggregate(bool async)
    {
        await base.Trying_to_access_unmapped_property_inside_aggregate(async);

        AssertSql();
    }

    public override async Task Trying_to_access_unmapped_property_inside_subquery(bool async)
    {
        await base.Trying_to_access_unmapped_property_inside_subquery(async);

        AssertSql();
    }

    public override async Task Trying_to_access_unmapped_property_inside_join_key_selector(bool async)
    {
        await base.Trying_to_access_unmapped_property_inside_join_key_selector(async);

        AssertSql();
    }

    public override async Task Client_projection_with_nested_unmapped_property_bubbles_up_translation_failure_info(bool async)
    {
        await base.Client_projection_with_nested_unmapped_property_bubbles_up_translation_failure_info(async);

        AssertSql();
    }

    public override async Task Include_after_select_with_cast_throws(bool async)
    {
        await base.Include_after_select_with_cast_throws(async);

        AssertSql();
    }

    public override async Task Include_after_select_with_entity_projection_throws(bool async)
    {
        await base.Include_after_select_with_entity_projection_throws(async);

        AssertSql();
    }

    public override async Task Include_after_select_anonymous_projection_throws(bool async)
    {
        await base.Include_after_select_anonymous_projection_throws(async);

        AssertSql();
    }

    public override async Task Group_by_with_aggregate_max_on_entity_type(bool async)
    {
        await base.Group_by_with_aggregate_max_on_entity_type(async);

        AssertSql();
    }

    public override async Task Include_collection_and_invalid_navigation_using_string_throws(bool async)
    {
        await base.Include_collection_and_invalid_navigation_using_string_throws(async);

        AssertSql();
    }

    public override async Task Include_with_concat(bool async)
    {
        await base.Include_with_concat(async);

        AssertSql();
    }

    public override async Task Join_with_complex_key_selector(bool async)
    {
        await base.Join_with_complex_key_selector(async);

        AssertSql(
"""
SELECT `s`.`Id`, `t0`.`Id` AS `TagId`
FROM `Squads` AS `s`
CROSS JOIN (
    SELECT `t`.`Id`
    FROM `Tags` AS `t`
    WHERE `t`.`Note` = 'Marcus'' Tag'
) AS `t0`
""");
    }

    public override async Task Streaming_correlated_collection_issue_11403_returning_ordered_enumerable_throws(bool async)
    {
        await base.Streaming_correlated_collection_issue_11403_returning_ordered_enumerable_throws(async);

        AssertSql();
    }

    public override async Task Select_correlated_filtered_collection_returning_queryable_throws(bool async)
    {
        await base.Select_correlated_filtered_collection_returning_queryable_throws(async);

        AssertSql();
    }

    public override async Task Client_method_on_collection_navigation_in_predicate(bool async)
    {
        await base.Client_method_on_collection_navigation_in_predicate(async);

        AssertSql();
    }

    public override async Task Client_method_on_collection_navigation_in_predicate_accessed_by_ef_property(bool async)
    {
        await base.Client_method_on_collection_navigation_in_predicate_accessed_by_ef_property(async);

        AssertSql();
    }

    public override async Task Client_method_on_collection_navigation_in_order_by(bool async)
    {
        await base.Client_method_on_collection_navigation_in_order_by(async);

        AssertSql();
    }

    public override async Task Client_method_on_collection_navigation_in_additional_from_clause(bool async)
    {
        await base.Client_method_on_collection_navigation_in_additional_from_clause(async);

        AssertSql();
    }

    public override async Task Include_multiple_one_to_one_and_one_to_many_self_reference(bool async)
    {
        await base.Include_multiple_one_to_one_and_one_to_many_self_reference(async);

        AssertSql();
    }

    public override async Task Include_multiple_one_to_one_and_one_to_one_and_one_to_many(bool async)
    {
        await base.Include_multiple_one_to_one_and_one_to_one_and_one_to_many(async);

        AssertSql();
    }

    public override async Task Include_multiple_include_then_include(bool async)
    {
        await base.Include_multiple_include_then_include(async);

        AssertSql();
    }

    public override async Task Select_Where_Navigation_Client(bool async)
    {
        await base.Select_Where_Navigation_Client(async);

        AssertSql();
    }

    public override async Task Where_subquery_equality_to_null_with_composite_key(bool async)
    {
        await base.Where_subquery_equality_to_null_with_composite_key(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE NOT EXISTS (
    SELECT 1
    FROM (
        SELECT `g`.`SquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`SquadId`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE `s`.`Id` = `u`.`SquadId`)
""");
    }

    public override async Task Where_subquery_equality_to_null_without_composite_key(bool async)
    {
        await base.Where_subquery_equality_to_null_without_composite_key(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE NOT EXISTS (
    SELECT 1
    FROM `Weapons` AS `w`
    WHERE `u`.`FullName` = `w`.`OwnerFullName`)
""");
    }

    public override async Task Include_reference_on_derived_type_using_EF_Property(bool async)
    {
        await base.Include_reference_on_derived_type_using_EF_Property(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u0` ON (`u`.`DefeatedByNickname` = `u0`.`Nickname`) AND (`u`.`DefeatedBySquadId` = `u0`.`SquadId`)
""");
    }

    public override async Task Include_collection_on_derived_type_using_EF_Property(bool async)
    {
        await base.Include_collection_on_derived_type_using_EF_Property(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task EF_Property_based_Include_navigation_on_derived_type(bool async)
    {
        await base.EF_Property_based_Include_navigation_on_derived_type(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`
FROM (
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON (`u`.`Nickname` = `u0`.`LeaderNickname`) AND (`u`.`SquadId` = `u0`.`LeaderSquadId`)
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`
""");
    }

    public override async Task ToString_string_property_projection(bool async)
    {
        await base.ToString_string_property_projection(async);

        AssertSql(
"""
SELECT `w`.`Name`
FROM `Weapons` AS `w`
""");
    }

    public override async Task ElementAt_basic_with_OrderBy(bool async)
    {
        await base.ElementAt_basic_with_OrderBy(async);

        AssertSql(
"""
@__p_0='0'

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
ORDER BY `u`.`FullName`
LIMIT 1 OFFSET @__p_0
""");
    }

    public override async Task ElementAtOrDefault_basic_with_OrderBy(bool async)
    {
        await base.ElementAtOrDefault_basic_with_OrderBy(async);

        AssertSql(
"""
@__p_0='1'

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
ORDER BY `u`.`FullName`
LIMIT 1 OFFSET @__p_0
""");
    }

    public override async Task ElementAtOrDefault_basic_with_OrderBy_parameter(bool async)
    {
        await base.ElementAtOrDefault_basic_with_OrderBy_parameter(async);

        AssertSql(
"""
@__p_0='2'

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
ORDER BY `u`.`FullName`
LIMIT 1 OFFSET @__p_0
""");
    }

    public override async Task Where_subquery_with_ElementAtOrDefault_equality_to_null_with_composite_key(bool async)
    {
        await base.Where_subquery_with_ElementAtOrDefault_equality_to_null_with_composite_key(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE NOT EXISTS (
    SELECT 1
    FROM (
        SELECT `g`.`Nickname`, `g`.`SquadId`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`Nickname`, `o`.`SquadId`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE `s`.`Id` = `u`.`SquadId`
    ORDER BY `u`.`Nickname`
    LIMIT 18446744073709551610 OFFSET 2)
""");
    }

    [SupportedServerVersionCondition(nameof(ServerVersionSupport.LimitWithNonConstantValue))]
    public override async Task Where_subquery_with_ElementAt_using_column_as_index(bool async)
    {
        await base.Where_subquery_with_ElementAt_using_column_as_index(async);

        AssertSql("");
    }

    public override async Task Using_indexer_on_byte_array_and_string_in_projection(bool async)
    {
        await base.Using_indexer_on_byte_array_and_string_in_projection(async);

        AssertSql(
"""
SELECT `s`.`Id`, ASCII(SUBSTRING(`s`.`Banner`, 0 + 1, 1)), `s`.`Name`
FROM `Squads` AS `s`
""");
    }

    public override async Task DateTimeOffset_to_unix_time_milliseconds(bool async)
    {
        await base.DateTimeOffset_to_unix_time_milliseconds(async);

        AssertSql(
"""
@__unixEpochMilliseconds_0='0'

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`, `s1`.`SquadId`, `s1`.`MissionId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
LEFT JOIN `SquadMissions` AS `s1` ON `s`.`Id` = `s1`.`SquadId`
WHERE NOT EXISTS (
    SELECT 1
    FROM `SquadMissions` AS `s0`
    INNER JOIN `Missions` AS `m` ON `s0`.`MissionId` = `m`.`Id`
    WHERE (`s`.`Id` = `s0`.`SquadId`) AND (@__unixEpochMilliseconds_0 = (TIMESTAMPDIFF(microsecond, TIMESTAMP '1970-01-01 00:00:00', `m`.`Timeline`)) DIV (1000)))
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `s`.`Id`, `s1`.`SquadId`
""");
    }

    public override async Task DateTimeOffset_to_unix_time_seconds(bool async)
    {
        await base.DateTimeOffset_to_unix_time_seconds(async);

        AssertSql(
"""
@__unixEpochSeconds_0='0'

SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`, `s1`.`SquadId`, `s1`.`MissionId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
LEFT JOIN `SquadMissions` AS `s1` ON `s`.`Id` = `s1`.`SquadId`
WHERE NOT EXISTS (
    SELECT 1
    FROM `SquadMissions` AS `s0`
    INNER JOIN `Missions` AS `m` ON `s0`.`MissionId` = `m`.`Id`
    WHERE (`s`.`Id` = `s0`.`SquadId`) AND (@__unixEpochSeconds_0 = TIMESTAMPDIFF(second, TIMESTAMP '1970-01-01 00:00:00', `m`.`Timeline`)))
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `s`.`Id`, `s1`.`SquadId`
""");
    }

    public override async Task Set_operator_with_navigation_in_projection_groupby_aggregate(bool async)
    {
        await base.Set_operator_with_navigation_in_projection_groupby_aggregate(async);

        AssertSql(
"""
SELECT `s`.`Name`, (
    SELECT COALESCE(SUM(CHAR_LENGTH(`c`.`Location`)), 0)
    FROM (
        SELECT `g2`.`SquadId`, `g2`.`CityOfBirthName`
        FROM `Gears` AS `g2`
        UNION ALL
        SELECT `o2`.`SquadId`, `o2`.`CityOfBirthName`
        FROM `Officers` AS `o2`
    ) AS `u3`
    INNER JOIN `Squads` AS `s0` ON `u3`.`SquadId` = `s0`.`Id`
    INNER JOIN `Cities` AS `c` ON `u3`.`CityOfBirthName` = `c`.`Name`
    WHERE 'Marcus' IN (
        SELECT `g3`.`Nickname`
        FROM `Gears` AS `g3`
        UNION ALL
        SELECT `o3`.`Nickname`
        FROM `Officers` AS `o3`
        UNION ALL
        SELECT `g4`.`Nickname`
        FROM `Gears` AS `g4`
        UNION ALL
        SELECT `o4`.`Nickname`
        FROM `Officers` AS `o4`
    ) AND ((`s`.`Name` = `s0`.`Name`) OR (`s`.`Name` IS NULL AND (`s0`.`Name` IS NULL)))) AS `SumOfLengths`
FROM (
    SELECT `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u`
INNER JOIN `Squads` AS `s` ON `u`.`SquadId` = `s`.`Id`
WHERE 'Marcus' IN (
    SELECT `g0`.`Nickname`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`
    FROM `Officers` AS `o0`
    UNION ALL
    SELECT `g1`.`Nickname`
    FROM `Gears` AS `g1`
    UNION ALL
    SELECT `o1`.`Nickname`
    FROM `Officers` AS `o1`
)
GROUP BY `s`.`Name`
""");
    }

    public override async Task Where_subquery_equality_to_null_with_composite_key_should_match_nulls(bool async)
    {
        await base.Where_subquery_equality_to_null_with_composite_key_should_match_nulls(async);

        AssertSql(
"""
SELECT `s`.`Id`, `s`.`Banner`, `s`.`Banner5`, `s`.`InternalNumber`, `s`.`Name`
FROM `Squads` AS `s`
WHERE NOT EXISTS (
    SELECT 1
    FROM (
        SELECT `g`.`SquadId`, `g`.`FullName`
        FROM `Gears` AS `g`
        UNION ALL
        SELECT `o`.`SquadId`, `o`.`FullName`
        FROM `Officers` AS `o`
    ) AS `u`
    WHERE (`s`.`Id` = `u`.`SquadId`) AND (`u`.`FullName` = 'Anthony Carmine'))
""");
    }

    public override async Task Where_subquery_equality_to_null_without_composite_key_should_match_null(bool async)
    {
        await base.Where_subquery_equality_to_null_without_composite_key_should_match_null(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
WHERE NOT EXISTS (
    SELECT 1
    FROM `Weapons` AS `w`
    WHERE (`u`.`FullName` = `w`.`OwnerFullName`) AND (`w`.`Name` = 'Hammer of Dawn'))
""");
    }

    public override async Task Nav_expansion_inside_Contains_argument(bool async)
    {
        await base.Nav_expansion_inside_Contains_argument(async);

        AssertSql(
"""
SELECT `t`.`Nickname`, `t`.`SquadId`, `t`.`AssignedCityName`, `t`.`CityOfBirthName`, `t`.`FullName`, `t`.`HasSoulPatch`, `t`.`LeaderNickname`, `t`.`LeaderSquadId`, `t`.`Rank`, `t`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `t`
WHERE CASE
    WHEN EXISTS (
        SELECT 1
        FROM `Weapons` AS `w`
        WHERE `t`.`FullName` = `w`.`OwnerFullName`) THEN 1
    ELSE 0
END IN (1, -1)
""");
    }

    public override async Task Nav_expansion_with_member_pushdown_inside_Contains_argument(bool async)
    {
        await base.Nav_expansion_with_member_pushdown_inside_Contains_argument(async);

        AssertSql(
"""
SELECT `t`.`Nickname`, `t`.`SquadId`, `t`.`AssignedCityName`, `t`.`CityOfBirthName`, `t`.`FullName`, `t`.`HasSoulPatch`, `t`.`LeaderNickname`, `t`.`LeaderSquadId`, `t`.`Rank`, `t`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `t`
WHERE (
    SELECT `w`.`Name`
    FROM `Weapons` AS `w`
    WHERE `t`.`FullName` = `w`.`OwnerFullName`
    ORDER BY `w`.`Id`
    LIMIT 1) IN ('Marcus'' Lancer', 'Dom''s Gnasher')
""");
    }

    public override async Task Subquery_inside_Take_argument(bool async)
    {
        await base.Subquery_inside_Take_argument(async);

        AssertSql("");
    }

    public override async Task Nav_expansion_inside_Skip_correlated_to_source(bool async)
    {
        await base.Nav_expansion_inside_Skip_correlated_to_source(async);

        AssertSql("");
    }

    public override async Task Nav_expansion_inside_Take_correlated_to_source(bool async)
    {
        await base.Nav_expansion_inside_Take_correlated_to_source(async);

        AssertSql("");
    }

    public override async Task Nav_expansion_with_member_pushdown_inside_Take_correlated_to_source(bool async)
    {
        await base.Nav_expansion_with_member_pushdown_inside_Take_correlated_to_source(async);

        AssertSql("");
    }

    public override async Task Nav_expansion_inside_ElementAt_correlated_to_source(bool async)
    {
        await base.Nav_expansion_inside_ElementAt_correlated_to_source(async);

        AssertSql("");
    }

    public override async Task Unicode_string_literals_is_used_for_non_unicode_column_with_concat(bool async)
    {
        await base.Unicode_string_literals_is_used_for_non_unicode_column_with_concat(async);

        AssertSql(
"""
SELECT `c`.`Name`, `c`.`Location`, `c`.`Nation`
FROM `Cities` AS `c`
WHERE CONCAT(COALESCE(`c`.`Location`, ''), 'Added') LIKE '%Add%'
""");
    }

    public override async Task Include_one_to_many_on_composite_key_then_orderby_key_properties(bool async)
    {
        await base.Include_one_to_many_on_composite_key_then_orderby_key_properties(async);

        AssertSql(
"""
SELECT `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u`.`SquadId`, `u`.`Nickname`
""");
    }

    public override async Task Find_underlying_property_after_GroupJoin_DefaultIfEmpty(bool async)
    {
        await base.Find_underlying_property_after_GroupJoin_DefaultIfEmpty(async);

        AssertSql(
"""
SELECT `u`.`FullName`, CAST(`u0`.`ThreatLevel` AS signed) AS `ThreatLevel`
FROM (
    SELECT `g`.`Nickname`, `g`.`FullName`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`FullName`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `l`.`ThreatLevel`, `l`.`DefeatedByNickname`
    FROM `LocustCommanders` AS `l`
) AS `u0` ON `u`.`Nickname` = `u0`.`DefeatedByNickname`
""");
    }

    public override async Task Join_include_coalesce_simple(bool async)
    {
        await base.Join_include_coalesce_simple(async);

        AssertSql(
"""
SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `u`.`Nickname` = 'Marcus'
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`LeaderNickname` = `u0`.`Nickname`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`, `u0`.`SquadId`
""",
                //
                """
SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `u`.`Nickname`, `u`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`LeaderNickname` = `u0`.`Nickname`
LEFT JOIN `Weapons` AS `w` ON `u0`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`, `u0`.`SquadId`
""",
                //
                """
SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `u`.`Nickname`, `u`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`LeaderNickname` = `u0`.`Nickname`
LEFT JOIN `Weapons` AS `w` ON `u0`.`FullName` = `w`.`OwnerFullName`
LEFT JOIN `Weapons` AS `w0` ON `u`.`FullName` = `w0`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`, `u0`.`SquadId`, `w`.`Id`
""");
    }

    public override async Task Join_include_coalesce_nested(bool async)
    {
        await base.Join_include_coalesce_nested(async);

        AssertSql(
"""
SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `u`.`Nickname` = 'Marcus'
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`LeaderNickname` = `u0`.`Nickname`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`, `u0`.`SquadId`
""",
                //
                """
SELECT `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `u`.`Nickname`, `u`.`SquadId`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `w0`.`Id`, `w0`.`AmmunitionType`, `w0`.`IsAutomatic`, `w0`.`Name`, `w0`.`OwnerFullName`, `w0`.`SynergyWithId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w1`.`Id`, `w1`.`AmmunitionType`, `w1`.`IsAutomatic`, `w1`.`Name`, `w1`.`OwnerFullName`, `w1`.`SynergyWithId`
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`LeaderNickname` = `u0`.`Nickname`
LEFT JOIN `Weapons` AS `w` ON `u0`.`FullName` = `w`.`OwnerFullName`
LEFT JOIN `Weapons` AS `w0` ON `u0`.`FullName` = `w0`.`OwnerFullName`
LEFT JOIN `Weapons` AS `w1` ON `u0`.`FullName` = `w1`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`, `u0`.`SquadId`, `w`.`Id`, `w0`.`Id`
""");
    }

    public override async Task Join_include_conditional(bool async)
    {
        await base.Join_include_conditional(async);

        AssertSql(
"""
SELECT `u0`.`Nickname` IS NOT NULL AND (`u0`.`SquadId` IS NOT NULL), `u0`.`Nickname`, `u0`.`SquadId`, `u0`.`AssignedCityName`, `u0`.`CityOfBirthName`, `u0`.`FullName`, `u0`.`HasSoulPatch`, `u0`.`LeaderNickname`, `u0`.`LeaderSquadId`, `u0`.`Rank`, `u0`.`Discriminator`, `u`.`Nickname`, `u`.`SquadId`, `u`.`AssignedCityName`, `u`.`CityOfBirthName`, `u`.`FullName`, `u`.`HasSoulPatch`, `u`.`LeaderNickname`, `u`.`LeaderSquadId`, `u`.`Rank`, `u`.`Discriminator`, `w`.`Id`, `w`.`AmmunitionType`, `w`.`IsAutomatic`, `w`.`Name`, `w`.`OwnerFullName`, `w`.`SynergyWithId`, `u`.`Nickname` = 'Marcus'
FROM (
    SELECT `g`.`Nickname`, `g`.`SquadId`, `g`.`AssignedCityName`, `g`.`CityOfBirthName`, `g`.`FullName`, `g`.`HasSoulPatch`, `g`.`LeaderNickname`, `g`.`LeaderSquadId`, `g`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`, `o`.`AssignedCityName`, `o`.`CityOfBirthName`, `o`.`FullName`, `o`.`HasSoulPatch`, `o`.`LeaderNickname`, `o`.`LeaderSquadId`, `o`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o`
) AS `u`
LEFT JOIN (
    SELECT `g0`.`Nickname`, `g0`.`SquadId`, `g0`.`AssignedCityName`, `g0`.`CityOfBirthName`, `g0`.`FullName`, `g0`.`HasSoulPatch`, `g0`.`LeaderNickname`, `g0`.`LeaderSquadId`, `g0`.`Rank`, 'Gear' AS `Discriminator`
    FROM `Gears` AS `g0`
    UNION ALL
    SELECT `o0`.`Nickname`, `o0`.`SquadId`, `o0`.`AssignedCityName`, `o0`.`CityOfBirthName`, `o0`.`FullName`, `o0`.`HasSoulPatch`, `o0`.`LeaderNickname`, `o0`.`LeaderSquadId`, `o0`.`Rank`, 'Officer' AS `Discriminator`
    FROM `Officers` AS `o0`
) AS `u0` ON `u`.`LeaderNickname` = `u0`.`Nickname`
LEFT JOIN `Weapons` AS `w` ON `u`.`FullName` = `w`.`OwnerFullName`
ORDER BY `u`.`Nickname`, `u`.`SquadId`, `u0`.`Nickname`, `u0`.`SquadId`
""");
    }

    public override async Task Derived_reference_is_skipped_when_base_type(bool async)
    {
        await base.Derived_reference_is_skipped_when_base_type(async);

        AssertSql(
"""
SELECT `u`.`Name`, `u`.`LocustHordeId`, `u`.`ThreatLevel`, `u`.`ThreatLevelByte`, `u`.`ThreatLevelNullableByte`, `u`.`DefeatedByNickname`, `u`.`DefeatedBySquadId`, `u`.`HighCommandId`, `u`.`Discriminator`, `l1`.`Id`, `l1`.`IsOperational`, `l1`.`Name`
FROM (
    SELECT `l`.`Name`, `l`.`LocustHordeId`, `l`.`ThreatLevel`, `l`.`ThreatLevelByte`, `l`.`ThreatLevelNullableByte`, NULL AS `DefeatedByNickname`, NULL AS `DefeatedBySquadId`, NULL AS `HighCommandId`, 'LocustLeader' AS `Discriminator`
    FROM `LocustLeaders` AS `l`
    UNION ALL
    SELECT `l0`.`Name`, `l0`.`LocustHordeId`, `l0`.`ThreatLevel`, `l0`.`ThreatLevelByte`, `l0`.`ThreatLevelNullableByte`, `l0`.`DefeatedByNickname`, `l0`.`DefeatedBySquadId`, `l0`.`HighCommandId`, 'LocustCommander' AS `Discriminator`
    FROM `LocustCommanders` AS `l0`
) AS `u`
LEFT JOIN `LocustHighCommands` AS `l1` ON `u`.`HighCommandId` = `l1`.`Id`
""");
    }

    public override async Task Nested_contains_with_enum(bool async)
    {
        await base.Nested_contains_with_enum(async);

        AssertSql("");
    }

    public override async Task ToString_boolean_computed_nullable(bool async)
    {
        await base.ToString_boolean_computed_nullable(async);

        AssertSql(
"""
SELECT CASE (`l`.`Eradicated` = TRUE) OR ((`l`.`CommanderName` = 'Unknown') AND `l`.`CommanderName` IS NOT NULL)
    WHEN FALSE THEN 'False'
    WHEN TRUE THEN 'True'
    ELSE ''
END
FROM `LocustHordes` AS `l`
""");
    }

    public override async Task Select_inverted_nullable_boolean(bool async)
    {
        await base.Select_inverted_nullable_boolean(async);

        AssertSql(
"""
SELECT `l`.`Id`, `l`.`Eradicated` = FALSE AS `Alive`
FROM `LocustHordes` AS `l`
""");
    }

    public override async Task Where_TimeOnly_FromDateTime_compared_to_property(bool async)
    {
        await base.Where_TimeOnly_FromDateTime_compared_to_property(async);

        AssertSql(
"""
SELECT `t`.`Id` AS `TagId`, `m`.`Id` AS `MissionId`
FROM `Tags` AS `t`
CROSS JOIN `Missions` AS `m`
WHERE TIME(`t`.`IssueDate`) = `m`.`Time`
""");
    }

    public override async Task Where_TimeOnly_FromDateTime_compared_to_parameter(bool async)
    {
        await base.Where_TimeOnly_FromDateTime_compared_to_parameter(async);

        AssertSql(
"""
@__time_0='02:00' (DbType = Time)

SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
LEFT JOIN (
    SELECT `g`.`Nickname`, `g`.`SquadId`
    FROM `Gears` AS `g`
    UNION ALL
    SELECT `o`.`Nickname`, `o`.`SquadId`
    FROM `Officers` AS `o`
) AS `u` ON (`t`.`GearNickName` = `u`.`Nickname`) AND (`t`.`GearSquadId` = `u`.`SquadId`)
WHERE (`u`.`Nickname` IS NOT NULL AND (`u`.`SquadId` IS NOT NULL)) AND (TIME(DATE_ADD(`t`.`IssueDate`, INTERVAL CAST(CAST(`u`.`SquadId` AS double) AS signed) hour)) = @__time_0)
""");
    }

    public override async Task Where_TimeOnly_FromDateTime_compared_to_constant(bool async)
    {
        await base.Where_TimeOnly_FromDateTime_compared_to_constant(async);

        AssertSql(
"""
SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
WHERE TIME(DATE_ADD(`t`.`IssueDate`, INTERVAL CAST(CAST(CHAR_LENGTH(`t`.`Note`) AS double) AS signed) hour)) > TIME '09:00:00'
""");
    }

    public override async Task Where_TimeOnly_FromTimeSpan_compared_to_property(bool async)
    {
        await base.Where_TimeOnly_FromTimeSpan_compared_to_property(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE `m`.`Duration` < `m`.`Time`
""");
    }

    public override async Task Where_TimeOnly_FromTimeSpan_compared_to_parameter(bool async)
    {
        await base.Where_TimeOnly_FromTimeSpan_compared_to_parameter(async);

        AssertSql(
"""
@__time_0='01:02' (DbType = Time)

SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
WHERE `m`.`Duration` = @__time_0
""");
    }

    public override async Task Order_by_TimeOnly_FromTimeSpan(bool async)
    {
        await base.Order_by_TimeOnly_FromTimeSpan(async);

        AssertSql(
"""
SELECT `m`.`Id`, `m`.`CodeName`, `m`.`Date`, `m`.`Difficulty`, `m`.`Duration`, `m`.`Rating`, `m`.`Time`, `m`.`Timeline`
FROM `Missions` AS `m`
ORDER BY `m`.`Duration`
""");
    }

    public override async Task Where_DateOnly_FromDateTime_compared_to_property(bool async)
    {
        await base.Where_DateOnly_FromDateTime_compared_to_property(async);

        AssertSql(
"""
SELECT `t`.`Id` AS `TagId`, `m`.`Id` AS `MissionId`
FROM `Tags` AS `t`
CROSS JOIN `Missions` AS `m`
WHERE DATE(`t`.`IssueDate`) > `m`.`Date`
""");
    }

    public override async Task Where_DateOnly_FromDateTime_compared_to_constant_and_parameter(bool async)
    {
        await base.Where_DateOnly_FromDateTime_compared_to_constant_and_parameter(async);

        AssertSql(
"""
@__prm_0='10/11/0002' (DbType = Date)

SELECT `t`.`Id`, `t`.`GearNickName`, `t`.`GearSquadId`, `t`.`IssueDate`, `t`.`Note`
FROM `Tags` AS `t`
WHERE DATE(`t`.`IssueDate`) IN (@__prm_0, DATE '0015-03-07')
""");
    }

    // TODO: Implement once TimeSpan is translated as ticks instead of TIME.
    public override async Task Non_string_concat_uses_appropriate_type_mapping(bool async)
    {
        var exception = await Assert.ThrowsAsync<InvalidCastException>(() => base.Non_string_concat_uses_appropriate_type_mapping(async));
        Assert.Equal("Unable to cast object of type 'System.Decimal' to type 'System.TimeSpan'.", exception.Message);
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);
}
