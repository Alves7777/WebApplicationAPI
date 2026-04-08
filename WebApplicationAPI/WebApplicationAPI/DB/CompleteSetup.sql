-- =============================================
-- SCRIPT COMPLETO DE SETUP DO BANCO DE DADOS
-- Execute este script para criar todas as tabelas e procedures
-- =============================================

USE WebAppDB;
GO

-- =============================================
-- PARTE 1: CRIAR TABELAS
-- =============================================

-- Criar tabela Expenses
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Expenses')
BEGIN
    CREATE TABLE Expenses (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Month INT NOT NULL,
        Year INT NOT NULL,
        Description NVARCHAR(500) NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        Category NVARCHAR(100) NOT NULL,
        Status NVARCHAR(50) NOT NULL,
        PaymentMethod NVARCHAR(100) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Tabela Expenses criada com sucesso!';
END
ELSE
BEGIN
    PRINT 'Tabela Expenses já existe.';
END
GO

-- Criar tabela Users
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE
    );
    PRINT 'Tabela Users criada com sucesso!';
END
ELSE
BEGIN
    PRINT 'Tabela Users já existe.';
END
GO

-- =============================================
-- PARTE 2: STORED PROCEDURES PARA EXPENSES
-- =============================================

-- 1. Criar nova despesa
CREATE OR ALTER PROCEDURE sp_CreateExpense
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

    INSERT INTO Expenses (Month, Year, Description, Amount, Category, Status, PaymentMethod, CreatedAt)
    VALUES (@Month, @Year, @Description, @Amount, @Category, @Status, @PaymentMethod, @CreatedAt);

    SET @Id = CAST(SCOPE_IDENTITY() AS INT);
END
GO

-- 2. Atualizar despesa
CREATE OR ALTER PROCEDURE sp_UpdateExpense
    @Id INT,
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
    WHERE Id = @Id;

    SELECT Id, Month, Year, Description, Amount, Category, Status, PaymentMethod, CreatedAt
    FROM Expenses
    WHERE Id = @Id;
END
GO

-- 3. Deletar despesa
CREATE OR ALTER PROCEDURE sp_DeleteExpense
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Expenses
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- 4. Buscar despesa por ID
CREATE OR ALTER PROCEDURE sp_GetExpenseById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Month, Year, Description, Amount, Category, Status, PaymentMethod, CreatedAt
    FROM Expenses
    WHERE Id = @Id;
END
GO

-- 5. Buscar despesas com filtros
CREATE OR ALTER PROCEDURE sp_GetExpenses
    @Month INT = NULL,
    @Year INT = NULL,
    @Category NVARCHAR(100) = NULL,
    @Status NVARCHAR(50) = NULL,
    @PaymentMethod NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Month, Year, Description, Amount, Category, Status, PaymentMethod, CreatedAt
    FROM Expenses
    WHERE (@Month IS NULL OR Month = @Month)
      AND (@Year IS NULL OR Year = @Year)
      AND (@Category IS NULL OR Category = @Category)
      AND (@Status IS NULL OR Status = @Status)
      AND (@PaymentMethod IS NULL OR PaymentMethod = @PaymentMethod)
    ORDER BY Year DESC, Month DESC, CreatedAt DESC;
END
GO

-- 6. Calcular totais por categoria
CREATE OR ALTER PROCEDURE sp_GetExpenseTotalsByCategory
    @Month INT = NULL,
    @Year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Category,
        SUM(Amount) AS TotalAmount,
        COUNT(*) AS Count
    FROM Expenses
    WHERE (@Month IS NULL OR Month = @Month)
      AND (@Year IS NULL OR Year = @Year)
    GROUP BY Category
    ORDER BY TotalAmount DESC;
END
GO

-- 7. Calcular totais mensais
CREATE OR ALTER PROCEDURE sp_GetExpenseTotalsByMonth
    @Year INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Year,
        Month,
        SUM(Amount) AS TotalAmount,
        COUNT(*) AS Count
    FROM Expenses
    WHERE (@Year IS NULL OR Year = @Year)
    GROUP BY Year, Month
    ORDER BY Year DESC, Month DESC;
END
GO

-- =============================================
-- PARTE 3: STORED PROCEDURES PARA USERS
-- =============================================

-- 1. Buscar todos os usuários
CREATE OR ALTER PROCEDURE sp_GetAllUsers
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Email
    FROM Users
    ORDER BY Id;
END
GO

-- 2. Buscar usuário por ID
CREATE OR ALTER PROCEDURE sp_GetUserById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Email
    FROM Users
    WHERE Id = @Id;
END
GO

-- 3. Buscar usuário por Email
CREATE OR ALTER PROCEDURE sp_GetUserByEmail
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Email
    FROM Users
    WHERE Email = @Email;
END
GO

-- 4. Criar novo usuário
CREATE OR ALTER PROCEDURE sp_CreateUser
    @Name NVARCHAR(100),
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Users (Name, Email)
    VALUES (@Name, @Email);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

-- 5. Atualizar usuário
CREATE OR ALTER PROCEDURE sp_UpdateUser
    @Id INT,
    @Name NVARCHAR(100),
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Users
    SET Name = @Name,
        Email = @Email
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- 6. Deletar usuário
CREATE OR ALTER PROCEDURE sp_DeleteUser
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Users
    WHERE Id = @Id;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- 7. Buscar usuários com paginação e filtro
CREATE OR ALTER PROCEDURE sp_SearchUsers
    @SearchTerm NVARCHAR(100) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    DECLARE @TotalCount INT;
    SELECT @TotalCount = COUNT(*) 
    FROM Users
    WHERE (@SearchTerm IS NULL 
           OR Name LIKE '%' + @SearchTerm + '%' 
           OR Email LIKE '%' + @SearchTerm + '%');

    SELECT 
        Id, 
        Name, 
        Email,
        @TotalCount AS TotalCount
    FROM Users
    WHERE (@SearchTerm IS NULL 
           OR Name LIKE '%' + @SearchTerm + '%' 
           OR Email LIKE '%' + @SearchTerm + '%')
    ORDER BY Id
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

PRINT '==============================================';
PRINT 'Setup do banco de dados concluído com sucesso!';
PRINT '==============================================';
PRINT 'Tabelas criadas: Expenses, Users';
PRINT 'Stored Procedures criadas: sp_CreateExpense, sp_UpdateExpense, sp_DeleteExpense, sp_GetExpenseById, sp_GetExpenses, sp_GetExpenseTotalsByCategory, sp_GetExpenseTotalsByMonth, sp_GetAllUsers, sp_GetUserById, sp_GetUserByEmail, sp_CreateUser, sp_UpdateUser, sp_DeleteUser, sp_SearchUsers';
PRINT '==============================================';
