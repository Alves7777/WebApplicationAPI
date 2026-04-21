-- =============================================
-- FIX: Recreate stored procedures with correct QUOTED_IDENTIFIER settings
-- =============================================
USE WebAppDB;
GO

-- 1. CREATE CREDIT CARD (with UserId)
IF OBJECT_ID('sp_CreateCreditCard', 'P') IS NOT NULL
    DROP PROCEDURE sp_CreateCreditCard;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_CreateCreditCard
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
PRINT '? sp_CreateCreditCard recriado com QUOTED_IDENTIFIER ON';

-- 2. UPDATE CREDIT CARD (with UserId validation)
IF OBJECT_ID('sp_UpdateCreditCard', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateCreditCard;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_UpdateCreditCard
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
PRINT '? sp_UpdateCreditCard recriado com QUOTED_IDENTIFIER ON';

-- 3. DELETE CREDIT CARD (with UserId validation)
IF OBJECT_ID('sp_DeleteCreditCard', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteCreditCard;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_DeleteCreditCard
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
PRINT '? sp_DeleteCreditCard recriado com QUOTED_IDENTIFIER ON';

-- 4. GET CREDIT CARD BY ID (with optional UserId validation)
IF OBJECT_ID('sp_GetCreditCardById', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetCreditCardById;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_GetCreditCardById
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
PRINT '? sp_GetCreditCardById recriado com QUOTED_IDENTIFIER ON';

-- 5. GET ALL CREDIT CARDS (by UserId)
IF OBJECT_ID('sp_GetAllCreditCards', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetAllCreditCards;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_GetAllCreditCards
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
PRINT '? sp_GetAllCreditCards recriado com QUOTED_IDENTIFIER ON';

-- 6. GET CREDIT CARDS BY USER ID
IF OBJECT_ID('sp_GetCreditCardsByUserId', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetCreditCardsByUserId;
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_GetCreditCardsByUserId
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
PRINT '? sp_GetCreditCardsByUserId recriado com QUOTED_IDENTIFIER ON';

PRINT '';
PRINT '??? CORREÇÃO DE QUOTED_IDENTIFIER CONCLUÍDA! ???';
GO
