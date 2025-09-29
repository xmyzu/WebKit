# Build and install WebKit on Windows.

dotnet pack -c Release WebKit.csproj
dotnet tool uninstall WebKit --global > $NULL
dotnet tool install --global --add-source ./bin/nupkg WebKit --version 1.0.0
