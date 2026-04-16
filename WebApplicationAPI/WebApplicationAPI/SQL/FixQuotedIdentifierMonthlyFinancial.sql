-- =============================================
-- FIX: Recreate MonthlyFinancial stored procedures with correct QUOTED_IDENTIFIER settings
-- =============================================
USE WebAppDB;
GO

-- 1. INSERT
IF OBJECT_ID('sp_InsertMonthlyFinancial', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertMonthlyFinancial;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_InsertMonthlyFinancial
    @UserId INT,
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
    INSERT INTO MonthlyFinancialControl (UserId, Year, Month, Money, RV, Debit, Others, Reserve, CreatedAt, UpdatedAt)
    VALUES (@UserId, @Year, @Month, @Money, @RV, @Debit, @Others, @Reserve, GETDATE(), GETDATE());
    SET @Id = CAST(SCOPE_IDENTITY() AS INT);
END
GO
PRINT '? sp_InsertMonthlyFinancial recriado com QUOTED_IDENTIFIER ON';

-- 2. UPDATE
IF OBJECT_ID('sp_UpdateMonthlyFinancial', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateMonthlyFinancial;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_UpdateMonthlyFinancial
    @Id INT,
    @UserId INT,
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
    SET Year = @Year, Month = @Month, Money = @Money, RV = @RV, Debit = @Debit, Others = @Others, Reserve = @Reserve, UpdatedAt = GETDATE()
    WHERE Id = @Id AND UserId = @UserId;
END
GO
PRINT '? sp_UpdateMonthlyFinancial recriado com QUOTED_IDENTIFIER ON';

-- 3. DELETE
IF OBJECT_ID('sp_DeleteMonthlyFinancial', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteMonthlyFinancial;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_DeleteMonthlyFinancial
    @Id INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM MonthlyFinancialControl WHERE Id = @Id AND UserId = @UserId;
END
GO
PRINT '? sp_DeleteMonthlyFinancial recriado com QUOTED_IDENTIFIER ON';

-- 4. GET BY ID
IF OBJECT_ID('sp_GetMonthlyFinancialById', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetMonthlyFinancialById;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_GetMonthlyFinancialById
    @Id INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM MonthlyFinancialControl WHERE Id = @Id AND UserId = @UserId;
END
GO
PRINT '? sp_GetMonthlyFinancialById recriado com QUOTED_IDENTIFIER ON';

-- 5. GET ALL
IF OBJECT_ID('sp_GetAllMonthlyFinancial', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetAllMonthlyFinancial;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_GetAllMonthlyFinancial
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM MonthlyFinancialControl WHERE UserId = @UserId ORDER BY Year DESC, Month DESC;
END
GO
PRINT '? sp_GetAllMonthlyFinancial recriado com QUOTED_IDENTIFIER ON';

-- 6. GET BY USER ID
IF OBJECT_ID('sp_GetMonthlyFinancialByUserId', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetMonthlyFinancialByUserId;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_GetMonthlyFinancialByUserId
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM MonthlyFinancialControl WHERE UserId = @UserId ORDER BY Year DESC, Month DESC;
END
GO
PRINT '? sp_GetMonthlyFinancialByUserId recriado com QUOTED_IDENTIFIER ON';

-- 7. GET BY YEAR AND MONTH
IF OBJECT_ID('sp_GetMonthlyFinancialByYearMonth', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetMonthlyFinancialByYearMonth;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_GetMonthlyFinancialByYearMonth
    @UserId INT,
    @Year INT,
    @Month INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM MonthlyFinancialControl WHERE UserId = @UserId AND Year = @Year AND Month = @Month;
END
GO
PRINT '? sp_GetMonthlyFinancialByYearMonth recriado com QUOTED_IDENTIFIER ON';

-- 8. GET EXPENSES TOTAL BY YEAR AND MONTH
IF OBJECT_ID('sp_GetExpensesTotalByYearAndMonth', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetExpensesTotalByYearAndMonth;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_GetExpensesTotalByYearAndMonth
    @UserId INT,
    @Year INT,
    @Month INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ISNULL(SUM(Amount), 0) AS TotalExpenses
    FROM Expenses 
    WHERE UserId = @UserId AND Year = @Year AND Month = @Month;
END
GO
PRINT '? sp_GetExpensesTotalByYearAndMonth recriado com QUOTED_IDENTIFIER ON';

PRINT '';
PRINT '??? CORREÇÃO DE QUOTED_IDENTIFIER PARA MONTHLY FINANCIAL CONCLUÍDA! ???';
GO
