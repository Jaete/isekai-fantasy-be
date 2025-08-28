#!/bin/bash
PROJECT_PATH=$(pwd)

is_package_installed() {
  local package_name=$1
  grep -q "<PackageReference Include=\"$package_name\"" "$PROJECT_PATH/*.csproj"
}


if ! is_package_installed "Microsoft.EntityFrameworkCore"; then
  echo "Pacote 'Microsoft.EntityFrameworkCore' não encontrado. Instalando..."
  dotnet add package Microsoft.EntityFrameworkCore
else
  echo "Pacote 'Microsoft.EntityFrameworkCore' já está instalado."
fi


if ! is_package_installed "MySql.EntityFrameworkCore"; then
  echo "Pacote 'MySql.EntityFrameworkCore' não encontrado. Instalando..."
  dotnet add package MySql.EntityFrameworkCore
else
  echo "Pacote 'MySql.EntityFrameworkCore' já está instalado."
fi


if ! is_package_installed "Microsoft.AspNetCore.Authentication.JwtBearer"; then
  echo "Pacote 'Microsoft.AspNetCore.Authentication.JwtBearer' não encontrado. Instalando..."
  dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
else
  echo "Pacote 'Microsoft.AspNetCore.Authentication.JwtBearer' já está instalado."
fi


if ! is_package_installed "xunit"; then
  echo "Pacote 'xunit' não encontrado. Instalando..."
  dotnet add package xunit
else
  echo "Pacote 'xunit' já está instalado."
fi


echo "Restaurando pacotes NuGet..."
dotnet restore

echo "Processo concluído."
