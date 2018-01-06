@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)
 
set version=1.0.0
if not "%PackageVersion%" == "" (
   set version=%PackageVersion%
)

set nuget=
if "%nuget%" == "" (
	set nuget=Build\nuget
)

REM echo Detect MSBuild 15.0 path
REM if exist "%programfiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe" (
REM     set msbuild="%programfiles(x86)%\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MSBuild.exe"
REM )
REM if exist "%programfiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe" (
REM     set msbuild="%programfiles(x86)%\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe"
REM )
REM if exist "%programfiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe" (
REM     set msbuild="%programfiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
REM )

echo Restore...
call %nuget% restore BuildSource\SSC.NetFx.Core\SSC.NetFx.Core.sln
call dotnet restore BuildSource\SSC.NetFx.Core\SSC.NetFx.Core.sln

echo Building...
REM call dotnet build BuildSource\SSC.NetFx.Core\SSC.NetFx.Core.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=diag /nr:false
call "%MsBuildExe%" BuildSource\SSC.NetFx.Core\SSC.NetFx.Core.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=diag /nr:false

mkdir Build
mkdir Build\SSC.NetFx.Core
mkdir Build\SSC.NetFx.Core\lib
mkdir Build\SSC.NetFx.Core\lib\net45
mkdir Build\SSC.NetFx.Core\lib\netcoreapp2.0

%nuget% pack "Build\SSC.NetFx.Core\SSC.NetFx.Core.nuspec" -NoPackageAnalysis -verbosity detailed -o Build -Version %version% -p Configuration="%config%"