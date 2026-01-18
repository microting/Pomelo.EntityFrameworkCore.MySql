# Known Issues

This directory contains documentation for known issues and their workarounds.

## Current Issues

### JSON Owned Entity Updates (EF Core 10)

**Issue**: Updating properties within JSON-mapped owned entities fails with "Invalid JSON text" error

**Tracking**: [dotnet/efcore#37411](https://github.com/dotnet/efcore/issues/37411) 🔗

**Status**: ⏳ Open - Waiting for EF Core fix

**Documentation**: [json-owned-entity-updates.md](json-owned-entity-updates.md)

**Workaround**: ✅ Available

---

## How to Use This Directory

1. Check issue links to see if they've been resolved in newer EF Core versions
2. Read the detailed documentation for each issue
3. Apply the documented workarounds until upstream fixes are available
4. Monitor the tracking links for status updates

## Reporting New Issues

If you encounter a new issue:

1. Search existing issues at [github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues)
2. Check if it's an EF Core issue at [github.com/dotnet/efcore/issues](https://github.com/dotnet/efcore/issues)
3. Create a new issue with:
   - Clear reproduction steps
   - EF Core version
   - Database version (MySQL/MariaDB)
   - Expected vs actual behavior
