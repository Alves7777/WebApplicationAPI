# ?? Guia de Acesso Mobile e Notebook

## ?? Configurações Aplicadas

### ? Backend (API) - WebApplicationAPI
- ? Configurado para aceitar conexões de qualquer IP (0.0.0.0)
- ? CORS configurado para aceitar qualquer origem
- ? Portas: HTTP 5296, HTTPS 7005

### ? Frontend (Blazor) - FinancialControlUI
- ? Configurado para aceitar conexões de qualquer IP (0.0.0.0)
- ? URL da API configurável via appsettings.json
- ? Portas: HTTP 5291, HTTPS 7031

## ?? Como Usar

### 1?? Abrir Portas no Firewall (Execute APENAS UMA VEZ)

**Abra PowerShell como Administrador** e execute:

```powershell
cd "C:\Users\Lucas Alves\Documents\Lucas\.net"
.\configure-firewall.ps1
```

### 2?? Iniciar as Aplicações

1. **Inicie a API primeiro:**
   - No Visual Studio, inicie o projeto `WebApplicationAPI`

2. **Inicie o Blazor:**
   - No Visual Studio, inicie o projeto `FinancialControlUI`

### 3?? Acessar do Celular ou Notebook

**?? IMPORTANTE: Certifique-se que o dispositivo está na MESMA REDE Wi-Fi!**

**Seu IP atual:** `192.168.1.114`

#### ?? URLs para Acessar:

**Opção 1 - HTTP (Recomendado para celular):**
```
http://192.168.1.114:5291
```

**Opção 2 - HTTPS:**
```
https://192.168.1.114:7031
```
*Nota: Você verá um aviso de certificado. Clique em "Avançado" e "Continuar".*

## ?? Configuração Avançada

### Mudou de Rede Wi-Fi? Seu IP pode ter mudado!

Se você conectar em outra rede Wi-Fi, seu IP pode mudar. Para descobrir o novo IP:

```powershell
ipconfig
```

Procure por "Adaptador de Rede sem Fio Wi-Fi" e veja o "Endereço IPv4".

Depois, atualize o arquivo `FinancialControlUI/wwwroot/appsettings.json`:

```json
{
  "ApiBaseUrl": "http://SEU_NOVO_IP:5296/api/"
}
```

## ? Troubleshooting

### Não consigo acessar do celular

1. ? Verifique se celular e PC estão na mesma rede Wi-Fi
2. ? Confirme que executou o script de firewall como Administrador
3. ? Confirme que as aplicações estão rodando (API e Blazor)
4. ? Tente acessar primeiro pelo navegador do PC: http://192.168.1.114:5291
5. ? Desative temporariamente antivírus/firewall de terceiros

### Erro de CORS

- ? Reinicie a API após as alterações
- ? Limpe o cache do navegador (Ctrl+Shift+Del)

### Certificado HTTPS inválido

- Use HTTP (porta 5291) no celular
- Ou aceite o certificado temporariamente no navegador

## ?? Informações Técnicas

- **Blazor WebAssembly:** Roda no navegador do cliente
- **API:** Roda no seu PC (192.168.1.114)
- **CORS:** Configurado para aceitar qualquer origem (desenvolvimento)
- **Firewall:** Portas 5291, 5296, 7031, 7005 abertas

## ?? Segurança

**Esta configuração é APENAS para desenvolvimento!**

Para produção, você deve:
- Configurar CORS específico
- Usar certificados SSL válidos
- Restringir IPs permitidos
- Usar autenticação/autorização
