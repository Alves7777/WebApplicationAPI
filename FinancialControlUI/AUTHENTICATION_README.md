# Sistema de Autenticação JWT - Financial Control UI

## ? Implementação Concluída

O sistema de autenticação JWT foi implementado com sucesso no frontend Blazor WebAssembly.

## ?? O que foi implementado

### 1. **Modelos de Autenticação** (`Models/ApiModels.cs`)
- `LoginRequest` - Modelo para login
- `RegisterRequest` - Modelo para registro
- `AuthResponse` - Resposta com token JWT e dados do usuário

### 2. **Serviço de Autenticação** (`Services/AuthService.cs`)
Funcionalidades:
- ? Login e Registro
- ? Armazenamento de token no localStorage
- ? Verificação de autenticação e expiração do token
- ? Logout com limpeza de dados
- ? Recuperação de informações do usuário (ID, nome, email)
- ? Inicialização automática do token ao carregar a aplicação

### 3. **Páginas de Autenticação**

#### Login (`Pages/Login.razor`)
- Formulário de login com email e senha
- Validação de campos
- Mensagens de erro personalizadas
- Redirecionamento automático se já autenticado
- Link para página de registro

#### Register (`Pages/Register.razor`)
- Formulário de registro com nome, email, senha e confirmação
- Validação de senha forte (mínimo 6 caracteres)
- Validação de confirmação de senha
- Tratamento de erros (email já cadastrado)
- Redirecionamento automático após sucesso
- Link para página de login

### 4. **Componente de Proteção de Rotas** (`Components/AuthorizeRoute.razor`)
- Verifica autenticação antes de renderizar conteúdo
- Redireciona para /login se não autenticado
- Mostra spinner de carregamento durante verificação

### 5. **Páginas Protegidas**
Todas as páginas principais foram protegidas com `<AuthorizeRoute>`:
- ? Home (/)
- ? Dashboard (/dashboard)
- ? Despesas (/expenses)
- ? Categorias (/categories)
- ? Cartões de Crédito (/creditcards)
- ? Controle Mensal (/monthly-financial)
- ? Simulador (/simulator)
- ? Usuários (/users)
- ? Despesas do Cartão (/creditcard/{id}/expenses)
- ? Parcelas (/creditcard/{id}/installments)
- ? Fatura (/creditcard/{id}/statement)
- ? Importar CSV (/import-csv)

### 6. **Menu de Usuário** (`Layout/NavMenu.razor`)
- Exibe informações do usuário logado (nome e email)
- Avatar com ícone
- Botão de logout
- Visível apenas quando autenticado

### 7. **Configuração da Aplicação** (`Program.cs`)
- Registro do `AuthService` no container DI
- Inicialização automática do token JWT ao carregar a aplicação

## ?? Fluxo de Autenticação

### Cenário 1: Primeiro Acesso
1. Usuário acessa qualquer página ? redireciona para `/login`
2. Clica em "Criar conta" ? vai para `/register`
3. Preenche formulário ? backend valida e retorna token
4. Frontend salva token no localStorage
5. Redireciona para `/` (home)
6. Token é automaticamente incluído em todas as requisições

### Cenário 2: Usuário Retornando
1. Usuário acessa a aplicação
2. Frontend verifica token no localStorage
3. Se válido e não expirado ? permite acesso
4. Se inválido/expirado ? redireciona para `/login`

### Cenário 3: Logout
1. Usuário clica em "Sair" no menu
2. Frontend remove token do localStorage
3. Limpa header de autorização do HttpClient
4. Redireciona para `/login`

## ?? Configuração da API

A aplicação está configurada para usar a API em:
```
Base URL: http://localhost:5296/api/
```

Para alterar, edite o arquivo `wwwroot/appsettings.json`:
```json
{
  "ApiBaseUrl": "https://sua-api.com/api/"
}
```

## ?? Endpoints Utilizados

### Autenticação
- `POST /api/auth/login` - Fazer login
- `POST /api/auth/register` - Criar conta

### Endpoints Protegidos (requerem token JWT)
Todos os outros endpoints agora exigem o header:
```
Authorization: Bearer {token}
```

## ?? Armazenamento de Dados

Os seguintes dados são armazenados no `localStorage`:
- `authToken` - Token JWT
- `tokenExpires` - Data de expiração do token
- `userId` - ID do usuário
- `userName` - Nome do usuário
- `userEmail` - Email do usuário

## ?? Tratamento de Erros

### 401 Unauthorized
- Token inválido ou expirado
- Ação: Logout automático e redirecionamento para login

### 403 Forbidden
- Tentativa de acesso a recurso de outro usuário
- Ação: Mensagem de erro exibida

### 400 Bad Request
- Email já cadastrado (no registro)
- Credenciais inválidas (no login)
- Ação: Mensagem de erro específica exibida

## ?? Como Usar

### 1. Iniciar a API
```bash
cd WebApplicationAPI\WebApplicationAPI
dotnet run
```

### 2. Iniciar o Frontend
```bash
cd FinancialControlUI
dotnet run
```

### 3. Acessar a Aplicação
```
http://localhost:5000
```

### 4. Criar uma Conta
1. Clique em "Criar conta"
2. Preencha: Nome, Email, Senha
3. Clique em "Criar Conta"
4. Você será automaticamente logado e redirecionado

### 5. Fazer Login
1. Acesse `/login`
2. Digite seu email e senha
3. Clique em "Entrar"
4. Você será redirecionado para a home

## ?? Segurança

### Token JWT
- Tempo de expiração: 60 minutos (configurado no backend)
- Armazenamento: localStorage (navegador)
- Transmissão: Header Authorization (Bearer Token)

### Validações Implementadas
- ? Senha mínima de 6 caracteres
- ? Verificação de confirmação de senha
- ? Email único (validado no backend)
- ? Token expirado é automaticamente removido
- ? Todas as páginas protegidas requerem autenticação

## ?? Próximos Passos (Opcional)

### Melhorias Sugeridas
1. **Refresh Token**: Implementar renovação automática de token
2. **Remember Me**: Opção de manter login por mais tempo
3. **Recuperação de Senha**: Funcionalidade de "Esqueci minha senha"
4. **Two-Factor Authentication (2FA)**: Camada adicional de segurança
5. **Session Timeout Warning**: Aviso antes do token expirar
6. **Validação de Email**: Confirmar email após registro

## ?? Troubleshooting

### Problema: "Não autorizado" após login
- Verifique se a API está rodando
- Verifique a URL da API no `appsettings.json`
- Limpe o localStorage: F12 ? Application ? Local Storage ? Clear All

### Problema: Redirecionamento infinito
- Limpe o cache do navegador
- Verifique se o token não está expirado
- Limpe o localStorage

### Problema: Erros de CORS
- Configure CORS no backend para permitir a origem do frontend
- Certifique-se de que a API aceita o header `Authorization`

## ? Recursos Destacados

1. **Proteção Automática**: Todas as páginas são protegidas automaticamente
2. **Experiência Fluida**: Token é carregado automaticamente ao iniciar
3. **Feedback Visual**: Mensagens de erro e sucesso claras
4. **UX Otimizada**: Spinners de carregamento durante operações
5. **Isolamento de Dados**: Cada usuário vê apenas seus próprios dados
6. **Logout Seguro**: Limpeza completa de dados ao sair

## ?? Referências

- [ASP.NET Core Blazor WebAssembly](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [JWT Authentication](https://jwt.io/)
- [Blazor Security](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/)

---

**Implementado com sucesso! ??**

Data: $(Get-Date -Format "dd/MM/yyyy HH:mm")
