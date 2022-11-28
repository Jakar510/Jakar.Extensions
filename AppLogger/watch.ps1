clear

set DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER true

#dotnet clean
#dotnet restore ./Jakar.AppLogger.Portal/Jakar.AppLogger.Portal.csproj 

dotnet watch --project ./Jakar.AppLogger.Portal/Jakar.AppLogger.Portal.csproj 