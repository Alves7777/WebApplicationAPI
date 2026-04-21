using System.Net.Http.Json;
using FinancialControlUI.Models;

namespace FinancialControlUI.Services;

public class CreditCardService
{
    private readonly HttpClient _http;

    public CreditCardService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CreditCard>> GetAllCardsAsync()
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<List<CreditCard>>>("creditcard");
        return response?.Data ?? new List<CreditCard>();
    }

    public async Task<CreditCard?> GetCardByIdAsync(int id)
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<CreditCard>>($"creditcard/{id}");
        return response?.Data;
    }

    public async Task<CreditCard?> CreateCardAsync(CreateCreditCardRequest request)
    {
        var response = await _http.PostAsJsonAsync("creditcard", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CreditCard>>();
        return result?.Data;
    }

    public async Task<CreditCard?> UpdateCardAsync(int id, UpdateCreditCardRequest request)
    {
        var response = await _http.PutAsJsonAsync($"creditcard/{id}", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CreditCard>>();
        return result?.Data;
    }

    public async Task<bool> DeleteCardAsync(int id)
    {
        var response = await _http.DeleteAsync($"creditcard/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<SimulatePurchaseResponse?> SimulatePurchaseAsync(int cardId, SimulatePurchaseRequest request)
    {
        var response = await _http.PostAsJsonAsync($"creditcard/{cardId}/simulate-purchase", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<SimulatePurchaseResponse>>();
        return result?.Data;
    }

    public async Task<bool> ConfirmPurchaseAsync(int cardId, ConfirmPurchaseRequest request)
    {
        var response = await _http.PostAsJsonAsync($"creditcard/{cardId}/confirm-purchase", request);
        return response.IsSuccessStatusCode;
    }

    // Gerenciamento de despesas do cartão
    public async Task<List<CreditCardExpense>> GetExpensesByCardAsync(int cardId, int? month = null, int? year = null, string? category = null)
    {
        var queryParams = new List<string>();
        if (month.HasValue) queryParams.Add($"month={month}");
        if (year.HasValue) queryParams.Add($"year={year}");
        if (!string.IsNullOrEmpty(category)) queryParams.Add($"category={category}");

        var url = $"creditcard/{cardId}/expenses" + (queryParams.Any() ? "?" + string.Join("&", queryParams) : "");
        var response = await _http.GetFromJsonAsync<ApiResponse<List<CreditCardExpense>>>(url);
        return response?.Data ?? new List<CreditCardExpense>();
    }

    public async Task<CreditCardExpense?> GetExpenseByIdAsync(int expenseId)
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<CreditCardExpense>>($"creditcard/expenses/{expenseId}");
        return response?.Data;
    }

    public async Task<CreditCardExpense?> CreateExpenseAsync(int cardId, CreateCreditCardExpenseRequest request)
    {
        var response = await _http.PostAsJsonAsync($"creditcard/{cardId}/expenses", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CreditCardExpense>>();
        return result?.Data;
    }

    public async Task<CreditCardExpense?> UpdateExpenseAsync(int expenseId, UpdateCreditCardExpenseRequest request)
    {
        var response = await _http.PutAsJsonAsync($"creditcard/expenses/{expenseId}", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CreditCardExpense>>();
        return result?.Data;
    }

    public async Task<bool> DeleteExpenseAsync(int expenseId)
    {
        var response = await _http.DeleteAsync($"creditcard/expenses/{expenseId}");
        return response.IsSuccessStatusCode;
    }

    // Import CSV para cartão
    public async Task<CsvImportResult?> ImportCsvAsync(int cardId, Stream fileStream, string fileName)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
        content.Add(fileContent, "file", fileName);

        var response = await _http.PostAsync($"creditcard/{cardId}/import-csv", content);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao importar CSV: {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CsvImportResult>>();
        return result?.Data;
    }

    // Extrato do cartão
    public async Task<CreditCardStatement?> GetStatementAsync(int cardId, int month, int year)
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<CreditCardStatement>>($"creditcard/{cardId}/statement?month={month}&year={year}");
        return response?.Data;
    }

    public async Task<CreditCardStatement?> GetStatementByPeriodAsync(int cardId, DateTime startDate, DateTime endDate)
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<CreditCardStatement>>($"creditcard/{cardId}/statement-period?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
        return response?.Data;
    }

    // Análise por categoria
    public async Task<List<CategoryAnalysis>> GetCategoryAnalysisAsync(int cardId, int month, int year)
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<List<CategoryAnalysis>>>($"creditcard/{cardId}/by-category?month={month}&year={year}");
        return response?.Data ?? new List<CategoryAnalysis>();
    }

    // Compras parceladas
    public async Task<List<InstallmentPurchase>> GetInstallmentPurchasesAsync(int cardId)
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<List<InstallmentPurchase>>>($"creditcard/{cardId}/installment-purchases");
        return response?.Data ?? new List<InstallmentPurchase>();
    }
}

public class MonthlyFinancialService
{
    private readonly HttpClient _http;

    public MonthlyFinancialService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<MonthlyFinancial>> GetAllAsync(int? year = null, int? month = null)
    {
        var url = "v1/monthly-financial";
        if (year.HasValue && month.HasValue)
        {
            url += $"?year={year}&month={month}";
        }
        else if (year.HasValue)
        {
            url += $"?year={year}";
        }

        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return new List<MonthlyFinancial>();
        }

        var json = await response.Content.ReadAsStringAsync();
        var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Tenta como lista
        try
        {
            var listResult = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<List<MonthlyFinancial>>>(json, options);
            if (listResult?.Data != null)
                return listResult.Data;
        }
        catch { }

        // Tenta como objeto único
        try
        {
            var singleResult = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<MonthlyFinancial>>(json, options);
            if (singleResult?.Data != null)
                return new List<MonthlyFinancial> { singleResult.Data };
        }
        catch { }

        return new List<MonthlyFinancial>();
    }

    public async Task<MonthlyFinancial?> GetCurrentMonthAsync()
    {
        var month = DateTime.Now.Month;
        var year = DateTime.Now.Year;

        Console.WriteLine($"=== GetCurrentMonthAsync ===");
        Console.WriteLine($"Buscando: {month}/{year}");

        // Log do token
        if (_http.DefaultRequestHeaders.Authorization != null)
        {
            var token = _http.DefaultRequestHeaders.Authorization.Parameter;
            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"Token: {token.Substring(0, Math.Min(20, token.Length))}...");
            }
        }
        else
        {
            Console.WriteLine("?? NENHUM TOKEN NO HTTPCLIENT!");
        }

        var url = $"v1/monthly-financial?year={year}&month={month}";
        Console.WriteLine($"URL: {url}");

        try
        {
            var response = await _http.GetFromJsonAsync<ApiResponse<MonthlyFinancial>>(url);
            Console.WriteLine($"Resposta recebida: {response?.Status}");
            return response?.Data;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
            throw;
        }
    }

    public async Task<MonthlyFinancial?> CreateAsync(CreateMonthlyFinancialRequest request)
    {
        var response = await _http.PostAsJsonAsync("v1/monthly-financial", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<MonthlyFinancial>>();
        return result?.Data;
    }

    public async Task<MonthlyFinancial?> UpdateAsync(int id, UpdateMonthlyFinancialRequest request)
    {
        var response = await _http.PutAsJsonAsync($"v1/monthly-financial/{id}", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<MonthlyFinancial>>();
        return result?.Data;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"v1/monthly-financial/{id}");
        return response.IsSuccessStatusCode;
    }
}

