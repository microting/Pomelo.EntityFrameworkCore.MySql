## About

_Pomelo.EntityFrameworkCore.MySql.NetTopologySuite_ adds spatial support for [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite) to [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql).


## DISCLAIMER!!!
This is a fast moving version of [Pomelo.EntityFrameworkCore.MySql.NetTopologySuite](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.NetTopologySuite) intented to follow .net release cycle closely!
There are no intentions for this package to implement new features. Feature requests needs to be directed at the originating [Github repository](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)

## How to Use

```csharp
optionsBuilder.UseMySql(
    connectionString,
    serverVersion,
    options => options.UseNetTopologySuite())
```

## Related Packages

* [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql)
* [NetTopologySuite](https://www.nuget.org/packages/NetTopologySuite)

## License

_Pomelo.EntityFrameworkCore.MySql.NetTopologySuite_ is released as open source under the [MIT license](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/blob/main/LICENSE).

## Feedback

Checkout the originating [Github repository](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql) before filing any new issues.
Bug reports and contributions are welcome at our [GitHub repository](https://github.com/microting/Pomelo.EntityFrameworkCore.MySql).