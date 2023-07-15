@echo off

if exist x-ukncbtl\MINER.BIN del x-ukncbtl\MINER.BIN
C:\bin\Sav2Cart.exe MINER.SAV MINER.BIN
move MINER.BIN x-ukncbtl\MINER.BIN

del x-ukncbtl\sys1002.dsk
@if exist "x-ukncbtl\sys1002.dsk" (
  echo.
  echo ####### FAILED to delete old disk image file #######
  exit /b
)
copy D:\Work\MyProjects-old\svn-ukncbtl\lib\disks\sys1002.dsk .
C:\bin\rt11dsk a sys1002.dsk MINER.SAV
move sys1002.dsk x-ukncbtl\sys1002.dsk

@if not exist "x-ukncbtl\sys1002.dsk" (
  echo ####### ERROR disk image file not found #######
  exit /b
)

start x-ukncbtl\UKNCBTL.exe /boot
