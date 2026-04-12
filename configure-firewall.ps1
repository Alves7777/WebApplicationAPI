# Script para abrir portas no Firewall do Windows
# Execute como Administrador

Write-Host "Abrindo portas no Firewall..." -ForegroundColor Green

# Blazor
New-NetFirewallRule -DisplayName "Blazor HTTP" -Direction Inbound -LocalPort 5291 -Protocol TCP -Action Allow
New-NetFirewallRule -DisplayName "Blazor HTTPS" -Direction Inbound -LocalPort 7031 -Protocol TCP -Action Allow

# API
New-NetFirewallRule -DisplayName "API HTTP" -Direction Inbound -LocalPort 5296 -Protocol TCP -Action Allow
New-NetFirewallRule -DisplayName "API HTTPS" -Direction Inbound -LocalPort 7005 -Protocol TCP -Action Allow

Write-Host "Portas abertas com sucesso!" -ForegroundColor Green
Write-Host ""
Write-Host "Seu IP local: 192.168.1.114" -ForegroundColor Yellow
Write-Host ""
Write-Host "URLs para acessar no celular/notebook:" -ForegroundColor Cyan
Write-Host "  Blazor HTTP:  http://192.168.1.114:5291" -ForegroundColor White
Write-Host "  Blazor HTTPS: https://192.168.1.114:7031" -ForegroundColor White
Write-Host "  API HTTP:     http://192.168.1.114:5296" -ForegroundColor White
Write-Host "  API HTTPS:    https://192.168.1.114:7005" -ForegroundColor White
