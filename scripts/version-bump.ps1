$ErrorActionPreference = "Stop"

$version = $env:APPVEYOR_BUILD_VERSION

if ($env:APPVEYOR_REPO_BRANCH -eq 'develop') {
	$version = $version -replace "develop","beta"
}

if ($env:APPVEYOR_REPO_TAG_NAME) {
	$version = $env:APPVEYOR_REPO_TAG_NAME
}

$buildFolder = (Get-Item $PSScriptRoot).Parent.FullName

Write-Host "Working from build folder: $buildFolder"

if($env:APPVEYOR_BUILD_FOLDER) {
	$buildFolder = $env:APPVEYOR_BUILD_FOLDER
}

$projectFilePaths = Get-Item (Join-Path $buildFolder "src\Halcyon*\project.json")

Write-Host "Bumping version to $version"

$projectFilePaths |
  Foreach {
	Write-Host "Updating $_ to $version"

	(gc -Path $_) `
	  -replace "(?<=`"version`":\s`")[.\w\-\*]*(?=`",)", "$version" |
	  sc -Path $_ -Encoding UTF8
  }
