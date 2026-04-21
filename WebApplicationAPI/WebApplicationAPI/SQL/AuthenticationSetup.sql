-- =============================================
-- SCRIPT DE AUTENTICAÇÃO E MULTI-TENANCY
-- Execute este script para adicionar autenticação ao sistema
-- =============================================

USE WebAppDB;
GO

-- =============================================
-- PARTE 1: ATUALIZAR TABELA USERS
-- =============================================

-- Verificar se as colunas já existem antes de adicionar
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'PasswordHash')
BEGIN
    ALTER TABLE Users ADD PasswordHash NVARCHAR(255) NULL;
    PRINT 'Coluna PasswordHash adicionada à tabela Users';
END
ELSE
BEGIN
    PRINT 'Coluna PasswordHash já existe na tabela Users';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'Role')
BEGIN
    ALTER TABLE Users ADD Role NVARCHAR(50) NOT NULL DEFAULT 'User';
    PRINT 'Coluna Role adicionada à tabela Users';
END
ELSE
BEGIN
    PRINT 'Coluna Role já existe na tabela Users';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'IsActive')
BEGIN
    ALTER TABLE Users ADD IsActive BIT NOT NULL DEFAULT 1;
    PRINT 'Coluna IsActive adicionada à tabela Users';
END
ELSE
BEGIN
    PRINT 'Coluna IsActive já existe na tabela Users';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE Users ADD CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE();
    PRINT 'Coluna CreatedAt adicionada à tabela Users';
END
ELSE
BEGIN
    PRINT 'Coluna CreatedAt já existe na tabela Users';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE Users ADD UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE();
    PRINT 'Coluna UpdatedAt adicionada à tabela Users';
END
ELSE
BEGIN
    PRINT 'Coluna UpdatedAt já existe na tabela Users';
END
GO

-- =============================================
-- PARTE 2: ADICIONAR USERID EM EXPENSES
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Expenses') AND name = 'UserId')
BEGIN
    ALTER TABLE Expenses ADD UserId INT NULL;
    PRINT 'Coluna UserId adicionada à tabela Expenses';

    -- Criar índice para melhor performance
    CREATE INDEX IX_Expenses_UserId ON Expenses(UserId);
    PRINT 'Índice IX_Expenses_UserId criado';
END
ELSE
BEGIN
    PRINT 'Coluna UserId já existe na tabela Expenses';
END
GO

-- =============================================
-- PARTE 3: ADICIONAR USERID EM CREDITCARDS
-- =============================================

-- Verificar se a tabela CreditCards existe
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CreditCards')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CreditCards') AND name = 'UserId')
    BEGIN
        ALTER TABLE CreditCards ADD UserId INT NULL;
        PRINT 'Coluna UserId adicionada à tabela CreditCards';

        -- Criar índice para melhor performance
        CREATE INDEX IX_CreditCards_UserId ON CreditCards(UserId);
        PRINT 'Índice IX_CreditCards_UserId criado';
    END
    ELSE
    BEGIN
        PRINT 'Coluna UserId já existe na tabela CreditCards';
    END
END
ELSE
BEGIN
    PRINT 'Tabela CreditCards não existe ainda';
END
GO

-- =============================================
-- PARTE 4: ADICIONAR USERID EM OUTRAS TABELAS
-- =============================================

-- MonthlyFinancialControl
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'MonthlyFinancialControl')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('MonthlyFinancialControl') AND name = 'UserId')
    BEGIN
        ALTER TABLE MonthlyFinancialControl ADD UserId INT NULL;
        CREATE INDEX IX_MonthlyFinancialControl_UserId ON MonthlyFinancialControl(UserId);
        PRINT 'Coluna UserId adicionada à tabela MonthlyFinancialControl';
    END
END
GO

-- InstallmentPurchases
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'InstallmentPurchases')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InstallmentPurchases') AND name = 'UserId')
    BEGIN
        ALTER TABLE InstallmentPurchases ADD UserId INT NULL;
        CREATE INDEX IX_InstallmentPurchases_UserId ON InstallmentPurchases(UserId);
        PRINT 'Coluna UserId adicionada à tabela InstallmentPurchases';
    END
END
GO

-- CreditCardExpenses
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CreditCardExpenses')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CreditCardExpenses') AND name = 'UserId')
    BEGIN
        ALTER TABLE CreditCardExpenses ADD UserId INT NULL;
        CREATE INDEX IX_CreditCardExpenses_UserId ON CreditCardExpenses(UserId);
        PRINT 'Coluna UserId adicionada à tabela CreditCardExpenses';
    END
END
GO

-- =============================================
-- PARTE 5: STORED PROCEDURES DE AUTENTICAÇÃO
-- =============================================

