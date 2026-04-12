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

    public async Task<bool> ConfirmPurchaseAsync(int cardId, SimulatePurchaseRequest request)
    {
        var response = await _http.PostAsJsonAsync($"creditcard/{cardId}/confirm-purchase", request);
        return response.IsSuccessStatusCode;
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

        var response = await _http.GetFromJsonAsync<ApiResponse<MonthlyFinancial>>($"v1/monthly-financial?year={year}&month={month}");
        return response?.Data;
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

    public SummaryService(HttpClient http)
    {
        _http = http;
    }

    public async Task<FinancialSummary?> GetFinancialSummaryAsync()
    {
        var response = await _http.GetFromJsonAsync<FinancialSummary>("Summary");
        return response;
    }
}
