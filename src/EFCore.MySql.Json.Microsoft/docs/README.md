﻿## About

_Pomelo.EntityFrameworkCore.MySql.Json.Microsoft_ adds JSON support for `System.Text.Json` (the Microsoft JSON stack) to [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql).


## DISCLAIMER!!!
This is a fast moving version of [Pomelo.EntityFrameworkCore.MySql.Json.Microsoft](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql.Json.Microsoft) intented to follow .net release cycle closely!
There are no intentions for this package to implement new features. Feature requests needs to be directed at the originating [Github repository](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)

## How to Use

```csharp
optionsBuilder.UseMySql(
    connectionString,
    serverVersion,
    options => options.UseMicrosoftJson())
```

## Related Packages

* [Microting.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql)
* [System.Text.Json](https://www.nuget.org/packages/System.Text.Json)

## License

_Pomelo.EntityFrameworkCore.MySql.Json.Microsoft_ is released as open source under the [MIT license](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/blob/main/LICENSE).

## Feedback

Checkout the originating [Github repository](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql) before filing any new issues.

Bug reports and contributions are welcome at our [GitHub repository](https://github.com/microting/Pomelo.EntityFrameworkCore.MySql).