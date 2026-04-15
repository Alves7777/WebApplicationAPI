-- =============================================
-- CORREÇÃO: ADICIONAR COLUNAS FALTANTES
-- Execute este script para adicionar colunas que estavam faltando
-- =============================================

USE WebAppDB;
GO

PRINT '?? Corrigindo colunas faltantes...';
PRINT '====================================';
GO

-- =============================================
-- TABELA: CreditCardExpenses
-- =============================================

PRINT '';
PRINT '?? Verificando tabela CreditCardExpenses...';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CreditCardExpenses')
BEGIN
    -- Adicionar coluna Installments
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CreditCardExpenses') AND name = 'Installments')
    BEGIN
        ALTER TABLE CreditCardExpenses ADD Installments INT NOT NULL DEFAULT 1;
        PRINT '? CreditCardExpenses.Installments adicionada';
    END
    ELSE
        PRINT '??  CreditCardExpenses.Installments já existe';

    -- Adicionar coluna PurchaseDate se não existir
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CreditCardExpenses') AND name = 'PurchaseDate')
    BEGIN
        ALTER TABLE CreditCardExpenses ADD PurchaseDate DATETIME NOT NULL DEFAULT GETUTCDATE();
        PRINT '? CreditCardExpenses.PurchaseDate adicionada';
    END
    ELSE
        PRINT '??  CreditCardExpenses.PurchaseDate já existe';
END
ELSE
BEGIN
    PRINT '? Tabela CreditCardExpenses não existe!';
    PRINT '   Crie a tabela primeiro ou execute o setup completo do CreditCard';
END
GO

-- =============================================
-- TABELA: InstallmentPurchases
-- =============================================

PRINT '';
PRINT '?? Verificando tabela InstallmentPurchases...';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'InstallmentPurchases')
BEGIN
    -- Adicionar coluna Installments
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InstallmentPurchases') AND name = 'Installments')
    BEGIN
        ALTER TABLE InstallmentPurchases ADD Installments INT NOT NULL DEFAULT 1;
        PRINT '? InstallmentPurchases.Installments adicionada';
    END
    ELSE
        PRINT '??  InstallmentPurchases.Installments já existe';

    -- Adicionar coluna PurchaseDate
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InstallmentPurchases') AND name = 'PurchaseDate')
    BEGIN
        ALTER TABLE InstallmentPurchases ADD PurchaseDate DATETIME NOT NULL DEFAULT GETUTCDATE();
        PRINT '? InstallmentPurchases.PurchaseDate adicionada';
    END
    ELSE
        PRINT '??  InstallmentPurchases.PurchaseDate já existe';

    -- Adicionar coluna Category
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InstallmentPurchases') AND name = 'Category')
    BEGIN
        ALTER TABLE InstallmentPurchases ADD Category NVARCHAR(100) NULL;
        PRINT '? InstallmentPurchases.Category adicionada';
    END
    ELSE
        PRINT '??  InstallmentPurchases.Category já existe';

    -- Adicionar coluna InstallmentAmount se não existir
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InstallmentPurchases') AND name = 'InstallmentAmount')
    BEGIN
        ALTER TABLE InstallmentPurchases ADD InstallmentAmount DECIMAL(18,2) NOT NULL DEFAULT 0;
        PRINT '? InstallmentPurchases.InstallmentAmount adicionada';
    END
    ELSE
        PRINT '??  InstallmentPurchases.InstallmentAmount já existe';
END
ELSE
BEGIN
    PRINT '? Tabela InstallmentPurchases não existe!';
    PRINT '   Crie a tabela primeiro ou execute o setup completo do CreditCard';
END
GO

-- =============================================
-- RE-CRIAR STORED PROCEDURES COM COLUNAS CORRETAS
-- =============================================

PRINT '';
PRINT '?? Re-criando Stored Procedures...';
PRINT '====================================';
GO

-- Criar despesa de cartão (CORRIGIDO)
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

PRINT '? sp_CreateCreditCardExpense re-criada';

-- Buscar despesas de cartão por período (CORRIGIDO)
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

PRINT '? sp_GetCreditCardExpensesByPeriod re-criada';

-- Atualizar despesa de cartão (CORRIGIDO)
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

PRINT '? sp_UpdateCreditCardExpense re-criada';

-- Criar compra parcelada (CORRIGIDO)
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

PRINT '? sp_CreateInstallmentPurchase re-criada';

-- Buscar compras parceladas de um cartão (CORRIGIDO)
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

PRINT '? sp_GetInstallmentPurchasesByCreditCard re-criada';

PRINT '';
PRINT '? Correção concluída com sucesso!';
PRINT '';
PRINT '?? Resumo:';
PRINT '   - Colunas adicionadas nas tabelas';
PRINT '   - Stored Procedures re-criadas sem erros';
PRINT '';
PRINT '?? Próximos passos:';
PRINT '   1. Verifique as tabelas: SELECT * FROM CreditCardExpenses';
PRINT '   2. Verifique as tabelas: SELECT * FROM InstallmentPurchases';
PRINT '   3. Teste as Stored Procedures';
PRINT '   4. Inicie a API e teste os endpoints';
GO
