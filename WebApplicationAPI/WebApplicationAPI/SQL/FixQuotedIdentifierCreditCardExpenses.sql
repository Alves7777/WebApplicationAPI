-- =============================================
-- FIX: Recreate CreditCardExpense stored procedures with QUOTED_IDENTIFIER and UserId
-- =============================================
USE WebAppDB;
GO

-- 1. CREATE CREDIT CARD EXPENSE (with UserId)
IF OBJECT_ID('sp_CreateCreditCardExpense', 'P') IS NOT NULL
    DROP PROCEDURE sp_CreateCreditCardExpense;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_CreateCreditCardExpense
    @UserId INT,
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
PRINT '? sp_CreateCreditCardExpense recriado com QUOTED_IDENTIFIER ON e UserId';

-- 2. UPDATE CREDIT CARD EXPENSE
IF OBJECT_ID('sp_UpdateCreditCardExpense', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateCreditCardExpense;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_UpdateCreditCardExpense
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
PRINT '? sp_UpdateCreditCardExpense recriado com QUOTED_IDENTIFIER ON';

-- 3. DELETE CREDIT CARD EXPENSE
IF OBJECT_ID('sp_DeleteCreditCardExpense', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteCreditCardExpense;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_DeleteCreditCardExpense
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM CreditCardExpenses
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
PRINT '? sp_DeleteCreditCardExpense recriado com QUOTED_IDENTIFIER ON';

-- 4. GET CREDIT CARD EXPENSE BY ID
IF OBJECT_ID('sp_GetCreditCardExpenseById', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetCreditCardExpenseById;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_GetCreditCardExpenseById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, CreditCardId, PurchaseDate, Description, Amount, Category, Month, Year, Status, CreatedAt
    FROM CreditCardExpenses
    WHERE Id = @Id;
END
GO
PRINT '? sp_GetCreditCardExpenseById recriado com QUOTED_IDENTIFIER ON';

-- 5. GET CREDIT CARD EXPENSES BY CARD (with UserId validation)
IF OBJECT_ID('sp_GetCreditCardExpensesByCard', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetCreditCardExpensesByCard;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_GetCreditCardExpensesByCard
    @CreditCardId INT,
    @UserId INT,
    @Month INT = NULL,
    @Year INT = NULL,
    @Category NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT e.Id, e.CreditCardId, e.PurchaseDate, e.Description, e.Amount, e.Category, e.Month, e.Year, e.Status, e.CreatedAt
    FROM CreditCardExpenses e
    INNER JOIN CreditCards c ON e.CreditCardId = c.Id
    WHERE e.CreditCardId = @CreditCardId
      AND c.UserId = @UserId
      AND (@Month IS NULL OR e.Month = @Month)
      AND (@Year IS NULL OR e.Year = @Year)
      AND (@Category IS NULL OR e.Category = @Category)
    ORDER BY e.PurchaseDate DESC;
END
GO
PRINT '? sp_GetCreditCardExpensesByCard recriado com QUOTED_IDENTIFIER ON e UserId';

-- 6. GET CREDIT CARD EXPENSES BY PERIOD (with UserId validation)
IF OBJECT_ID('sp_GetCreditCardExpensesByPeriod', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetCreditCardExpensesByPeriod;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_GetCreditCardExpensesByPeriod
    @CreditCardId INT,
    @UserId INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT e.Id, e.CreditCardId, e.PurchaseDate, e.Description, e.Amount, e.Category, e.Month, e.Year, e.Status, e.CreatedAt
    FROM CreditCardExpenses e
    INNER JOIN CreditCards c ON e.CreditCardId = c.Id
    WHERE e.CreditCardId = @CreditCardId
      AND c.UserId = @UserId
      AND e.PurchaseDate >= @StartDate
      AND e.PurchaseDate <= @EndDate
    ORDER BY e.PurchaseDate;
END
GO
PRINT '? sp_GetCreditCardExpensesByPeriod recriado com QUOTED_IDENTIFIER ON e UserId';

-- 7. CHECK IF CREDIT CARD EXPENSE EXISTS
IF OBJECT_ID('sp_CheckCreditCardExpenseExists', 'P') IS NOT NULL
    DROP PROCEDURE sp_CheckCreditCardExpenseExists;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_CheckCreditCardExpenseExists
    @CreditCardId INT,
    @PurchaseDate DATE,
    @Description NVARCHAR(500),
    @Amount DECIMAL(18,2),
    @Exists BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 
        FROM CreditCardExpenses 
        WHERE CreditCardId = @CreditCardId 
          AND PurchaseDate = @PurchaseDate 
          AND Description = @Description 
          AND Amount = @Amount
    )
        SET @Exists = 1;
    ELSE
        SET @Exists = 0;
END
GO
PRINT '? sp_CheckCreditCardExpenseExists recriado com QUOTED_IDENTIFIER ON';

-- 8. GET CREDIT CARD STATEMENT
IF OBJECT_ID('sp_GetCreditCardStatement', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetCreditCardStatement;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_GetCreditCardStatement
    @CreditCardId INT,
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, CreditCardId, PurchaseDate, Description, Amount, Category, Month, Year, Status, CreatedAt
    FROM CreditCardExpenses
    WHERE CreditCardId = @CreditCardId
      AND Month = @Month
      AND Year = @Year
    ORDER BY PurchaseDate;
END
GO
PRINT '? sp_GetCreditCardStatement recriado com QUOTED_IDENTIFIER ON';

-- 9. GET CREDIT CARD EXPENSES BY CATEGORY
IF OBJECT_ID('sp_GetCreditCardExpensesByCategory', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetCreditCardExpensesByCategory;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_GetCreditCardExpensesByCategory
    @CreditCardId INT,
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ISNULL(Category, 'Sem Categoria') AS Category,
        SUM(Amount) AS TotalAmount
    FROM CreditCardExpenses
    WHERE CreditCardId = @CreditCardId
      AND Month = @Month
      AND Year = @Year
    GROUP BY Category
    ORDER BY TotalAmount DESC;
END
GO
PRINT '? sp_GetCreditCardExpensesByCategory recriado com QUOTED_IDENTIFIER ON';

PRINT '';
PRINT '??? CORREÇÃO DE QUOTED_IDENTIFIER PARA CREDIT CARD EXPENSES CONCLUÍDA! ???';
GO
