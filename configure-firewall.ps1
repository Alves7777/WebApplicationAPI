# Script para abrir portas no Firewall do Windows
# Execute como Administrador

Write-Host "Abrindo portas no Firewall..." -ForegroundColor Green

# Remove regras antigas se existirem
Remove-NetFirewallRule -DisplayName "Blazor HTTP" -ErrorAction SilentlyContinue
Remove-NetFirewallRule -DisplayName "Blazor HTTPS" -ErrorAction SilentlyContinue
Remove-NetFirewallRule -DisplayName "API HTTP" -ErrorAction SilentlyContinue
Remove-NetFirewallRule -DisplayName "API HTTPS" -ErrorAction SilentlyContinue

# Blazor
New-NetFirewallRule -DisplayName "Blazor HTTP" -Direction Inbound -LocalPort 5291 -Protocol TCP -Action Allow
New-NetFirewallRule -DisplayName "Blazor HTTPS" -Direction Inbound -LocalPort 7031 -Protocol TCP -Action Allow

# API
New-NetFirewallRule -DisplayName "API HTTP" -Direction Inbound -LocalPort 5296 -Protocol TCP -Action Allow
New-NetFirewallRule -DisplayName "API HTTPS" -Direction Inbound -LocalPort 7005 -Protocol TCP -Action Allow

Write-Host "Portas abertas com sucesso!" -ForegroundColor Green
Write-Host ""

# Obter IP atual automaticamente
$ipAddress = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.InterfaceAlias -notlike "*Loopback*" -and $_.IPAddress -notlike "169.254.*" } | Select-Object -First 1).IPAddress

Write-Host "Seu IP local atual: $ipAddress" -ForegroundColor Yellow
Write-Host ""
Write-Host "URLs para acessar no celular/notebook:" -ForegroundColor Cyan
Write-Host "  Blazor HTTP:  http://${ipAddress}:5291" -ForegroundColor White
Write-Host "  Blazor HTTPS: https://${ipAddress}:7031" -ForegroundColor White
Write-Host "  API HTTP:     http://${ipAddress}:5296" -ForegroundColor White
Write-Host "  API HTTPS:    https://${ipAddress}:7005" -ForegroundColor White
Write-Host ""
Write-Host "IMPORTANTE: Atualize o arquivo 'FinancialControlUI\wwwroot\appsettings.json' com:" -ForegroundColor Yellow
Write-Host "  {" -ForegroundColor White
Write-Host "    `"ApiBaseUrl`": `"http://${ipAddress}:5296/api/`"" -ForegroundColor White
Write-Host "  }" -ForegroundColor White
