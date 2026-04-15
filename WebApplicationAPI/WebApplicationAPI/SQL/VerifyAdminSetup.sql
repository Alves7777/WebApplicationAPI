-- =============================================
-- VERIFICAÇÃO RÁPIDA - Sistema Admin
-- Execute para verificar se tudo está configurado
-- =============================================

USE WebAppDB;
GO

PRINT '?? Verificando configuração do sistema Admin...';
PRINT '=================================================';
GO

-- =============================================
-- 1. Verificar Usuários
-- =============================================

PRINT '';
PRINT '?? Usuários cadastrados:';
PRINT '========================';

SELECT 
    Id,
    Name,
    Email,
    Role,
    IsActive,
    CASE 
        WHEN Role = 'Admin' THEN '?? ADMIN'
        ELSE '?? User'
    END AS TipoUsuario
FROM Users
ORDER BY Id;

-- Verificar Admin
IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = 2002 AND Role = 'Admin')
BEGIN
    PRINT '';
    PRINT '??  AVISO: UserId 2002 não está configurado como Admin!';
    PRINT '   Execute: UPDATE Users SET Role = ''Admin'' WHERE Id = 2002;';
END
ELSE
BEGIN
    PRINT '';
    PRINT '? UserId 2002 está configurado como Admin';
END

GO

-- =============================================
-- 2. Verificar Dados Vinculados
-- =============================================

PRINT '';
PRINT '?? Distribuição de dados por usuário:';
PRINT '======================================';

-- Expenses
SELECT 
    'Expenses' AS Tabela,
    UserId,
    u.Name AS Usuario,
    COUNT(*) AS Total
FROM Expenses e
LEFT JOIN Users u ON e.UserId = u.Id
GROUP BY UserId, u.Name
ORDER BY UserId;

-- CreditCards
SELECT 
    'CreditCards' AS Tabela,
    UserId,
    u.Name AS Usuario,
    COUNT(*) AS Total
FROM CreditCards c
LEFT JOIN Users u ON c.UserId = u.Id
GROUP BY UserId, u.Name
ORDER BY UserId;

GO

-- =============================================
-- 3. Verificar Função fn_IsAdmin
-- =============================================

PRINT '';
PRINT '?? Verificando função fn_IsAdmin:';
PRINT '===================================';

IF OBJECT_ID('dbo.fn_IsAdmin', 'FN') IS NOT NULL
BEGIN
    PRINT '? Função fn_IsAdmin existe';

    -- Testar função
    DECLARE @TestAdmin BIT = dbo.fn_IsAdmin(2002);
    DECLARE @TestUser BIT = dbo.fn_IsAdmin(2004);

    PRINT '   - fn_IsAdmin(2002): ' + CASE WHEN @TestAdmin = 1 THEN '? Admin' ELSE '? Não é Admin' END;
    PRINT '   - fn_IsAdmin(2004): ' + CASE WHEN @TestUser = 0 THEN '? User comum' ELSE '? Erro' END;
END
ELSE
BEGIN
    PRINT '? Função fn_IsAdmin NÃO existe!';
    PRINT '   Execute: StoredProceduresWithAdmin.sql';
END

GO

-- =============================================
-- 4. Verificar Stored Procedures
-- =============================================

PRINT '';
PRINT '?? Stored Procedures atualizadas:';
PRINT '===================================';

DECLARE @SPsToCheck TABLE (SPName NVARCHAR(100));

INSERT INTO @SPsToCheck VALUES 
    ('sp_GetExpensesByPeriod'),
    ('sp_GetExpensesByUserId'),
    ('sp_GetCreditCardsByUserId'),
    ('sp_GetCreditCardById'),
    ('sp_GetCreditCardExpensesByPeriod'),
    ('sp_GetAllUsers'),
    ('sp_GetFinancialSummary');

