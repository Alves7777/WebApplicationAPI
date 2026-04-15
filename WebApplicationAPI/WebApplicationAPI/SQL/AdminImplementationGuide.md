# ?? Guia de Implementação - Sistema Admin

## ?? Objetivo

Implementar um sistema onde:
- **Admin (UserId 2002)**: Tem acesso a **TODOS** os dados de todos os usuários
- **Usuários comuns**: Veem apenas **seus próprios** dados

---

## ?? Passo a Passo de Implementação

### **PASSO 1: Executar Scripts SQL** ? **FAÇA PRIMEIRO**

Execute os scripts **NA ORDEM**:

#### **1.1. Vincular dados existentes ao UserId 2004**

```sql
-- Arquivo: SQL\LinkDataToUser2004.sql
-- Vincula todos os registros órfãos (sem UserId) ao UserId 2004
```

**O que faz:**
- Atualiza `Expenses` sem UserId ? UserId = 2004
- Atualiza `CreditCards` sem UserId ? UserId = 2004
- Atualiza `CreditCardExpenses` sem UserId ? UserId = 2004
- Atualiza `InstallmentPurchases` sem UserId ? UserId = 2004
- Configura UserId 2002 com Role = 'Admin'

**Como executar:**
1. Abra SSMS
2. Abra o arquivo `LinkDataToUser2004.sql`
3. Execute (F5)

---

#### **1.2. Criar Stored Procedures com suporte a Admin**

```sql
-- Arquivo: SQL\StoredProceduresWithAdmin.sql
-- Cria SPs que verificam se o usuário é Admin
```

**O que faz:**
- Cria função `fn_IsAdmin()` para verificar se usuário é Admin
- Atualiza todas as SPs para suportar Admin
- Admin vê TODOS os dados
- Usuários comuns veem apenas seus dados

**Como executar:**
1. Abra SSMS
2. Abra o arquivo `StoredProceduresWithAdmin.sql`
3. Execute (F5)

---

### **PASSO 2: Verificar Usuários no Banco**

Após executar os scripts, verifique:

```sql
-- Ver todos os usuários
SELECT 
    Id,
    Name,
    Email,
    Role,
    IsActive
FROM Users
ORDER BY Id;
```

**Resultado esperado:**

| Id | Name | Email | Role | IsActive |
|----|------|-------|------|----------|
| 2002 | João Silva | joao@teste.com | **Admin** | 1 |
| 2003 | Maria Santos | maria@teste.com | User | 1 |
| 2004 | ... | ... | User | 1 |

---

### **PASSO 3: Testar no SSMS**

#### **Teste 1: Usuário Admin (2002) vê TUDO**

```sql
-- Deve retornar TODAS as expenses de TODOS os usuários
EXEC sp_GetExpensesByUserId @UserId = 2002;

-- Deve retornar TODOS os cartões de TODOS os usuários
EXEC sp_GetCreditCardsByUserId @UserId = 2002;
```

#### **Teste 2: Usuário comum (2004) vê apenas seus dados**

```sql
-- Deve retornar APENAS as expenses do UserId 2004
EXEC sp_GetExpensesByUserId @UserId = 2004;

-- Deve retornar APENAS os cartões do UserId 2004
EXEC sp_GetCreditCardsByUserId @UserId = 2004;
```

---

### **PASSO 4: Atualizar Backend .NET** (Opcional - Já está funcionando)

Os Repositories já chamam as SPs corretas. **NÃO** precisa alterar código .NET!

Os controllers já pegam o `UserId` do token JWT automaticamente:

```csharp
[HttpGet]
[Authorize]
public async Task<IActionResult> GetAll()
{
    var userId = this.GetUserId(); // Pega do token JWT

    // Se userId = 2002 (Admin), retorna tudo
    // Se userId = 2004 (User), retorna apenas dele
    var cards = await _service.GetAllAsync(userId);

    return Ok(cards);
}
```

**? As SPs fazem a mágica automaticamente!**

---

### **PASSO 5: Testar na API**

#### **5.1. Login como Admin (2002)**

```http
POST http://localhost:5296/api/auth/login
Content-Type: application/json

{
  "email": "joao@teste.com",
  "password": "senha123"
}
```

**Copie o token do Admin!**

---

#### **5.2. Listar cartões como Admin**

```http
GET http://localhost:5296/api/creditcard
Authorization: Bearer TOKEN_DO_ADMIN
```

**Resultado:** Deve retornar **TODOS** os cartões de **TODOS** os usuários! ??

---

#### **5.3. Login como Usuário comum (2004)**

```http
POST http://localhost:5296/api/auth/login
Content-Type: application/json

{
  "email": "usuario@teste.com",
  "password": "senha456"
}
```

