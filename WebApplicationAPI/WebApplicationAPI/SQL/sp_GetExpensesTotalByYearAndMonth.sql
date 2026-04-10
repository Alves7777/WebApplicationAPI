-- =============================================
-- sp_GetExpensesTotalByYearAndMonth
-- =============================================
-- Retorna o total de despesas de um determinado mês/ano
-- =============================================

USE WebAppDB;
GO

CREATE OR ALTER PROCEDURE sp_GetExpensesTotalByYearAndMonth
    @Year INT,
    @Month INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ISNULL(SUM(Amount), 0) AS Total
    FROM Expenses
    WHERE Year = @Year AND Month = @Month;
END
GO

PRINT '? sp_GetExpensesTotalByYearAndMonth criada com sucesso!';
GO
