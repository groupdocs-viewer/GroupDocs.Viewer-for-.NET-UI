<#
.SYNOPSIS
  Builds the GroupDocs.Viewer.UI Docker image locally.
.EXAMPLE
  .\build-docker.ps1
  .\build-docker.ps1 -Tag "26.3.0"
#>

param(
    [string]$Tag = "latest"
)

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

$imageName = "groupdocs/viewer-ui"

echo "build-docker: Building image ${imageName}:${Tag}"

exec { & docker build -f docker/Dockerfile -t "${imageName}:${Tag}" . }

if ($Tag -ne "latest") {
    exec { & docker tag "${imageName}:${Tag}" "${imageName}:latest" }
}

echo "build-docker: Image built successfully - ${imageName}:${Tag}"
