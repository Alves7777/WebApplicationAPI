-- =============================================
-- ADICIONAR COLUNAS DE SOFT DELETE - SIMPLES
-- Execute este script para adicionar as colunas que faltam
-- =============================================

USE WebAppDB;
GO

PRINT '?? Adicionando colunas de Soft Delete...';
PRINT '==========================================';
GO

-- =============================================
-- EXPENSES
-- =============================================

PRINT '';
PRINT '?? Tabela: Expenses';

-- UpdatedAt
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Expenses') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE Expenses ADD UpdatedAt DATETIME NOT NULL CONSTRAINT DF_Expenses_UpdatedAt DEFAULT GETUTCDATE();
    PRINT '? Expenses.UpdatedAt adicionada';
END
ELSE
    PRINT '? Expenses.UpdatedAt já existe';
GO

-- UpdatedBy
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Expenses') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Expenses ADD UpdatedBy INT NULL;
    PRINT '? Expenses.UpdatedBy adicionada';
END
ELSE
    PRINT '? Expenses.UpdatedBy já existe';
GO

-- DeletedAt
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Expenses') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE Expenses ADD DeletedAt DATETIME NULL;
    PRINT '? Expenses.DeletedAt adicionada';
END
ELSE
    PRINT '? Expenses.DeletedAt já existe';
GO

-- DeletedBy
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Expenses') AND name = 'DeletedBy')
BEGIN
    ALTER TABLE Expenses ADD DeletedBy INT NULL;
    PRINT '? Expenses.DeletedBy adicionada';
END
ELSE
    PRINT '? Expenses.DeletedBy já existe';
GO

-- =============================================
-- CREDITCARDS
-- =============================================

PRINT '';
PRINT '?? Tabela: CreditCards';

-- CreatedBy
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CreditCards') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE CreditCards ADD CreatedBy INT NULL;
    PRINT '? CreditCards.CreatedBy adicionada';
END
ELSE
    PRINT '? CreditCards.CreatedBy já existe';
GO

-- UpdatedBy
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CreditCards') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE CreditCards ADD UpdatedBy INT NULL;
    PRINT '? CreditCards.UpdatedBy adicionada';
END
ELSE
    PRINT '? CreditCards.UpdatedBy já existe';
GO

-- DeletedAt
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CreditCards') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE CreditCards ADD DeletedAt DATETIME NULL;
    PRINT '? CreditCards.DeletedAt adicionada';
END
ELSE
    PRINT '? CreditCards.DeletedAt já existe';
GO

-- DeletedBy
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CreditCards') AND name = 'DeletedBy')
BEGIN
    ALTER TABLE CreditCards ADD DeletedBy INT NULL;
    PRINT '? CreditCards.DeletedBy adicionada';
END
ELSE
    PRINT '? CreditCards.DeletedBy já existe';
GO

-- =============================================
-- VERIFICAÇÃO
-- =============================================

PRINT '';
PRINT '?? Verificando colunas criadas...';
PRINT '==================================';

-- Verificar Expenses
SELECT 
    'Expenses' AS Tabela,
    COLUMN_NAME AS Coluna,
    DATA_TYPE AS Tipo,
    IS_NULLABLE AS Aceita_Null
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Expenses'
  AND COLUMN_NAME IN ('UpdatedAt', 'UpdatedBy', 'DeletedAt', 'DeletedBy')
ORDER BY ORDINAL_POSITION;

-- Verificar CreditCards
SELECT 
    'CreditCards' AS Tabela,
    COLUMN_NAME AS Coluna,
    DATA_TYPE AS Tipo,
    IS_NULLABLE AS Aceita_Null
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'CreditCards'
  AND COLUMN_NAME IN ('CreatedBy', 'UpdatedBy', 'DeletedAt', 'DeletedBy')
ORDER BY ORDINAL_POSITION;

PRINT '';
PRINT '? Colunas adicionadas com sucesso!';
PRINT '';
PRINT '?? Próximos passos:';
PRINT '   1. Execute novamente: SoftDeleteFix.sql';
PRINT '   2. Ou continue sem Soft Delete por enquanto';
PRINT '   3. Inicie a API: dotnet run';
GO
