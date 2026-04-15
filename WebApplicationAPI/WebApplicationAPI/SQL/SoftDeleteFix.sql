-- =============================================
-- SOFT DELETE - CORREÇÃO COMPLETA
-- Execute este script para corrigir todos os erros
-- =============================================

USE WebAppDB;
GO

PRINT '?? Corrigindo implementação de Soft Delete...';
PRINT '================================================';
GO

-- =============================================
-- PARTE 1: ADICIONAR COLUNAS EM EXPENSES
-- =============================================

PRINT '';
PRINT '?? Corrigindo tabela Expenses...';

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

-- Criar índice
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Expenses_DeletedAt' AND object_id = OBJECT_ID('Expenses'))
BEGIN
    CREATE INDEX IX_Expenses_DeletedAt ON Expenses(DeletedAt) WHERE DeletedAt IS NOT NULL;
    PRINT '? Índice IX_Expenses_DeletedAt criado';
END
GO

-- =============================================
-- PARTE 2: VERIFICAR E CORRIGIR CREDITCARDS
-- =============================================

PRINT '';
PRINT '?? Verificando tabela CreditCards...';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CreditCards')
BEGIN
    -- Verificar se DeletedAt existe
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CreditCards') AND name = 'DeletedAt')
    BEGIN
        ALTER TABLE CreditCards ADD DeletedAt DATETIME NULL;
        PRINT '? CreditCards.DeletedAt adicionada';
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CreditCards') AND name = 'DeletedBy')
    BEGIN
        ALTER TABLE CreditCards ADD DeletedBy INT NULL;
        PRINT '? CreditCards.DeletedBy adicionada';
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CreditCards') AND name = 'CreatedBy')
    BEGIN
        ALTER TABLE CreditCards ADD CreatedBy INT NULL;
        PRINT '? CreditCards.CreatedBy adicionada';
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CreditCards') AND name = 'UpdatedBy')
    BEGIN
        ALTER TABLE CreditCards ADD UpdatedBy INT NULL;
        PRINT '? CreditCards.UpdatedBy adicionada';
    END

    -- Criar índice
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CreditCards_DeletedAt' AND object_id = OBJECT_ID('CreditCards'))
    BEGIN
        CREATE INDEX IX_CreditCards_DeletedAt ON CreditCards(DeletedAt) WHERE DeletedAt IS NOT NULL;
        PRINT '? Índice IX_CreditCards_DeletedAt criado';
    END
END
GO

PRINT '';
PRINT '? Colunas adicionadas/verificadas com sucesso!';
PRINT '';
GO

-- =============================================
-- PARTE 3: STORED PROCEDURES DE SOFT DELETE
-- =============================================

PRINT '?? Criando Stored Procedures de Soft Delete...';
PRINT '================================================';
GO

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

PRINT '? sp_SoftDeleteExpense criada';
GO

-- Restaurar Expense
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

PRINT '? sp_RestoreExpense criada';
GO

-- Buscar Expenses (com opção de incluir deletados)
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
        UpdatedBy,
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

PRINT '? sp_GetExpensesByPeriod atualizada';
GO

-- Atualizar Expense (com UpdatedBy)
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

PRINT '? sp_UpdateExpense atualizada';
GO

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

PRINT '? sp_SoftDeleteCreditCard criada';
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

PRINT '? sp_RestoreCreditCard criada';
GO

-- Buscar CreditCards (com opção de incluir deletados)
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
        CreatedBy,
        UpdatedAt,
        UpdatedBy,
        DeletedAt,
        DeletedBy
    FROM CreditCards
    WHERE UserId = @UserId
      AND (@OnlyActive = 0 OR IsActive = 1)
      AND (@IncludeDeleted = 1 OR DeletedAt IS NULL)
    ORDER BY CreatedAt DESC;
END
GO

PRINT '? sp_GetCreditCardsByUserId atualizada';
GO

-- Soft Delete de User
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
        DeletedBy = ISNULL(@DeletedBy, @Id),
        IsActive = 0
    WHERE Id = @Id
      AND DeletedAt IS NULL;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

PRINT '? sp_SoftDeleteUser criada';
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

PRINT '? sp_RestoreUser criada';
GO

-- =============================================
-- PARTE 4: VIEW DE AUDITORIA
-- =============================================

PRINT '';
PRINT '?? Criando View de Auditoria...';
GO

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

PRINT '? View vw_AuditLog criada';
GO

-- =============================================
-- VERIFICAÇÃO FINAL
-- =============================================

PRINT '';
PRINT '? Soft Delete implementado com sucesso!';
PRINT '';
PRINT '?? Resumo:';
PRINT '   ? Colunas adicionadas em todas as tabelas';
PRINT '   ? Stored Procedures criadas/atualizadas';
PRINT '   ? View de auditoria criada';
PRINT '';
PRINT '?? Stored Procedures disponíveis:';
PRINT '   - sp_SoftDeleteExpense';
PRINT '   - sp_RestoreExpense';
PRINT '   - sp_SoftDeleteCreditCard';
PRINT '   - sp_RestoreCreditCard';
PRINT '   - sp_SoftDeleteUser';
PRINT '   - sp_RestoreUser';
PRINT '';
PRINT '?? Para ver dados deletados:';
PRINT '   SELECT * FROM vw_AuditLog ORDER BY DeletedAt DESC;';
PRINT '';
PRINT '?? Próximos passos:';
PRINT '   1. Models já foram atualizados com as novas colunas';
PRINT '   2. Inicie a API e teste: dotnet run';
PRINT '   3. Consulte SOFT_DELETE_GUIDE.md para exemplos de uso';
GO
