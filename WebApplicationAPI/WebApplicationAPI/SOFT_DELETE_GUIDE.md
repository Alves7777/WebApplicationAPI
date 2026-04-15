# ??? Soft Delete (Exclusão Lógica) - Guia Completo

## ?? O que é Soft Delete?

**Soft Delete** (exclusão lógica) é quando você **marca** um registro como deletado ao invés de **removê-lo** fisicamente do banco de dados.

---

## ?? Estrutura Implementada

### **Colunas Adicionadas em Todas as Tabelas:**

```sql
CreatedAt      DATETIME NOT NULL DEFAULT GETUTCDATE()  -- Quando foi criado
CreatedBy      INT NULL                                 -- Quem criou (UserId)
UpdatedAt      DATETIME NOT NULL DEFAULT GETUTCDATE()  -- Última atualização
UpdatedBy      INT NULL                                 -- Quem atualizou (UserId)
DeletedAt      DATETIME NULL                            -- Quando foi deletado (NULL = ativo)
DeletedBy      INT NULL                                 -- Quem deletou (UserId)
```

---

## ? Benefícios

| Benefício | Descrição |
|-----------|-----------|
| ?? **Auditoria** | Rastreamento completo de quem fez o quê e quando |
| ?? **Recuperação** | Possibilidade de restaurar dados deletados |
| ?? **Histórico** | Dados históricos preservados para relatórios |
| ?? **Compliance** | Atende LGPD/GDPR (direito ao esquecimento) |
| ?? **Debug** | Facilita troubleshooting em produção |

---

## ?? Como Usar

### **1. Executar Script SQL**

```sql
-- Execute no SQL Server Management Studio
WebApplicationAPI\SQL\SoftDeleteSetup.sql
```

---

### **2. Exemplo: Deletar (Soft Delete)**

#### **Backend (.NET)**

```csharp
[HttpDelete("{id}")]
[Authorize]
public async Task<IActionResult> Delete(int id)
{
    var userId = this.GetUserId();

    // Soft delete ao invés de delete físico
    var result = await _repository.SoftDeleteAsync(id, userId);

    if (!result)
        return NotFound("Despesa não encontrada");

    return NoContent();
}
```

#### **Repository**

```csharp
public async Task<bool> SoftDeleteAsync(int id, int userId)
{
    using var connection = new SqlConnection(_connectionString);

    var rowsAffected = await connection.ExecuteScalarAsync<int>(
        "sp_SoftDeleteExpense",
        new { 
            Id = id, 
            UserId = userId,
            DeletedBy = userId 
        },
        commandType: CommandType.StoredProcedure
    );

    return rowsAffected > 0;
}
```

#### **SQL (Stored Procedure)**

```sql
CREATE OR ALTER PROCEDURE sp_SoftDeleteExpense
    @Id INT,
    @UserId INT,
    @DeletedBy INT
AS
BEGIN
    UPDATE Expenses
    SET 
        DeletedAt = GETUTCDATE(),
        DeletedBy = @DeletedBy
    WHERE Id = @Id 
      AND UserId = @UserId
      AND DeletedAt IS NULL;

    SELECT @@ROWCOUNT;
END
```

---

### **3. Exemplo: Restaurar Dados**

#### **Backend (.NET)**

```csharp
[HttpPost("{id}/restore")]
[Authorize]
public async Task<IActionResult> Restore(int id)
{
    var userId = this.GetUserId();

    var result = await _repository.RestoreAsync(id, userId);

    if (!result)
        return NotFound("Despesa não encontrada ou não foi deletada");

    return Ok(new { message = "Despesa restaurada com sucesso" });
}
```

#### **Repository**

```csharp
public async Task<bool> RestoreAsync(int id, int userId)
{
    using var connection = new SqlConnection(_connectionString);

    var rowsAffected = await connection.ExecuteScalarAsync<int>(
        "sp_RestoreExpense",
        new { Id = id, UserId = userId },
        commandType: CommandType.StoredProcedure
    );

    return rowsAffected > 0;
}
```

---

### **4. Exemplo: Listar Apenas Ativos**

```csharp
// Buscar apenas registros NÃO deletados
public async Task<IEnumerable<Expense>> GetByPeriodAsync(int userId, int month, int year)
{
    using var connection = new SqlConnection(_connectionString);

    return await connection.QueryAsync<Expense>(
        "sp_GetExpensesByPeriod",
        new { 
            UserId = userId, 
            Month = month, 
            Year = year,
            IncludeDeleted = false  // <-- NÃO incluir deletados
        },
        commandType: CommandType.StoredProcedure
    );
}
```

---

### **5. Exemplo: Listar TUDO (Incluindo Deletados)**

