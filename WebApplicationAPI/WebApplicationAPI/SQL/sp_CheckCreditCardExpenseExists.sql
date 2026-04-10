-- =============================================
-- sp_CheckCreditCardExpenseExists
-- =============================================
-- Verifica se uma despesa já existe (evitar duplicidade)
-- =============================================

USE WebAppDB;
GO

CREATE OR ALTER PROCEDURE sp_CheckCreditCardExpenseExists
    @CreditCardId INT,
    @PurchaseDate DATE,
    @Description NVARCHAR(500),
    @Amount DECIMAL(18,2),
    @Exists BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Count INT;

    SELECT @Count = COUNT(1)
    FROM CreditCardExpenses
    WHERE CreditCardId = @CreditCardId
      AND PurchaseDate = @PurchaseDate
      AND Description = @Description
      AND Amount = @Amount;

    SET @Exists = CASE WHEN @Count > 0 THEN 1 ELSE 0 END;
END
GO

PRINT '? sp_CheckCreditCardExpenseExists criada com sucesso!';
GO
