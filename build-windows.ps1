function Remove-Folder {
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

# Remove all build artifacts
Remove-Folder -Path '.\build'
Remove-Folder -Path '.\RestAPI\bin'
Remove-Folder -Path '.\RestAPI\obj'
Remove-Folder -Path '.\RestAPI\build'

# Build the backend
Write-Host "Building backend..." -ForegroundColor Cyan
Invoke-Dotnet -Command 'tool' -Arguments 'restore'
Invoke-Dotnet -Command 'publish' -Arguments '.\RestAPI\RestAPI.csproj /p:PublishProfile=RestAPI\Properties\PublishProfiles\Windows-x64.pubxml -c Release'
Write-Host "Backend build complete" -ForegroundColor Green