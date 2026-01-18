# JSON Owned Entity Update Issue in EF Core 10

> **⚠️ IMPORTANT**: This is a known bug in EF Core 10 (not Pomelo-specific)  
> **Track upstream fix**: [dotnet/efcore#37411](https://github.com/dotnet/efcore/issues/37411)  
> **Workaround available**: See below for tested solutions

## Issue Description

When using `.ToJson()` with owned entities in EF Core 10, updating individual properties within the JSON-mapped entity can fail with:

```
MySqlException: Invalid JSON text: "Invalid value." at position 0 in value for column 'TableName.ColumnName'.
```

This is a known issue in EF Core 10 (see [dotnet/efcore#37411](https://github.com/dotnet/efcore/issues/37411)) that affects multiple database providers including MySQL, SQLite, and others.

## Root Cause

EF Core 10's change tracking incorrectly handles updates to JSON-owned entities:
- During INSERT: The entire JSON object is correctly serialized and sent to the database
- During UPDATE: Only the modified property value is sent, with an incorrect type mapping (`MySqlStringTypeMapping` instead of `MySqlStructuralJsonTypeMapping`)

This causes MySQL to receive a plain string (e.g., `"New Name"`) instead of a complete JSON object, resulting in the "Invalid JSON text" error.

## Example

```csharp
public class User
{
    public string Id { get; set; }
    public PasskeyData Data { get; set; }  // Mapped with .ToJson()
}

public class PasskeyData
{
    public string Name { get; set; }
    public string[] Transports { get; set; }
    // ... other properties
}

// This fails:
var user = await context.Users.FindAsync(id);
user.Data.Name = "New Name";
await context.SaveChangesAsync();  // ❌ MySqlException: Invalid JSON text
```

## Workarounds

Until EF Core fixes this issue, use one of these workarounds:

### Workaround 1: Detach and Re-attach (RECOMMENDED)

Query with `AsNoTracking()`, modify, then use `Update()`:

```csharp
// Query without tracking
var user = await context.Users
    .AsNoTracking()
    .FirstAsync(u => u.Id == id);

// Modify the property
user.Data.Name = "New Name";

// Update the detached entity
context.Users.Update(user);
await context.SaveChangesAsync();  // ✓ Works
```

### Workaround 2: Replace the Entire Object

Create a new instance of the owned entity:

```csharp
var user = await context.Users.FindAsync(id);

// Create a new instance with updated values
user.Data = new PasskeyData
{
    Name = "New Name",
    Transports = user.Data.Transports,
    // ... copy other properties
};

await context.SaveChangesAsync();  // ✓ Works
```

### Workaround 3: Direct Database Update

Use a raw SQL command (not recommended as it bypasses EF Core):

```csharp
await context.Database.ExecuteSqlRawAsync(
    @"UPDATE Users SET Data = JSON_SET(Data, '$.Name', {0}) WHERE Id = {1}",
    "New Name", userId);
```

## Status and Tracking

| Item | Details |
|------|---------|
| **EF Core Bug** | [dotnet/efcore#37411](https://github.com/dotnet/efcore/issues/37411) 🔗 |
| **Status** | Open - Waiting for EF Core fix |
| **Affects** | EF Core 10.0+ (all database providers) |
| **First Reported** | December 2025 |
| **Workaround** | Available (see above) ✅ |

**Check issue status**: Click the link above to see the latest updates from the EF Core team.

## Future Plans

Pomelo may implement MySQL's `JSON_SET()` function for partial JSON updates (similar to Npgsql's `jsonb_set()` approach) to work around this EF Core limitation.

## Related Issues

- ASP.NET Core Identity PassKeys are affected when renaming passkeys
- Any scenario involving updates to properties within JSON-mapped owned entities
