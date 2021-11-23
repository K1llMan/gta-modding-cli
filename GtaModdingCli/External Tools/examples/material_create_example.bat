@echo off
echo material create [material-info-json] [out-path]
echo   Creates material with settings from json.
echo   material-info-json: Path to json material definition.
echo   out-path: Path to output directory.
echo on

..\GtaModdingCli.exe "material" "create" ".\MaterialData.json" "C:\\