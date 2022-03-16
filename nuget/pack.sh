#!/bin/bash 2> nul

:; set -o errexit
:; function goto() { return $?; }


nuget.exe pack Jakar.Extensions.Xamarin.Forms.nuspec || goto :error


nuget.exe pack Jakar.Extensions.nuspec || goto :error


nuget.exe pack Jakar.Xml.nuspec  || goto :error


:; exit 0
exit /b 0

:error
exit /b %errorlevel%


