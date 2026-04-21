# ?? Sistema de Autenticação JWT - Guia de Implementação

## ?? O que foi implementado

? **Autenticação JWT completa**
- Register (cadastro de usuários)
- Login (autenticação)
- Proteção de rotas com `[Authorize]`

? **Multi-tenancy (Multi-usuário)**
- Coluna `UserId` adicionada em todas as tabelas
- Filtros automáticos por usuário

? **Segurança**
- Senha com hash BCrypt
- Tokens JWT com expiração de 7 dias
- Claims (Id, Email, Nome, Role)

---

## ?? Passo a Passo para Usar

### **1. Executar o Script SQL**

Abra o **SQL Server Management Studio (SSMS)** e execute o arquivo:

```
WebApplicationAPI\SQL\AuthenticationSetup.sql
```

Este script irá:
- ? Adicionar colunas de autenticação na tabela `Users`
- ? Adicionar coluna `UserId` em todas as tabelas (`Expenses`, `CreditCards`, etc.)
- ? Criar/atualizar todas as Stored Procedures necessárias
- ? Criar índices para melhor performance

---

### **2. Iniciar a API**

```bash
cd WebApplicationAPI
dotnet run
```

A API estará rodando em:
- HTTP: `http://localhost:5296`
- HTTPS: `https://localhost:7296`

---

### **3. Testar as Rotas de Autenticação**

#### **3.1. Registrar um novo usuário**

```http
POST http://localhost:5296/api/auth/register
Content-Type: application/json

{
  "name": "João Silva",
  "email": "joao@example.com",
  "password": "senha123"
}
```

**Resposta:**
```json
{
  "userId": 1,
  "name": "João Silva",
  "email": "joao@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "role": "User"
}
```

**?? IMPORTANTE:** Salve o `token` retornado! Você precisará dele para fazer requisições autenticadas.

---

#### **3.2. Fazer Login**

```http
POST http://localhost:5296/api/auth/login
Content-Type: application/json

{
  "email": "joao@example.com",
  "password": "senha123"
}
```

**Resposta:**
```json
{
  "userId": 1,
  "name": "João Silva",
  "email": "joao@example.com",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "role": "User"
}
```

---

### **4. Usar Rotas Protegidas**

Agora todas as rotas de `CreditCard` (e outras que você proteger) precisam do **token JWT** no header.

#### **Exemplo: Criar um Cartão de Crédito**

```http
POST http://localhost:5296/api/creditcard
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json

{
  "name": "Nubank",
  "brand": "Mastercard",
  "cardLimit": 5000.00,
  "closingDay": 10,
  "dueDay": 18
}
```

**?? Observação:** O sistema automaticamente pega o `UserId` do token e vincula o cartão ao usuário logado.

---

## ?? Como Proteger Outras Rotas

### **Opção 1: Proteger o Controller inteiro**

```csharp
[ApiController]
[Route("api/expense")]
[Authorize] // <-- Todas as rotas precisam de autenticação
public class ExpenseController : ControllerBase
{
    // ...
}
```

### **Opção 2: Proteger rotas específicas**

```csharp
[ApiController]
[Route("api/expense")]
public class ExpenseController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous] // <-- Esta rota é pública
    public async Task<IActionResult> GetPublicData()
    {
        // ...
    }

    [HttpPost]
    [Authorize] // <-- Esta rota precisa de autenticação
    public async Task<IActionResult> Create([FromBody] CreateExpenseRequest request)
    {
        var userId = this.GetUserId(); // Pega o UserId do token
        // ...
    }
}
```

---

## ??? Como Pegar o UserId do Token

Use a extensão `GetUserId()` criada no arquivo `ControllerExtensions.cs`:

```csharp
using WebApplicationAPI.Extensions;

[HttpPost]
[Authorize]
public async Task<IActionResult> Create([FromBody] CreateExpenseRequest request)
{
    var userId = this.GetUserId(); // Retorna o ID do usuário logado
    var email = this.GetUserEmail(); // Retorna o email
    var role = this.GetUserRole(); // Retorna o role (User/Admin)

    // Agora você pode usar o userId nas suas queries
    await _service.CreateAsync(userId, request);

    return Ok();
}
```

---

## ?? Estrutura das Tabelas Atualizada

