@echo off

set mypath=%~dp0
@echo %mypath%

echo Comparing two files: %mypath%App.Dev.config with %mypath%App.config

if not exist %mypath%App.Dev.config goto File1NotFound
if not exist %mypath%App.config goto File2NotFound

fc %mypath%App.Dev.config App.config
if %ERRORLEVEL%==0 GOTO NoCopy

echo Files are not the same.  Copying %mypath%App.Dev.config over %mypath%App.config
copy %mypath%App.Dev.config %mypath%App.config /y
copy %mypath%App.Dev.config %mypath%bin\debug\ApiProject.exe.config /y & goto END

:NoCopy
echo Files are the same.  Did nothing
goto END

:File1NotFound
echo %mypath%App.Dev.config not found.
goto END

:File2NotFound
copy %mypath%App.Dev.config App.config /y
goto END

:END
echo Done.

@echo off
rem Lapsed Activities
SET var1= 2018
SET var2= 2019
start %mypath%bin\debug\ApiProject.exe %var1% %var2%