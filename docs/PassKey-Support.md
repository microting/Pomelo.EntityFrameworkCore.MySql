# ASP.NET Core Identity PassKey Support

## Overview

Pomelo.EntityFrameworkCore.MySql fully supports ASP.NET Core Identity PassKeys (WebAuthn credentials) introduced in .NET 10 / ASP.NET Core Identity Schema Version 3.

## Background

There was a concern raised in [dotnet/aspnetcore#64939](https://github.com/dotnet/aspnetcore/issues/64939) that MySQL providers might not support PassKeys because the `IdentityPasskeyData` class contains properties with non-primitive types (specifically `string[]` for the `Transports` property) that need to be stored as JSON using EF Core's `.ToJson()` method.

## Verification Results

✅ **Pomelo.EntityFrameworkCore.MySql works perfectly with PassKeys!**

We've created and tested a complete sample application (`samples/PassKeyTest`) that demonstrates:

1. ✅ Creating the `AspNetUserPasskeys` table with proper schema
2. ✅ Storing PassKey data with complex types including `string[]` arrays
3. ✅ Reading back PassKey records with full data integrity
4. ✅ JSON serialization/deserialization of all properties
5. ✅ Query support using MariaDB/MySQL JSON functions

## How It Works

Pomelo uses `MySqlStructuralJsonTypeMapping` to handle JSON columns created by `.ToJson()`:

- **Storage**: JSON data is stored as `longtext` (MariaDB) or `json` (MySQL) column type
- **Serialization**: Automatic conversion between .NET objects and JSON strings
- **Type Support**: Full support for complex types including arrays (`string[]`, `byte[]`, etc.)
- **Performance**: Efficient read/write operations with proper type mapping

## Usage Example

### 1. Configure DbContext

```csharp
public class ApplicationDbContext : IdentityDbContext<
    IdentityUser, 
    IdentityRole, 
    string,
    IdentityUserClaim<string>,
    IdentityUserRole<string>,
    IdentityUserLogin<string>,
    IdentityRoleClaim<string>,
    IdentityUserToken<string>,
    IdentityUserPasskey<string>>  // Add this!
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure PassKey entity with JSON storage
        builder.Entity<IdentityUserPasskey<string>>(b =>
        {
            b.HasKey(p => p.CredentialId);
            b.ToTable("AspNetUserPasskeys");
            b.Property(p => p.CredentialId).HasMaxLength(1024);
            b.OwnsOne(p => p.Data).ToJson();  // This enables JSON storage!
        });
    }
}
```

### 2. Configure Services

```csharp
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, serverVersion)
);

services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
```

### 3. Use PassKeys

Once configured, you can use PassKeys with ASP.NET Core Identity's standard APIs:

```csharp
// Register a new passkey
var passkey = new IdentityUserPasskey<string>
{
    UserId = user.Id,
    CredentialId = credentialIdBytes,
    Data = new IdentityPasskeyData
    {
        PublicKey = publicKeyBytes,
        Name = "My Security Key",
        Transports = new[] { "usb", "nfc", "ble" },  // string[] works!
        CreatedAt = DateTimeOffset.UtcNow,
        // ... other properties
    }
};

context.Set<IdentityUserPasskey<string>>().Add(passkey);
await context.SaveChangesAsync();

// Query passkeys
var userPasskeys = await context.Set<IdentityUserPasskey<string>>()
    .Where(p => p.UserId == userId)
    .ToListAsync();
```

## Tested With

- ✅ .NET 10.0.101
- ✅ Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.1
- ✅ Pomelo.EntityFrameworkCore.MySql (current version)
- ✅ MySqlConnector 2.5.0
- ✅ MariaDB 11.6.2
- ✅ MySQL 8.0+

## Known Issues

### ⚠️ Updating JSON Owned Entities (EF Core 10 Bug)

> **Upstream Issue**: [dotnet/efcore#37411](https://github.com/dotnet/efcore/issues/37411) - Track this for EF Core fix status

When updating properties within a JSON-mapped owned entity (like renaming a passkey), you may encounter:

```
MySqlException: Invalid JSON text: "Invalid value." at position 0 in value for column 'AspNetUserPasskeys.Data'.
```

**This is an EF Core 10 bug** that affects multiple database providers (not just Pomelo).

**Workaround**: Use `AsNoTracking()` + `Update()` pattern:

```csharp
// ❌ This fails with EF Core 10:
var passkey = await context.UserPasskeys.FirstAsync(p => p.CredentialId == id);
passkey.Data.Name = "New Name";
await context.SaveChangesAsync();  // Error!

// ✅ Use this workaround instead:
var passkey = await context.UserPasskeys
    .AsNoTracking()  // Query without tracking
    .FirstAsync(p => p.CredentialId == id);
passkey.Data.Name = "New Name";
context.UserPasskeys.Update(passkey);  // Update detached entity
await context.SaveChangesAsync();  // Works!
```

For more details and alternative workarounds, see: [JSON Owned Entity Updates](known-issues/json-owned-entity-updates.md)

## Complete Sample

See `samples/PassKeyTest` for a complete, runnable example that demonstrates:
- Database creation
- PassKey insertion with complex data
- Data retrieval and verification
- JSON structure validation

## Important Notes

1. **Column Type**: The `Data` column is created as:
   - `longtext` on MariaDB (which supports JSON validation)
   - `json` on MySQL 8.0+

2. **Array Support**: Non-primitive types like `string[]` are fully supported and correctly serialized as JSON arrays

3. **JSON Validation**: Both MariaDB and MySQL validate the JSON structure, ensuring data integrity

4. **Query Support**: You can query JSON fields using database-specific JSON functions:
   ```sql
   SELECT JSON_EXTRACT(Data, '$.Transports') FROM AspNetUserPasskeys;
   ```

## Migration from Other Providers

If you're migrating from a different MySQL provider (like MySQL.EntityFrameworkCore) that doesn't support PassKeys:

1. Update your package reference to Pomelo.EntityFrameworkCore.MySql
2. Configure the DbContext as shown above
3. Create and apply migrations
4. Your PassKey data will work immediately!

## References

- [ASP.NET Core Identity PassKey Issue](https://github.com/dotnet/aspnetcore/issues/64939)
- [IdentityPasskeyData Source](https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Extensions.Stores/src/IdentityPasskeyData.cs)
- [IdentityUserPasskey Source](https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Extensions.Stores/src/IdentityUserPasskey.cs)
- [WebAuthn Specification](https://www.w3.org/TR/webauthn-3/)
