-- =============================================
-- STORED PROCEDURES FALTANTES
-- Execute este script para criar as SPs que faltam
-- =============================================

USE WebAppDB;
GO

PRINT '?? Criando Stored Procedures faltantes...';
PRINT '==========================================';
GO

-- =============================================
-- CATEGORY PROCEDURES
-- =============================================

-- Buscar categorias por UserId
CREATE OR ALTER PROCEDURE sp_GetCategoriesByUserId
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
        Name,
        Description,
        IsActive,
        CreatedAt
    FROM Categories
    WHERE UserId = @UserId
      AND IsActive = 1
    ORDER BY Name;
END
GO

PRINT '? sp_GetCategoriesByUserId criada';
GO

-- =============================================
-- MONTHLY FINANCIAL PROCEDURES
-- =============================================

-- Buscar controles mensais por UserId
CREATE OR ALTER PROCEDURE sp_GetMonthlyFinancialByUserId
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
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
    WHERE UserId = @UserId
    ORDER BY Year DESC, Month DESC;
END
GO

PRINT '? sp_GetMonthlyFinancialByUserId criada';
GO

-- =============================================
-- ATUALIZAR SP CREATE CATEGORY
-- =============================================

CREATE OR ALTER PROCEDURE sp_CreateCategory
    @UserId INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500),
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Categories (UserId, Name, Description, IsActive, CreatedAt)
    VALUES (@UserId, @Name, @Description, @IsActive, GETUTCDATE());

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

PRINT '? sp_CreateCategory atualizada com UserId';
GO

-- =============================================
-- ATUALIZAR SP CREATE MONTHLY FINANCIAL
-- =============================================

CREATE OR ALTER PROCEDURE sp_CreateMonthlyFinancial
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

    DECLARE @CreatedAt DATETIME = GETUTCDATE();
    DECLARE @UpdatedAt DATETIME = GETUTCDATE();

    INSERT INTO MonthlyFinancialControl (UserId, Year, Month, Money, RV, Debit, Others, Reserve, CreatedAt, UpdatedAt)
    VALUES (@UserId, @Year, @Month, @Money, @RV, @Debit, @Others, @Reserve, @CreatedAt, @UpdatedAt);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

PRINT '? sp_CreateMonthlyFinancial atualizada com UserId';
GO

PRINT '';
PRINT '? Todas as Stored Procedures faltantes foram criadas!';
PRINT '';
PRINT '?? SPs criadas/atualizadas:';
PRINT '   - sp_GetCategoriesByUserId';
PRINT '   - sp_GetMonthlyFinancialByUserId';
PRINT '   - sp_CreateCategory (com UserId)';
PRINT '   - sp_CreateMonthlyFinancial (com UserId)';
PRINT '';
PRINT '?? Próximos passos:';
PRINT '   1. Execute LinkDataToUser2004.sql (se ainda não executou)';
PRINT '   2. Execute StoredProceduresWithAdmin.sql';
PRINT '   3. Reinicie a API: dotnet run';
PRINT '   4. Teste todos os endpoints';
GO
