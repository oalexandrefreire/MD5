#!/usr/bin/env bash

set -euo pipefail

if [[ $# -ne 1 ]]; then
    echo "Uso: $0 <versao-semver>"
    echo "Exemplo: $0 2.4.0"
    exit 1
fi

version="$1"

if [[ ! "$version" =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
    echo "A versao deve usar o formato MAJOR.MINOR.PATCH (por exemplo, 2.4.0)." >&2
    exit 1
fi

root_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
solution_file="$root_dir/MD5CSharp/MD5.sln"
project_file="$root_dir/MD5CSharp/MD5.csproj"
package_output="$root_dir/artifacts/nuget"

cd "$root_dir"

if [[ -n "$(git status --porcelain)" ]]; then
    echo "A arvore de trabalho precisa estar limpa antes de criar um release." >&2
    exit 1
fi

if git rev-parse -q --verify "refs/tags/$version" >/dev/null; then
    echo "A tag $version ja existe." >&2
    exit 1
fi

current_version="$(sed -n 's:.*<Version>\([^<]*\)</Version>.*:\1:p' "$project_file" | head -n 1)"

if [[ -z "$current_version" ]]; then
    echo "Nao foi possivel localizar a versao no arquivo $project_file." >&2
    exit 1
fi

if [[ "$current_version" != "$version" ]]; then
    RELEASE_VERSION="$version" perl -0pi -e 's{<(Version|AssemblyVersion|FileVersion)>[^<]+</\1>}{"<$1>$ENV{RELEASE_VERSION}</$1>"}ge' "$project_file"
fi

dotnet restore "$solution_file" --disable-parallel
dotnet build "$solution_file" --configuration Release --no-restore --disable-build-servers
dotnet test "$solution_file" --configuration Release --no-build --no-restore --disable-build-servers
dotnet pack "$project_file" --configuration Release --no-build --no-restore --disable-build-servers --output "$package_output"

if [[ "$current_version" != "$version" ]]; then
    git add -- "$project_file"
    git commit -m "release $version"
fi

git tag -a "$version" -m "Release $version"

echo
echo "Release $version preparado."
echo "Pacote: $package_output/MD5.$version.nupkg"
echo "Para publicar: dotnet nuget push $package_output/MD5.$version.nupkg --api-key \"<NUGET_API_KEY>\" --source https://api.nuget.org/v3/index.json"
echo "Para enviar o commit e a tag: git push origin HEAD && git push origin $version"
