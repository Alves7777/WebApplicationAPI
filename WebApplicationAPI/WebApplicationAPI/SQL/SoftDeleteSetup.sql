-- =============================================
-- SOFT DELETE (EXCLUSÃO LÓGICA) - SETUP
-- Execute após AuthenticationSetup.sql
-- =============================================

USE WebAppDB;
GO

PRINT '??? Implementando Soft Delete (Exclusão Lógica)...';
PRINT '================================================';
GO

-- =============================================
-- PARTE 1: ADICIONAR COLUNAS DE AUDITORIA
-- =============================================

-- ============================================
-- TABELA: Users
-- ============================================
PRINT '';
PRINT '?? Atualizando tabela Users...';

-- DeletedAt
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE Users ADD DeletedAt DATETIME NULL;
    PRINT '? Users.DeletedAt adicionada';
END
ELSE
    PRINT '??  Users.DeletedAt já existe';

-- DeletedBy
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'DeletedBy')
BEGIN
    ALTER TABLE Users ADD DeletedBy INT NULL;
    PRINT '? Users.DeletedBy adicionada';
END
ELSE
    PRINT '??  Users.DeletedBy já existe';

-- CreatedBy
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE Users ADD CreatedBy INT NULL;
    PRINT '? Users.CreatedBy adicionada';
END
ELSE
    PRINT '??  Users.CreatedBy já existe';

-- UpdatedBy
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Users ADD UpdatedBy INT NULL;
    PRINT '? Users.UpdatedBy adicionada';
END
ELSE
    PRINT '??  Users.UpdatedBy já existe';

GO

-- ============================================
-- TABELA: Expenses
-- ============================================
PRINT '';
PRINT '?? Atualizando tabela Expenses...';

-- DeletedAt
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Expenses') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE Expenses ADD DeletedAt DATETIME NULL;
    PRINT '? Expenses.DeletedAt adicionada';
END
ELSE
    PRINT '??  Expenses.DeletedAt já existe';

-- DeletedBy
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Expenses') AND name = 'DeletedBy')
BEGIN
    ALTER TABLE Expenses ADD DeletedBy INT NULL;
    PRINT '? Expenses.DeletedBy adicionada';
END
ELSE
    PRINT '??  Expenses.DeletedBy já existe';

-- UpdatedAt
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Expenses') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE Expenses ADD UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE();
    PRINT '? Expenses.UpdatedAt adicionada';
END
ELSE
    PRINT '??  Expenses.UpdatedAt já existe';

-- UpdatedBy
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Expenses') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Expenses ADD UpdatedBy INT NULL;
    PRINT '? Expenses.UpdatedBy adicionada';
END
ELSE
    PRINT '??  Expenses.UpdatedBy já existe';

-- Criar índice para melhor performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Expenses_DeletedAt')
BEGIN
    CREATE INDEX IX_Expenses_DeletedAt ON Expenses(DeletedAt) WHERE DeletedAt IS NOT NULL;
    PRINT '? Índice IX_Expenses_DeletedAt criado';
END

GO

-- ============================================
-- TABELA: CreditCards
-- ============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CreditCards')
BEGIN
    PRINT '';
    PRINT '?? Atualizando tabela CreditCards...';

    -- DeletedAt
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CreditCards') AND name = 'DeletedAt')
    BEGIN
        ALTER TABLE CreditCards ADD DeletedAt DATETIME NULL;
        PRINT '? CreditCards.DeletedAt adicionada';
    END
    ELSE
        PRINT '??  CreditCards.DeletedAt já existe';

    -- DeletedBy
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CreditCards') AND name = 'DeletedBy')
    BEGIN
        ALTER TABLE CreditCards ADD DeletedBy INT NULL;
        PRINT '? CreditCards.DeletedBy adicionada';
    END
    ELSE
        PRINT '??  CreditCards.DeletedBy já existe';

    -- CreatedBy
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CreditCards') AND name = 'CreatedBy')
    BEGIN
        ALTER TABLE CreditCards ADD CreatedBy INT NULL;
        PRINT '? CreditCards.CreatedBy adicionada';
    END
    ELSE
        PRINT '??  CreditCards.CreatedBy já existe';

    -- UpdatedBy
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CreditCards') AND name = 'UpdatedBy')
    BEGIN
        ALTER TABLE CreditCards ADD UpdatedBy INT NULL;
        PRINT '? CreditCards.UpdatedBy adicionada';
    END
    ELSE
        PRINT '??  CreditCards.UpdatedBy já existe';

    -- Criar índice
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CreditCards_DeletedAt')
    BEGIN
        CREATE INDEX IX_CreditCards_DeletedAt ON CreditCards(DeletedAt) WHERE DeletedAt IS NOT NULL;
        PRINT '? Índice IX_CreditCards_DeletedAt criado';
    END
