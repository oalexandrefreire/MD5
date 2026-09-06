# Repository Guidelines

## Project Structure & Module Organization

- `MD5CSharp/` contains the `netstandard2.0` library, including the `MD5Hash.Hash` extension methods and `EncodingType` enum.
- `MD5CSharp.Test/MD5.Test/` contains the .NET 6 xUnit test project, fixtures, models, and the `Rondonia.pdf` stream-test asset.
- `MD5CSharp/MD5.sln` is the Visual Studio solution joining the library and tests.
- `docs/` contains the static project site (`index.html`, SCSS, JavaScript, and CSS); `README.md` contains package usage and installation examples.

## Build, Test, and Development Commands

Run these commands from the repository root:

```bash
dotnet restore MD5CSharp/MD5.sln
dotnet build MD5CSharp/MD5.sln
dotnet test MD5CSharp/MD5.sln
dotnet pack MD5CSharp/MD5.csproj -c Release
```

Restore resolves NuGet dependencies, build compiles both projects, test runs the xUnit suite, and pack creates the NuGet package. Use `dotnet test ... --collect:"XPlat Code Coverage"` when coverage data is needed.

## Coding Style & Naming Conventions

Use four-space indentation, braces on their own lines, and standard C# casing: PascalCase for types and public members, camelCase for locals and parameters, and descriptive method names. Keep public API changes consistent with the existing extension-method design and update `README.md` examples when behavior changes. No formatter or linter configuration is checked in; keep changes consistent with the surrounding code.

## Testing Guidelines

Tests use xUnit with `[Fact]` methods. Name tests after the behavior they verify, such as `MD5HashGetMD5WithSalt_ShouldReturnSameHashForSameInputAndSameSalt`. Add deterministic vectors for new hashing or encoding behavior, and include null, alternate encoding, byte-array, object, and stream cases where relevant. Run the full solution test command before submitting changes.

## Commit & Pull Request Guidelines

Existing history uses short, direct subjects (for example, `new release` and `MD5 with Salt`). Keep commits focused and write concise imperative subjects. Pull requests should explain the API or behavior change, include tests, note any package/version impact, and update documentation when public usage changes. Include screenshots only for `docs/` or other visual changes.

## Security & Configuration Notes

MD5 is provided for compatibility and checksums, not password storage or modern authentication. Do not add secrets to source control; keep local NuGet configuration in the ignored `MD5CSharp/nuget.config` file and use `nuget.config.example` as the template.