public class CategoryService
{
    private readonly HttpClient _http;

    public CategoryService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<List<Category>>>("Category");
        return response?.Data ?? new List<Category>();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        var response = await _http.GetFromJsonAsync<ApiResponse<Category>>($"Category/{id}");
        return response?.Data;
    }

    public async Task<Category?> CreateAsync(CreateCategoryRequest request)
    {
        var response = await _http.PostAsJsonAsync("Category", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<Category>>();
        return result?.Data;
    }
}

public class ExpenseService
{
    private readonly HttpClient _http;

    public ExpenseService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Expense>> GetAllAsync(int? month = null, int? year = null, string? category = null, string? status = null, string? paymentMethod = null)
    {
        var queryParams = new List<string>();
        if (month.HasValue) queryParams.Add($"month={month}");
        if (year.HasValue) queryParams.Add($"year={year}");
        if (!string.IsNullOrEmpty(category)) queryParams.Add($"category={category}");
        if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");
        if (!string.IsNullOrEmpty(paymentMethod)) queryParams.Add($"paymentMethod={paymentMethod}");

        var url = "Expense" + (queryParams.Any() ? "?" + string.Join("&", queryParams) : "");
        var response = await _http.GetFromJsonAsync<ApiResponse<List<Expense>>>(url);
        return response?.Data ?? new List<Expense>();
    }