END
GO

-- ============================================
-- TABELA: CreditCardExpenses
-- ============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CreditCardExpenses')
BEGIN
    PRINT '';
    PRINT '?? Atualizando tabela CreditCardExpenses...';

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CreditCardExpenses') AND name = 'DeletedAt')
    BEGIN
        ALTER TABLE CreditCardExpenses ADD DeletedAt DATETIME NULL;
        ALTER TABLE CreditCardExpenses ADD DeletedBy INT NULL;
        ALTER TABLE CreditCardExpenses ADD UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE();
        ALTER TABLE CreditCardExpenses ADD UpdatedBy INT NULL;
        PRINT '? CreditCardExpenses colunas adicionadas';
    END
END
GO

-- ============================================
-- TABELA: InstallmentPurchases
-- ============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'InstallmentPurchases')
BEGIN
    PRINT '';
    PRINT '?? Atualizando tabela InstallmentPurchases...';

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InstallmentPurchases') AND name = 'DeletedAt')
    BEGIN
        ALTER TABLE InstallmentPurchases ADD DeletedAt DATETIME NULL;
        ALTER TABLE InstallmentPurchases ADD DeletedBy INT NULL;
        ALTER TABLE InstallmentPurchases ADD UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE();
        ALTER TABLE InstallmentPurchases ADD UpdatedBy INT NULL;
        PRINT '? InstallmentPurchases colunas adicionadas';
    END
END
GO

-- ============================================
-- TABELA: MonthlyFinancialControl
-- ============================================
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'MonthlyFinancialControl')
BEGIN
    PRINT '';
    PRINT '?? Atualizando tabela MonthlyFinancialControl...';

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('MonthlyFinancialControl') AND name = 'DeletedAt')
    BEGIN
        ALTER TABLE MonthlyFinancialControl ADD DeletedAt DATETIME NULL;
        ALTER TABLE MonthlyFinancialControl ADD DeletedBy INT NULL;
        ALTER TABLE MonthlyFinancialControl ADD UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE();
        ALTER TABLE MonthlyFinancialControl ADD UpdatedBy INT NULL;
        PRINT '? MonthlyFinancialControl colunas adicionadas';
    END
END
GO

-- =============================================
-- PARTE 2: STORED PROCEDURES COM SOFT DELETE
-- =============================================

PRINT '';
PRINT '?? Criando/Atualizando Stored Procedures...';
PRINT '==========================================';

-- ============================================
-- EXPENSES - SOFT DELETE
-- ============================================

-- Soft Delete de Expense
CREATE OR ALTER PROCEDURE sp_SoftDeleteExpense
    @Id INT,
    @UserId INT,
    @DeletedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DeletedAt DATETIME = GETUTCDATE();

    UPDATE Expenses
    SET 
        DeletedAt = @DeletedAt,
        DeletedBy = @DeletedBy
    WHERE Id = @Id 
      AND UserId = @UserId
      AND DeletedAt IS NULL;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- Restaurar Expense deletada
CREATE OR ALTER PROCEDURE sp_RestoreExpense
    @Id INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Expenses
    SET 
        DeletedAt = NULL,
        DeletedBy = NULL
    WHERE Id = @Id 
      AND UserId = @UserId
      AND DeletedAt IS NOT NULL;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- Buscar Expenses (excluindo deletadas)
CREATE OR ALTER PROCEDURE sp_GetExpensesByPeriod
    @UserId INT,
    @Month INT,
    @Year INT,
    @IncludeDeleted BIT = 0
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
        CreatedAt,
        UpdatedAt,
        DeletedAt,
        DeletedBy
    FROM Expenses
    WHERE UserId = @UserId 
      AND Month = @Month 
      AND Year = @Year
      AND (@IncludeDeleted = 1 OR DeletedAt IS NULL)
    ORDER BY CreatedAt DESC;
END
GO

-- Atualizar Expense (com UpdatedAt e UpdatedBy)
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
    @UpdatedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UpdatedAt DATETIME = GETUTCDATE();

    UPDATE Expenses
    SET 
        Month = @Month,
        Year = @Year,
        Description = @Description,
        Amount = @Amount,
        Category = @Category,
        Status = @Status,
        PaymentMethod = @PaymentMethod,
        UpdatedAt = @UpdatedAt,
        UpdatedBy = @UpdatedBy
    WHERE Id = @Id 
      AND UserId = @UserId
      AND DeletedAt IS NULL;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- ============================================
