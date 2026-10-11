#!/bin/sh
set -eu
project_dir=$(CDPATH= cd -- "$(dirname -- "$0")/.." && pwd)
cd "$project_dir"
architecture=$(uname -m)
case "$architecture" in
  arm64) runtime=maccatalyst-arm64 ;;
  x86_64) runtime=maccatalyst-x64 ;;
  *) echo "Unsupported Mac architecture: $architecture" >&2; exit 1 ;;
esac
dotnet build backendKiosk/backendKiosk/backendKiosk.csproj -f net9.0-maccatalyst -r "$runtime" -p:TargetFrameworks=net9.0-maccatalyst
exec "backendKiosk/backendKiosk/bin/Debug/net9.0-maccatalyst/$runtime/backendKiosk.app/Contents/MacOS/backendKiosk"
