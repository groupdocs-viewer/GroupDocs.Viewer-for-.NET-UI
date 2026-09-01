# Taken from psake https://github.com/psake/psake

<#
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } "Error executing SVN. Please verify SVN command-line client is installed"
#>
function Exec {
    [CmdletBinding()]
    param(
        [Parameter(Position = 0, Mandatory = 1)][scriptblock]$cmd,
        [Parameter(Position = 1, Mandatory = 0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

if (-Not (Test-Path .\artifacts)) { throw ("Folder '.\artifacts' does not exit.") }

# The shim release of a retired ID must go last: its dependency on the replacement should
# never resolve to a package that is not on the feed yet. nuget.org does not validate this
# at push time, so nothing enforces it for us - hence the explicit ordering rather than
# relying on Get-ChildItem happening to sort the replacement first.
$shimId = "GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform"

$all = Get-ChildItem .\artifacts -Filter *.nupkg
$shims = @($all | Where-Object { $_.Name -like "$shimId.*" })
$rest = @($all | Where-Object { $_.Name -notlike "$shimId.*" })

foreach ($item in $rest + $shims) {
    $fileName = $item.Name
    $package = ".\artifacts\${fileName}"

    Write-Host "Package $package will be published."

    exec { & dotnet nuget push $package --api-key "$env:NUGET_API_KEY_PROD" --source https://api.nuget.org/v3/index.json --skip-duplicate }
}