-- CREDITCARDS - SOFT DELETE
-- ============================================

-- Soft Delete de CreditCard
CREATE OR ALTER PROCEDURE sp_SoftDeleteCreditCard
    @Id INT,
    @UserId INT,
    @DeletedBy INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DeletedAt DATETIME = GETUTCDATE();

    UPDATE CreditCards
    SET 
        DeletedAt = @DeletedAt,
        DeletedBy = @DeletedBy,
        IsActive = 0
    WHERE Id = @Id 
      AND UserId = @UserId
      AND DeletedAt IS NULL;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- Restaurar CreditCard
CREATE OR ALTER PROCEDURE sp_RestoreCreditCard
    @Id INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE CreditCards
    SET 
        DeletedAt = NULL,
        DeletedBy = NULL,
        IsActive = 1
    WHERE Id = @Id 
      AND UserId = @UserId
      AND DeletedAt IS NOT NULL;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- Buscar CreditCards (excluindo deletados)
CREATE OR ALTER PROCEDURE sp_GetCreditCardsByUserId
    @UserId INT,
    @OnlyActive BIT = 1,
    @IncludeDeleted BIT = 0
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
        UpdatedAt,
        DeletedAt,
        DeletedBy
    FROM CreditCards
    WHERE UserId = @UserId
      AND (@OnlyActive = 0 OR IsActive = 1)
      AND (@IncludeDeleted = 1 OR DeletedAt IS NULL)
    ORDER BY CreatedAt DESC;
END
GO

-- ============================================
-- USERS - SOFT DELETE
-- ============================================

-- Soft Delete de User (desativar conta)
CREATE OR ALTER PROCEDURE sp_SoftDeleteUser
    @Id INT,
    @DeletedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DeletedAt DATETIME = GETUTCDATE();

    UPDATE Users
    SET 
        DeletedAt = @DeletedAt,
        DeletedBy = ISNULL(@DeletedBy, @Id), -- Se não informado, assume que o próprio usuário se deletou
        IsActive = 0
    WHERE Id = @Id
      AND DeletedAt IS NULL;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- Restaurar User
CREATE OR ALTER PROCEDURE sp_RestoreUser
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET 
        DeletedAt = NULL,
        DeletedBy = NULL,
        IsActive = 1
    WHERE Id = @Id
      AND DeletedAt IS NOT NULL;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- =============================================
-- PARTE 3: VIEW PARA AUDITORIA
-- =============================================

PRINT '';
PRINT '?? Criando Views de Auditoria...';

-- View de auditoria de todas as mudanças
CREATE OR ALTER VIEW vw_AuditLog AS
SELECT 
    'Expense' AS TableName,
    Id AS RecordId,
    UserId,
    DeletedAt,
    DeletedBy,
    CreatedAt,
    UpdatedAt AS LastUpdatedAt
FROM Expenses
WHERE DeletedAt IS NOT NULL

UNION ALL

SELECT 
    'CreditCard' AS TableName,
    Id AS RecordId,
    UserId,
    DeletedAt,
    DeletedBy,
    CreatedAt,
    UpdatedAt AS LastUpdatedAt
FROM CreditCards
WHERE DeletedAt IS NOT NULL

UNION ALL

SELECT 
    'User' AS TableName,
    Id AS RecordId,
    Id AS UserId,
    DeletedAt,
    DeletedBy,
    CreatedAt,
    UpdatedAt AS LastUpdatedAt
FROM Users
WHERE DeletedAt IS NOT NULL;
GO

PRINT '';
PRINT '? Soft Delete implementado com sucesso!';
PRINT '';
PRINT '?? Novos recursos disponíveis:';
PRINT '   - Exclusão lógica (dados não são deletados)';
PRINT '   - Rastreamento de quem criou/atualizou/deletou';
PRINT '   - Possibilidade de restaurar dados';
PRINT '   - View de auditoria: SELECT * FROM vw_AuditLog';
PRINT '';
PRINT '?? Stored Procedures criadas:';
PRINT '   - sp_SoftDeleteExpense';
PRINT '   - sp_RestoreExpense';
PRINT '   - sp_SoftDeleteCreditCard';
PRINT '   - sp_RestoreCreditCard';
PRINT '   - sp_SoftDeleteUser';
PRINT '   - sp_RestoreUser';
PRINT '';
PRINT '?? Próximos passos:';
PRINT '   1. Atualizar Models no .NET para incluir novas colunas';
PRINT '   2. Atualizar Repositories para usar sp_SoftDelete*';
PRINT '   3. Adicionar endpoints de restauração nos Controllers';
GO
