-- =============================================
-- INSTALLMENT PURCHASES - Compras Parceladas
-- =============================================
-- Sistema de controle de compras parceladas
-- =============================================

USE WebAppDB;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'InstallmentPurchases')
BEGIN
    CREATE TABLE InstallmentPurchases (
        Id INT PRIMARY KEY IDENTITY(1,1),
        CreditCardId INT NOT NULL,
        Description NVARCHAR(500) NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        InstallmentCount INT NOT NULL,
        InstallmentAmount DECIMAL(18,2) NOT NULL,
        FirstInstallmentMonth INT NOT NULL,
        FirstInstallmentYear INT NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'ATIVA',
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_InstallmentPurchases_CreditCard FOREIGN KEY (CreditCardId) 
            REFERENCES CreditCards(Id) ON DELETE CASCADE,
        CONSTRAINT CHK_InstallmentCount CHECK (InstallmentCount >= 1 AND InstallmentCount <= 24),
        CONSTRAINT CHK_Status CHECK (Status IN ('ATIVA', 'FINALIZADA', 'CANCELADA'))
    );
    PRINT '? Tabela InstallmentPurchases criada com sucesso!';
END
ELSE
BEGIN
    PRINT '?? Tabela InstallmentPurchases já existe.';
END
GO

CREATE OR ALTER PROCEDURE sp_GetActiveInstallmentsByMonth
    @CreditCardId INT,
    @Month INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        CreditCardId,
        Description,
        TotalAmount,
        InstallmentCount,
        InstallmentAmount,
        FirstInstallmentMonth,
        FirstInstallmentYear,
        Status,
        CreatedAt
    FROM InstallmentPurchases
    WHERE CreditCardId = @CreditCardId
      AND Status = 'ATIVA'
      AND (
          (@Year > FirstInstallmentYear) OR
          (@Year = FirstInstallmentYear AND @Month >= FirstInstallmentMonth)
      )
      AND (
          DATEDIFF(MONTH, 
              DATEFROMPARTS(FirstInstallmentYear, FirstInstallmentMonth, 1),
              DATEFROMPARTS(@Year, @Month, 1)
          ) < InstallmentCount
      );
END
GO

CREATE OR ALTER PROCEDURE sp_CreateInstallmentPurchase
    @CreditCardId INT,
    @Description NVARCHAR(500),
    @TotalAmount DECIMAL(18,2),
    @InstallmentCount INT,
    @InstallmentAmount DECIMAL(18,2),
    @FirstInstallmentMonth INT,
    @FirstInstallmentYear INT,
    @Status NVARCHAR(50) = 'ATIVA',
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO InstallmentPurchases (
        CreditCardId, Description, TotalAmount, InstallmentCount, 
        InstallmentAmount, FirstInstallmentMonth, FirstInstallmentYear, 
        Status, CreatedAt
    )
    VALUES (
        @CreditCardId, @Description, @TotalAmount, @InstallmentCount,
        @InstallmentAmount, @FirstInstallmentMonth, @FirstInstallmentYear,
        @Status, GETDATE()
    );

    SET @Id = CAST(SCOPE_IDENTITY() AS INT);
END
GO

CREATE OR ALTER PROCEDURE sp_GetAllInstallmentPurchases
    @CreditCardId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        CreditCardId,
        Description,
        TotalAmount,
        InstallmentCount,
        InstallmentAmount,
        FirstInstallmentMonth,
        FirstInstallmentYear,
        Status,
        CreatedAt
    FROM InstallmentPurchases
    WHERE CreditCardId = @CreditCardId
    ORDER BY CreatedAt DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_UpdateInstallmentPurchaseStatus
    @Id INT,
    @Status NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE InstallmentPurchases
    SET Status = @Status
    WHERE Id = @Id;
END
GO

PRINT '? sp_GetActiveInstallmentsByMonth criada!';
PRINT '? sp_CreateInstallmentPurchase criada!';
PRINT '? sp_GetAllInstallmentPurchases criada!';
PRINT '? sp_UpdateInstallmentPurchaseStatus criada!';
GO
