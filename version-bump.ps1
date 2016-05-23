$ErrorActionPreference = "Stop"

$version = $env:APPVEYOR_BUILD_VERSION

if ($env:APPVEYOR_REPO_BRANCH -eq 'develop') {
	$version = "$version-beta"
} elseif ($env:APPVEYOR_REPO_BRANCH -ne 'master') {
	$version = "$version-pr"
}

if ($env:APPVEYOR_REPO_TAG_NAME) {
	$version = $env:APPVEYOR_REPO_TAG_NAME
}

$buildFolder = '.'

if($env:APPVEYOR_BUILD_FOLDER) {
	$buildFolder = $env:APPVEYOR_BUILD_FOLDER
}

$projectFilePaths = Get-Item (Join-Path $buildFolder "src\Halcyon*\project.json")

Write-Host "Bumping version to $version"

$projectFilePaths | 
  Foreach {
	(gc -Path $_) `
	  -replace "(?<=`"version`":\s`")[.\w-]*(?=`",)", "$version" |
	  sc -Path $_ -Encoding UTF8
  }