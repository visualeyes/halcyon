$ErrorActionPreference = "Stop"

$buildNumber = $env:APPVEYOR_BUILD_VERSION
$buildFolder = '.'

if($env:APPVEYOR_BUILD_FOLDER) {
	$buildFolder = $env:APPVEYOR_BUILD_FOLDER
}

$projectFilePath = Join-Path $buildFolder "src\Halcyon*\project.json"

Write-Host "Bumping version for project file: $projectFilePath to $buildNumber"

(gc -Path $projectFilePath) `
	-replace "(?<=`"version`":\s`")[.\w-]*(?=`",)", "$buildNumber-alpha" |
	sc -Path $projectFilePath -Encoding UTF8
