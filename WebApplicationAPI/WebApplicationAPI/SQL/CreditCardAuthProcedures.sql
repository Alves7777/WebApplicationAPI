-- =============================================
-- STORED PROCEDURES DE CREDITCARD COM USERID
-- Execute após o AuthenticationSetup.sql
-- =============================================

USE WebAppDB;
GO

-- =============================================
-- CREDITCARD PROCEDURES
-- =============================================

-- Criar cartão de crédito
CREATE OR ALTER PROCEDURE sp_CreateCreditCard
    @UserId INT,
    @Name NVARCHAR(100),
    @Brand NVARCHAR(50) = NULL,
    @CardLimit DECIMAL(18,2),
    @ClosingDay INT = NULL,
    @DueDay INT = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CreatedAt DATETIME = GETUTCDATE();
    DECLARE @UpdatedAt DATETIME = GETUTCDATE();

    INSERT INTO CreditCards (UserId, Name, Brand, CardLimit, ClosingDay, DueDay, IsActive, CreatedAt, UpdatedAt)
    VALUES (@UserId, @Name, @Brand, @CardLimit, @ClosingDay, @DueDay, @IsActive, @CreatedAt, @UpdatedAt);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

-- Atualizar cartão de crédito
CREATE OR ALTER PROCEDURE sp_UpdateCreditCard
    @Id INT,
    @UserId INT,
    @Name NVARCHAR(100),
    @Brand NVARCHAR(50) = NULL,
    @CardLimit DECIMAL(18,2),
    @ClosingDay INT = NULL,
    @DueDay INT = NULL,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UpdatedAt DATETIME = GETUTCDATE();

    UPDATE CreditCards
    SET 
        Name = @Name,
        Brand = @Brand,
        CardLimit = @CardLimit,
        ClosingDay = @ClosingDay,
        DueDay = @DueDay,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id AND UserId = @UserId;

    SELECT @@ROWCOUNT;
END
GO

-- Deletar cartão de crédito
CREATE OR ALTER PROCEDURE sp_DeleteCreditCard
    @Id INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM CreditCards 
    WHERE Id = @Id AND UserId = @UserId;

    SELECT @@ROWCOUNT;
END
GO

-- Buscar cartão por ID e UserId
CREATE OR ALTER PROCEDURE sp_GetCreditCardById
    @Id INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
        Name,
        Brand,
        CardLimit,
        ClosingDay,
        DueDay,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM CreditCards
    WHERE Id = @Id AND UserId = @UserId;
END
GO

-- Buscar todos os cartões de um usuário
CREATE OR ALTER PROCEDURE sp_GetCreditCardsByUserId
    @UserId INT,
    @OnlyActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
        Name,
        Brand,
        CardLimit,
        ClosingDay,
        DueDay,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM CreditCards
    WHERE UserId = @UserId
      AND (@OnlyActive = 0 OR IsActive = 1)
    ORDER BY CreatedAt DESC;
END
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
        THROW 50002, 'Cartão de crédito não encontrado ou não pertence ao usuário', 1;
        RETURN;
    END

    DECLARE @CreatedAt DATETIME = GETUTCDATE();

    INSERT INTO CreditCardExpenses (UserId, CreditCardId, Description, Amount, Category, PurchaseDate, Installments, CreatedAt)
    VALUES (@UserId, @CreditCardId, @Description, @Amount, @Category, @PurchaseDate, @Installments, @CreatedAt);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
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
        THROW 50002, 'Cartão de crédito não encontrado ou não pertence ao usuário', 1;
        RETURN;
    END

    DECLARE @CreatedAt DATETIME = GETUTCDATE();

    INSERT INTO InstallmentPurchases (UserId, CreditCardId, Description, TotalAmount, Installments, InstallmentAmount, PurchaseDate, Category, CreatedAt)
    VALUES (@UserId, @CreditCardId, @Description, @TotalAmount, @Installments, @InstallmentAmount, @PurchaseDate, @Category, @CreatedAt);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
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

PRINT '? Stored Procedures de CreditCard criadas com sucesso!';
GO