-- Procedure para criar usuário (com autenticação)
CREATE OR ALTER PROCEDURE sp_CreateUser
    @Name NVARCHAR(100),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(255) = NULL,
    @Role NVARCHAR(50) = 'User',
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    -- Verificar se email já existe
    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
    BEGIN
        THROW 50001, 'Email já cadastrado', 1;
        RETURN;
    END

    DECLARE @CreatedAt DATETIME = GETUTCDATE();
    DECLARE @UpdatedAt DATETIME = GETUTCDATE();

    INSERT INTO Users (Name, Email, PasswordHash, Role, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Email, @PasswordHash, @Role, @IsActive, @CreatedAt, @UpdatedAt);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

-- Procedure para atualizar usuário
CREATE OR ALTER PROCEDURE sp_UpdateUser
    @Id INT,
    @Name NVARCHAR(100),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(255) = NULL,
    @Role NVARCHAR(50) = NULL,
    @IsActive BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UpdatedAt DATETIME = GETUTCDATE();

    UPDATE Users
    SET 
        Name = @Name,
        Email = @Email,
        PasswordHash = ISNULL(@PasswordHash, PasswordHash),
        Role = ISNULL(@Role, Role),
        IsActive = ISNULL(@IsActive, IsActive),
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;

    SELECT @@ROWCOUNT;
END
GO

-- Procedure para buscar usuário por email (para login)
CREATE OR ALTER PROCEDURE sp_GetUserByEmail
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        Name,
        Email,
        PasswordHash,
        Role,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Users
    WHERE Email = @Email;
END
GO

-- Procedure para buscar usuário por ID
CREATE OR ALTER PROCEDURE sp_GetUserById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        Name,
        Email,
        PasswordHash,
        Role,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Users
    WHERE Id = @Id;
END
GO

-- Procedure para listar todos os usuários
CREATE OR ALTER PROCEDURE sp_GetAllUsers
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        Name,
        Email,
        PasswordHash,
        Role,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM Users
    ORDER BY CreatedAt DESC;
END
GO

-- Procedure para deletar usuário
CREATE OR ALTER PROCEDURE sp_DeleteUser
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Users WHERE Id = @Id;

    SELECT @@ROWCOUNT;
END
GO

-- =============================================
-- PARTE 6: ATUALIZAR STORED PROCEDURES EXISTENTES
-- =============================================

-- Atualizar sp_CreateExpense para incluir UserId
CREATE OR ALTER PROCEDURE sp_CreateExpense
    @UserId INT,
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

    INSERT INTO Expenses (UserId, Month, Year, Description, Amount, Category, Status, PaymentMethod, CreatedAt)
    VALUES (@UserId, @Month, @Year, @Description, @Amount, @Category, @Status, @PaymentMethod, @CreatedAt);

    SET @Id = CAST(SCOPE_IDENTITY() AS INT);
END
GO

-- Atualizar sp_UpdateExpense para incluir UserId
CREATE OR ALTER PROCEDURE sp_UpdateExpense
    @Id INT,
    @UserId INT,
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
    WHERE Id = @Id AND UserId = @UserId;

    SELECT @@ROWCOUNT;
END
GO

-- Atualizar sp_GetExpensesByPeriod para filtrar por UserId
CREATE OR ALTER PROCEDURE sp_GetExpensesByPeriod
    @UserId INT,
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
        Month,
        Year,
        Description,
        Amount,
        Category,
        Status,
        PaymentMethod,
        CreatedAt
    FROM Expenses
    WHERE UserId = @UserId 
      AND Month = @Month 
      AND Year = @Year
    ORDER BY CreatedAt DESC;
END
GO

-- Atualizar sp_DeleteExpense para verificar UserId
CREATE OR ALTER PROCEDURE sp_DeleteExpense
    @Id INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Expenses 
    WHERE Id = @Id AND UserId = @UserId;

    SELECT @@ROWCOUNT;
END
GO

-- Procedure para buscar todas as expenses de um usuário
CREATE OR ALTER PROCEDURE sp_GetExpensesByUserId
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
        Month,
        Year,
        Description,
        Amount,
        Category,
        Status,
        PaymentMethod,
        CreatedAt
    FROM Expenses
    WHERE UserId = @UserId
    ORDER BY Year DESC, Month DESC, CreatedAt DESC;
END
GO

PRINT '? Script de autenticação executado com sucesso!';
PRINT '?? Próximos passos:';
PRINT '   1. Execute o sistema e teste as rotas /api/auth/register e /api/auth/login';
PRINT '   2. Atualize as procedures de CreditCard e outras entidades conforme necessário';
PRINT '   3. Adicione [Authorize] nos controllers que precisam de proteção';
GO
