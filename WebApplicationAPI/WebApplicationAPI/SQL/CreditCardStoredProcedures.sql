-- =============================================
-- CREDIT CARD MANAGEMENT - STORED PROCEDURES
-- =============================================
-- Gerenciamento completo de cartões de crédito
-- =============================================

USE WebAppDB;
GO

-- =============================================
-- PARTE 1: CRIAR TABELAS
-- =============================================

-- Tabela: CreditCards
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CreditCards')
BEGIN
    CREATE TABLE CreditCards (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Brand NVARCHAR(50) NULL,
        CardLimit DECIMAL(18,2) NOT NULL DEFAULT 0,
        ClosingDay INT NULL,
        DueDay INT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
    );
    PRINT '? Tabela CreditCards criada com sucesso!';
END
ELSE
BEGIN
    PRINT '?? Tabela CreditCards já existe.';
END
GO

-- Tabela: CreditCardExpenses
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CreditCardExpenses')
BEGIN
    CREATE TABLE CreditCardExpenses (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CreditCardId INT NOT NULL,
        PurchaseDate DATE NOT NULL,
        Description NVARCHAR(500) NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        Category NVARCHAR(100) NULL,
        Month INT NOT NULL,
        Year INT NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'PENDENTE',
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_CreditCardExpenses_CreditCard FOREIGN KEY (CreditCardId) REFERENCES CreditCards(Id) ON DELETE CASCADE
    );
    PRINT '? Tabela CreditCardExpenses criada com sucesso!';
END
ELSE
BEGIN
    PRINT '?? Tabela CreditCardExpenses já existe.';
END
GO

-- =============================================
-- PARTE 2: STORED PROCEDURES - CREDIT CARDS
-- =============================================

-- 1. CREATE CREDIT CARD
GO
CREATE OR ALTER PROCEDURE sp_CreateCreditCard
    @Name NVARCHAR(100),
    @Brand NVARCHAR(50) = NULL,
    @CardLimit DECIMAL(18,2) = 0,
    @ClosingDay INT = NULL,
    @DueDay INT = NULL,
    @IsActive BIT = 1,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO CreditCards (Name, Brand, CardLimit, ClosingDay, DueDay, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Brand, @CardLimit, @ClosingDay, @DueDay, @IsActive, GETDATE(), GETDATE());

    SET @Id = CAST(SCOPE_IDENTITY() AS INT);
END
GO

-- 2. UPDATE CREDIT CARD
GO
CREATE OR ALTER PROCEDURE sp_UpdateCreditCard
    @Id INT,
    @Name NVARCHAR(100),
    @Brand NVARCHAR(50) = NULL,
    @CardLimit DECIMAL(18,2) = 0,
    @ClosingDay INT = NULL,
    @DueDay INT = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE CreditCards
    SET 
        Name = @Name,
        Brand = @Brand,
        CardLimit = @CardLimit,
        ClosingDay = @ClosingDay,
        DueDay = @DueDay,
        IsActive = @IsActive,
        UpdatedAt = GETDATE()
    WHERE Id = @Id;
END
GO

-- 3. DELETE CREDIT CARD
GO
CREATE OR ALTER PROCEDURE sp_DeleteCreditCard
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM CreditCards
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- 4. GET CREDIT CARD BY ID
GO
CREATE OR ALTER PROCEDURE sp_GetCreditCardById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Brand, CardLimit, ClosingDay, DueDay, IsActive, CreatedAt, UpdatedAt
    FROM CreditCards
    WHERE Id = @Id;
END
GO

-- 5. GET ALL CREDIT CARDS
GO
CREATE OR ALTER PROCEDURE sp_GetAllCreditCards
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Brand, CardLimit, ClosingDay, DueDay, IsActive, CreatedAt, UpdatedAt
    FROM CreditCards
    ORDER BY IsActive DESC, Name;
END
GO

-- =============================================
-- PARTE 3: STORED PROCEDURES - CREDIT CARD EXPENSES
-- =============================================

-- 6. CREATE CREDIT CARD EXPENSE
GO
CREATE OR ALTER PROCEDURE sp_CreateCreditCardExpense
    @CreditCardId INT,
    @PurchaseDate DATE,
    @Description NVARCHAR(500),
    @Amount DECIMAL(18,2),
    @Category NVARCHAR(100) = NULL,
    @Month INT,
    @Year INT,
    @Status NVARCHAR(50) = 'PENDENTE',
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO CreditCardExpenses (CreditCardId, PurchaseDate, Description, Amount, Category, Month, Year, Status, CreatedAt)
    VALUES (@CreditCardId, @PurchaseDate, @Description, @Amount, @Category, @Month, @Year, @Status, GETDATE());

    SET @Id = CAST(SCOPE_IDENTITY() AS INT);
