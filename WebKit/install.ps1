# Build and install WebKit on Windows.

dotnet pack -c Release WebKit.csproj
dotnet tool uninstall Boson.WebKit --global > $NULL
dotnet tool install --global --add-source ./bin/nupkg Boson.WebKit --version 1.3.0
