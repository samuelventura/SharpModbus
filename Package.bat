TITLE NuGet Packager
@ECHO ON

CD %~dp0
CD ..
nuget pack SharpModbus\SharpModbus\package.nuspec
PAUSE

