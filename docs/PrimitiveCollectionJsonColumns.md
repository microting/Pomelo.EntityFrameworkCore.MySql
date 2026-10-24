# JSON columns for primitive collections (#329)

Primitive collections (`string[]`, `List<int>`, ...) are serialized to JSON and by default stored in `longtext` columns, so the database does not validate the content.

Opt in to the native `json` store type:

```csharp
options.UseMySql(connectionString, serverVersion, b => b.EnableJsonColumnsForPrimitiveCollections());
```

| Server | Result |
|---|---|
| MySQL 5.7.8+ | native `JSON` column, invalid documents are rejected |
| MariaDB 10.2.4+ | `JSON` (alias of `LONGTEXT`), MariaDB adds `CHECK (json_valid(col))` automatically |
| older | option ignored, `longtext` |

- Off by default: it changes the store type of existing columns, so the next migration contains an `ALTER COLUMN ... json`, which fails if a column holds invalid JSON.
- A property with an explicit `HasColumnType(...)` is never changed (`HasColumnType("json")` already works per property).
- Independent of `EnablePrimitiveCollectionsSupport()` (query translation).
