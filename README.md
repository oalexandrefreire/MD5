![Nuget](https://img.shields.io/nuget/dt/md5)
![Nuget](https://img.shields.io/nuget/v/md5)

# MD5

[![NuGet](https://img.shields.io/nuget/v/MD5.svg)](https://www.nuget.org/packages/MD5/)

## Overview

MD5 is a .NET library that provides methods to generate MD5 hash from string content. It uses the System.Security.Cryptography.MD5 class to generate the hash.

## Usage

To use the library, you can call the `GetMD5` method on a string to get the MD5 hash:

```csharp
string hash = "hello world".GetMD5();
```
You can also call the Content method on the MD5Hash.Hash class to get the hash from a string:

```csharp
string hash = MD5Hash.Hash.Content("hello world");
```

## Installation
You can install the library via NuGet package manager by searching for MD5 or by executing the following command in the Package Manager Console:

```shell
PM> Install-Package MD5
```

## License
This library is licensed under the MIT License. See the LICENSE file for more details.

## Contributions
Contributions to this library are welcome. Please open an issue or submit a pull request with your changes.