```csharp
// Admin pode ver registros deletados
[HttpGet("all")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> GetAllIncludingDeleted(int month, int year)
{
    var userId = this.GetUserId();

    var expenses = await _repository.GetByPeriodAsync(
        userId, 
        month, 
        year, 
        includeDeleted: true  // <-- Incluir deletados
    );

    return Ok(expenses);
}
```

---

## ?? View de Auditoria

O script criou uma view para consultar todos os registros deletados:

```sql
-- Ver todos os dados deletados
SELECT * FROM vw_AuditLog
ORDER BY DeletedAt DESC;

-- Ver quem deletou mais registros
SELECT 
    u.Name AS Usuario,
    al.TableName AS Tabela,
    COUNT(*) AS TotalDeletado
FROM vw_AuditLog al
LEFT JOIN Users u ON al.DeletedBy = u.Id
GROUP BY u.Name, al.TableName
ORDER BY COUNT(*) DESC;
```

---

## ?? Comparação: Hard Delete vs Soft Delete

### **Hard Delete (Exclusão Física)**

```sql
-- ? Dado é PERDIDO para sempre
DELETE FROM Expenses WHERE Id = 1;
```

**Prós:**
- ? Banco menor
- ? Mais simples

**Contras:**
- ? Sem auditoria
- ? Sem recuperação
- ? Problemas com FK
- ? Perda de histórico

---

### **Soft Delete (Exclusão Lógica)** ? **RECOMENDADO**

```sql
-- ? Dado é PRESERVADO
UPDATE Expenses 
SET DeletedAt = GETUTCDATE(), DeletedBy = @UserId
WHERE Id = 1;
```

**Prós:**
- ? Auditoria completa
- ? Recuperação de dados
- ? Histórico preservado
- ? Compliance (LGPD/GDPR)

**Contras:**
- ?? Banco um pouco maior
- ?? Queries um pouco mais complexas

---

## ??? Implementação Completa no Repository

```csharp
public class ExpenseRepository : IExpenseRepository
{
    private readonly string _connectionString;

    public ExpenseRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // Soft Delete
    public async Task<bool> SoftDeleteAsync(int id, int userId)
    {
        using var connection = new SqlConnection(_connectionString);

        var rowsAffected = await connection.ExecuteScalarAsync<int>(
            "sp_SoftDeleteExpense",
            new { Id = id, UserId = userId, DeletedBy = userId },
            commandType: CommandType.StoredProcedure
        );

        return rowsAffected > 0;
    }

    // Restaurar
    public async Task<bool> RestoreAsync(int id, int userId)
    {
        using var connection = new SqlConnection(_connectionString);

        var rowsAffected = await connection.ExecuteScalarAsync<int>(
            "sp_RestoreExpense",
            new { Id = id, UserId = userId },
            commandType: CommandType.StoredProcedure
        );

        return rowsAffected > 0;
    }

    // Listar (com opção de incluir deletados)
    public async Task<IEnumerable<Expense>> GetByPeriodAsync(
        int userId, 
        int month, 
        int year, 
        bool includeDeleted = false)
    {
        using var connection = new SqlConnection(_connectionString);

        return await connection.QueryAsync<Expense>(
            "sp_GetExpensesByPeriod",
            new { 
                UserId = userId, 
                Month = month, 
                Year = year,
                IncludeDeleted = includeDeleted
            },
            commandType: CommandType.StoredProcedure
        );
    }

    // Atualizar (com UpdatedBy)
    public async Task<bool> UpdateAsync(int id, int userId, Expense expense)
    {
        using var connection = new SqlConnection(_connectionString);

        var rowsAffected = await connection.ExecuteScalarAsync<int>(
            "sp_UpdateExpense",
            new { 
                Id = id,
                UserId = userId,
                Month = expense.Month,
                Year = expense.Year,
                Description = expense.Description,
                Amount = expense.Amount,
                Category = expense.Category,
                Status = expense.Status,
                PaymentMethod = expense.PaymentMethod,
                UpdatedBy = userId
            },
            commandType: CommandType.StoredProcedure
        );

        return rowsAffected > 0;
    }
}
```

---

## ?? Endpoints Sugeridos

