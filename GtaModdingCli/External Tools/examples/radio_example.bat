@echo off
echo radio [asset] [tracks] [pak-path] [mod-name]
echo   Build radio uasset with tracks from directory.
echo   asset: Uasset file name.
echo   tracks: Path to tracks assets directory.
echo   pak-path: Path inside pak.
echo   mod-name: Mod file name.
echo on

..\GtaModdingCli.exe radio "c:\5\RADIO_FLASH.uasset" "c:\5\tracks" "/Game/GTA3/Audio/Streams/RadioStreams/FlashFM" "500-my_radio_P.pak"
pause