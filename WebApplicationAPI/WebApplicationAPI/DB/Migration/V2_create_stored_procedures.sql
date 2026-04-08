-- =============================================
-- MIGRATION: Criar Stored Procedures
-- =============================================

-- 1. Buscar todos os usuários
CREATE PROCEDURE sp_GetAllUsers
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Name, Email
    FROM Users
    ORDER BY Id;
END
GO

-- 2. Buscar usuário por ID
CREATE PROCEDURE sp_GetUserById
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
CREATE PROCEDURE sp_GetUserByEmail
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
CREATE PROCEDURE sp_CreateUser
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
CREATE PROCEDURE sp_UpdateUser
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
CREATE PROCEDURE sp_DeleteUser
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
CREATE PROCEDURE sp_SearchUsers
    @SearchTerm NVARCHAR(100) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    -- Total de registros (com filtro se informado)
    DECLARE @TotalCount INT;
    SELECT @TotalCount = COUNT(*) 
    FROM Users
    WHERE (@SearchTerm IS NULL 
           OR Name LIKE '%' + @SearchTerm + '%' 
           OR Email LIKE '%' + @SearchTerm + '%');
    
    -- Registros paginados com TotalCount
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

-- =============================================
-- CONTROLE FINANCEIRO: Criar Stored Procedures
-- =============================================

-- Create
CREATE OR ALTER PROCEDURE sp_CreateExpense
    @Month INT,
    @Year INT,
    @Description NVARCHAR(255),
    @Amount DECIMAL(18,2),
    @Category NVARCHAR(100),
    @Status NVARCHAR(50),
    @PaymentMethod NVARCHAR(50),
    @CreatedAt DATETIME,
    @Id INT OUTPUT
AS
BEGIN
    INSERT INTO Expenses (Month, Year, Description, Amount, Category, Status, PaymentMethod, CreatedAt)
    VALUES (@Month, @Year, @Description, @Amount, @Category, @Status, @PaymentMethod, @CreatedAt)
    SET @Id = SCOPE_IDENTITY()
END
GO

-- Update
CREATE OR ALTER PROCEDURE sp_UpdateExpense
    @Id INT,
    @Month INT,
    @Year INT,
    @Description NVARCHAR(255),
    @Amount DECIMAL(18,2),
    @Category NVARCHAR(100),
    @Status NVARCHAR(50),
    @PaymentMethod NVARCHAR(50)
AS
BEGIN
    UPDATE Expenses
    SET Month = @Month,
        Year = @Year,
        Description = @Description,
        Amount = @Amount,
        Category = @Category,
        Status = @Status,
        PaymentMethod = @PaymentMethod
    WHERE Id = @Id
END
GO

-- Delete
CREATE OR ALTER PROCEDURE sp_DeleteExpense
    @Id INT
AS
BEGIN
    DELETE FROM Expenses WHERE Id = @Id
END
GO

-- Get Expenses (with filters)
CREATE OR ALTER PROCEDURE sp_GetExpenses
    @Month INT = NULL,
    @Year INT = NULL,
    @Category NVARCHAR(100) = NULL,
    @Status NVARCHAR(50) = NULL,
    @PaymentMethod NVARCHAR(50) = NULL
AS
BEGIN
    SELECT * FROM Expenses
    WHERE (@Month IS NULL OR Month = @Month)
      AND (@Year IS NULL OR Year = @Year)
      AND (@Category IS NULL OR Category = @Category)
      AND (@Status IS NULL OR Status = @Status)
      AND (@PaymentMethod IS NULL OR PaymentMethod = @PaymentMethod)
END
GO

-- Get by Id (for update validation)
CREATE OR ALTER PROCEDURE sp_GetExpenseById
    @Id INT
AS
BEGIN
    SELECT * FROM Expenses WHERE Id = @Id
END
GO