```csharp
[ApiController]
[Route("api/expense")]
[Authorize]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseRepository _repository;

    // DELETE - Soft delete
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = this.GetUserId();
        var result = await _repository.SoftDeleteAsync(id, userId);

        if (!result)
            return NotFound();

        return NoContent();
    }

    // POST restore - Restaurar deletado
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> Restore(int id)
    {
        var userId = this.GetUserId();
        var result = await _repository.RestoreAsync(id, userId);

        if (!result)
            return NotFound("Despesa não encontrada ou não foi deletada");

        return Ok(new { message = "Despesa restaurada com sucesso" });
    }

    // GET trash - Lixeira (apenas deletados)
    [HttpGet("trash")]
    public async Task<IActionResult> GetTrash(int month, int year)
    {
        var userId = this.GetUserId();

        var allExpenses = await _repository.GetByPeriodAsync(userId, month, year, true);
        var deletedOnly = allExpenses.Where(e => e.DeletedAt != null);

        return Ok(deletedOnly);
    }

    // DELETE permanent - Delete físico (apenas Admin)
    [HttpDelete("{id}/permanent")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PermanentDelete(int id)
    {
        var userId = this.GetUserId();
        var result = await _repository.HardDeleteAsync(id, userId);

        if (!result)
            return NotFound();

        return NoContent();
    }
}
```

---

## ?? Limpeza de Dados Antigos (Opcional)

### **Job para deletar permanentemente dados antigos**

```sql
-- Deletar permanentemente registros soft-deleted há mais de 1 ano
CREATE OR ALTER PROCEDURE sp_CleanupOldDeletedRecords
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CutoffDate DATETIME = DATEADD(YEAR, -1, GETUTCDATE());

    -- Deletar expenses
    DELETE FROM Expenses 
    WHERE DeletedAt IS NOT NULL 
      AND DeletedAt < @CutoffDate;

    -- Deletar cartões
    DELETE FROM CreditCards 
    WHERE DeletedAt IS NOT NULL 
      AND DeletedAt < @CutoffDate;

    PRINT 'Limpeza concluída';
END
GO

-- Agendar no SQL Server Agent para rodar todo mês
```

---

## ? Checklist de Implementação

### **Database**
- [ ] Executar `SoftDeleteSetup.sql`
- [ ] Testar SPs de soft delete
- [ ] Verificar view de auditoria

### **Backend**
- [ ] Atualizar Models com novas colunas
- [ ] Atualizar Repositories com métodos SoftDelete/Restore
- [ ] Atualizar Controllers com endpoints de restore
- [ ] Adicionar filtro `includeDeleted` nas listagens

### **Frontend (Opcional)**
- [ ] Adicionar botão "Restaurar" na UI
- [ ] Criar página "Lixeira"
- [ ] Mostrar indicador visual para itens deletados

---

## ?? Boas Práticas

### ? **DO (Faça):**

1. **Sempre use Soft Delete por padrão**
   ```csharp
   // ? CORRETO
   await _repository.SoftDeleteAsync(id, userId);
   ```

2. **Filtre deletados nas queries**
   ```sql
   WHERE DeletedAt IS NULL
   ```

3. **Registre quem deletou**
   ```csharp
   DeletedBy = userId
   ```

4. **Ofereça função de restaurar**
   ```csharp
   [HttpPost("{id}/restore")]
   ```

---

### ? **DON'T (Não faça):**

1. **Não use Hard Delete sem motivo**
   ```csharp
   // ? EVITE
   await _repository.HardDeleteAsync(id);
   ```

2. **Não esqueça de filtrar deletados**
   ```sql
   -- ? ERRADO
   SELECT * FROM Expenses WHERE UserId = @UserId

   -- ? CORRETO
   SELECT * FROM Expenses 
   WHERE UserId = @UserId AND DeletedAt IS NULL
   ```

3. **Não permita restaurar sem validação**
   ```csharp
   // ? ERRADO
   await _repository.RestoreAsync(id); // Qualquer um pode restaurar

   // ? CORRETO
   await _repository.RestoreAsync(id, userId); // Apenas o dono
   ```

---

## ?? Relatórios de Auditoria

```sql
-- Relatório: Usuários mais ativos
SELECT 
    u.Name,
    COUNT(DISTINCT e.Id) AS DespesasCriadas,
    COUNT(DISTINCT CASE WHEN e.UpdatedBy IS NOT NULL THEN e.Id END) AS DespesasEditadas,
    COUNT(DISTINCT CASE WHEN e.DeletedBy IS NOT NULL THEN e.Id END) AS DespesasDeletadas
FROM Users u
LEFT JOIN Expenses e ON u.Id = e.UserId
WHERE u.DeletedAt IS NULL
GROUP BY u.Name
ORDER BY DespesasCriadas DESC;

-- Relatório: Atividade por data
SELECT 
    CAST(CreatedAt AS DATE) AS Data,
    COUNT(*) AS TotalCriado
FROM Expenses
WHERE UserId = @UserId
GROUP BY CAST(CreatedAt AS DATE)
ORDER BY Data DESC;
```

---

**?? Soft Delete implementado com sucesso!**

Agora você tem:
- ? Auditoria completa
- ? Recuperação de dados
- ? Rastreamento de mudanças
- ? Compliance com LGPD/GDPR
