for /f %%f in ('"dir /s /b %1 | findstr "MI_.*\.uasset""') do ..\GtaModdingCli.exe "material" "%%f" "(.*)(T_)(.+?)(_all)?(_.+)" "/Game/Weapons/{pack}/T_{pack}$5"
pause