@echo off
echo material empty [uasset] [out-path]
echo   Creates empty material from existing.
echo   uasset: Path to material asset.
echo   out-path: Path to output file.
echo on

..\GtaModdingCli.exe "material" "empty" "C:\\MI_ak47.uasset"
pause