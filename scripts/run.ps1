# Execute no PowerShell do Windows 11, na pasta raiz do projeto.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root 'src\Kp4040.App\Kp4040.App.csproj'
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error 'Instale o .NET 10 SDK para Windows: https://dotnet.microsoft.com/download/dotnet/10.0'
    exit 1
}
& dotnet run --project $project
exit $LASTEXITCODE
