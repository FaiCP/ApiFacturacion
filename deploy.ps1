# =============================================================================
# deploy.ps1 — Despliegue automatizado de GestorAdmi.Core a somee.com
# =============================================================================
# USO:
#   .\deploy.ps1                  → corre tests + publica + sube por FTP
#   .\deploy.ps1 -SkipTests       → omite tests (no recomendado)
#   .\deploy.ps1 -WhatIf          → muestra lo que haría sin ejecutar nada
# =============================================================================

param(
    [string]$FtpHost      = "ftp://CNEAPI.somee.com",
    [string]$FtpPath      = "www.CNEAPI.somee.com",
    [string]$FtpUser      = "Fairez",
    [string]$FtpPass      = "Pechugas36.",
    [string]$Configuration = "Release",
    [switch]$SkipTests,
    [switch]$UploadOnly,
    [switch]$WhatIf
)

$ErrorActionPreference = "Stop"
$RootDir    = $PSScriptRoot
$ApiProject = "$RootDir\src\API\API.csproj"
$UnitTests  = "$RootDir\tests\GestorAdmi.Tests.Unit\GestorAdmi.Tests.Unit.csproj"
$IntTests   = "$RootDir\tests\GestorAdmi.Tests.Integration\GestorAdmi.Tests.Integration.csproj"
$PublishDir = "$RootDir\publish"

# ─── Helpers de salida ───────────────────────────────────────────────────────
function Write-Step    { param([string]$m) Write-Host "`n===> $m" -ForegroundColor Cyan    }
function Write-Ok      { param([string]$m) Write-Host "  OK  $m"  -ForegroundColor Green   }
function Write-Warn    { param([string]$m) Write-Host "  !!  $m"  -ForegroundColor Yellow  }
function Write-Err     { param([string]$m) Write-Host " ERR  $m"  -ForegroundColor Red     }
function Abort         { param([string]$m) Write-Err $m; exit 1                             }

if ($WhatIf) {
    Write-Warn "Modo WhatIf: solo se muestran los pasos, no se ejecuta nada."
}

Write-Host @"

  ╔══════════════════════════════════════════════════════╗
  ║       GestorAdmi.Core — Deploy a somee.com           ║
  ╚══════════════════════════════════════════════════════╝
  Configuracion : $Configuration
  Destino FTP   : $FtpHost/$FtpPath
  Directorio    : $PublishDir

"@ -ForegroundColor White

if ($UploadOnly) {
    Write-Warn "Modo -UploadOnly: se omiten build y tests, solo se sube la carpeta publish existente."
    if (-not (Test-Path $PublishDir)) { Abort "No existe la carpeta publish. Ejecuta el script sin -UploadOnly primero." }
} else {
    # ─────────────────────────────────────────────────────────────────────────────
    # PASO 1 — Restaurar paquetes
    # ─────────────────────────────────────────────────────────────────────────────
    Write-Step "PASO 1/4 — Restaurando dependencias NuGet..."
    if (-not $WhatIf) {
        dotnet restore "$RootDir\GestorAdmi.Core.slnx" --verbosity quiet
        if ($LASTEXITCODE -ne 0) { Abort "Fallo al restaurar paquetes." }
    }
    Write-Ok "Paquetes restaurados."

    # ─────────────────────────────────────────────────────────────────────────────
    # PASO 2 — Ejecutar pruebas
    # ─────────────────────────────────────────────────────────────────────────────
    Write-Step "PASO 2/4 — Ejecutando pruebas..."

    if ($SkipTests) {
        Write-Warn "Tests OMITIDOS por flag -SkipTests. Proceder con precaucion."
    } else {
        # Pruebas unitarias
        Write-Host "  >> Pruebas unitarias..." -ForegroundColor Gray
        if (-not $WhatIf) {
            dotnet test $UnitTests `
                --configuration $Configuration `
                --no-restore `
                --verbosity minimal `
                --logger "console;verbosity=minimal"
            if ($LASTEXITCODE -ne 0) { Abort "Pruebas UNITARIAS fallaron. Despliegue cancelado." }
        }
        Write-Ok "Pruebas unitarias: PASARON"

        # Pruebas de integracion
        Write-Host "  >> Pruebas de integracion..." -ForegroundColor Gray
        if (-not $WhatIf) {
            dotnet test $IntTests `
                --configuration $Configuration `
                --no-restore `
                --verbosity minimal `
                --logger "console;verbosity=minimal"
            if ($LASTEXITCODE -ne 0) { Abort "Pruebas de INTEGRACION fallaron. Despliegue cancelado." }
        }
        Write-Ok "Pruebas de integracion: PASARON"
    }

    # ─────────────────────────────────────────────────────────────────────────────
    # PASO 3 — Publicar la API
    # ─────────────────────────────────────────────────────────────────────────────
    Write-Step "PASO 3/4 — Publicando API (modo $Configuration)..."

    if (-not $WhatIf) {
        if (Test-Path $PublishDir) {
            Remove-Item -Recurse -Force $PublishDir
        }

        dotnet publish $ApiProject `
            --configuration $Configuration `
            --output $PublishDir `
            --self-contained false `
            --verbosity minimal

        if ($LASTEXITCODE -ne 0) { Abort "Fallo al publicar el proyecto." }
    }
    Write-Ok "Publicado en: $PublishDir"
}

