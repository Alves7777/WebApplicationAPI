-- =============================================
-- MIGRATION: Add UserId to CreditCards
-- =============================================
USE WebAppDB;
GO

-- 1. Add UserId column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'CreditCards') AND name = 'UserId')
BEGIN
    ALTER TABLE CreditCards
    ADD UserId INT NULL;
    PRINT '? Coluna UserId adicionada à tabela CreditCards';
END
ELSE
BEGIN
    PRINT '?? Coluna UserId já existe na tabela CreditCards';
END
GO

-- 2. Add Foreign Key constraint if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_CreditCards_User')
BEGIN
    ALTER TABLE CreditCards
    ADD CONSTRAINT FK_CreditCards_User FOREIGN KEY (UserId) REFERENCES Users(Id);
    PRINT '? Foreign Key FK_CreditCards_User criada';
END
ELSE
BEGIN
    PRINT '?? Foreign Key FK_CreditCards_User já existe';
END
GO

-- =============================================
-- UPDATE STORED PROCEDURES
-- =============================================

-- 1. CREATE CREDIT CARD (with UserId)
GO
CREATE OR ALTER PROCEDURE sp_CreateCreditCard
    @UserId INT,
    @Name NVARCHAR(100),
    @Brand NVARCHAR(50) = NULL,
    @CardLimit DECIMAL(18,2) = 0,
    @ClosingDay INT = NULL,
    @DueDay INT = NULL,
    @IsActive BIT = 1,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO CreditCards (UserId, Name, Brand, CardLimit, ClosingDay, DueDay, IsActive, CreatedAt, UpdatedAt)
    VALUES (@UserId, @Name, @Brand, @CardLimit, @ClosingDay, @DueDay, @IsActive, GETDATE(), GETDATE());

    SET @Id = CAST(SCOPE_IDENTITY() AS INT);
END
GO
PRINT '? sp_CreateCreditCard atualizado';

-- 2. UPDATE CREDIT CARD (with UserId validation)
GO
CREATE OR ALTER PROCEDURE sp_UpdateCreditCard
    @Id INT,
    @UserId INT,
    @Name NVARCHAR(100),
    @Brand NVARCHAR(50) = NULL,
    @CardLimit DECIMAL(18,2) = 0,
    @ClosingDay INT = NULL,
    @DueDay INT = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE CreditCards
    SET 
        Name = @Name,
        Brand = @Brand,
        CardLimit = @CardLimit,
        ClosingDay = @ClosingDay,
        DueDay = @DueDay,
        IsActive = @IsActive,
        UpdatedAt = GETDATE()
    WHERE Id = @Id AND UserId = @UserId;
END
GO
PRINT '? sp_UpdateCreditCard atualizado';

-- 3. DELETE CREDIT CARD (with UserId validation)
GO
CREATE OR ALTER PROCEDURE sp_DeleteCreditCard
    @Id INT,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM CreditCards
    WHERE Id = @Id AND UserId = @UserId;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
PRINT '? sp_DeleteCreditCard atualizado';

-- 4. GET CREDIT CARD BY ID (with optional UserId validation)
GO
CREATE OR ALTER PROCEDURE sp_GetCreditCardById
    @Id INT,
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, UserId, Name, Brand, CardLimit, ClosingDay, DueDay, IsActive, CreatedAt, UpdatedAt
    FROM CreditCards
    WHERE Id = @Id 
      AND (@UserId IS NULL OR UserId = @UserId);
END
GO
PRINT '? sp_GetCreditCardById atualizado';

-- 5. GET ALL CREDIT CARDS (by UserId)
GO
CREATE OR ALTER PROCEDURE sp_GetAllCreditCards
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, UserId, Name, Brand, CardLimit, ClosingDay, DueDay, IsActive, CreatedAt, UpdatedAt
    FROM CreditCards
    WHERE UserId = @UserId
    ORDER BY IsActive DESC, Name;
END
GO
PRINT '? sp_GetAllCreditCards atualizado';

-- 6. GET CREDIT CARDS BY USER ID
GO
CREATE OR ALTER PROCEDURE sp_GetCreditCardsByUserId
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, UserId, Name, Brand, CardLimit, ClosingDay, DueDay, IsActive, CreatedAt, UpdatedAt
    FROM CreditCards
    WHERE UserId = @UserId
    ORDER BY IsActive DESC, Name;
END
GO
PRINT '? sp_GetCreditCardsByUserId criado/atualizado';

PRINT '';
PRINT '??? MIGRAÇÃO CONCLUÍDA COM SUCESSO! ???';
GO
