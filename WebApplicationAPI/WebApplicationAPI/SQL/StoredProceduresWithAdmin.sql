-- =============================================
-- STORED PROCEDURES COM SUPORTE A ADMIN
-- Admin (UserId 2002) vê TODOS os dados
-- Outros usuários veem apenas seus dados
-- =============================================

USE WebAppDB;
GO

PRINT '?? Criando Stored Procedures com suporte a Admin...';
PRINT '====================================================';
GO

-- =============================================
-- FUNÇÃO AUXILIAR: Verificar se é Admin
-- =============================================

CREATE OR ALTER FUNCTION dbo.fn_IsAdmin(@UserId INT)
RETURNS BIT
AS
BEGIN
    DECLARE @IsAdmin BIT = 0;

    SELECT @IsAdmin = 1
    FROM Users
    WHERE Id = @UserId AND Role = 'Admin';

    RETURN ISNULL(@IsAdmin, 0);
END
GO

PRINT '? Função fn_IsAdmin criada';
GO

-- =============================================
-- EXPENSES - COM ADMIN
-- =============================================

-- Buscar expenses por período (Admin vê tudo)
CREATE OR ALTER PROCEDURE sp_GetExpensesByPeriod
    @UserId INT,
    @Month INT,
    @Year INT,
    @IncludeDeleted BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IsAdmin BIT = dbo.fn_IsAdmin(@UserId);

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
    WHERE Month = @Month 
      AND Year = @Year
      AND (@IsAdmin = 1 OR UserId = @UserId) -- Admin vê tudo, outros apenas seus
      AND (@IncludeDeleted = 1 OR DeletedAt IS NULL)
    ORDER BY CreatedAt DESC;
END
GO

PRINT '? sp_GetExpensesByPeriod (com Admin) criada';
GO

-- Buscar todas as expenses de um usuário (Admin vê tudo)
CREATE OR ALTER PROCEDURE sp_GetExpensesByUserId
    @UserId INT,
    @IncludeDeleted BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IsAdmin BIT = dbo.fn_IsAdmin(@UserId);

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
    WHERE (@IsAdmin = 1 OR UserId = @UserId)
      AND (@IncludeDeleted = 1 OR DeletedAt IS NULL)
    ORDER BY Year DESC, Month DESC, CreatedAt DESC;
END
GO

PRINT '? sp_GetExpensesByUserId (com Admin) criada';
GO

-- =============================================
-- CREDITCARDS - COM ADMIN
-- =============================================

-- Buscar cartões de um usuário (Admin vê tudo)
CREATE OR ALTER PROCEDURE sp_GetCreditCardsByUserId
    @UserId INT,
    @OnlyActive BIT = 1,
    @IncludeDeleted BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IsAdmin BIT = dbo.fn_IsAdmin(@UserId);

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
    WHERE (@IsAdmin = 1 OR UserId = @UserId)
      AND (@OnlyActive = 0 OR IsActive = 1)
      AND (@IncludeDeleted = 1 OR DeletedAt IS NULL)
    ORDER BY CreatedAt DESC;
END
GO

PRINT '? sp_GetCreditCardsByUserId (com Admin) criada';
GO

-- Buscar cartão por ID (Admin pode acessar qualquer um)
CREATE OR ALTER PROCEDURE sp_GetCreditCardById
    @Id INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IsAdmin BIT = dbo.fn_IsAdmin(@UserId);

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
    WHERE Id = @Id 
      AND (@IsAdmin = 1 OR UserId = @UserId);
END
GO

PRINT '? sp_GetCreditCardById (com Admin) criada';
GO

-- =============================================
-- CREDITCARD EXPENSES - COM ADMIN
-- =============================================