    public async Task<Expense?> CreateAsync(CreateExpenseRequest request)
    {
        var response = await _http.PostAsJsonAsync("Expense", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<Expense>>();
        return result?.Data;
    }

    public async Task<Expense?> UpdateAsync(int id, UpdateExpenseRequest request)
    {
        var response = await _http.PutAsJsonAsync($"Expense/{id}", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<Expense>>();
        return result?.Data;
    }

    public async Task<Expense?> PatchAsync(int id, PatchExpenseRequest request)
    {
        var response = await _http.PatchAsJsonAsync($"Expense/{id}", request);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<Expense>>();
        return result?.Data;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"Expense/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<ImportCsvResponse?> ImportCsvAsync(Stream fileStream, string fileName)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
        content.Add(fileContent, "file", fileName);

        var response = await _http.PostAsync("Expense/import-csv", content);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao importar CSV: {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ImportCsvResponse>>();
        return result?.Data;
    }
}

public class UserService
{
    private readonly HttpClient _http;

    public UserService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<User>> GetAllAsync()
    {
        var response = await _http.GetFromJsonAsync<List<User>>("User");
        return response ?? new List<User>();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        var response = await _http.GetFromJsonAsync<User>($"User/{id}");
        return response;
    }

    public async Task<User?> CreateAsync(CreateUserRequest request)
    {
        var response = await _http.PostAsJsonAsync("User", request);
        return await response.Content.ReadFromJsonAsync<User>();
    }

    public async Task<User?> UpdateAsync(int id, UpdateUserRequest request)
    {
        var response = await _http.PutAsJsonAsync($"User/{id}", request);
        return await response.Content.ReadFromJsonAsync<User>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"User/{id}");
        return response.IsSuccessStatusCode;
    }
}

public class SummaryService
{
    private readonly HttpClient _http;
    private readonly MonthlyFinancialService _monthlyService;

    public SummaryService(HttpClient http, MonthlyFinancialService monthlyService)
    {
        _http = http;
        _monthlyService = monthlyService;
    }

    public async Task<FinancialSummary?> GetFinancialSummaryAsync()
    {
        try
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            Console.WriteLine($"Buscando Summary para {currentMonth}/{currentYear}");

            // Busca o controle mensal atual para pegar salário e reserva
            MonthlyFinancial? monthlyControl = null;
            decimal salary = 0;
            decimal reserve = 0;

            try
            {
                monthlyControl = await _monthlyService.GetCurrentMonthAsync();
                if (monthlyControl != null)
                {
                    salary = monthlyControl.SalaryTotal;
                    reserve = monthlyControl.Reserve;
                    Console.WriteLine($"Controle mensal encontrado - Salário: {salary}, Reserva: {reserve}");
                }
                else
                {
                    Console.WriteLine("Nenhum controle mensal encontrado para o mês atual");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar controle mensal: {ex.Message}");
            }

            // Monta a URL com os parâmetros
            var url = $"Summary?month={currentMonth}&year={currentYear}&salary={salary}&reserve={reserve}";
            Console.WriteLine($"Chamando API: {url}");

            // Tenta buscar com wrapper ApiResponse
            try
            {
                var responseWithWrapper = await _http.GetFromJsonAsync<ApiResponse<FinancialSummary>>(url);
                if (responseWithWrapper?.Data != null)
                {
                    Console.WriteLine($"Response com wrapper recebido");
                    Console.WriteLine($"TotalExpenses: {responseWithWrapper.Data.TotalExpenses}");
                    Console.WriteLine($"Balance: {responseWithWrapper.Data.Balance}");
                    Console.WriteLine($"AvailableToSpend: {responseWithWrapper.Data.AvailableToSpend}");
                    return responseWithWrapper.Data;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Tentativa com wrapper falhou: {ex.Message}");
            }

            // Tenta buscar sem wrapper
            var response = await _http.GetFromJsonAsync<FinancialSummary>(url);
            Console.WriteLine($"Response direto recebido: {response != null}");

            if (response != null)
            {
                Console.WriteLine($"TotalExpenses: {response.TotalExpenses}");
                Console.WriteLine($"Balance: {response.Balance}");
                Console.WriteLine($"AvailableToSpend: {response.AvailableToSpend}");
            }

            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar Summary: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");

            // Retorna um summary vazio em vez de null
            return new FinancialSummary
            {
                TotalExpenses = 0,
                Balance = 0,
                AvailableToSpend = 0,
                TotalByCategory = new Dictionary<string, decimal>(),
                TotalByStatus = new Dictionary<string, decimal>()
            };
        }
    }
}
