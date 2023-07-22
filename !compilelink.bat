@echo off
set rt11exe=C:\bin\rt11\rt11.exe

rem Define ESCchar to use in ANSI escape sequences
rem https://stackoverflow.com/questions/2048509/how-to-echo-with-different-colors-in-the-windows-command-line
for /F "delims=#" %%E in ('"prompt #$E# & for %%E in (1) do rem"') do set "ESCchar=%%E"

for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "YY=%dt:~2,2%" & set "YYYY=%dt:~0,4%" & set "MM=%dt:~4,2%" & set "DD=%dt:~6,2%"
set "DATESTAMP=%YYYY%-%MM%-%DD%"
for /f %%i in ('git rev-list HEAD --count') do (set REVISION=%%i)
echo REV.%REVISION% %DATESTAMP%

echo VERSTR:	.ASCII /REV.%REVISION% %DATESTAMP%/ > VERSIO.MAC

@if exist TILES.OBJ del TILES.OBJ
@if exist MINER.LST del MINER.LST
@if exist MINER.OBJ del MINER.OBJ

%rt11exe% MACRO/LIST:DK: MINER.MAC

for /f "delims=" %%a in ('findstr /B "Errors detected" MINER.LST') do set "errdet=%%a"
if "%errdet%"=="Errors detected:  0" (
  echo COMPILED SUCCESSFULLY
) ELSE (
  findstr /RC:"^[ABDEILMNOPQRTUZ] " MINER.LST
  echo ======= %errdet% =======
  exit /b
)

@if exist MINER.MAP del MINER.MAP
@if exist MINER.SAV del MINER.SAV

%rt11exe% LINK MINER /MAP:MINER.MAP

for /f "delims=" %%a in ('findstr /B "Undefined globals" MINER.MAP') do set "undefg=%%a"
if "%undefg%"=="" (
  type MINER.MAP
  echo.
  echo %ESCchar%[92mLINKED SUCCESSFULLY%ESCchar%[0m
) ELSE (
  echo %ESCchar%[91m======= LINK FAILED =======%ESCchar%[0m
  exit /b
)
