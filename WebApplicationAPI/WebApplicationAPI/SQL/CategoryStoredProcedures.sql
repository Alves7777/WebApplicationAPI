-- =============================================
-- STORED PROCEDURES PARA TABELA CATEGORIES
-- =============================================
-- Execute este script no SQL Server para criar as stored procedures
-- =============================================

USE WebAppDB;
GO

-- =============================================
-- PARTE 1: CRIAR TABELA CATEGORIES (se não existir)
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
BEGIN
    CREATE TABLE Categories (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Tabela Categories criada com sucesso!';
END
ELSE
BEGIN
    PRINT 'Tabela Categories já existe.';
END
GO

-- =============================================
-- PARTE 2: STORED PROCEDURES
-- =============================================

-- 1. Buscar todas as categorias
GO
CREATE OR ALTER PROCEDURE sp_GetAllCategories
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id, 
        Name, 
        Description, 
        IsActive, 
        CreatedAt
    FROM Categories
    ORDER BY Name;
END
GO

-- 2. Buscar categoria por ID
GO
CREATE OR ALTER PROCEDURE sp_GetCategoryById
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id, 
        Name, 
        Description, 
        IsActive, 
        CreatedAt
    FROM Categories
    WHERE Id = @Id;
END
GO

-- 3. Criar nova categoria
GO
CREATE OR ALTER PROCEDURE sp_CreateCategory
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id INT;

    INSERT INTO Categories (Name, Description, IsActive, CreatedAt)
    VALUES (@Name, @Description, @IsActive, GETDATE());

    SET @Id = CAST(SCOPE_IDENTITY() AS INT);

    -- Retorna o ID da categoria criada
    SELECT @Id AS Id;
END
GO

-- 4. Atualizar categoria
GO
CREATE OR ALTER PROCEDURE sp_UpdateCategory
    @Id INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Categories
    SET 
        Name = @Name,
        Description = @Description,
        IsActive = @IsActive
    WHERE Id = @Id;

    -- Retorna o número de linhas afetadas
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- 5. Deletar categoria
GO
CREATE OR ALTER PROCEDURE sp_DeleteCategory
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Categories
    WHERE Id = @Id;

    -- Retorna o número de linhas afetadas
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- 6. Buscar categorias ativas
GO
CREATE OR ALTER PROCEDURE sp_GetActiveCategories
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id, 
        Name, 
        Description, 
        IsActive, 
        CreatedAt
    FROM Categories
    WHERE IsActive = 1
    ORDER BY Name;
END
GO

-- =============================================
-- PARTE 3: DADOS DE TESTE (OPCIONAL)
-- =============================================

-- Inserir algumas categorias de exemplo
/*
INSERT INTO Categories (Name, Description, IsActive) VALUES 
('Alimentação', 'Despesas com alimentação e refeições', 1),
('Transporte', 'Despesas com transporte e combustível', 1),
('Saúde', 'Despesas médicas e farmácia', 1),
('Educação', 'Cursos, livros e materiais educativos', 1),
('Lazer', 'Entretenimento e atividades recreativas', 1);
*/

PRINT 'Stored Procedures para Categories criadas com sucesso!';
GO
