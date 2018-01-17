@echo ------ Build started ------ 
@echo off

set COMPASS_VAR=%~dp0
set COMPASS_ROOT_PATH=%COMPASS_VAR:~0,-1%


rem ===========================================================Build
@echo Build NaverCompass.sln ...

rem "%VS140COMNTOOLS%/../IDE/devenv" OssGit.sln /Rebuild "Release|x86"
rem if ERRORLEVEL 1 goto ERROR_BUILD_32

Tools\NuGet.exe restore Src\NaverCompass.sln
Tools\MSBuild.exe Src\NaverCompass.sln /t:Rebuild  /p:Configuration=Release;ResGenExecuteAsTool=true;ResgenToolPath="%COMPASS_VAR%\Tools";AlToolPath="%COMPASS_VAR%\Tools"

if ERRORLEVEL 1 goto ERROR_BUILD_64

@echo Build Install Package
"C:\Program Files (x86)\NSIS\makensis.exe"   ".\Install\protoNow.nsi"

goto END

:ERROR_BUILD_32
echo ERROR: Build win32 project failed!
exit /b 1

:ERROR_BUILD_64
echo ERROR: Build x64 project failed!
exit /b 1

:END
echo ------ Build succeeded ------
