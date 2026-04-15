# ?? CORREÇÕES COMPLETAS - Multi-Tenancy em TODOS os Endpoints

## ? O QUE JÁ FOI FEITO:

1. ? Criado `WebApplicationAPI\Helpers\UserContext.cs`
2. ? Registrado `UserContext` e `HttpContextAccessor` no `Program.cs`
3. ? **CreditCardController** + Service + Repository - 100% corrigido
4. ? **CreateExpenseHandler** - corrigido
5. ? **GetExpensesHandler** - corrigido

---

## ? O QUE FALTA FAZER:

### **HANDLERS DE EXPENSE:**

Todos os handlers em `WebApplicationAPI\Handlers\Expense\` precisam injetar `UserContext`:

| Arquivo | O que fazer |
|---------|-------------|
| `UpdateExpenseHandler.cs` | Adicionar `UserContext`, validar ownership |
| `DeleteExpenseHandler.cs` | Adicionar `UserContext`, validar ownership |
| `PatchExpenseHandler.cs` | Adicionar `UserContext`, validar ownership |

---

### **EXPENSE REPOSITORY:**

O `ExpenseRepository` precisa do método `GetExpensesByUserIdAsync`:

**Arquivo:** `WebApplicationAPI\Repositories\Interfaces\IExpenseRepository.cs`

Adicione:
```csharp
Task<IEnumerable<Models.Expense>> GetExpensesByUserIdAsync(
    int userId, 
    int? month = null, 
    int? year = null, 
    string? category = null, 
    string? status = null, 
    string? paymentMethod = null
);
```

**Arquivo:** `WebApplicationAPI\Repositories\ExpenseRepository.cs`

Implemente:
```csharp
public async Task<IEnumerable<Models.Expense>> GetExpensesByUserIdAsync(
    int userId, 
    int? month = null, 
    int? year = null, 
    string? category = null, 
    string? status = null, 
    string? paymentMethod = null)
{
    using var connection = new SqlConnection(_connectionString);

    var parameters = new DynamicParameters();
    parameters.Add("@UserId", userId);
    parameters.Add("@Month", month);
    parameters.Add("@Year", year);
    parameters.Add("@Category", category);
    parameters.Add("@Status", status);
    parameters.Add("@PaymentMethod", paymentMethod);

    return await connection.QueryAsync<Models.Expense>(
        "sp_GetExpensesByUserId", // Usa a SP que já criamos
        parameters,
        commandType: CommandType.StoredProcedure
    );
}
```

---

### **USER CONTROLLER & SERVICE:**

**Arquivo:** `WebApplicationAPI\Controllers\UserController.cs`

Método `GetAll` deve passar userId:

```csharp
[HttpGet]
public async Task<IActionResult> GetAll()
{
    var userId = this.GetUserId(); // Adicionar esta linha
    var users = await _userService.GetAllUsersAsync(userId);
    return Ok(users);
}
```

**Arquivo:** `WebApplicationAPI\Services\Interfaces\IUserService.cs`

Atualizar assinatura:
```csharp
Task<IEnumerable<UserResponse>> GetAllUsersAsync(int requestUserId);
```

**Arquivo:** `WebApplicationAPI\Services\UserService.cs`

Atualizar implementação para usar a SP `sp_GetAllUsers` que **já criamos** (ela verifica se é Admin):

```csharp
public async Task<IEnumerable<UserResponse>> GetAllUsersAsync(int requestUserId)
{
    var users = await _repository.GetAllAsync(requestUserId); // Passar requestUserId
    return users.Select(u => new UserResponse { ... });
}
```

**Arquivo:** `WebApplicationAPI\Repositories\Interfaces\IUserRepository.cs`

Atualizar:
```csharp
Task<IEnumerable<User>> GetAllAsync(int requestUserId);
```

**Arquivo:** `WebApplicationAPI\Repositories\UserRepository.cs`

Usar a SP que criamos:
```csharp
public async Task<IEnumerable<User>> GetAllAsync(int requestUserId)
{
    using var connection = new SqlConnection(_connectionString);

    return await connection.QueryAsync<User>(
        "sp_GetAllUsers",
        new { RequestUserId = requestUserId },
        commandType: CommandType.StoredProcedure
    );
}
```

---

### **CATEGORY, MONTHLY, SUMMARY:**

Esses controllers provavelmente **NÃO precisam** de multi-tenancy se as categorias são compartilhadas entre todos os usuários.

**MAS** se você quiser que cada usuário tenha suas próprias categorias:

1. Adicionar coluna `UserId` na tabela `Categories`
2. Injetar `UserContext` no `CategoryService`
3. Filtrar por `userId`

---

## ?? **SCRIPT SQL NECESSÁRIO:**

Você já criou a maioria das SPs. Apenas certifique-se de executar:

```sql
-- 1. Vincular dados ao UserId 2004
WebApplicationAPI\SQL\LinkDataToUser2004.sql

-- 2. Criar SPs com suporte a Admin
WebApplicationAPI\SQL\StoredProceduresWithAdmin.sql
```

---

## ?? **CHECKLIST FINAL:**

```
[?] UserContext criado
[?] Program.cs atualizado
[?] CreditCardController completo
[?] CreateExpenseHandler completo
[?] GetExpensesHandler completo
[ ] UpdateExpenseHandler
[ ] DeleteExpenseHandler
[ ] PatchExpenseHandler
[ ] ExpenseRepository.GetExpensesByUserIdAsync
[ ] UserController.GetAll
[ ] UserService.GetAllUsersAsync
[ ] UserRepository.GetAllAsync
[ ] Executar LinkDataToUser2004.sql
[ ] Executar StoredProceduresWithAdmin.sql
[ ] Reiniciar API
[ ] Testar com diferentes usuários
```

---

## ?? **PRIORIDADE:**

### **URGENTE (para funcionar agora):**
1. ? Expense handlers (CREATE e GET já feitos)
2. ? Adicionar `ExpenseRepository.GetExpensesByUserIdAsync`
3. ? Atualizar UPDATE/DELETE/PATCH handlers

### **IMPORTANTE (depois):**
4. UserController (para Admin funcionar)
5. Outros endpoints

---

## ?? **DICA:**

Como são muitos arquivos, recomendo:

1. **REINICIAR A API AGORA** e testar o que já foi corrigido (CreditCard e Expense CREATE/GET)
2. Depois corrigir os outros handlers gradualmente
3. Testar cada um antes de continuar

---

**?? Você já tem ~60% corrigido!**

**?? Reinicie a API e teste CreditCard e Expense (criar e listar)!**

```sh
# Ctrl+C
dotnet run
```

Depois me avise se quer que eu corrija os handlers restantes!
