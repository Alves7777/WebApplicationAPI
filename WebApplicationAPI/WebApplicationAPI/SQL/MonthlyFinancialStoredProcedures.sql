-- =============================================
-- STORED PROCEDURES - MONTHLY FINANCIAL CONTROL
-- =============================================
-- Execute este script no SQL Server para criar tabela e procedures
-- =============================================

USE WebAppDB;
GO

-- =============================================
-- PARTE 1: CRIAR TABELA
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MonthlyFinancialControl')
BEGIN
    CREATE TABLE MonthlyFinancialControl (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Year INT NOT NULL,
        Month INT NOT NULL,
        Money DECIMAL(18,2) NOT NULL DEFAULT 0,
        RV DECIMAL(18,2) NOT NULL DEFAULT 0,
        Debit DECIMAL(18,2) NOT NULL DEFAULT 0,
        Others DECIMAL(18,2) NOT NULL DEFAULT 0,
        Reserve DECIMAL(18,2) NOT NULL DEFAULT 0,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT UQ_YearMonth UNIQUE (Year, Month)
    );
    PRINT '? Tabela MonthlyFinancialControl criada com sucesso!';
END
ELSE
BEGIN
    PRINT '?? Tabela MonthlyFinancialControl já existe.';
END
GO

-- =============================================
-- PARTE 2: STORED PROCEDURES
-- =============================================

-- 1. INSERT
GO
CREATE OR ALTER PROCEDURE sp_InsertMonthlyFinancial
    @Year INT,
    @Month INT,
    @Money DECIMAL(18,2),
    @RV DECIMAL(18,2),
    @Debit DECIMAL(18,2),
    @Others DECIMAL(18,2),
    @Reserve DECIMAL(18,2),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO MonthlyFinancialControl (Year, Month, Money, RV, Debit, Others, Reserve, CreatedAt, UpdatedAt)
    VALUES (@Year, @Month, @Money, @RV, @Debit, @Others, @Reserve, GETDATE(), GETDATE());

    SET @Id = CAST(SCOPE_IDENTITY() AS INT);
END
GO

-- 2. UPDATE
GO
CREATE OR ALTER PROCEDURE sp_UpdateMonthlyFinancial
    @Id INT,
    @Year INT,
    @Month INT,
    @Money DECIMAL(18,2),
    @RV DECIMAL(18,2),
    @Debit DECIMAL(18,2),
    @Others DECIMAL(18,2),
    @Reserve DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE MonthlyFinancialControl
    SET 
        Year = @Year,
        Month = @Month,
        Money = @Money,
        RV = @RV,
        Debit = @Debit,
        Others = @Others,
        Reserve = @Reserve,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END
GO

-- 3. DELETE
GO
CREATE OR ALTER PROCEDURE sp_DeleteMonthlyFinancial
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM MonthlyFinancialControl
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- 4. GET BY ID
GO
CREATE OR ALTER PROCEDURE sp_GetMonthlyFinancialById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        Year,
        Month,
        Money,
        RV,
        Debit,
        Others,
        Reserve,
        CreatedAt,
        UpdatedAt
    FROM MonthlyFinancialControl
    WHERE Id = @Id;
END
GO

-- 5. GET ALL
GO
CREATE OR ALTER PROCEDURE sp_GetAllMonthlyFinancial
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        Year,
        Month,
        Money,
        RV,
        Debit,
        Others,
        Reserve,
        CreatedAt,
        UpdatedAt
    FROM MonthlyFinancialControl
    ORDER BY Year DESC, Month DESC;
END
GO

-- 6. GET BY YEAR AND MONTH (Para validação de duplicidade)
GO
CREATE OR ALTER PROCEDURE sp_GetMonthlyFinancialByYearMonth
    @Year INT,
    @Month INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        Year,
        Month,
        Money,
        RV,
        Debit,
        Others,
        Reserve,
        CreatedAt,
        UpdatedAt
    FROM MonthlyFinancialControl
    WHERE Year = @Year AND Month = @Month;
END
GO

-- =============================================
-- PARTE 3: PROCEDURE COM CÁLCULOS (OPCIONAL - PARA RELATÓRIOS)
-- =============================================

GO
CREATE OR ALTER PROCEDURE sp_GetMonthlyFinancialWithCalculations
    @Year INT = NULL,
    @Month INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        mfc.Id,
        mfc.Year,
        mfc.Month,
        mfc.Money,
        mfc.RV,
        mfc.Debit,
        mfc.Others,
        mfc.Reserve,

        -- Cálculo: SalaryTotal
        (mfc.Money + mfc.RV + mfc.Debit + mfc.Others) AS SalaryTotal,

        -- Cálculo: ExpensesTotal
        ISNULL((
            SELECT SUM(Amount)
            FROM Expenses e
            WHERE e.Year = mfc.Year AND e.Month = mfc.Month
        ), 0) AS ExpensesTotal,

        -- Cálculo: Balance
        (mfc.Money + mfc.RV + mfc.Debit + mfc.Others) - ISNULL((
            SELECT SUM(Amount)
            FROM Expenses e
            WHERE e.Year = mfc.Year AND e.Month = mfc.Month
        ), 0) AS Balance,

        -- Cálculo: CanSpend
        (mfc.Money + mfc.RV + mfc.Debit + mfc.Others) - ISNULL((
            SELECT SUM(Amount)
            FROM Expenses e
            WHERE e.Year = mfc.Year AND e.Month = mfc.Month
        ), 0) - mfc.Reserve AS CanSpend,

        mfc.CreatedAt,
        mfc.UpdatedAt
    FROM MonthlyFinancialControl mfc
    WHERE (@Year IS NULL OR mfc.Year = @Year)
      AND (@Month IS NULL OR mfc.Month = @Month)
    ORDER BY mfc.Year DESC, mfc.Month DESC;
END
GO

PRINT '?? Stored Procedures criadas com sucesso!';
PRINT '? sp_InsertMonthlyFinancial';
PRINT '? sp_UpdateMonthlyFinancial';
PRINT '? sp_DeleteMonthlyFinancial';
PRINT '? sp_GetMonthlyFinancialById';
PRINT '? sp_GetAllMonthlyFinancial';
PRINT '? sp_GetMonthlyFinancialByYearMonth';
PRINT '? sp_GetMonthlyFinancialWithCalculations';
GO
