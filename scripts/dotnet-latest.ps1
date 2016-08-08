# Set up everything for using the dotnet cli. This should mean we do not have to wait for Appveyor images to be updated.

# Clean and recreate the folder in which all output packages should be placed
$ArtifactsPath = "artifacts"

if (Test-Path $ArtifactsPath) { 
	Remove-Item -Path $ArtifactsPath -Recurse -Force -ErrorAction Ignore 
}

New-Item $ArtifactsPath -ItemType Directory -ErrorAction Ignore | Out-Null

Write-Host "Created artifacts folder '$ArtifactsPath'"

# Install the latest dotnet cli   
if (Get-Command "dotnet.exe" -ErrorAction SilentlyContinue) {
    Write-Host "dotnet SDK already installed"
	dotnet --version 
} else {
    Write-Host "Installing dotnet SDK"
    
    $installScript = Join-Path $ArtifactsPath "dotnet-install.ps1"
    
	Write-Host $installScript

    Invoke-WebRequest "https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0/scripts/obtain/dotnet-install.ps1" `
       -OutFile $installScript
    
    & $installScript
}



<# Possible alternative
  - ps: .\scripts\show-dotnet-info.ps1
  # Download install script to install .NET cli in .dotnet dir
  - ps: mkdir -Force ".\scripts\obtain\" | Out-Null
  - ps: Invoke-WebRequest "https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0-preview2/scripts/obtain/dotnet-install.ps1" -OutFile ".\scripts\obtain\install.ps1"
  - ps: $env:DOTNET_INSTALL_DIR = "$pwd\.dotnetcli"
  - ps: '& .\scripts\obtain\install.ps1 -Channel "preview" -version "$env:CLI_VERSION" -InstallDir "$env:DOTNET_INSTALL_DIR" -NoPath'
  # add dotnet to PATH
  - ps: $env:Path = "$env:DOTNET_INSTALL_DIR;$env:Path"
#>