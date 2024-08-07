﻿
![Nuget](https://img.shields.io/nuget/dt/md5)
![Nuget](https://img.shields.io/nuget/v/md5)
[![codecov](https://codecov.io/gh/oalexandrefreire/MD5/graph/badge.svg?token=9TLI2KMZHX)](https://codecov.io/gh/oalexandrefreire/MD5)

# MD5

[![NuGet](https://img.shields.io/nuget/v/MD5.svg)](https://www.nuget.org/packages/MD5/)

## Overview

MD5 is a .NET library that provides methods to generate MD5 hash from string, byteArray, object or stream content. It uses the System.Security.Cryptography.MD5 class to generate the hash.

## Usage

To use the library, you can call the `GetMD5` method on a string, byteArray, object or stream to get the MD5 hash.
Examples:

```csharp
string hash1 = "hello world".GetMD5();
// or specific encoding type
string hash2 = "hello world".GetMD5(EncodingType.UTF8);
```
```csharp
var stream = File.OpenRead("Rondonia.pdf");
string hash = stream.GetMD5();
```
```csharp
byte[] byteArray = Encoding.UTF8.GetBytes("Hello, World!");
string hash = byteArray.GetMD5();
```
```csharp
BrasilModel obj = new BrasilModel() { Id = 1, Details = "Maior país da América do Sul" };
string hash = obj.GetMD5();
```
To number you can use:
```csharp
string hash = myNumber.ToString().GetMD5():
```

## New Feature: MD5 with Salt

This new feature allows you to generate a salted MD5 hash, enhancing security.

### Usage

To use the salted hash functionality, you can call the `GetMD5WithSalt` method on a string, byteArray, or object to get the MD5 hash with a salt.
Examples:

```csharp
string salt = "randomSalt";
string hash1 = "hello world".GetMD5WithSalt(salt);
// or specific encoding type
string hash2 = "hello world".GetMD5WithSalt(salt, EncodingType.UTF8);
```
```csharp
byte[] byteArray = Encoding.UTF8.GetBytes("Hello, World!");
byte[] saltBytes = Encoding.UTF8.GetBytes("randomSalt");
string hash = byteArray.GetMD5WithSalt(saltBytes);
```
```csharp
BrasilModel obj = new BrasilModel() { Id = 1, Details = "Maior país da América do Sul" };
string salt = "randomSalt";
string hash = obj.GetMD5WithSalt(salt);
```
To number you can use:
```csharp
string salt = "randomSalt";
string hash = myNumber.ToString().GetMD5WithSalt(salt);
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
