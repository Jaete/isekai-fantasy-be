
$projectPath = Get-Path

Set-Location -Path $projectPath

function Is-PackageInstalled {
    param (
        [string]$packageName
    )

    $csprojPath = "$projectPath\*.csproj"
    $csprojContent = Get-Content -Path $csprojPath

    return $csprojContent -contains $packageName
}

if (-not (Is-PackageInstalled -packageName "Microsoft.EntityFrameworkCore")) {
    Write-Host "Pacote 'Microsoft.EntityFrameworkCore' não encontrado. Instalando..."
    dotnet add package Microsoft.EntityFrameworkCore --version 6.0.0
} else {
    Write-Host "Pacote 'Microsoft.EntityFrameworkCore' já está instalado."
}

if (-not (Is-PackageInstalled -packageName "MySql.EntityFrameworkCore")) {
    Write-Host "Pacote 'MySql.EntityFrameworkCore' não encontrado. Instalando..."
    dotnet add package MySql.EntityFrameworkCore --version 6.0.0
} else {
    Write-Host "Pacote 'MySql.EntityFrameworkCore' já está instalado."
}

if (-not (Is-PackageInstalled -packageName "Microsoft.AspNetCore.Authentication.JwtBearer")) {
    Write-Host "Pacote 'Microsoft.AspNetCore.Authentication.JwtBearer' não encontrado. Instalando..."
    dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 6.0.0
} else {
    Write-Host "Pacote 'Microsoft.AspNetCore.Authentication.JwtBearer' já está instalado."
}

if (-not (Is-PackageInstalled -packageName "xunit")) {
    Write-Host "Pacote 'xunit' não encontrado. Instalando..."
    dotnet add package xunit
} else {
    Write-Host "Pacote 'xunit' já está instalado."
}

Write-Host "Restaurando pacotes NuGet..."
dotnet restore

Write-Host "Processo concluído."
