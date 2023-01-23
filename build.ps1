function RemoveFolder {
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true)]
        [System.String]
        $Path
    )

    if (Test-Path $Path) {
        Remove-Item $Path -Recurse
    }
}
function Invoke-Dotnet {
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true)]
        [System.String]
        $Command,

        [Parameter(Mandatory = $true)]
        [System.String]
        $Arguments
    )

    $DotnetArgs = @()
    $DotnetArgs = $DotnetArgs + $Command
    $DotnetArgs = $DotnetArgs + ($Arguments -split '\s+')

    [void]($Output = & dotnet $DotnetArgs)

    # Should throw if the last command failed.
    if ($LASTEXITCODE -ne 0) {
        Write-Warning -Message ($Output -join "; ")
        throw "There was an issue running the specified dotnet command."
    }
}
function  Invoke-Npm {
    param (
        [Parameter(Mandatory = $true)]
        [System.String]
        $Command,

        [Parameter(Mandatory = $false)]
        [System.String]
        $Arguments = ''
    )
    
    $NpmArgs = @()
    $NpmArgs = $NpmArgs + $Command
    if ($Arguments) {
        $NpmArgs = $NpmArgs + ($Arguments -split '\s+')
    }

    [void]($Output = & npm $NpmArgs)

    # Should throw if the last command failed.
    if ($LASTEXITCODE -ne 0) {
        Write-Warning -Message ($Output -join "; ")
        throw "There was an issue running the specified npm command."
    }
}
function DownloadAndRun {
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true)]
        [System.String]
        $Url,

        [Parameter(Mandatory = $true)]
        [System.String]
        $FileName,

        [Parameter(Mandatory = $true)]
        [System.String]
        $Arguments
    )

    $exePath = "$env:TEMP\$FileName"

    Write-Host "Downloading..."
    (New-Object Net.WebClient).DownloadFile($Url, $exePath)

    Write-Host "Installing..."
    [void]($Output = & $exePath $Arguments)

    # Should throw if the last command failed.
    if ($LASTEXITCODE -ne 0) {
        Write-Warning -Message ($Output -join "; ")
        throw "There was an issue running the specified installer."
    }
}
function ReloadEnvironment {
   foreach($level in "Machine","User") {
      [Environment]::GetEnvironmentVariables($level).GetEnumerator() | % {
         # For Path variables, append the new values, if they're not already in there
         if($_.Name -match 'Path$') { 
            $_.Value = ($((Get-Content "Env:$($_.Name)") + ";$($_.Value)") -split ';' | Select -unique) -join ';'
         }
         $_
      } | Set-Content -Path { "Env:$($_.Name)" }
   }
}

# Remove all build artifacts
RemoveFolder -Path '.\build'
RemoveFolder -Path '.\backend\bin'
RemoveFolder -Path '.\backend\obj'
RemoveFolder -Path '.\backend\build'
RemoveFolder -Path '.\frontend\build'
RemoveFolder -Path '.\frontend\src\Api\Generated'

# Build the backend
Write-Host "Building backend..." -ForegroundColor Cyan
Invoke-Dotnet -Command 'publish' -Arguments '.\backend\backend.csproj /p:PublishProfile=backend\Properties\PublishProfiles\FolderProfile.pubxml -c Release'
Write-Host "Backend built!" -ForegroundColor Green

# Install Rust
if (!(Test-Path "$env:USERPROFILE\.cargo\bin\rustup.exe")) {
    Write-Host "Installing Rust..." -ForegroundColor Cyan
    DownloadAndRun -Url 'https://win.rustup.rs/x86_64' -FileName 'rustup-init.exe' -Arguments '-y'
    Write-Host "Rust installed!" -ForegroundColor Green
}

# Install wasm-pack
$wasmPackPath = "$env:USERPROFILE\.cargo\bin\wasm-pack.exe"
if (!(Test-Path $wasmPackPath)) {
    Write-Host "Downloading wasm-pack..." -ForegroundColor Cyan
    (New-Object Net.WebClient).DownloadFile('https://github.com/rustwasm/wasm-pack/releases/download/v0.10.3/wasm-pack-init.exe', $wasmPackPath)
    Write-Host "wasm-pack installed!" -ForegroundColor Green
}

# Reload environment variables
ReloadEnvironment

# Build the frontend
Write-Host "Building frontend..." -ForegroundColor Cyan
Set-Location frontend
Write-Host "Building WASM..."
Invoke-Npm -Command 'run' -Arguments 'build:wasm'
Write-Host "Installing NPM packages..."
Invoke-Npm -Command 'install'
Write-Host "Generating API..."
Invoke-Npm -Command 'run' -Arguments 'generate:api'
Write-Host "Finalizing build..."
Invoke-Npm -Command 'run' -Arguments 'build'
Set-Location ..
Write-Host "Frontend built!" -ForegroundColor Green

# Copy the frontend build to the backend
Write-Host "Copying frontend build to backend..." -ForegroundColor Cyan
RemoveFolder -Path '.\build\wwwroot'
Copy-Item -Path '.\frontend\build\*' -Destination '.\build\wwwroot' -Recurse
Write-Host "Frontend build copied!" -ForegroundColor Green