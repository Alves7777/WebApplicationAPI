# ?? Exemplos Práticos de Uso da Autenticação

## ?? Índice
1. [Testando com cURL](#testando-com-curl)
2. [Testando com Postman](#testando-com-postman)
3. [Integrando no Frontend (Blazor)](#integrando-no-frontend-blazor)
4. [Como Proteger Repositories](#como-proteger-repositories)

---

## 1?? Testando com cURL

### **Registrar Usuário**
```bash
curl -X POST http://localhost:5296/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João Silva",
    "email": "joao@example.com",
    "password": "senha123"
  }'
```

### **Login**
```bash
curl -X POST http://localhost:5296/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao@example.com",
    "password": "senha123"
  }'
```

### **Criar Cartão (com autenticação)**
```bash
curl -X POST http://localhost:5296/api/creditcard \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -d '{
    "name": "Nubank",
    "brand": "Mastercard",
    "cardLimit": 5000.00,
    "closingDay": 10,
    "dueDay": 18
  }'
```

### **Listar Cartões do Usuário Logado**
```bash
curl -X GET http://localhost:5296/api/creditcard \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

---

## 2?? Testando com Postman

### **Configurar Headers Globais**

1. Abra o Postman
2. Clique em **Collections** ? **New Collection**
3. Nomeie como "FinancialControl API"
4. Clique nos `...` ? **Edit**
5. Vá em **Authorization**
6. Selecione **Type: Bearer Token**
7. Cole o token no campo **Token**

Agora todas as requisições dessa collection terão o token automaticamente!

### **Criar Environment (Opcional)**

1. Clique no ícone de **Environments** (??)
2. Crie um novo environment "Local"
3. Adicione variáveis:
   - `baseUrl`: `http://localhost:5296`
   - `token`: (deixe vazio por enquanto)

4. Use nas requisições:
   ```
   {{baseUrl}}/api/auth/login
   {{baseUrl}}/api/creditcard
   ```

5. Após o login, vá em **Tests** e adicione:
   ```javascript
   if (pm.response.code === 200) {
       var jsonData = pm.response.json();
       pm.environment.set("token", jsonData.token);
   }
   ```

Agora o token será salvo automaticamente após o login!

---

## 3?? Integrando no Frontend (Blazor)

### **3.1. Criar AuthService.cs**

```csharp
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", new 
        { 
            Email = email, 
            Password = password 
        });

        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

            // Salvar token no localStorage
            await _localStorage.SetItemAsync("authToken", authResponse.Token);
            await _localStorage.SetItemAsync("userName", authResponse.Name);
            await _localStorage.SetItemAsync("userId", authResponse.UserId);

            // Configurar header Authorization para todas as próximas requisições
            _http.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", authResponse.Token);

            return authResponse;
        }

        throw new Exception("Login falhou");
    }

    public async Task<AuthResponse> RegisterAsync(string name, string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", new 
        { 
            Name = name,
            Email = email, 
            Password = password 
        });

        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

            await _localStorage.SetItemAsync("authToken", authResponse.Token);
            await _localStorage.SetItemAsync("userName", authResponse.Name);
            await _localStorage.SetItemAsync("userId", authResponse.UserId);

            _http.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", authResponse.Token);

            return authResponse;
        }

        throw new Exception("Registro falhou");
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("userName");
        await _localStorage.RemoveItemAsync("userId");

        _http.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        return !string.IsNullOrEmpty(token);
    }

    public async Task InitializeAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (!string.IsNullOrEmpty(token))
        {
            _http.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}

public class AuthResponse
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public string Role { get; set; }
}
```

### **3.2. Registrar no Program.cs (Blazor)**

```csharp
using Blazored.LocalStorage;

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthService>();

// Configurar HttpClient base
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5296") 
});
```

### **3.3. Criar Página de Login**

```razor
@page "/login"
@inject AuthService AuthService
@inject NavigationManager Navigation

<h3>Login</h3>

<EditForm Model="@loginModel" OnValidSubmit="@HandleLogin">
    <div class="mb-3">
        <label>Email</label>
        <InputText @bind-Value="loginModel.Email" class="form-control" />
    </div>

    <div class="mb-3">
        <label>Senha</label>
        <InputText @bind-Value="loginModel.Password" type="password" class="form-control" />
    </div>

    @if (!string.IsNullOrEmpty(errorMessage))
    {
        <div class="alert alert-danger">@errorMessage</div>
    }

    <button type="submit" class="btn btn-primary">Entrar</button>
</EditForm>

@code {
    private LoginModel loginModel = new();
    private string errorMessage = "";

    private async Task HandleLogin()
    {
        try
        {
            var response = await AuthService.LoginAsync(loginModel.Email, loginModel.Password);
            Navigation.NavigateTo("/");
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
```

### **3.4. Inicializar Auth no App.razor**

```razor
@inject AuthService AuthService

<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
</Router>

@code {
    protected override async Task OnInitializedAsync()
    {
        await AuthService.InitializeAsync();
    }
}
```

---

## 4?? Como Proteger Repositories

### **Exemplo: ExpenseRepository com UserId**

```csharp
public class ExpenseRepository : IExpenseRepository
{
    private readonly string _connectionString;

    public ExpenseRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // Buscar despesas do usuário por período
    public async Task<IEnumerable<Expense>> GetByPeriodAsync(int userId, int month, int year)
    {
        using var connection = new SqlConnection(_connectionString);

        return await connection.QueryAsync<Expense>(
            "sp_GetExpensesByPeriod",
            new { UserId = userId, Month = month, Year = year },
            commandType: CommandType.StoredProcedure
        );
    }

    // Criar despesa vinculada ao usuário
    public async Task<int> CreateAsync(int userId, Expense expense)
    {
        using var connection = new SqlConnection(_connectionString);

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);
        parameters.Add("@Month", expense.Month);
        parameters.Add("@Year", expense.Year);
        parameters.Add("@Description", expense.Description);
        parameters.Add("@Amount", expense.Amount);
        parameters.Add("@Category", expense.Category);
        parameters.Add("@Status", expense.Status);
        parameters.Add("@PaymentMethod", expense.PaymentMethod);
        parameters.Add("@CreatedAt", expense.CreatedAt);
        parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await connection.ExecuteAsync(
            "sp_CreateExpense",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return parameters.Get<int>("@Id");
    }

    // Deletar apenas se pertencer ao usuário
    public async Task<bool> DeleteAsync(int userId, int id)
    {
        using var connection = new SqlConnection(_connectionString);

        var rowsAffected = await connection.ExecuteScalarAsync<int>(
            "sp_DeleteExpense",
            new { Id = id, UserId = userId },
            commandType: CommandType.StoredProcedure
        );

        return rowsAffected > 0;
    }
}
```

### **Exemplo: ExpenseService com UserId**

```csharp
public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _repository;

    public ExpenseService(IExpenseRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ExpenseResponse>> GetByPeriodAsync(int userId, int month, int year)
    {
        var expenses = await _repository.GetByPeriodAsync(userId, month, year);

        return expenses.Select(e => new ExpenseResponse
        {
            Id = e.Id,
            Month = e.Month,
            Year = e.Year,
            Description = e.Description,
            Amount = e.Amount,
            Category = e.Category,
            Status = e.Status,
            PaymentMethod = e.PaymentMethod,
            CreatedAt = e.CreatedAt
        });
    }

    public async Task<int> CreateAsync(int userId, CreateExpenseRequest request)
    {
        var expense = new Expense
        {
            Month = request.Month,
            Year = request.Year,
            Description = request.Description,
            Amount = request.Amount,
            Category = request.Category,
            Status = request.Status,
            PaymentMethod = request.PaymentMethod,
            CreatedAt = DateTime.UtcNow
        };

        return await _repository.CreateAsync(userId, expense);
    }
}
```

### **Exemplo: ExpenseController Protegido**

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI.Extensions;

[ApiController]
[Route("api/expense")]
[Authorize]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _service;

    public ExpenseController(IExpenseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetByPeriod([FromQuery] int month, [FromQuery] int year)
    {
        var userId = this.GetUserId(); // Pega do token JWT
        var expenses = await _service.GetByPeriodAsync(userId, month, year);
        return Ok(expenses);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExpenseRequest request)
    {
        var userId = this.GetUserId();
        var id = await _service.CreateAsync(userId, request);
        return Created($"/api/expense/{id}", new { id });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = this.GetUserId();
        var result = await _service.DeleteAsync(userId, id);

        if (!result)
            return NotFound("Despesa não encontrada ou não pertence ao usuário");

        return NoContent();
    }
}
```

---

## ?? Checklist de Implementação por Entidade

Para cada entidade do sistema, siga este checklist:

### ? **Model**
- [ ] Adicionar propriedade `public int UserId { get; set; }`

### ? **Repository**
- [ ] Adicionar parâmetro `int userId` em todos os métodos
- [ ] Atualizar chamadas às SPs para incluir `@UserId`
- [ ] Garantir que `WHERE UserId = @UserId` em todas as queries

### ? **Service**
- [ ] Adicionar parâmetro `int userId` em todos os métodos
- [ ] Passar `userId` para o repository

### ? **Controller**
- [ ] Adicionar `[Authorize]` no controller ou métodos
- [ ] Usar `this.GetUserId()` para pegar o ID do usuário logado
- [ ] Passar `userId` para o service

### ? **Stored Procedures**
- [ ] Adicionar parâmetro `@UserId INT` em todas as SPs
- [ ] Adicionar `WHERE UserId = @UserId` em SELECT, UPDATE, DELETE
- [ ] Adicionar coluna `UserId` em INSERT

---

## ?? Boas Práticas de Segurança

1. **Nunca confie no ID enviado pelo cliente**
   ```csharp
   // ? ERRADO
   public async Task<IActionResult> Delete(int userId, int id)
   {
       await _service.DeleteAsync(userId, id);
   }

   // ? CORRETO
   public async Task<IActionResult> Delete(int id)
   {
       var userId = this.GetUserId(); // Do token
       await _service.DeleteAsync(userId, id);
   }
   ```

2. **Sempre valide que o recurso pertence ao usuário**
   ```csharp
   var expense = await _repository.GetByIdAsync(id);
   if (expense.UserId != userId)
       return Forbid();
   ```

3. **Use HTTPS em produção**
   ```csharp
   app.UseHttpsRedirection();
   ```

4. **Não retorne mensagens de erro detalhadas em produção**
   ```csharp
   catch (Exception ex)
   {
       // ? ERRADO em produção
       return StatusCode(500, ex.Message);

       // ? CORRETO
       _logger.LogError(ex, "Erro ao criar despesa");
       return StatusCode(500, "Erro interno do servidor");
   }
   ```

5. **Rotacione a chave JWT regularmente**
   - Use Azure Key Vault ou variáveis de ambiente
   - Nunca commite a chave no Git

---

**?? Pronto! Agora você tem um guia completo de como usar a autenticação JWT no seu sistema!**
