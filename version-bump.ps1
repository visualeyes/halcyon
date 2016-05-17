$ErrorActionPreference = "Stop"

$buildNumber = $env:APPVEYOR_BUILD_VERSION
$buildFolder = '.'

if($env:APPVEYOR_BUILD_FOLDER) {
	$buildFolder = $env:APPVEYOR_BUILD_FOLDER
}

$projectFilePaths = Get-Item (Join-Path $buildFolder "src\Halcyon*\project.json")

Write-Host "Bumping version to $buildNumber"

$projectFilePaths | 
  Foreach {
	(gc -Path $_) `
	  -replace "(?<=`"version`":\s`")[.\w-]*(?=`",)", "$buildNumber-alpha" |
	  sc -Path $_ -Encoding UTF8
  }