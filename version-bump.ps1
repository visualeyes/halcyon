
$buildNumber = $env:APPVEYOR_BUILD_VERSION

$projectFilePath = ".\src\Halcyon.Mvc\project.json"

$projectConfig = Get-Content $projectFilePath | 
					ConvertFrom-Json

$projectConfig.version = $buildNumber

$projectConfig | 
	ConvertTo-Json |
	Out-File $projectFilePath