**Copie o token do usuário comum!**

---

#### **5.4. Listar cartões como usuário comum**

```http
GET http://localhost:5296/api/creditcard
Authorization: Bearer TOKEN_DO_USER
```

**Resultado:** Deve retornar **APENAS** os cartões do UserId 2004!

---

## ?? **Como Funciona Internamente**

### **1. Função fn_IsAdmin**

```sql
CREATE FUNCTION fn_IsAdmin(@UserId INT)
RETURNS BIT
AS
BEGIN
    RETURN (SELECT CASE WHEN Role = 'Admin' THEN 1 ELSE 0 END 
            FROM Users WHERE Id = @UserId)
END
```

---

### **2. Stored Procedure com verificação**

```sql
CREATE PROCEDURE sp_GetCreditCardsByUserId
    @UserId INT
AS
BEGIN
    DECLARE @IsAdmin BIT = dbo.fn_IsAdmin(@UserId);

    SELECT * 
    FROM CreditCards
    WHERE @IsAdmin = 1              -- Admin vê tudo
       OR UserId = @UserId;         -- User vê apenas seus
END
```

---

### **3. Controller não precisa mudar**

```csharp
[HttpGet]
[Authorize]
public async Task<IActionResult> GetAll()
{
    var userId = this.GetUserId(); // Do token JWT

    // Chama o service normalmente
    var cards = await _service.GetAllAsync(userId);

    // Se userId = 2002 ? SP retorna tudo
    // Se userId = 2004 ? SP retorna só dele

    return Ok(cards);
}
```

**? Simples e automático!**

---

## ?? **Tabela Resumo**

| Ação | Admin (2002) | User (2004) |
|------|--------------|-------------|
| GET /api/creditcard | Todos os cartões | Apenas seus cartões |
| GET /api/expense | Todas as despesas | Apenas suas despesas |
| GET /api/user | Todos os usuários | Apenas ele mesmo |
| POST /api/creditcard | Cria para qualquer user | Cria apenas para si |
| DELETE /api/creditcard/:id | Deleta qualquer | Deleta apenas seus |

---

## ? **Checklist de Implementação**

```
[ ] 1. Executar LinkDataToUser2004.sql
[ ] 2. Executar StoredProceduresWithAdmin.sql
[ ] 3. Verificar Users no banco (2002 deve ser Admin)
[ ] 4. Testar SPs no SSMS
    [ ] Admin vê tudo
    [ ] User vê apenas seus dados
[ ] 5. Reiniciar API (dotnet run)
[ ] 6. Testar login como Admin
[ ] 7. Testar GET /api/creditcard como Admin (deve retornar tudo)
[ ] 8. Testar login como User
[ ] 9. Testar GET /api/creditcard como User (deve retornar só dele)
```

---

## ?? **Troubleshooting**

### **Admin não vê todos os dados**

? **Solução:**
1. Verifique se o Role está como 'Admin':
```sql
SELECT Id, Name, Email, Role FROM Users WHERE Id = 2002;
```
2. Se não estiver, atualize:
```sql
UPDATE Users SET Role = 'Admin' WHERE Id = 2002;
```

---

### **Usuário comum vê dados de outros**

? **Solução:**
1. Verifique se as SPs foram atualizadas:
```sql
SELECT OBJECT_DEFINITION(OBJECT_ID('sp_GetCreditCardsByUserId'));
```
2. Deve conter `dbo.fn_IsAdmin`
3. Se não, execute novamente `StoredProceduresWithAdmin.sql`

---

### **Erro "fn_IsAdmin não existe"**

? **Solução:**
```sql
-- Verificar se existe
SELECT * FROM sys.objects WHERE name = 'fn_IsAdmin';

-- Se não existir, execute:
-- StoredProceduresWithAdmin.sql
```

---

## ?? **Resumo Final**

**O que você precisa fazer:**

1. ? **Executar 2 scripts SQL** (LinkDataToUser2004.sql + StoredProceduresWithAdmin.sql)
2. ? **Reiniciar a API**
3. ? **Testar**

**O que NÃO precisa fazer:**

- ? Alterar código C#
- ? Alterar controllers
- ? Alterar services
- ? Alterar repositories

**Tudo funciona automaticamente via Stored Procedures!** ??

---

## ?? **Arquivos Criados**

```
WebApplicationAPI\SQL\
   ??? LinkDataToUser2004.sql              ? Execute primeiro
   ??? StoredProceduresWithAdmin.sql       ? Execute segundo
   ??? AdminImplementationGuide.md         ?? Este arquivo
```

---

**?? Pronto! Agora você tem um sistema completo com Admin que vê tudo!**

**?? Execute os scripts SQL e teste!**
