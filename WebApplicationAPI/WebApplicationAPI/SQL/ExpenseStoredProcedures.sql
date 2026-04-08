-- =============================================
-- STORED PROCEDURES PARA TABELA EXPENSES
-- =============================================

-- 1. Criar nova despesa
GO
CREATE OR ALTER PROCEDURE sp_CreateExpense
    @Month INT,
    @Year INT,
    @Description NVARCHAR(500),
    @Amount DECIMAL(18,2),
    @Category NVARCHAR(100),
    @Status NVARCHAR(50),
    @PaymentMethod NVARCHAR(100),
    @CreatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Expenses (Month, Year, Description, Amount, Category, Status, PaymentMethod, CreatedAt)
    VALUES (@Month, @Year, @Description, @Amount, @Category, @Status, @PaymentMethod, @CreatedAt);

    SET @Id = CAST(SCOPE_IDENTITY() AS INT);
END
GO

-- 2. Atualizar despesa
GO
CREATE OR ALTER PROCEDURE sp_UpdateExpense
    @Id INT,
    @Month INT,
    @Year INT,
    @Description NVARCHAR(500),
    @Amount DECIMAL(18,2),
    @Category NVARCHAR(100),
    @Status NVARCHAR(50),
    @PaymentMethod NVARCHAR(100),
    @CreatedAt DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Expenses
    SET Month = @Month,
        Year = @Year,
        Description = @Description,
        Amount = @Amount,
        Category = @Category,
        Status = @Status,
        PaymentMethod = @PaymentMethod,
        CreatedAt = @CreatedAt
    WHERE Id = @Id;

    SELECT Id, Month, Year, Description, Amount, Category, Status, PaymentMethod, CreatedAt
    FROM Expenses
    WHERE Id = @Id;
END
GO

-- 3. Deletar despesa
GO
CREATE OR ALTER PROCEDURE sp_DeleteExpense
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Expenses
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- 4. Buscar despesa por ID
GO
CREATE OR ALTER PROCEDURE sp_GetExpenseById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Month, Year, Description, Amount, Category, Status, PaymentMethod, CreatedAt
    FROM Expenses
    WHERE Id = @Id;
END
GO

-- 5. Buscar despesas com filtros
GO
CREATE OR ALTER PROCEDURE sp_GetExpenses
    @Month INT = NULL,
    @Year INT = NULL,
    @Category NVARCHAR(100) = NULL,
    @Status NVARCHAR(50) = NULL,
    @PaymentMethod NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Month, Year, Description, Amount, Category, Status, PaymentMethod, CreatedAt
    FROM Expenses
    WHERE (@Month IS NULL OR Month = @Month)
      AND (@Year IS NULL OR Year = @Year)
      AND (@Category IS NULL OR Category = @Category)
      AND (@Status IS NULL OR Status = @Status)
      AND (@PaymentMethod IS NULL OR PaymentMethod = @PaymentMethod)
    ORDER BY Year DESC, Month DESC, CreatedAt DESC;
END
GO

-- 6. Calcular totais por categoria
GO
CREATE OR ALTER PROCEDURE sp_GetExpenseTotalsByCategory
    @Month INT = NULL,
    @Year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Category,
        SUM(Amount) AS TotalAmount,
        COUNT(*) AS Count
    FROM Expenses
    WHERE (@Month IS NULL OR Month = @Month)
      AND (@Year IS NULL OR Year = @Year)
    GROUP BY Category
    ORDER BY TotalAmount DESC;
END
GO

-- 7. Calcular totais mensais
GO
CREATE OR ALTER PROCEDURE sp_GetExpenseTotalsByMonth
    @Year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Year,
        Month,
        SUM(Amount) AS TotalAmount,
        COUNT(*) AS Count
    FROM Expenses
    WHERE (@Year IS NULL OR Year = @Year)
    GROUP BY Year, Month
    ORDER BY Year DESC, Month DESC;
END
GO