# ─────────────────────────────────────────────────────────────────────────────
# PASO 4 — Subir por FTP a somee.com (via curl)
# ─────────────────────────────────────────────────────────────────────────────
Write-Step "PASO 4/4 — Subiendo archivos por FTP a somee.com..."

if ($WhatIf) {
    Write-Warn "WhatIf: Se subirian los archivos de '$PublishDir' a '$FtpHost/$FtpPath'"
} else {
    # Verificar que curl esté disponible
    if (-not (Get-Command curl.exe -ErrorAction SilentlyContinue)) {
        Abort "curl.exe no encontrado. Instala curl o usa Windows 11 que lo incluye por defecto."
    }

    $UploadedFiles = 0
    $FailedFiles   = @()
    $AllFiles      = Get-ChildItem -Recurse -File $PublishDir
    $TotalFiles    = $AllFiles.Count

    Write-Host "  Total de archivos a subir: $TotalFiles" -ForegroundColor Gray

    foreach ($file in $AllFiles) {
        # Ruta relativa desde el directorio publish
        $relativePath = $file.FullName.Substring($PublishDir.Length).TrimStart('\', '/')
        # Convertir separadores Windows a FTP (forward slash)
        $relativePath = $relativePath.Replace('\', '/')

        $ftpTarget = "$FtpHost/$FtpPath/$relativePath"

        try {
            # curl: --ftp-create-dirs crea subdirectorios automáticamente
            $curlOutput = & curl.exe --ftp-create-dirs `
                --silent `
                --show-error `
                --user "$FtpUser`:$FtpPass" `
                --upload-file $file.FullName `
                $ftpTarget 2>&1

            if ($LASTEXITCODE -ne 0) {
                throw "curl exit code $LASTEXITCODE — $curlOutput"
            }

            $UploadedFiles++
            $pct = [math]::Round(($UploadedFiles / $TotalFiles) * 100)
            Write-Progress -Activity "Subiendo archivos..." `
                           -Status "$relativePath [$UploadedFiles/$TotalFiles]" `
                           -PercentComplete $pct
        } catch {
            $FailedFiles += $relativePath
            Write-Warn "  Error subiendo: $relativePath — $_"
        }
    }

    Write-Progress -Activity "Subiendo archivos..." -Completed

    if ($FailedFiles.Count -gt 0) {
        Write-Warn "$($FailedFiles.Count) archivos fallaron:"
        $FailedFiles | ForEach-Object { Write-Host "    - $_" -ForegroundColor Yellow }
        Abort "Despliegue completado con errores."
    }
}

# ─────────────────────────────────────────────────────────────────────────────
# Resumen final
# ─────────────────────────────────────────────────────────────────────────────
Write-Host @"

  ╔══════════════════════════════════════════════════════╗
  ║              DESPLIEGUE COMPLETADO                   ║
  ║  URL: https://CNEAPI.somee.com                       ║
  ║  Swagger: https://CNEAPI.somee.com/swagger           ║
  ╚══════════════════════════════════════════════════════╝

"@ -ForegroundColor Green
