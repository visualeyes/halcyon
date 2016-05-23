$ErrorActionPreference = "Stop"

$buildNumber = $env:APPVEYOR_BUILD_VERSION

if ($env:APPVEYOR_REPO_BRANCH -eq 'develop') {
	$buildNumber = "$buildNumber-beta"
} elseif ($env:APPVEYOR_REPO_BRANCH -ne 'master') {
	$buildNumber = "$buildNumber-$env:APPVEYOR_REPO_BRANCH"
}

if ($env:APPVEYOR_REPO_TAG_NAME) {
	$buildNumber = $env:APPVEYOR_REPO_TAG_NAME
}

$buildFolder = '.'

if($env:APPVEYOR_BUILD_FOLDER) {
	$buildFolder = $env:APPVEYOR_BUILD_FOLDER
}

$projectFilePaths = Get-Item (Join-Path $buildFolder "src\Halcyon*\project.json")

Write-Host "Bumping version to $buildNumber"

$projectFilePaths | 
  Foreach {
	(gc -Path $_) `
	  -replace "(?<=`"version`":\s`")[.\w-]*(?=`",)", "$buildNumber" |
	  sc -Path $_ -Encoding UTF8
  }