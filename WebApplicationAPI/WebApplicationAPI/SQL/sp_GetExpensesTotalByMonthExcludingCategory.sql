-- =============================================
-- sp_GetExpensesTotalByMonthExcludingCategory
-- =============================================
-- Retorna total de despesas EXCLUINDO uma categoria
-- Usado para evitar duplicação de cartão de crédito
-- =============================================

USE WebAppDB;
GO

CREATE OR ALTER PROCEDURE sp_GetExpensesTotalByMonthExcludingCategory
    @Year INT,
    @Month INT,
    @ExcludeCategory NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ISNULL(SUM(Amount), 0) AS Total
    FROM Expenses
    WHERE Year = @Year 
      AND Month = @Month
      AND (Category IS NULL OR Category != @ExcludeCategory);
END
GO

PRINT '? sp_GetExpensesTotalByMonthExcludingCategory criada com sucesso!';
GO
