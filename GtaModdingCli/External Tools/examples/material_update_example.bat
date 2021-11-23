@echo off
echo  material update [uasset] [material-name] [new-material-path]
echo    Creates material with settings from json.
echo    uasset: Path to material asset.
echo    material-name: Old material name to replace.
echo    new-material-path: New material path inside pak.
echo on

::..\GtaModdingCli.exe "material" "update" "C:\\SM_ak47.uasset" "MI_ak47" "/Game/Weapons/MI_my_ak_47"
pause