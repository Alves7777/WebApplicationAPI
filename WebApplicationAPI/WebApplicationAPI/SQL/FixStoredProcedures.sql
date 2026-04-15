-- =============================================
-- RE-CRIAR STORED PROCEDURES - VERSÃO CORRIGIDA
-- Execute este script para corrigir as SPs
-- =============================================

USE WebAppDB;
GO

PRINT '?? Re-criando Stored Procedures...';
PRINT '====================================';
GO

-- =============================================
-- CREDITCARD EXPENSES PROCEDURES
-- =============================================

-- Criar despesa de cartão
CREATE OR ALTER PROCEDURE sp_CreateCreditCardExpense
    @UserId INT,
    @CreditCardId INT,
    @Description NVARCHAR(500),
    @Amount DECIMAL(18,2),
    @Category NVARCHAR(100),
    @PurchaseDate DATETIME,
    @Installments INT = 1
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar se o cartão pertence ao usuário
    IF NOT EXISTS (SELECT 1 FROM CreditCards WHERE Id = @CreditCardId AND UserId = @UserId)
    BEGIN
        RAISERROR('Cartão de crédito não encontrado ou não pertence ao usuário', 16, 1);
        RETURN;
    END

    DECLARE @CreatedAt DATETIME = GETUTCDATE();

    INSERT INTO CreditCardExpenses (UserId, CreditCardId, Description, Amount, Category, PurchaseDate, Installments, CreatedAt)
    VALUES (@UserId, @CreditCardId, @Description, @Amount, @Category, @PurchaseDate, @Installments, @CreatedAt);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

PRINT '? sp_CreateCreditCardExpense criada';
GO

-- Buscar despesas de cartão por período
CREATE OR ALTER PROCEDURE sp_GetCreditCardExpensesByPeriod
    @UserId INT,
    @CreditCardId INT,
    @Month INT = NULL,
    @Year INT = NULL,
    @Category NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
        CreditCardId,
        Description,
        Amount,
        Category,
        PurchaseDate,
        Installments,
        CreatedAt
    FROM CreditCardExpenses
    WHERE UserId = @UserId
      AND CreditCardId = @CreditCardId
      AND (@Month IS NULL OR MONTH(PurchaseDate) = @Month)
      AND (@Year IS NULL OR YEAR(PurchaseDate) = @Year)
      AND (@Category IS NULL OR Category = @Category)
    ORDER BY PurchaseDate DESC;
END
GO

PRINT '? sp_GetCreditCardExpensesByPeriod criada';
GO

-- Atualizar despesa de cartão
CREATE OR ALTER PROCEDURE sp_UpdateCreditCardExpense
    @Id INT,
    @UserId INT,
    @Description NVARCHAR(500),
    @Amount DECIMAL(18,2),
    @Category NVARCHAR(100),
    @PurchaseDate DATETIME,
    @Installments INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE CreditCardExpenses
    SET 
        Description = @Description,
        Amount = @Amount,
        Category = @Category,
        PurchaseDate = @PurchaseDate,
        Installments = @Installments
    WHERE Id = @Id AND UserId = @UserId;

    SELECT @@ROWCOUNT;
END
GO

PRINT '? sp_UpdateCreditCardExpense criada';
GO

-- Deletar despesa de cartão
CREATE OR ALTER PROCEDURE sp_DeleteCreditCardExpense
    @Id INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM CreditCardExpenses 
    WHERE Id = @Id AND UserId = @UserId;

    SELECT @@ROWCOUNT;
END
GO

PRINT '? sp_DeleteCreditCardExpense criada';
GO

-- =============================================
-- INSTALLMENT PURCHASES PROCEDURES
-- =============================================

-- Criar compra parcelada
CREATE OR ALTER PROCEDURE sp_CreateInstallmentPurchase
    @UserId INT,
    @CreditCardId INT,
    @Description NVARCHAR(500),
    @TotalAmount DECIMAL(18,2),
    @Installments INT,
    @InstallmentAmount DECIMAL(18,2),
    @PurchaseDate DATETIME,
    @Category NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar se o cartão pertence ao usuário
    IF NOT EXISTS (SELECT 1 FROM CreditCards WHERE Id = @CreditCardId AND UserId = @UserId)
    BEGIN
        RAISERROR('Cartão de crédito não encontrado ou não pertence ao usuário', 16, 1);
        RETURN;
    END

    DECLARE @CreatedAt DATETIME = GETUTCDATE();

    INSERT INTO InstallmentPurchases (UserId, CreditCardId, Description, TotalAmount, Installments, InstallmentAmount, PurchaseDate, Category, CreatedAt)
    VALUES (@UserId, @CreditCardId, @Description, @TotalAmount, @Installments, @InstallmentAmount, @PurchaseDate, @Category, @CreatedAt);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

PRINT '? sp_CreateInstallmentPurchase criada';
GO

-- Buscar compras parceladas de um cartão
CREATE OR ALTER PROCEDURE sp_GetInstallmentPurchasesByCreditCard
    @UserId INT,
    @CreditCardId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
        CreditCardId,
        Description,
        TotalAmount,
        Installments,
        InstallmentAmount,
        PurchaseDate,
        Category,
        CreatedAt
    FROM InstallmentPurchases
    WHERE UserId = @UserId AND CreditCardId = @CreditCardId
    ORDER BY PurchaseDate DESC;
END
GO

PRINT '? sp_GetInstallmentPurchasesByCreditCard criada';
GO

-- Deletar compra parcelada
CREATE OR ALTER PROCEDURE sp_DeleteInstallmentPurchase
    @Id INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM InstallmentPurchases 
    WHERE Id = @Id AND UserId = @UserId;

    SELECT @@ROWCOUNT;
END
GO

PRINT '? sp_DeleteInstallmentPurchase criada';
GO

-- =============================================
-- VERIFICAÇÃO FINAL
-- =============================================

PRINT '';
PRINT '? Todas as Stored Procedures foram criadas com sucesso!';
PRINT '';
PRINT '?? Stored Procedures criadas:';
PRINT '   - sp_CreateCreditCardExpense';
PRINT '   - sp_GetCreditCardExpensesByPeriod';
PRINT '   - sp_UpdateCreditCardExpense';
PRINT '   - sp_DeleteCreditCardExpense';
PRINT '   - sp_CreateInstallmentPurchase';
PRINT '   - sp_GetInstallmentPurchasesByCreditCard';
PRINT '   - sp_DeleteInstallmentPurchase';
PRINT '';
PRINT '?? Próximos passos:';
PRINT '   1. Inicie a API: dotnet run';
PRINT '   2. Teste POST /api/auth/register';
PRINT '   3. Teste POST /api/auth/login';
PRINT '   4. Teste endpoints de CreditCard com token';
GO
