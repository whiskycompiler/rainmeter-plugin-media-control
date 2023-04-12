$ErrorActionPreference = "Stop"
$jsonPath = "$($args[0])\\CMakePresets.json";
$pluginName = $args[1];
$pluginVersion = $args[2];
$copyright = $args[3];
$json = Get-Content $jsonPath | ConvertFrom-Json;

$windowsBasePreset = ($json.configurePresets | Where-Object -FilterScript { $_.name -eq "windows-base" })[0];
$windowsBasePreset.cacheVariables | Add-Member -NotePropertyName PLUGIN_NAME -NotePropertyValue $pluginName -Force;
$windowsBasePreset.cacheVariables | Add-Member -NotePropertyName PLUGIN_VERSION -NotePropertyValue $pluginVersion -Force;
$windowsBasePreset.cacheVariables | Add-Member -NotePropertyName COPYRIGHT -NotePropertyValue $copyright -Force;

Set-Content -Path $jsonPath -Value ($json | ConvertTo-Json -Depth 10);