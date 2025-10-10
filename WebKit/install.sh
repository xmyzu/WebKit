#!/bin/bash

# Build and install WebKit on Linux/MacOS.

dotnet pack -c Release WebKit.csproj
dotnet tool uninstall Boson.WebKit --global > /dev/null
dotnet tool install --global --add-source ./bin/nupkg Boson.WebKit --version 1.3.0
