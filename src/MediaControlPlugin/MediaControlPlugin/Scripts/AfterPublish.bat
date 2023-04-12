call C:\"Program Files"\"Microsoft Visual Studio"\2022\Community\VC\Auxiliary\Build\vcvarsall.bat %5%
set shimFolder=%1..\Plugin.Shim
powershell -file Scripts/UpdateCMakePreset.ps1 "%shimFolder%" %2 %3 %4
cd %shimFolder%
cmake . --preset %5%-release
cmake --build --preset %5%
cmake --install %1/../../obj/Plugin.Shim/%5%-release