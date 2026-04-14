# ?? Guia de Configuração - Acesso Remoto (Celular/Notebook)

## ?? Pré-requisitos
1. Computador e dispositivo móvel conectados na **mesma rede Wi-Fi**
2. PowerShell com permissões de **Administrador**

## ?? Configuração Inicial

### 1?? Configure o Firewall (Execute como Administrador)
```powershell
.\configure-firewall.ps1
```

Este script irá:
- ? Abrir as portas necessárias (5291, 7031, 5296, 7005)
- ? Detectar automaticamente seu IP local
- ? Mostrar as URLs para acessar no celular/notebook

### 2?? Seu IP Atual
Após executar o script, você verá algo como:
```
Seu IP local atual: 192.168.0.178
```

### 3?? Arquivo de Configuração Atualizado
O arquivo `FinancialControlUI\wwwroot\appsettings.json` já está configurado com o IP:
```json
{
  "ApiBaseUrl": "http://192.168.0.178:5296/api/"
}
```

> ?? **IMPORTANTE**: Se você mudar de rede, você precisa:
> 1. Executar o script `configure-firewall.ps1` novamente
> 2. Atualizar o IP no arquivo `appsettings.json`

## ?? Como Executar

### Iniciar a API
```powershell
cd WebApplicationAPI\WebApplicationAPI
dotnet run
```
A API estará disponível em: `http://192.168.0.178:5296`

### Iniciar o Blazor WebAssembly
```powershell
cd FinancialControlUI
dotnet run
```
O Blazor estará disponível em: `http://192.168.0.178:5291`

## ?? Acesso pelo Celular/Notebook

### No Navegador do Celular/Notebook, acesse:
```
http://192.168.0.178:5291
```

### Ou use o QR Code (se disponível):
```
[Escaneie o QR Code no terminal]
```

## ?? Verificar IP Local
Se precisar verificar manualmente seu IP:
```powershell
ipconfig
```
Procure por "Endereço IPv4" na interface de rede ativa (geralmente algo como 192.168.0.x ou 192.168.1.x)

## ?? Solução de Problemas

### Erro de CORS
Se aparecer erro de CORS no navegador:
1. Verifique se a API está rodando
2. Confirme que o IP no `appsettings.json` está correto
3. Reinicie o Blazor após alterar o IP

### Não Consegue Conectar
1. ? Verifique se ambos os dispositivos estão na mesma rede
2. ? Execute o script `configure-firewall.ps1` novamente
3. ? Desative temporariamente antivírus/firewall de terceiros
4. ? Tente acessar pelo IP no próprio computador primeiro

### API Retorna Erro 404
1. ? Confirme que a API está rodando (`http://192.168.0.178:5296/swagger`)
2. ? Verifique se o IP no `appsettings.json` está correto
3. ? Reinicie ambos os projetos

## ?? URLs de Referência

| Serviço | URL Local | URL Remota |
|---------|-----------|------------|
| Blazor | http://localhost:5291 | http://192.168.0.178:5291 |
| API | http://localhost:5296 | http://192.168.0.178:5296 |
| Swagger | http://localhost:5296/swagger | http://192.168.0.178:5296/swagger |

## ?? Mudanças Recentes

### ? Implementado
- Botão "? Voltar" em todas as páginas
- IP atualizado de `192.168.1.114` ? `192.168.0.178`
- CORS configurado para aceitar qualquer origem
- Kestrel configurado para escutar em todas as interfaces (0.0.0.0)

### ?? Páginas Disponíveis
1. ?? **Home** - `/` - Página inicial
2. ?? **Dashboard** - `/dashboard` - Resumo financeiro
3. ?? **Despesas** - `/expenses` - Gerenciar despesas
4. ??? **Categorias** - `/categories` - Gerenciar categorias
5. ?? **Cartões** - `/creditcards` - Gerenciar cartões de crédito
6. ?? **Controle Mensal** - `/monthly-financial` - Controle mensal
7. ?? **Simulador** - `/simulator` - Simular compras parceladas
8. ?? **Usuários** - `/users` - Gerenciar usuários

## ?? Próximos Passos
Veja o arquivo `MAPEAMENTO-ENDPOINTS.md` para ver todos os endpoints disponíveis e sugestões de implementação futura.
