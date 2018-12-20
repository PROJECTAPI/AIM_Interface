@echo off

set mypath=%~dp0
@echo %mypath%

echo Comparing two files: %mypath%App.SB.config with %mypath%App.config

if not exist %mypath%App.SB.config goto File1NotFound
if not exist %mypath%App.config goto File2NotFound

fc %mypath%App.SB.config App.config
if %ERRORLEVEL%==0 GOTO NoCopy

echo Files are not the same.  Copying %mypath%App.SB.config over %mypath%App.config
copy %mypath%App.SB.config %mypath%App.config /y
copy %mypath%App.SB.config %mypath%bin\debug\ApiProject.exe.config /y & goto END

:NoCopy
echo Files are the same.  Did nothing
goto END

:File1NotFound
echo %mypath%App.SB.config not found.
goto END

:File2NotFound
copy %mypath%App.SB.config App.config /y
goto END

:END
echo Done.

@echo off
rem Lapsed Activities
SET var1= 2018
SET var2= 2019
start %mypath%bin\debug\ApiProject.exe %var1% %var2%