-- =============================================
-- sp_GetAllActiveInstallmentsByMonth
-- =============================================
-- Retorna parcelas ativas de TODOS os cartões em um mês
-- Usado para considerar todas as parcelas na simulação
-- =============================================

USE WebAppDB;
GO

CREATE OR ALTER PROCEDURE sp_GetAllActiveInstallmentsByMonth
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
    WHERE Status = 'ATIVA'
      AND (
          (@Year > FirstInstallmentYear) OR
          (@Year = FirstInstallmentYear AND @Month >= FirstInstallmentMonth)
      )
      AND (
          DATEDIFF(MONTH, 
              DATEFROMPARTS(FirstInstallmentYear, FirstInstallmentMonth, 1),
              DATEFROMPARTS(@Year, @Month, 1)
          ) < InstallmentCount
      )
    ORDER BY CreditCardId, CreatedAt;
END
GO

PRINT '? sp_GetAllActiveInstallmentsByMonth criada com sucesso!';
GO
