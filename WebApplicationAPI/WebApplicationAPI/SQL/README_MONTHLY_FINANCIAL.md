# ?? Monthly Financial Control - CRUD Completo

Sistema de controle financeiro mensal com cálculos automáticos integrados com despesas (Expenses).

---

## ?? Funcionalidades

- ? Criar registro financeiro mensal
- ? Atualizar registro financeiro mensal
- ? Deletar registro financeiro mensal
- ? Listar todos os registros
- ? Buscar por ID
- ? Cálculos automáticos (SalaryTotal, ExpensesTotal, Balance, CanSpend)
- ? Validação de duplicidade (Year + Month)
- ? Enum para meses

---

## ??? Arquitetura

```
??? Controllers/
?   ??? MonthlyFinancialController.cs
??? DTO/
?   ??? MonthlyFinancialDTOs.cs
??? Enums/
?   ??? MonthEnum.cs
??? Models/
?   ??? MonthlyFinancialControl.cs
??? Repositories/
?   ??? Interfaces/
?   ?   ??? IMonthlyFinancialRepository.cs
?   ??? MonthlyFinancialRepository.cs
??? Services/
?   ??? Interfaces/
?   ?   ??? IMonthlyFinancialService.cs
?   ??? MonthlyFinancialService.cs
??? SQL/
    ??? MonthlyFinancialStoredProcedures.sql
```

---

## ?? Campos da Entidade

### Campos de Input (Enviados pelo Usuário):
- `Year` (int) - Ano
- `Month` (int) - Mês (1-12)
- `Money` (decimal) - Salário base
- `RV` (decimal) - Receita variável
- `Debit` (decimal) - Outros débitos
- `Others` (decimal) - Outras receitas
- `Reserve` (decimal) - Reserva financeira

### Campos Calculados Automaticamente:
- `SalaryTotal` = Money + RV + Debit + Others
- `ExpensesTotal` = Soma das despesas do mês/ano
- `Balance` = SalaryTotal - ExpensesTotal
- `CanSpend` = Balance - Reserve

---

## ?? Como Usar

### 1?? Executar SQL Script

Execute o script SQL para criar tabela e procedures:

```sql
-- Caminho: WebApplicationAPI/SQL/MonthlyFinancialStoredProcedures.sql
```

### 2?? Testar API

**Base URL:** `http://localhost:5296/api/v1/monthly-financial`

---

## ?? Endpoints

### 1. **POST /** - Criar novo registro

**Request:**
```json
{
  "year": 2025,
  "month": 5,
  "money": 5000.00,
  "rv": 1500.00,
  "debit": 200.00,
  "others": 300.00,
  "reserve": 1000.00
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "year": 2025,
  "month": 5,
  "monthName": "May",
  "money": 5000.00,
  "rv": 1500.00,
  "debit": 200.00,
  "others": 300.00,
  "reserve": 1000.00,
  "salaryTotal": 7000.00,
  "expensesTotal": 2500.00,
  "balance": 4500.00,
  "canSpend": 3500.00,
  "createdAt": "2025-05-15T10:30:00Z",
  "updatedAt": "2025-05-15T10:30:00Z"
}
```

---

