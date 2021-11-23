@echo off
echo Batch material uasset modification from directory passed in first argument.
echo material replace [uasset] [search-pattern] [replace-pattern]
echo   Replace existing texture name to new by pattern.
echo   uasset: Path to material asset.
echo   search-pattern: Regular expression to search string in path.
echo     Example: (.*)(T_)(.+?)(_all)?(_.+) - search matches in string and set it in group by number.
echo   replace-pattern: Replacement pattern. Can include special replacement {pack} - uasset name.
echo     Example: /Game/Weapons/{pack}/T_{pack}$5 - place all textures into /Game/Weapons/[uasset name]/T_[uasset name][texture type], $5 - group with type from [search-pattern]
echo on

for /f %%f in ('"dir /s /b %1 | findstr "MI_.*\.uasset""') do ..\GtaModdingCli.exe "material" "replace" "%%f" "(.*)(T_)(.+?)(_all)?(_.+)" "/Game/Weapons/{pack}/T_{pack}$5"
pause