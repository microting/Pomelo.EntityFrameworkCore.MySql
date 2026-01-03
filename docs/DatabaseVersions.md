# Database Versions Tested

This document lists all MySQL and MariaDB versions tested in the CI/CD pipeline as defined in `.github/workflows/pr-build.yml`.

## MySQL Versions

| Version | Ubuntu | Windows | Notes |
|---------|--------|---------|-------|
| 9.5.0   | ✓      | ✗       | Windows package not available yet |
| 9.4.0   | ✓      | ✓       | |
| 9.3.0   | ✓      | ✓       | |
| 9.2.0   | ✓      | ✓       | |
| 9.1.0   | ✓      | ✓       | |
| 9.0.1   | ✓      | ✓       | |
| 8.4.3   | ✓      | ✓       | |
| 8.0.40  | ✓      | ✓       | |

## MariaDB Versions

| Version | Ubuntu | Windows | Notes |
|---------|--------|---------|-------|
| 12.1.2  | ✓      | ✗       | Windows package not available yet |
| 12.0.2  | ✓      | ✗       | Windows package not available yet |
| 11.8.5  | ✓      | ✗       | Windows package not available yet |
| 11.7.2  | ✓      | ✗       | Different patch versions on platforms |
| 11.7.1  | ✗      | ✓       | Different patch versions on platforms |
| 11.6.2  | ✓      | ✓       | |
| 11.5.2  | ✓      | ✓       | |
| 11.4.4  | ✓      | ✓       | |
| 11.3.2  | ✓      | ✓       | |
| 10.11.10| ✓      | ✓       | |
| 10.6.20 | ✓      | ✓       | |
| 10.5.27 | ✓      | ✓       | |

## Summary

- **MySQL**: 8 versions tested (7 on Windows, 8 on Ubuntu)
- **MariaDB**: 12 versions tested (7 on Windows, 12 on Ubuntu)
- **Total combinations**: 31 OS/version combinations in the test matrix

## SQL Modes

The following SQL modes are used for testing:

### MySQL (Current - v8.0+)
```
ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION
```

### MySQL (Legacy - v5.7 and below)
```
ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION
```

### MariaDB
```
STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION
```

Note: MariaDB currently does not use `ONLY_FULL_GROUP_BY` mode (see issue #1167).
