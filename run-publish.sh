#!/bin/bash

# osx x64
echo "Publishing OSX x64..."
dotnet publish -c release --no-self-contained /p:PublishSingleFile=true -o published -r osx-x64 src/clik.csproj
zip -r -j published/clikit-darwin-x64.zip published/clik

# windows x64
echo "Publishing Windows x64..."
dotnet publish -c release --no-self-contained /p:PublishSingleFile=true -o published -r win10-x64 src/clik.csproj
zip -r -j published/clikit-windows-x64.zip published/clik.exe

# linux x64
echo "Publishing Linux x64..."
dotnet publish -c release --no-self-contained /p:PublishSingleFile=true -o published -r linux-x64 src/clik.csproj
zip -r -j published/clikit-linux-x64.zip published/clik
