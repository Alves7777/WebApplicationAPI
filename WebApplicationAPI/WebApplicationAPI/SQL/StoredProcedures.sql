-- =============================================
-- IMPORTANTE: Execute primeiro o script CreateTables.sql
-- para criar a tabela Users antes de criar esta procedure!
-- =============================================

-- =============================================
-- STORED PROCEDURES PARA TABELA USERS
-- =============================================

-- 1. Buscar todos os usuários
GO
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
GO
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
GO
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
GO
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
GO
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
GO
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
GO
CREATE OR ALTER PROCEDURE sp_SearchUsers
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
