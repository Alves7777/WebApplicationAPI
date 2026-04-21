-- MIGRATION: Add UserId to MonthlyFinancialControl
USE WebAppDB;
GO

-- 1. Add UserId column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'MonthlyFinancialControl') AND name = 'UserId')
BEGIN
    ALTER TABLE MonthlyFinancialControl
    ADD UserId INT NULL;
    PRINT '? Coluna UserId adicionada à tabela MonthlyFinancialControl';
END
ELSE
BEGIN
    PRINT '?? Coluna UserId já existe na tabela MonthlyFinancialControl';
END
GO

-- 2. Add Foreign Key constraint if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_MonthlyFinancial_User')
BEGIN
    ALTER TABLE MonthlyFinancialControl
    ADD CONSTRAINT FK_MonthlyFinancial_User FOREIGN KEY (UserId) REFERENCES Users(Id);
    PRINT '? Foreign Key FK_MonthlyFinancial_User criada';
END
ELSE
BEGIN
    PRINT '?? Foreign Key FK_MonthlyFinancial_User já existe';
END
GO

-- 3. Update Unique Constraint to include UserId
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ_YearMonth')
BEGIN
    ALTER TABLE MonthlyFinancialControl DROP CONSTRAINT UQ_YearMonth;
END
GO
ALTER TABLE MonthlyFinancialControl ADD CONSTRAINT UQ_UserYearMonth UNIQUE (UserId, Year, Month);
GO

-- 4. Update Procedures
GO
CREATE OR ALTER PROCEDURE sp_InsertMonthlyFinancial
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

CREATE OR ALTER PROCEDURE sp_UpdateMonthlyFinancial
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

CREATE OR ALTER PROCEDURE sp_DeleteMonthlyFinancial
    @Id INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM MonthlyFinancialControl WHERE Id = @Id AND UserId = @UserId;
END
GO

CREATE OR ALTER PROCEDURE sp_GetMonthlyFinancialById
    @Id INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM MonthlyFinancialControl WHERE Id = @Id AND UserId = @UserId;
END
GO

CREATE OR ALTER PROCEDURE sp_GetAllMonthlyFinancial
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM MonthlyFinancialControl WHERE UserId = @UserId ORDER BY Year DESC, Month DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_GetMonthlyFinancialByUserId
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM MonthlyFinancialControl WHERE UserId = @UserId ORDER BY Year DESC, Month DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_GetMonthlyFinancialByYearMonth
    @UserId INT,
    @Year INT,
    @Month INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM MonthlyFinancialControl WHERE UserId = @UserId AND Year = @Year AND Month = @Month;
END
GO

CREATE OR ALTER PROCEDURE sp_GetExpensesTotalByYearAndMonth
    @UserId INT,
    @Year INT,
    @Month INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT SUM(Money) FROM MonthlyFinancialControl WHERE UserId = @UserId AND Year = @Year AND Month = @Month;
END
GO

PRINT '??? MIGRAÇÃO MONTHLY FINANCIAL CONCLUÍDA ???';