END
GO

-- 7. UPDATE CREDIT CARD EXPENSE
GO
CREATE OR ALTER PROCEDURE sp_UpdateCreditCardExpense
    @Id INT,
    @PurchaseDate DATE,
    @Description NVARCHAR(500),
    @Amount DECIMAL(18,2),
    @Category NVARCHAR(100),
    @Status NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE CreditCardExpenses
    SET 
        PurchaseDate = @PurchaseDate,
        Description = @Description,
        Amount = @Amount,
        Category = @Category,
        Status = @Status,
        Month = MONTH(@PurchaseDate),
        Year = YEAR(@PurchaseDate)
    WHERE Id = @Id;
END
GO

-- 8. DELETE CREDIT CARD EXPENSE
GO
CREATE OR ALTER PROCEDURE sp_DeleteCreditCardExpense
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM CreditCardExpenses
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- 9. GET CREDIT CARD EXPENSE BY ID
GO
CREATE OR ALTER PROCEDURE sp_GetCreditCardExpenseById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, CreditCardId, PurchaseDate, Description, Amount, Category, Month, Year, Status, CreatedAt
    FROM CreditCardExpenses
    WHERE Id = @Id;
END
GO

-- 10. GET CREDIT CARD EXPENSES BY CARD
GO
CREATE OR ALTER PROCEDURE sp_GetCreditCardExpensesByCard
    @CreditCardId INT,
    @Month INT = NULL,
    @Year INT = NULL,
    @Category NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, CreditCardId, PurchaseDate, Description, Amount, Category, Month, Year, Status, CreatedAt
    FROM CreditCardExpenses
    WHERE CreditCardId = @CreditCardId
      AND (@Month IS NULL OR Month = @Month)
      AND (@Year IS NULL OR Year = @Year)
      AND (@Category IS NULL OR Category = @Category)
    ORDER BY PurchaseDate DESC;
END
GO

-- 11. GET CREDIT CARD STATEMENT (Fatura do Mês)
GO
CREATE OR ALTER PROCEDURE sp_GetCreditCardStatement
    @CreditCardId INT,
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        cce.Id,
        cce.PurchaseDate,
        cce.Description,
        cce.Amount,
        cce.Category,
        cce.Status,
        cc.Name AS CardName,
        cc.Brand
    FROM CreditCardExpenses cce
    INNER JOIN CreditCards cc ON cce.CreditCardId = cc.Id
    WHERE cce.CreditCardId = @CreditCardId
      AND cce.Month = @Month
      AND cce.Year = @Year
    ORDER BY cce.PurchaseDate DESC;

    -- Totais
    SELECT 
        SUM(Amount) AS TotalAmount,
        COUNT(*) AS TotalTransactions,
        cc.CardLimit
    FROM CreditCardExpenses cce
    INNER JOIN CreditCards cc ON cce.CreditCardId = cc.Id
    WHERE cce.CreditCardId = @CreditCardId
      AND cce.Month = @Month
      AND cce.Year = @Year
    GROUP BY cc.CardLimit;
END
GO

-- 12. GET EXPENSES BY CATEGORY
GO
CREATE OR ALTER PROCEDURE sp_GetCreditCardExpensesByCategory
    @CreditCardId INT,
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Category,
        SUM(Amount) AS TotalAmount,
        COUNT(*) AS TransactionCount,
        AVG(Amount) AS AverageAmount
    FROM CreditCardExpenses
    WHERE CreditCardId = @CreditCardId
      AND Month = @Month
      AND Year = @Year
    GROUP BY Category
    ORDER BY TotalAmount DESC;
END
GO

PRINT '?? Stored Procedures criadas com sucesso!';
PRINT '? sp_CreateCreditCard';
PRINT '? sp_UpdateCreditCard';
PRINT '? sp_DeleteCreditCard';
PRINT '? sp_GetCreditCardById';
PRINT '? sp_GetAllCreditCards';
PRINT '? sp_CreateCreditCardExpense';
PRINT '? sp_UpdateCreditCardExpense';
PRINT '? sp_DeleteCreditCardExpense';
PRINT '? sp_GetCreditCardExpenseById';
PRINT '? sp_GetCreditCardExpensesByCard';
PRINT '? sp_GetCreditCardStatement';
PRINT '? sp_GetCreditCardExpensesByCategory';
GO