-- Buscar despesas de cartão (Admin vê tudo)
CREATE OR ALTER PROCEDURE sp_GetCreditCardExpensesByPeriod
    @UserId INT,
    @CreditCardId INT,
    @Month INT = NULL,
    @Year INT = NULL,
    @Category NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IsAdmin BIT = dbo.fn_IsAdmin(@UserId);

    SELECT 
        cce.Id,
        cce.UserId,
        cce.CreditCardId,
        cce.Description,
        cce.Amount,
        cce.Category,
        cce.PurchaseDate,
        cce.Installments,
        cce.CreatedAt
    FROM CreditCardExpenses cce
    INNER JOIN CreditCards cc ON cce.CreditCardId = cc.Id
    WHERE cce.CreditCardId = @CreditCardId
      AND (@IsAdmin = 1 OR cc.UserId = @UserId) -- Verificar ownership do cartão
      AND (@Month IS NULL OR MONTH(cce.PurchaseDate) = @Month)
      AND (@Year IS NULL OR YEAR(cce.PurchaseDate) = @Year)
      AND (@Category IS NULL OR cce.Category = @Category)
    ORDER BY cce.PurchaseDate DESC;
END
GO

PRINT '? sp_GetCreditCardExpensesByPeriod (com Admin) criada';
GO

-- =============================================
-- USERS - LISTAR TODOS (APENAS ADMIN)
-- =============================================

-- Listar todos os usuários (APENAS ADMIN)
CREATE OR ALTER PROCEDURE sp_GetAllUsers
    @RequestUserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IsAdmin BIT = dbo.fn_IsAdmin(@RequestUserId);

    IF @IsAdmin = 0
    BEGIN
        -- Usuário comum só pode ver seus próprios dados
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
        WHERE Id = @RequestUserId;
    END
    ELSE
    BEGIN
        -- Admin vê todos
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
END
GO

PRINT '? sp_GetAllUsers (com Admin) criada';
GO

-- =============================================
-- DASHBOARD/SUMMARY - COM ADMIN
-- =============================================

-- Resumo financeiro (Admin vê tudo, outros apenas seus)
CREATE OR ALTER PROCEDURE sp_GetFinancialSummary
    @UserId INT,
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IsAdmin BIT = dbo.fn_IsAdmin(@UserId);

    -- Total de Despesas
    SELECT 
        @Month AS Month,
        @Year AS Year,
        COUNT(*) AS TotalExpenses,
        SUM(Amount) AS TotalAmount,
        AVG(Amount) AS AverageAmount
    FROM Expenses
    WHERE Month = @Month 
      AND Year = @Year
      AND (@IsAdmin = 1 OR UserId = @UserId)
      AND DeletedAt IS NULL;

    -- Por Categoria
    SELECT 
        Category,
        COUNT(*) AS Count,
        SUM(Amount) AS Total
    FROM Expenses
    WHERE Month = @Month 
      AND Year = @Year
      AND (@IsAdmin = 1 OR UserId = @UserId)
      AND DeletedAt IS NULL
    GROUP BY Category
    ORDER BY SUM(Amount) DESC;

    -- Por Status
    SELECT 
        Status,
        COUNT(*) AS Count,
        SUM(Amount) AS Total
    FROM Expenses
    WHERE Month = @Month 
      AND Year = @Year
      AND (@IsAdmin = 1 OR UserId = @UserId)
      AND DeletedAt IS NULL
    GROUP BY Status;
END
GO

PRINT '? sp_GetFinancialSummary (com Admin) criada';
GO

-- =============================================
-- VERIFICAÇÃO FINAL
-- =============================================

PRINT '';
PRINT '? Todas as Stored Procedures com suporte a Admin criadas!';
PRINT '';
PRINT '?? Stored Procedures criadas/atualizadas:';
PRINT '   - fn_IsAdmin (função auxiliar)';
PRINT '   - sp_GetExpensesByPeriod';
PRINT '   - sp_GetExpensesByUserId';
PRINT '   - sp_GetCreditCardsByUserId';
PRINT '   - sp_GetCreditCardById';
PRINT '   - sp_GetCreditCardExpensesByPeriod';
PRINT '   - sp_GetAllUsers (apenas Admin vê todos)';
PRINT '   - sp_GetFinancialSummary';
PRINT '';
PRINT '?? Como funciona:';
PRINT '   - UserId 2002 (Admin): Vê TODOS os dados';
PRINT '   - Outros usuários: Veem apenas seus próprios dados';
PRINT '';
PRINT '?? Próximos passos:';
PRINT '   1. Atualizar Repositories no .NET para usar essas SPs';
PRINT '   2. Reiniciar a API';
PRINT '   3. Testar com usuário Admin (2002) e usuário normal (2004)';
GO
