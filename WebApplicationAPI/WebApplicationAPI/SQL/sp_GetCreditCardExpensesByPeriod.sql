-- =============================================
-- sp_GetCreditCardExpensesByPeriod
-- =============================================
-- Busca despesas por período (com validação de 30 dias)
-- =============================================

USE WebAppDB;
GO

CREATE OR ALTER PROCEDURE sp_GetCreditCardExpensesByPeriod
    @CreditCardId INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id, 
        CreditCardId, 
        PurchaseDate, 
        Description, 
        Amount, 
        Category, 
        Month, 
        Year, 
        Status, 
        CreatedAt
    FROM CreditCardExpenses
    WHERE CreditCardId = @CreditCardId
      AND PurchaseDate >= @StartDate
      AND PurchaseDate <= @EndDate
    ORDER BY PurchaseDate DESC;
END
GO

PRINT '? sp_GetCreditCardExpensesByPeriod criada com sucesso!';
GO
