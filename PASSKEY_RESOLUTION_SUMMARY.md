# PassKey Rename Issue - Complete Resolution Summary

## Issue Overview

**Original Problem**: When renaming a passkey (updating a property inside a JSON column), MySQL throws:
```
MySqlException: Invalid JSON text: "Invalid value." at position 0 in value for column 'AspNetUserPasskeys.Data'.
```

**Root Cause**: EF Core 10 bug where it sends only the modified property value instead of the full JSON object during updates

**Resolution**: Documented with tested workarounds

---

## Investigation Summary

### What We Found

1. **EF Core Bug Identified**: [dotnet/efcore#37411](https://github.com/dotnet/efcore/issues/37411)
   - Affects all database providers (MySQL, SQLite, PostgreSQL, etc.)
   - Wrong type mapping used during JSON owned entity updates
   - Only sends modified property value instead of complete JSON object

2. **Npgsql Comparison** (as requested)
   - PostgreSQL provider uses `jsonb_set()` function for partial updates
   - Allows updating specific JSON properties without re-sending entire object
   - MySQL has `JSON_SET()` but Pomelo doesn't currently use it
   - Future enhancement opportunity identified

3. **Working Workaround**
   - Use `AsNoTracking()` when querying + `Update()` when saving
   - Forces EF Core to treat entity as modified and serialize complete JSON
   - Tested and validated with MySQL 8.0.44

---

## Solution Delivered

### 1. Comprehensive Documentation

**Created**: `docs/known-issues/json-owned-entity-updates.md`
- Complete explanation of the issue
- Root cause analysis with technical details
- Three tested workarounds with code examples
- Prominent EF Core issue tracking link
- Status table for easy monitoring

**Updated**: `docs/PassKey-Support.md`
- Added "Known Issues" section
- Quick reference with workaround
- Tracking link to EF Core issue

**Created**: `docs/known-issues/README.md`
- Index of all known issues
- Quick reference with status indicators
- How to report new issues

### 2. Tested Workaround

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

### 3. Validation

- ✅ Created minimal reproduction test
- ✅ Confirmed issue with standard usage
- ✅ Validated workaround fixes the problem
- ✅ Verified data integrity in MySQL 8.0.44
- ✅ Updated reference application code

---

## Files Changed

```
docs/known-issues/json-owned-entity-updates.md  (new)    - Complete documentation
docs/known-issues/README.md                     (new)    - Known issues index  
docs/PassKey-Support.md                         (updated) - Added known issue section
src/EFCore.MySql/Update/Internal/MySqlModificationCommandBatch.cs - No changes (investigation reverted)
```

---

## Impact Assessment

### Before This PR
- Users encounter cryptic "Invalid JSON text" error
- No documentation or workaround available
- PassKey rename operations fail
- Users may think Pomelo doesn't support JSON owned entities

### After This PR
- ✅ Clear documentation of the issue
- ✅ Root cause explained (EF Core bug, not Pomelo)
- ✅ Tested workaround available
- ✅ Easy tracking link to upstream issue
- ✅ Users can continue using PassKeys successfully

---

## Recommendations

### Immediate (Done ✅)
- Document the issue and workaround
- Update PassKey documentation
- Provide clear tracking information

### Short-term (Next Steps)
- Monitor EF Core issue #37411 for resolution
- Consider adding automated tests when EF Core is fixed
- Update documentation when workaround is no longer needed

### Long-term (Future Enhancement)
- Consider implementing MySQL `JSON_SET()` for partial updates
- Similar to Npgsql's `jsonb_set()` approach
- Would work around EF Core limitation at provider level

---

## Testing Evidence

### Test Environment
- MySQL 8.0.44
- .NET 10.0.101
- EF Core 10.0.1
- Pomelo.EntityFrameworkCore.MySql (current version)

### Test Results
```
✓ Database created
✓ Created user
✓ Created passkey with name: 'Original Name'
✓ Found passkey with current name: 'Original Name'
✓ Updated in-memory name to: 'Renamed PassKey'
✓ SUCCESS: Passkey renamed successfully!
✓ VERIFIED: Passkey name is now 'Renamed PassKey'
=== TEST PASSED ===
```

---

## References

- **EF Core Bug**: https://github.com/dotnet/efcore/issues/37411
- **Npgsql Reference**: https://github.com/npgsql/efcore.pg
- **Test Application**: https://github.com/ArcherTrister/AspNetIdentityPasskeys
- **MySQL JSON Functions**: https://dev.mysql.com/doc/refman/8.0/en/json-modification-functions.html

---

## Conclusion

This PR successfully:
1. ✅ Identified the root cause (EF Core bug, not Pomelo)
2. ✅ Compared with other providers (Npgsql analysis completed)
3. ✅ Developed and tested working workaround
4. ✅ Created comprehensive documentation
5. ✅ Provided easy tracking for upstream fix
6. ✅ Enabled users to successfully use PassKeys

The issue is now fully documented with a tested workaround. Users can track the EF Core issue to know when the upstream fix is available and the workaround is no longer needed.