### 2. **GET /** - Listar todos

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "year": 2025,
    "month": 5,
    "monthName": "May",
    "money": 5000.00,
    "rv": 1500.00,
    "debit": 200.00,
    "others": 300.00,
    "reserve": 1000.00,
    "salaryTotal": 7000.00,
    "expensesTotal": 2500.00,
    "balance": 4500.00,
    "canSpend": 3500.00,
    "createdAt": "2025-05-15T10:30:00Z",
    "updatedAt": "2025-05-15T10:30:00Z"
  }
]
```

---

### 3. **GET /{id}** - Buscar por ID

**Response (200 OK):**
```json
{
  "id": 1,
  "year": 2025,
  "month": 5,
  "monthName": "May",
  "money": 5000.00,
  "rv": 1500.00,
  "debit": 200.00,
  "others": 300.00,
  "reserve": 1000.00,
  "salaryTotal": 7000.00,
  "expensesTotal": 2500.00,
  "balance": 4500.00,
  "canSpend": 3500.00,
  "createdAt": "2025-05-15T10:30:00Z",
  "updatedAt": "2025-05-15T10:30:00Z"
}
```

---

### 4. **PUT /{id}** - Atualizar registro

**Request:**
```json
{
  "year": 2025,
  "month": 5,
  "money": 5500.00,
  "rv": 1500.00,
  "debit": 200.00,
  "others": 300.00,
  "reserve": 1200.00
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "year": 2025,
  "month": 5,
  "monthName": "May",
  "money": 5500.00,
  "rv": 1500.00,
  "debit": 200.00,
  "others": 300.00,
  "reserve": 1200.00,
  "salaryTotal": 7500.00,
  "expensesTotal": 2500.00,
  "balance": 5000.00,
  "canSpend": 3800.00,
  "createdAt": "2025-05-15T10:30:00Z",
  "updatedAt": "2025-05-15T11:45:00Z"
}
```

---

### 5. **DELETE /{id}** - Deletar registro

**Response (204 No Content)**

---

## ?? Cálculos Automáticos

### 1. Salary Total
```
SalaryTotal = Money + RV + Debit + Others
```

Exemplo:
```
Money: 5000.00
RV: 1500.00
Debit: 200.00
Others: 300.00
-------------------
SalaryTotal: 7000.00
```

### 2. Expenses Total
```sql
SELECT SUM(Amount) 
FROM Expenses 
WHERE Year = @Year AND Month = @Month
```

### 3. Balance
```
Balance = SalaryTotal - ExpensesTotal
```

Exemplo:
```
SalaryTotal: 7000.00
ExpensesTotal: 2500.00
-------------------
Balance: 4500.00
```

### 4. Can Spend
```
CanSpend = Balance - Reserve
```

Exemplo:
```
Balance: 4500.00
Reserve: 1000.00
-------------------
CanSpend: 3500.00
```

---

## ?? Validações

### 1. Duplicidade
Não é permitido criar dois registros para o mesmo **Year + Month**.

**Erro (400 Bad Request):**
```json
{
  "message": "Já existe um registro para 5/2025"
}
```

### 2. Campos Obrigatórios
- Year (2020-2100)
- Month (1-12)
- Money (? 0)

### 3. Valores Negativos
Campos numéricos não podem ser negativos (RV, Debit, Others, Reserve).

---

## ?? Enum de Meses

```csharp
public enum MonthEnum
{
    January = 1,
    February = 2,
    March = 3,
    April = 4,
    May = 5,
    June = 6,
    July = 7,
    August = 8,
    September = 9,
    October = 10,
    November = 11,
    December = 12
}
```

---

## ??? Stored Procedures Criadas

1. **sp_InsertMonthlyFinancial** - Inserir registro
2. **sp_UpdateMonthlyFinancial** - Atualizar registro
3. **sp_DeleteMonthlyFinancial** - Deletar registro
4. **sp_GetMonthlyFinancialById** - Buscar por ID
5. **sp_GetAllMonthlyFinancial** - Listar todos
6. **sp_GetMonthlyFinancialByYearMonth** - Buscar por ano/mês (validação)
7. **sp_GetMonthlyFinancialWithCalculations** - Relatório com cálculos (opcional)

---

## ?? Constraint Única

Tabela possui constraint para evitar duplicidade:

```sql
CONSTRAINT UQ_YearMonth UNIQUE (Year, Month)
```

---

## ? Checklist de Implementação

- [x] Model criado
- [x] DTOs criados
- [x] Enum de meses
- [x] Repository (Interface + Implementação)
- [x] Service (Interface + Implementação)
- [x] Controller
- [x] Stored Procedures SQL
- [x] Dependency Injection configurada
- [x] Validações implementadas
- [x] Cálculos automáticos
- [x] Integração com Expenses
- [x] Async/Await
- [x] Dapper
- [x] Arquitetura em camadas

---

## ?? Exemplos de Uso

### Cenário 1: Criar controle de Janeiro/2025

```bash
POST /api/v1/monthly-financial
```

```json
{
  "year": 2025,
  "month": 1,
  "money": 6000.00,
  "rv": 2000.00,
  "debit": 300.00,
  "others": 500.00,
  "reserve": 1500.00
}
```

**Resultado:**
- SalaryTotal: 8800.00
- ExpensesTotal: 3500.00 (buscado do banco)
- Balance: 5300.00
- CanSpend: 3800.00

---

### Cenário 2: Atualizar reserva

```bash
PUT /api/v1/monthly-financial/1
```

```json
{
  "year": 2025,
  "month": 1,
  "money": 6000.00,
  "rv": 2000.00,
  "debit": 300.00,
  "others": 500.00,
  "reserve": 2000.00
}
```

**Resultado:**
- CanSpend agora é: 3300.00 (diminuiu 500)

---

## ?? Pronto para Produção!

Este CRUD está completo e pronto para uso em ambiente de produção, seguindo:

? Clean Architecture  
? SOLID Principles  
? Separation of Concerns  
? Repository Pattern  
? Service Layer Pattern  
? DTO Pattern  
? Async Programming  
? Dapper ORM  
? SQL Server Stored Procedures  
? Validation  
? Error Handling  
? RESTful API Best Practices  

---

**Desenvolvido com ?? usando .NET 8**
