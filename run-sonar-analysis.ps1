# Script para ejecutar análisis de SonarQube en el proyecto BadCleanArch
# Fecha: 23 de noviembre de 2025

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  ANÁLISIS DE SONARQUBE - BadCleanArch  " -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Configuración
$projectKey = "BadCleanArch"
$projectName = "BadCleanArch - Clean Architecture"
$sonarUrl = "http://localhost:9000"
$sonarLogin = "admin"
$sonarPassword = "admin"

Write-Host " Configuración:" -ForegroundColor Yellow
Write-Host "   Proyecto: $projectName" -ForegroundColor White
Write-Host "   Key: $projectKey" -ForegroundColor White
Write-Host "   SonarQube: $sonarUrl`n" -ForegroundColor White

# Paso 1: Verificar que SonarQube está corriendo
Write-Host " Paso 1: Verificando SonarQube Server..." -ForegroundColor Cyan
$result = Test-NetConnection -ComputerName localhost -Port 9000 -WarningAction SilentlyContinue
if (-not $result.TcpTestSucceeded) {
    Write-Host " ERROR: SonarQube no está corriendo en $sonarUrl" -ForegroundColor Red
    Write-Host "   Por favor, inicia SonarQube primero.`n" -ForegroundColor Yellow
    exit 1
}
Write-Host " SonarQube está corriendo`n" -ForegroundColor Green

# Paso 2: Limpiar builds anteriores
Write-Host " Paso 2: Limpiando builds anteriores..." -ForegroundColor Cyan
dotnet clean --nologo
Write-Host " Limpieza completada`n" -ForegroundColor Green

# Paso 3: Iniciar análisis de SonarQube
Write-Host " Paso 3: Iniciando análisis de SonarQube..." -ForegroundColor Cyan
dotnet sonarscanner begin `
    /k:"$projectKey" `
    /n:"$projectName" `
    /d:sonar.host.url="$sonarUrl" `
    /d:sonar.login="$sonarLogin" `
    /d:sonar.password="$sonarPassword"

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n ERROR: Falló al iniciar el análisis" -ForegroundColor Red
    exit 1
}
Write-Host " Análisis iniciado`n" -ForegroundColor Green

# Paso 4: Compilar el proyecto
Write-Host " Paso 4: Compilando el proyecto..." -ForegroundColor Cyan
dotnet build --no-incremental --nologo

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n ERROR: Falló la compilación" -ForegroundColor Red
    exit 1
}
Write-Host " Compilación exitosa`n" -ForegroundColor Green

# Paso 5: Finalizar análisis y enviar a SonarQube
Write-Host " Paso 5: Enviando resultados a SonarQube..." -ForegroundColor Cyan
dotnet sonarscanner end `
    /d:sonar.login="$sonarLogin" `
    /d:sonar.password="$sonarPassword"

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n ERROR: Falló al enviar resultados" -ForegroundColor Red
    exit 1
}

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "   ANÁLISIS COMPLETADO EXITOSAMENTE  " -ForegroundColor Green
Write-Host "========================================`n" -ForegroundColor Green

Write-Host " Ver resultados en:" -ForegroundColor Cyan
Write-Host "   $sonarUrl/dashboard?id=$projectKey`n" -ForegroundColor Yellow

Write-Host " Dashboard de SonarQube:" -ForegroundColor Cyan
Write-Host "   - Bugs" -ForegroundColor White
Write-Host "   - Vulnerabilidades" -ForegroundColor White
Write-Host "   - Code Smells" -ForegroundColor White
Write-Host "   - Cobertura" -ForegroundColor White
Write-Host "   - Duplicaciones`n" -ForegroundColor White

Write-Host " Presiona Enter para abrir el dashboard en el navegador..." -ForegroundColor Yellow
Read-Host

Start-Process "$sonarUrl/dashboard?id=$projectKey"

Write-Host " ¡Listo! Revisa tu navegador para ver los resultados.`n" -ForegroundColor Green