SELECT 
    SPName,
    CASE 
        WHEN OBJECT_ID(SPName, 'P') IS NOT NULL THEN '? Existe'
        ELSE '? Não existe'
    END AS Status,
    CASE 
        WHEN OBJECT_ID(SPName, 'P') IS NOT NULL AND 
             OBJECT_DEFINITION(OBJECT_ID(SPName)) LIKE '%fn_IsAdmin%' 
        THEN '? Com suporte Admin'
        WHEN OBJECT_ID(SPName, 'P') IS NOT NULL 
        THEN '??  Sem suporte Admin'
        ELSE ''
    END AS SuporteAdmin
FROM @SPsToCheck
ORDER BY SPName;

GO

-- =============================================
-- 5. Teste Rápido
-- =============================================

PRINT '';
PRINT '?? Teste rápido de permissões:';
PRINT '================================';

-- Contar total de expenses
DECLARE @TotalExpenses INT;
SELECT @TotalExpenses = COUNT(*) FROM Expenses WHERE DeletedAt IS NULL;

PRINT '   Total de expenses no sistema: ' + CAST(@TotalExpenses AS NVARCHAR);

-- Testar como Admin (deve retornar tudo)
DECLARE @AdminCount INT;
SELECT @AdminCount = COUNT(*) 
FROM Expenses 
WHERE (dbo.fn_IsAdmin(2002) = 1 OR UserId = 2002)
  AND DeletedAt IS NULL;

PRINT '   Admin (2002) vê: ' + CAST(@AdminCount AS NVARCHAR) + ' expenses';

IF @AdminCount = @TotalExpenses
    PRINT '   ? Admin vê TODAS as expenses';
ELSE
    PRINT '   ? Admin NÃO está vendo todas as expenses';

-- Testar como User comum
DECLARE @UserCount INT;
SELECT @UserCount = COUNT(*) 
FROM Expenses 
WHERE UserId = 2004
  AND DeletedAt IS NULL;

PRINT '   User (2004) vê: ' + CAST(@UserCount AS NVARCHAR) + ' expenses';
PRINT '   ? User vê apenas suas expenses';

GO

-- =============================================
-- 6. Relatório Final
-- =============================================

PRINT '';
PRINT '?? RELATÓRIO FINAL';
PRINT '==================';
PRINT '';

-- Verificar se tudo está OK
DECLARE @AllOK BIT = 1;

-- Verificar Admin existe
IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = 2002 AND Role = 'Admin')
    SET @AllOK = 0;

-- Verificar função existe
IF OBJECT_ID('dbo.fn_IsAdmin', 'FN') IS NULL
    SET @AllOK = 0;

-- Verificar pelo menos 3 SPs com suporte Admin
DECLARE @SPsWithAdmin INT;
SELECT @SPsWithAdmin = COUNT(*)
FROM sys.procedures
WHERE OBJECT_DEFINITION(object_id) LIKE '%fn_IsAdmin%';

IF @SPsWithAdmin < 3
    SET @AllOK = 0;

IF @AllOK = 1
BEGIN
    PRINT '??? TUDO CONFIGURADO CORRETAMENTE! ???';
    PRINT '';
    PRINT '?? Próximos passos:';
    PRINT '   1. Reinicie a API: dotnet run';
    PRINT '   2. Faça login como Admin (2002)';
    PRINT '   3. Teste GET /api/creditcard (deve retornar TUDO)';
    PRINT '   4. Faça login como User (2004)';
    PRINT '   5. Teste GET /api/creditcard (deve retornar só dele)';
END
ELSE
BEGIN
    PRINT '??  CONFIGURAÇÃO INCOMPLETA';
    PRINT '';
    PRINT '?? Checklist:';
    PRINT '   [ ] Executar LinkDataToUser2004.sql';
    PRINT '   [ ] Executar StoredProceduresWithAdmin.sql';
    PRINT '   [ ] Verificar UserId 2002 com Role = Admin';
END

PRINT '';
GO
