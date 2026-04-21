-- =============================================
-- SCRIPT DE TESTES - AUTENTICAÇÃO E MULTI-TENANCY
-- Execute após AuthenticationSetup.sql
-- =============================================

USE WebAppDB;
GO

PRINT '?? Iniciando testes de autenticação...';
GO

-- =============================================
-- TESTE 1: Verificar se as colunas foram criadas
-- =============================================

PRINT '';
PRINT '?? TESTE 1: Verificando estrutura das tabelas';
PRINT '============================================';

-- Verificar colunas em Users
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'PasswordHash')
    PRINT '? Users.PasswordHash existe'
ELSE
    PRINT '? Users.PasswordHash NÃO existe'

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'Role')
    PRINT '? Users.Role existe'
ELSE
    PRINT '? Users.Role NÃO existe'

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'IsActive')
    PRINT '? Users.IsActive existe'
ELSE
    PRINT '? Users.IsActive NÃO existe'

-- Verificar colunas em Expenses
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Expenses') AND name = 'UserId')
    PRINT '? Expenses.UserId existe'
ELSE
    PRINT '? Expenses.UserId NÃO existe'

-- Verificar colunas em CreditCards
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'CreditCards')
BEGIN
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CreditCards') AND name = 'UserId')
        PRINT '? CreditCards.UserId existe'
    ELSE
        PRINT '? CreditCards.UserId NÃO existe'
END
ELSE
BEGIN
    PRINT '??  CreditCards não existe ainda (será criada em uso)'
END

GO

-- =============================================
-- TESTE 2: Verificar Stored Procedures
-- =============================================

PRINT '';
PRINT '?? TESTE 2: Verificando Stored Procedures';
PRINT '============================================';

IF OBJECT_ID('sp_CreateUser', 'P') IS NOT NULL
    PRINT '? sp_CreateUser existe'
ELSE
    PRINT '? sp_CreateUser NÃO existe'

IF OBJECT_ID('sp_GetUserByEmail', 'P') IS NOT NULL
    PRINT '? sp_GetUserByEmail existe'
ELSE
    PRINT '? sp_GetUserByEmail NÃO existe'

IF OBJECT_ID('sp_GetUserById', 'P') IS NOT NULL
    PRINT '? sp_GetUserById existe'
ELSE
    PRINT '? sp_GetUserById NÃO existe'

IF OBJECT_ID('sp_CreateExpense', 'P') IS NOT NULL
    PRINT '? sp_CreateExpense existe (atualizada com UserId)'
ELSE
    PRINT '? sp_CreateExpense NÃO existe'

GO

-- =============================================
-- TESTE 3: Criar usuários de teste
-- =============================================

PRINT '';
PRINT '?? TESTE 3: Criando usuários de teste';
PRINT '============================================';

-- Limpar dados de teste anteriores
DELETE FROM Users WHERE Email LIKE '%@teste.com';
DELETE FROM Expenses WHERE UserId IS NULL OR UserId IN (SELECT Id FROM Users WHERE Email LIKE '%@teste.com');

-- Criar Usuário 1
DECLARE @User1Id INT;
EXEC sp_CreateUser 
    @Name = 'João Silva',
    @Email = 'joao@teste.com',
    @PasswordHash = '$2a$11$dummyhashfortest123456789012345678901234567890',
    @Role = 'User',
    @IsActive = 1;

SET @User1Id = (SELECT Id FROM Users WHERE Email = 'joao@teste.com');
PRINT '? Usuário 1 criado: João Silva (ID: ' + CAST(@User1Id AS NVARCHAR) + ')';

-- Criar Usuário 2
DECLARE @User2Id INT;
EXEC sp_CreateUser 
    @Name = 'Maria Santos',
    @Email = 'maria@teste.com',
    @PasswordHash = '$2a$11$dummyhashfortest123456789012345678901234567890',
    @Role = 'User',
    @IsActive = 1;

SET @User2Id = (SELECT Id FROM Users WHERE Email = 'maria@teste.com');
PRINT '? Usuário 2 criado: Maria Santos (ID: ' + CAST(@User2Id AS NVARCHAR) + ')';

GO

-- =============================================
-- TESTE 4: Testar criação de despesas por usuário
-- =============================================

PRINT '';
PRINT '?? TESTE 4: Testando multi-tenancy com despesas';
PRINT '============================================';

DECLARE @User1Id INT = (SELECT Id FROM Users WHERE Email = 'joao@teste.com');
DECLARE @User2Id INT = (SELECT Id FROM Users WHERE Email = 'maria@teste.com');
DECLARE @ExpenseId INT;

-- Criar despesas para Usuário 1 (João)
EXEC sp_CreateExpense
    @UserId = @User1Id,
    @Month = 4,
    @Year = 2025,
    @Description = 'Aluguel - João',
    @Amount = 1500.00,
    @Category = 'Moradia',
    @Status = 'Pago',
    @PaymentMethod = 'Débito',
    @CreatedAt = '2025-04-15',
    @Id = @ExpenseId OUTPUT;

PRINT '? Despesa 1 criada para João (ID: ' + CAST(@ExpenseId AS NVARCHAR) + ')';

