-- =============================================
-- SCRIPT RÁPIDO - PROCEDURES ESSENCIAIS CATEGORY
-- =============================================
-- Copie e cole este script no SQL Server Management Studio
-- =============================================

USE WebAppDB;
GO

-- Criar tabela Categories (se não existir)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
BEGIN
    CREATE TABLE Categories (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- 1. BUSCAR TODAS AS CATEGORIAS
CREATE OR ALTER PROCEDURE sp_GetAllCategories
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Description, IsActive, CreatedAt
    FROM Categories
    ORDER BY Name;
END
GO

-- 2. BUSCAR CATEGORIA POR ID
CREATE OR ALTER PROCEDURE sp_GetCategoryById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Description, IsActive, CreatedAt
    FROM Categories
    WHERE Id = @Id;
END
GO

-- 3. CRIAR NOVA CATEGORIA
CREATE OR ALTER PROCEDURE sp_CreateCategory
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Categories (Name, Description, IsActive, CreatedAt)
    VALUES (@Name, @Description, @IsActive, GETDATE());

    -- Retorna o ID criado
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS Id;
END
GO

PRINT '? Stored Procedures criadas com sucesso!';
PRINT '? sp_GetAllCategories';
PRINT '? sp_GetCategoryById';
PRINT '? sp_CreateCategory';
GO
