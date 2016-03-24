
$buildNumber = $env:APPVEYOR_BUILD_VERSION
$buildFolder = '.'

if($env:APPVEYOR_BUILD_FOLDER) {
	$buildFolder = $env:APPVEYOR_BUILD_FOLDER
}

$projectFilePath = Join-Path $buildFolder "src\Halcyon.Mvc\project.json"

Write-Host "Bumping version for project file: $projectFilePath to $buildNumber"

$projectConfig = Get-Content $projectFilePath | 
					ConvertFrom-Json

$projectConfig.version = $buildNumber

$projectConfig | 
	ConvertTo-Json |
	Out-File $projectFilePath