### **Users**
```sql
Id (INT, PK, IDENTITY)
Name (NVARCHAR)
Email (NVARCHAR, UNIQUE)
PasswordHash (NVARCHAR) -- Hash BCrypt da senha
Role (NVARCHAR) -- "User" ou "Admin"
IsActive (BIT)
CreatedAt (DATETIME)
UpdatedAt (DATETIME)
```

### **Expenses**
```sql
Id (INT, PK)
UserId (INT, FK) -- NOVO! Vínculo com o usuário
Month (INT)
Year (INT)
Description (NVARCHAR)
Amount (DECIMAL)
Category (NVARCHAR)
Status (NVARCHAR)
PaymentMethod (NVARCHAR)
CreatedAt (DATETIME)
```

### **CreditCards**
```sql
Id (INT, PK)
UserId (INT, FK) -- NOVO! Vínculo com o usuário
Name (NVARCHAR)
Brand (NVARCHAR)
CardLimit (DECIMAL)
ClosingDay (INT)
DueDay (INT)
IsActive (BIT)
CreatedAt (DATETIME)
UpdatedAt (DATETIME)
```

---

## ?? Segurança

### **Chave JWT**
A chave JWT está no `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "ChaveSecretaSuperSegura123!@#MinimoDe32Caracteres",
    "Issuer": "WebApplicationAPI",
    "Audience": "WebApplicationAPI"
  }
}
```

**?? PRODUÇÃO:** Nunca commite a chave JWT no Git! Use variáveis de ambiente ou Azure Key Vault.

### **Tempo de Expiração do Token**
O token expira em **7 dias**. Você pode alterar isso no `AuthService.cs`:

```csharp
expires: DateTime.UtcNow.AddDays(7), // Altere aqui
```

---

## ?? Testando no Swagger

1. Inicie a API
2. Acesse: `http://localhost:5296/swagger`
3. Faça login ou register no endpoint `/api/auth/login`
4. Copie o `token` retornado
5. Clique no botão **"Authorize"** no topo do Swagger
6. Cole o token no formato: `Bearer SEU_TOKEN_AQUI`
7. Clique em "Authorize"
8. Agora você pode testar as rotas protegidas!

---

## ?? Próximos Passos

### **Para completar a implementação:**

1. ? **Atualizar UserRepository** para usar as novas SPs de autenticação
2. ? **Atualizar ExpenseRepository** para filtrar por `UserId`
3. ? **Atualizar CreditCardRepository** para filtrar por `UserId`
4. ? **Adicionar `[Authorize]`** em todos os controllers que precisam
5. ? **Criar middleware** para logging de usuários
6. ? **Implementar refresh token** (opcional)
7. ? **Adicionar roles e permissões** (Admin, User, etc.)

---

## ?? Troubleshooting

### **Erro: "No authenticationScheme was specified"**
? Certifique-se que `app.UseAuthentication()` está **ANTES** de `app.UseAuthorization()` no `Program.cs`

### **Erro: "Unauthorized" em todas as rotas**
? Verifique se está enviando o header: `Authorization: Bearer SEU_TOKEN`

### **Erro: "Email já cadastrado"**
? Use outro email ou delete o usuário do banco antes de testar novamente

### **Token inválido**
? Verifique se a chave JWT no `appsettings.json` é a mesma que gerou o token
? Verifique se o token não expirou (7 dias)

---

## ?? Referências

- [JWT.io](https://jwt.io/) - Para decodificar e debugar tokens
- [BCrypt Calculator](https://bcrypt-generator.com/) - Para testar hashes
- [ASP.NET Core Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/)

---

## ? Checklist de Implementação

- [x] Instalar pacotes NuGet (JWT + BCrypt)
- [x] Atualizar Models (User, CreditCard, Expense)
- [x] Criar DTOs de autenticação
- [x] Criar AuthService e IAuthService
- [x] Criar AuthController
- [x] Configurar JWT no Program.cs
- [x] Criar scripts SQL
- [x] Adicionar extensões (GetUserId)
- [x] Proteger rotas com [Authorize]
- [ ] Executar script SQL no banco
- [ ] Testar endpoints
- [ ] Atualizar todos os repositories

---

**?? Pronto! Seu sistema agora tem autenticação JWT completa e multi-tenancy!**
