#!/bin/bash

# Build and install WebKit on Linux/MacOS.

dotnet pack -c Release WebKit.csproj
dotnet tool uninstall WebKit --global > /dev/null
dotnet tool install --global --add-source ./bin/nupkg WebKit --version 1.0.0
