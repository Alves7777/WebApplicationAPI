-- =============================================
-- VINCULAR DADOS EXISTENTES AO USERID 2004
-- Execute este script para atualizar registros sem UserId
-- =============================================

USE WebAppDB;
GO

PRINT '?? Vinculando dados existentes ao UserId 2004...';
PRINT '==================================================';
GO

-- =============================================
-- CONFIGURAÇÃO
-- =============================================

DECLARE @DefaultUserId INT = 2004;
DECLARE @AdminUserId INT = 2002;

PRINT '';
PRINT '?? Configuração:';
PRINT '   - UserId padrão: ' + CAST(@DefaultUserId AS NVARCHAR);
PRINT '   - Admin UserId: ' + CAST(@AdminUserId AS NVARCHAR);
PRINT '';

-- =============================================
-- EXPENSES
-- =============================================

PRINT '?? Atualizando Expenses...';

-- Atualizar registros sem UserId
UPDATE Expenses
SET UserId = @DefaultUserId
WHERE UserId IS NULL;

PRINT '   ? ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' expenses atualizadas';

-- Verificar total
DECLARE @ExpensesCount INT;
SELECT @ExpensesCount = COUNT(*) FROM Expenses WHERE UserId = @DefaultUserId;
PRINT '   ?? Total de expenses do UserId 2004: ' + CAST(@ExpensesCount AS NVARCHAR);
GO

-- =============================================
-- CREDITCARDS
-- =============================================

PRINT '';
PRINT '?? Atualizando CreditCards...';

DECLARE @DefaultUserId INT = 2004;

UPDATE CreditCards
SET UserId = @DefaultUserId
WHERE UserId IS NULL;

PRINT '   ? ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' cartões atualizados';

DECLARE @CardsCount INT;
SELECT @CardsCount = COUNT(*) FROM CreditCards WHERE UserId = @DefaultUserId;
PRINT '   ?? Total de cartões do UserId 2004: ' + CAST(@CardsCount AS NVARCHAR);
GO

-- =============================================
-- CREDITCARDEXPENSES
-- =============================================

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CreditCardExpenses')
BEGIN
    PRINT '';
    PRINT '?? Atualizando CreditCardExpenses...';

    DECLARE @DefaultUserId INT = 2004;

    UPDATE CreditCardExpenses
    SET UserId = @DefaultUserId
    WHERE UserId IS NULL;

    PRINT '   ? ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' despesas de cartão atualizadas';
END
GO

-- =============================================
-- INSTALLMENTPURCHASES
-- =============================================

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'InstallmentPurchases')
BEGIN
    PRINT '';
    PRINT '?? Atualizando InstallmentPurchases...';

    DECLARE @DefaultUserId INT = 2004;

    UPDATE InstallmentPurchases
    SET UserId = @DefaultUserId
    WHERE UserId IS NULL;

    PRINT '   ? ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' compras parceladas atualizadas';
END
GO

-- =============================================
-- MONTHLYFINANCIALCONTROL
-- =============================================

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'MonthlyFinancialControl')
BEGIN
    PRINT '';
    PRINT '?? Atualizando MonthlyFinancialControl...';

    DECLARE @DefaultUserId INT = 2004;

    UPDATE MonthlyFinancialControl
    SET UserId = @DefaultUserId
    WHERE UserId IS NULL;

    PRINT '   ? ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' controles mensais atualizados';
END
GO

-- =============================================
-- CONFIGURAR ADMIN
-- =============================================

PRINT '';
PRINT '?? Configurando usuário Admin (UserId 2002)...';

-- Atualizar Role do Admin
UPDATE Users
SET Role = 'Admin'
WHERE Id = 2002;

PRINT '   ? UserId 2002 configurado como Admin';

-- Verificar se existe
IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = 2002)
BEGIN
    PRINT '   ??  AVISO: UserId 2002 não encontrado!';
    PRINT '   Crie um usuário com ID 2002 e role Admin';
END
GO

-- =============================================
-- RELATÓRIO FINAL
-- =============================================

PRINT '';
PRINT '?? RELATÓRIO FINAL';
PRINT '==========================================';

-- Contar por UserId
SELECT 
    UserId,
    u.Name AS Usuario,
    u.Role,
    COUNT(e.Id) AS TotalExpenses
FROM Expenses e
LEFT JOIN Users u ON e.UserId = u.Id
GROUP BY UserId, u.Name, u.Role
ORDER BY UserId;

SELECT 
    UserId,
    u.Name AS Usuario,
    u.Role,
    COUNT(c.Id) AS TotalCartoes
FROM CreditCards c
LEFT JOIN Users u ON c.UserId = u.Id
GROUP BY UserId, u.Name, u.Role
ORDER BY UserId;

PRINT '';
PRINT '? Vinculação concluída!';
PRINT '';
PRINT '?? Resumo:';
PRINT '   - Todos os dados órfãos foram vinculados ao UserId 2004';
PRINT '   - UserId 2002 configurado como Admin';
PRINT '   - Admin tem acesso a TODOS os dados';
PRINT '';
PRINT '?? Próximos passos:';
PRINT '   1. Execute o script UpdateRepositoriesWithAdmin.sql';
PRINT '   2. Reinicie a API';
PRINT '   3. Teste com ambos os usuários (2002 e 2004)';
GO
