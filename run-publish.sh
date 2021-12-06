#!/bin/bash

# osx x64
echo "Publishing OSX x64..."
dotnet publish -c release --no-self-contained /p:PublishSingleFile=true -o published -r osx-x64 src/gitpak.csproj
zip -r -j published/gitpak-darwin-x64.zip published/gitpak

# windows x64
echo "Publishing Windows x64..."
dotnet publish -c release --no-self-contained /p:PublishSingleFile=true -o published -r win10-x64 src/gitpak.csproj
zip -r -j published/gitpak-windows-x64.zip published/gitpak.exe

# linux x64
echo "Publishing Linux x64..."
dotnet publish -c release --no-self-contained /p:PublishSingleFile=true -o published -r linux-x64 src/gitpak.csproj
zip -r -j published/gitpak-linux-x64.zip published/gitpak
