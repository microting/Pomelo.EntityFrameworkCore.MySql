# Benchmark Implementation Summary

This document provides an overview of the benchmark implementation for the Pomelo.EntityFrameworkCore.MySql project.

## What Was Implemented

### 1. Database Versions Documentation
**File**: `docs/DatabaseVersions.md`

A comprehensive table documenting all MySQL and MariaDB versions tested in the CI/CD pipeline, including:
- 8 MySQL versions (8.0.40 to 9.5.0)
- 12 MariaDB versions (10.5.27 to 12.1.2)
- Platform availability (Ubuntu/Windows)
- SQL modes used for each database type
- **Benchmark performance columns** with `TIME_PLACEHOLDER` markers that can be replaced with actual timing data after benchmarks complete:
  - Insert (ms) - Average insert operation times
  - Update (ms) - Average update operation times
  - Query (ms) - Average query operation times

### 2. Benchmark Project
**Location**: `benchmark/EFCore.MySql.Benchmarks/`

A complete .NET 10.0 console application using BenchmarkDotNet with:

#### Project Structure
- `EFCore.MySql.Benchmarks.csproj` - Project file with dependencies
- `Models.cs` - Entity models for benchmarking (SimpleEntity, ComplexEntity, RelatedEntity)
- `BenchmarkConfig.cs` - Configuration and base classes for benchmarks
- `InsertBenchmarks.cs` - 6 insert operation benchmarks
- `UpdateBenchmarks.cs` - 5 update operation benchmarks
- `QueryBenchmarks.cs` - 12 query/retrieval operation benchmarks
- `Program.cs` - Main entry point with CLI interface
- `README.md` - Comprehensive usage documentation

#### Benchmark Categories

**Insert Benchmarks** (6 scenarios):
1. Single simple entity insert
2. Batch 10 simple entities
3. Batch 100 simple entities
4. Single complex entity with many columns
5. Complex entity with related entities (foreign keys)
6. Batch 10 complex entities

**Update Benchmarks** (5 scenarios):
1. Single entity by primary key
2. Batch 10 entities
3. Single complex entity with many columns
4. Entity found by query
5. Batch 10 complex entities with filter

**Query Benchmarks** (12 scenarios):
1. Find single by ID
2. First with filter
3. Top 10 rows
4. Filter and order
5. Complex entity with filter
6. Complex entity with join (Include)
7. GROUP BY with aggregations
8. Multiple filter conditions
9. COUNT query
10. ANY/EXISTS query
11. Complex aggregation (SUM, AVG, MAX)
12. Query with navigation property filter

### 3. GitHub Actions Integration
**File**: `.github/workflows/pr-build.yml`

Added benchmark step to the PR build workflow:
- Runs after all tests pass
- Executes for each database version in the matrix
- **Enabled by default** (can be disabled by setting `enableBenchmarks: false`)
- Uploads results as artifacts with 30-day retention
- Results saved per database version and OS combination

### 4. Cross-Platform Support

The benchmark project works on both:
- **Linux**: Tested and verified
- **Windows**: Full compatibility with PowerShell and Windows paths

### 5. Configuration

Benchmarks are configured via environment variables:
- `BENCHMARK_DB_HOST` - Database server (default: localhost)
- `BENCHMARK_DB_PORT` - Port (default: 3306)
- `BENCHMARK_DB_USER` - Username (default: root)
- `BENCHMARK_DB_PASSWORD` - Password (default: Password12!)
- `BENCHMARK_DB_NAME` - Database name (default: pomelo_benchmark)

## How to Use

### Running Locally

1. **Start a database server**:
   ```bash
   docker run --name mysql_benchmark -e MYSQL_ROOT_PASSWORD=Password12! -p 127.0.0.1:3306:3306 -d mysql:8.0
   ```

2. **Navigate to benchmark directory**:
   ```bash
   cd benchmark/EFCore.MySql.Benchmarks
   ```

3. **Run specific benchmarks**:
   ```bash
   # Insert benchmarks
   dotnet run -c Release -- insert
   
   # Update benchmarks
   dotnet run -c Release -- update
   
   # Query benchmarks
   dotnet run -c Release -- query
   
   # All benchmarks
   dotnet run -c Release -- all
   ```

4. **View results**:
   Results are saved to `BenchmarkDotNet.Artifacts/` directory with detailed reports.

### Running in CI

