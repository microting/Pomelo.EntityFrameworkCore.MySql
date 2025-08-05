﻿## About

_Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft_ adds JSON support for `Newtonsoft.Json` (the Newtonsoft JSON/Json.NET stack) to [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql).


## DISCLAIMER!!!
This is a fast moving version of [Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft) intented to follow .net release cycle closely!
There are no intentions for this package to implement new features. Feature requests needs to be directed at the originating [Github repository](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)

## How to Use

```csharp
optionsBuilder.UseMySql(
    connectionString,
    serverVersion,
    options => options.UseNewtonsoftJson())
```

## Related Packages

* [Microting.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql)
* [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)

## License

_Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft_ is released as open source under the [MIT license](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/blob/main/LICENSE).

## Feedback

Checkout the originating [Github repository](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql) before filing any new issues.

Bug reports and contributions are welcome at our [GitHub repository](https://github.com/microting/Pomelo.EntityFrameworkCore.MySql).