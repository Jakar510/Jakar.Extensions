@echo off

del *.nupkg

:; set -o errexit
:; function goto() { return $?; }


nuget.exe pack Jakar.Extensions.nuspec || goto :error

nuget.exe pack Jakar.DataBase.nuspec || goto :error

nuget.exe pack Jakar.Extensions.Blazor.nuspec || goto :error

:; exit 0
pause
exit /b 0

:error
pause;
exit /b %errorlevel%


