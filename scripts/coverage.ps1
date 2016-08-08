
$PROJECT_ROOT=(Split-Path $PSScriptRoot -Parent)

$PROJECTS_FILTER='Halcyon*'
$TEST_PROJECTS_FILTER='Halcyon.Tests'

$OPEN_COVER_VERSION='4.6.519'
$OPEN_COVER_PATH=(Join-Path $env:USERPROFILE ".nuget\packages\OpenCover\$OPEN_COVER_VERSION\tools\OpenCover.Console.exe")

$COVERALLS_VERSION='1.3.4'
$COVERALLS_PATH=(Join-Path $env:USERPROFILE ".nuget\packages\coveralls.io\$COVERALLS_VERSION\tools\coveralls.net.exe")

$COVERAGE_COMMAND=$OPEN_COVER_PATH +
	' -register:user' +
	' -target:"C:\Program Files\dotnet\dotnet.exe"' +
	" -filter:`"+[$PROJECTS_FILTER]* -[$TEST_PROJECTS_FILTER]*`""

$TARGET_ARGS="test $PROJECT_ROOT\test\Halcyon.Tests\"

if($env:APPVEYOR -eq $true) {
	Write-Host "Running on appveyor"
	$TARGET_ARGS+=' -appveyor'
}

$COVERAGE_COMMAND+=" -targetargs:`"$TARGET_ARGS`""

if($env:CI -eq $true) {
	Write-Host "Outputting ci format"
	$COVERAGE_COMMAND+=' -output:coverage.xml'
}

Write-Host "Executing $COVERAGE_COMMAND"
iex $COVERAGE_COMMAND

if($env:CI -eq $true) {
	Write-Host "Uploading to coveralls.io"
	& $COVERALLS_PATH --opencover --repo-token $env:COVERALLS_REPO_TOKEN --full-sources coverage.xml
}