EXEC sp_CreateExpense
    @UserId = @User1Id,
    @Month = 4,
    @Year = 2025,
    @Description = 'Mercado - João',
    @Amount = 300.00,
    @Category = 'Alimentação',
    @Status = 'Pago',
    @PaymentMethod = 'Crédito',
    @CreatedAt = '2025-04-16',
    @Id = @ExpenseId OUTPUT;

PRINT '? Despesa 2 criada para João (ID: ' + CAST(@ExpenseId AS NVARCHAR) + ')';

-- Criar despesas para Usuário 2 (Maria)
EXEC sp_CreateExpense
    @UserId = @User2Id,
    @Month = 4,
    @Year = 2025,
    @Description = 'Academia - Maria',
    @Amount = 150.00,
    @Category = 'Saúde',
    @Status = 'Pendente',
    @PaymentMethod = 'Débito',
    @CreatedAt = '2025-04-15',
    @Id = @ExpenseId OUTPUT;

PRINT '? Despesa 1 criada para Maria (ID: ' + CAST(@ExpenseId AS NVARCHAR) + ')';

EXEC sp_CreateExpense
    @UserId = @User2Id,
    @Month = 4,
    @Year = 2025,
    @Description = 'Internet - Maria',
    @Amount = 100.00,
    @Category = 'Moradia',
    @Status = 'Pago',
    @PaymentMethod = 'Débito',
    @CreatedAt = '2025-04-16',
    @Id = @ExpenseId OUTPUT;

PRINT '? Despesa 2 criada para Maria (ID: ' + CAST(@ExpenseId AS NVARCHAR) + ')';

GO

-- =============================================
-- TESTE 5: Verificar isolamento de dados (Multi-tenancy)
-- =============================================

PRINT '';
PRINT '?? TESTE 5: Verificando isolamento de dados';
PRINT '============================================';

DECLARE @User1Id INT = (SELECT Id FROM Users WHERE Email = 'joao@teste.com');
DECLARE @User2Id INT = (SELECT Id FROM Users WHERE Email = 'maria@teste.com');

-- Buscar despesas do João
DECLARE @JoaoExpenses INT;
SELECT @JoaoExpenses = COUNT(*) FROM Expenses WHERE UserId = @User1Id;
PRINT 'João tem ' + CAST(@JoaoExpenses AS NVARCHAR) + ' despesas';

-- Buscar despesas da Maria
DECLARE @MariaExpenses INT;
SELECT @MariaExpenses = COUNT(*) FROM Expenses WHERE UserId = @User2Id;
PRINT 'Maria tem ' + CAST(@MariaExpenses AS NVARCHAR) + ' despesas';

-- Verificar se cada usuário vê apenas suas despesas
EXEC sp_GetExpensesByPeriod @UserId = @User1Id, @Month = 4, @Year = 2025;
PRINT '? Despesas de João (devem ser 2):';

EXEC sp_GetExpensesByPeriod @UserId = @User2Id, @Month = 4, @Year = 2025;
PRINT '? Despesas de Maria (devem ser 2):';

GO

-- =============================================
-- TESTE 6: Verificar segurança - tentar acessar dados de outro usuário
-- =============================================

PRINT '';
PRINT '?? TESTE 6: Testando segurança (isolamento)';
PRINT '============================================';

DECLARE @User1Id INT = (SELECT Id FROM Users WHERE Email = 'joao@teste.com');
DECLARE @User2Id INT = (SELECT Id FROM Users WHERE Email = 'maria@teste.com');
DECLARE @JoaoExpenseId INT = (SELECT TOP 1 Id FROM Expenses WHERE UserId = @User1Id);

-- Tentar deletar despesa do João usando o ID da Maria
DECLARE @RowsAffected INT;
EXEC sp_DeleteExpense @Id = @JoaoExpenseId, @UserId = @User2Id;

IF (SELECT COUNT(*) FROM Expenses WHERE Id = @JoaoExpenseId) > 0
    PRINT '? Segurança OK: Maria NÃO conseguiu deletar despesa do João'
ELSE
    PRINT '? FALHA DE SEGURANÇA: Maria conseguiu deletar despesa do João!'

GO

-- =============================================
-- TESTE 7: Relatório de resumo
-- =============================================

PRINT '';
PRINT '?? RESUMO DOS TESTES';
PRINT '============================================';

SELECT 
    u.Name AS Usuario,
    u.Email,
    u.Role,
    u.IsActive,
    COUNT(e.Id) AS TotalDespesas,
    ISNULL(SUM(e.Amount), 0) AS TotalGasto
FROM Users u
LEFT JOIN Expenses e ON u.Id = e.UserId
WHERE u.Email LIKE '%@teste.com'
GROUP BY u.Id, u.Name, u.Email, u.Role, u.IsActive;

PRINT '';
PRINT '? Testes concluídos!';
PRINT '';
PRINT '?? Próximos passos:';
PRINT '   1. Inicie a API: dotnet run';
PRINT '   2. Teste POST /api/auth/register';
PRINT '   3. Teste POST /api/auth/login';
PRINT '   4. Use o token nas rotas protegidas';
PRINT '';
PRINT '?? Para limpar dados de teste, execute:';
PRINT '   DELETE FROM Expenses WHERE UserId IN (SELECT Id FROM Users WHERE Email LIKE ''%@teste.com'');';
PRINT '   DELETE FROM Users WHERE Email LIKE ''%@teste.com'';';

GO
