Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  INICIALIZANDO FINANCIAL CONTROL" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Obter IP local
$ipAddress = (Get-NetIPAddress -AddressFamily IPv4 -InterfaceAlias Wi-Fi* | Select-Object -First 1).IPAddress

Write-Host "?? SEU IP LOCAL: $ipAddress" -ForegroundColor Green
Write-Host ""
Write-Host "?? ACESSE DO CELULAR:" -ForegroundColor Yellow
Write-Host "   https://${ipAddress}:7031" -ForegroundColor White
Write-Host "   ou" -ForegroundColor Gray
Write-Host "   http://${ipAddress}:5291" -ForegroundColor White
Write-Host ""
Write-Host "??  IMPORTANTE: Certifique-se de que o firewall permite conexões nas portas 5291 e 7031" -ForegroundColor Yellow
Write-Host ""

# Verificar se firewall está configurado
Write-Host "?? Verificando regras de firewall..." -ForegroundColor Cyan
$firewallRule = Get-NetFirewallRule -DisplayName "Blazor Dev" -ErrorAction SilentlyContinue

if ($null -eq $firewallRule) {
    Write-Host "? Regra de firewall não encontrada!" -ForegroundColor Red
    Write-Host ""
    $response = Read-Host "Deseja criar a regra de firewall agora? (S/N)"

    if ($response -eq "S" -or $response -eq "s") {
        try {
            New-NetFirewallRule -DisplayName "Blazor Dev" -Direction Inbound -Protocol TCP -LocalPort 5291,7031 -Action Allow -ErrorAction Stop
            Write-Host "? Regra de firewall criada com sucesso!" -ForegroundColor Green
        } catch {
            Write-Host "? Erro ao criar regra. Execute como Administrador!" -ForegroundColor Red
        }
    }
} else {
    Write-Host "? Regra de firewall já configurada!" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  INSTRUÇÕES" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Abra outro terminal" -ForegroundColor White
Write-Host "2. Execute o backend:" -ForegroundColor White
Write-Host "   cd 'WebApplicationAPI\WebApplicationAPI'" -ForegroundColor Gray
Write-Host "   dotnet run" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Neste terminal, execute o frontend:" -ForegroundColor White
Write-Host "   cd 'FinancialControlUI'" -ForegroundColor Gray
Write-Host "   dotnet run" -ForegroundColor Gray
Write-Host ""
Write-Host "4. Acesse no navegador: https://localhost:7031" -ForegroundColor White
Write-Host ""
Write-Host "5. No celular (mesma rede WiFi): https://${ipAddress}:7031" -ForegroundColor White
Write-Host ""

Write-Host "Pressione qualquer tecla para continuar..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