Benchmarks are **enabled by default** in GitHub Actions for PR builds. They will:
1. Run after all tests pass
2. Execute for each database version in the matrix
3. Upload results as downloadable artifacts

To disable benchmarks in CI, set `enableBenchmarks: false` in the workflow environment variables.

### Customizing Database Connection

**Linux/macOS**:
```bash
export BENCHMARK_DB_HOST=127.0.0.1
export BENCHMARK_DB_PORT=3307
export BENCHMARK_DB_PASSWORD=MyPassword
dotnet run -c Release -- all
```

**Windows (PowerShell)**:
```powershell
$env:BENCHMARK_DB_HOST="127.0.0.1"
$env:BENCHMARK_DB_PORT="3307"
$env:BENCHMARK_DB_PASSWORD="MyPassword"
dotnet run -c Release -- all
```

## Performance Characteristics

The benchmarks are designed to:
- **Detect regressions**: Compare results across versions to identify performance issues
- **Realistic scenarios**: Use patterns common in real applications
- **Various complexities**: From simple CRUD to complex joins and aggregations
- **Memory profiling**: Track GC allocations and memory usage

### Benchmark Execution Time

- **Insert benchmarks**: ~2-5 minutes
- **Update benchmarks**: ~3-6 minutes (includes seeding)
- **Query benchmarks**: ~5-10 minutes (includes large dataset seeding)
- **Total for all benchmarks**: ~15-25 minutes per database version

## Technical Details

### Dependencies
- **BenchmarkDotNet 0.14.0**: Industry-standard .NET benchmarking library
- **Entity Framework Core 10.0**: Latest EF Core version
- **MySqlConnector 2.5.0**: MySQL ADO.NET driver
- **Pomelo.EntityFrameworkCore.MySql**: The provider being benchmarked

### Benchmark Configuration
- **Warmup iterations**: 1 (for faster CI execution)
- **Measurement iterations**: 5 (balance between accuracy and speed)
- **Memory diagnostics**: Enabled for all benchmarks
- **Job toolchain**: In-process emit for reliability

### Data Models
- **SimpleEntity**: Lightweight entity for basic CRUD tests
- **ComplexEntity**: Realistic entity with 10 columns of various types
- **RelatedEntity**: Child entity for testing relationships and joins

## Files Changed

1. `.github/workflows/pr-build.yml` - Added benchmark steps with enableBenchmarks flag
2. `Directory.Packages.props` - Added BenchmarkDotNet package version
3. `Pomelo.EFCore.MySql.sln` - Added benchmark project to solution
4. `docs/DatabaseVersions.md` - New documentation with benchmark time placeholders
5. `benchmark/` - New directory with entire benchmark project (9 files)
6. `scripts/update-benchmark-times.ps1` - Helper script for updating benchmark times

## Updating Benchmark Times

After benchmarks run in CI, the `TIME_PLACEHOLDER` values in `docs/DatabaseVersions.md` can be updated with actual performance data:

### Manual Update Process

1. **Download Artifacts**: After CI completes, download the benchmark artifacts from GitHub Actions
2. **Review Reports**: Open `BenchmarkDotNet.Artifacts/results/*.csv` or `*.html` files
3. **Extract Times**: Calculate average "Mean" execution times for each benchmark category
4. **Update Document**: Replace `TIME_PLACEHOLDER` with actual millisecond values in DatabaseVersions.md

### Automated Update (Future Enhancement)

The `scripts/update-benchmark-times.ps1` script provides a framework for automated updates:

```powershell
# After downloading artifacts
./scripts/update-benchmark-times.ps1 -ArtifactsPath ./benchmark-artifacts
```

This script is currently a placeholder and needs full implementation to:
- Parse BenchmarkDotNet CSV/JSON output files
- Calculate aggregate statistics per benchmark category
- Update the markdown table with actual values

## Future Enhancements

Potential improvements for future versions:
1. Add more specialized benchmarks (e.g., stored procedures, bulk operations)
2. Implement performance regression detection and alerts
3. Add comparison reports between database versions
4. Create dashboard for historical performance trends
5. Add benchmarks for JSON columns and spatial data (NTS)

## Troubleshooting

Common issues and solutions are documented in:
- `benchmark/EFCore.MySql.Benchmarks/README.md` - Comprehensive troubleshooting guide

Key points:
- Always run in Release mode (`-c Release`)
- Ensure database has sufficient resources
- The benchmark database will be **deleted and recreated** during setup
- First run may be slower due to cold caches
