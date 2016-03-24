
$buildNumber = $env:APPVEYOR_BUILD_VERSION
$buildFolder = '.'

if($env:APPVEYOR_BUILD_FOLDER) {
	$buildFolder = $env:APPVEYOR_BUILD_FOLDER
}

$projectFilePath = Join-Path $buildFolder "src\Halcyon.Mvc\project.json"

Write-Host "Bumping version for project file: $projectFilePath to $buildNumber"

$projectFileContent = Get-Content $projectFilePath

Write-Host $projectFileContent

$projectConfig = $projectFileContent | ConvertFrom-Json

$projectConfig.version = "$buildNumber-alpha"

$projectConfig | 
	ConvertTo-Json |
	Out-File $projectFilePath
