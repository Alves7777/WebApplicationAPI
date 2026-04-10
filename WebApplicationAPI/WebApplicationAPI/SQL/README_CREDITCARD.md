# ?? Credit Card Management - CRUD Completo

Sistema completo de gerenciamento de cartões de crédito com importação automática de CSV e categorização inteligente.

---

## ?? Funcionalidades

? CRUD completo de Cartões de Crédito  
? CRUD completo de Despesas do Cartão  
? **Importação automática de CSV (Nubank, etc)**  
? **Categorização automática de despesas**  
? Relatórios e análises por categoria  
? Fatura mensal detalhada  
? Controle de limite do cartão  

---

## ?? Endpoints

### **Cartões de Crédito**

```
POST   /api/creditcard                    ? Criar cartão
GET    /api/creditcard                    ? Listar todos
GET    /api/creditcard/{id}               ? Buscar por ID
PUT    /api/creditcard/{id}               ? Atualizar
DELETE /api/creditcard/{id}               ? Deletar
```

### **Despesas do Cartão**

```
POST   /api/creditcard/{id}/expenses      ? Criar despesa
GET    /api/creditcard/{id}/expenses      ? Listar despesas
GET    /api/creditcard/expenses/{id}      ? Buscar despesa por ID
PUT    /api/creditcard/expenses/{id}      ? Atualizar despesa
DELETE /api/creditcard/expenses/{id}      ? Deletar despesa
```

### **Importação CSV**

```
POST   /api/creditcard/{id}/import-csv    ? Importar CSV (Nubank)
```

### **Relatórios**

```
GET /api/creditcard/{id}/statement?month=3&year=2026    ? Fatura mensal
GET /api/creditcard/{id}/by-category?month=3&year=2026  ? Análise por categoria
```

---

## ?? Como Usar

### **1. Executar SQL Script**

Execute o script para criar tabelas e procedures:

```sql
-- Arquivo: WebApplicationAPI/SQL/CreditCardStoredProcedures.sql
```

### **2. Parar e Reiniciar a Aplicação**

```sh
# Parar debug (Shift + F5)
# Iniciar novamente (F5)
```

### **3. Criar um Cartão**

```sh
curl -X POST http://localhost:5296/api/creditcard \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Nubank",
    "brand": "Mastercard",
    "cardLimit": 5000.00,
    "closingDay": 10,
    "dueDay": 17,
    "isActive": true
  }'
```

**Response:**
```json
{
  "id": 1,
  "name": "Nubank",
  "brand": "Mastercard",
  "cardLimit": 5000.00,
  "closingDay": 10,
  "dueDay": 17,
  "isActive": true,
  "createdAt": "2026-04-10T14:30:00Z",
  "updatedAt": "2026-04-10T14:30:00Z"
}
```

### **4. Importar CSV do Nubank** ?

```sh
curl -X POST http://localhost:5296/api/creditcard/1/import-csv \
  -F "file=@Nubank_2026-03.csv"
```

**Response:**
```json
{
  "totalRecords": 58,
  "importedRecords": 56,
  "errorCount": 2,
  "errors": [
    "Data inválida: invalid-date - Descrição"
  ],
  "categoriesCount": {
    "Alimentação": 15,
    "Transporte": 12,
    "Saúde": 8,
    "Compras Online": 6,
    "Beleza": 3,
    "Telefonia": 2,
    "Outros": 10
  }
}
```

### **5. Ver Fatura do Mês**

```sh
curl -X GET "http://localhost:5296/api/creditcard/1/statement?month=3&year=2026"
```

**Response:**
```json
{
  "cardName": "Nubank",
  "brand": "Mastercard",
  "month": 3,
  "year": 2026,
  "totalAmount": 2145.67,
  "totalTransactions": 56,
  "cardLimit": 5000.00,
  "availableLimit": 2854.33,
  "usagePercentage": 42.91,
  "expenses": [...]
}
```

### **6. Análise por Categoria**

```sh
curl -X GET "http://localhost:5296/api/creditcard/1/by-category?month=3&year=2026"
```

**Response:**
```json
[
  {
    "category": "Alimentação",
    "totalAmount": 450.80,
    "transactionCount": 15,
    "averageAmount": 30.05,
    "percentage": 21.01
  },
  {
    "category": "Transporte",
    "totalAmount": 380.50,
    "transactionCount": 12,
    "averageAmount": 31.71,
    "percentage": 17.73
  },
  ...
]
```

---

## ?? Categorização Automática

O sistema categoriza automaticamente baseado em palavras-chave na descrição:

| Categoria | Palavras-chave |
|-----------|----------------|
| **Alimentação** | mercado, supermercado, mcdonald, buffet, restaurante, padaria, acai |
| **Transporte** | uber, posto, gasolina, taxi, 99, estacionamento |
| **Saúde** | farmacia, pague menos, clinica, medic, hospital, academia |
| **Pets** | pet, veterinari, animal |
| **Beleza** | barbearia, salao, cabelo, manicure |
| **Compras Online** | amazon, mercadolivre, shopee, aliexpress |
| **Telefonia** | claro, vivo, tim, oi, telefone, internet |
| **Vestuário** | calcado, roupa, loja, moda, tenis |
| **Lazer** | cinema, teatro, show, viagem, hotel, pousada |
| **Seguros** | seguro, azul seguros |
| **Educação** | curso, escola, faculdade, livro |
| **Outros** | Qualquer outra coisa não categorizada |

---

## ?? Formato do CSV

O CSV do Nubank deve ter 3 colunas:

```csv
date,title,amount
2026-03-27,Mercado901ltda,9.69
2026-03-25,Sobral e Palacio - Fin,14.60
2026-03-23,Mc Donalds Tzm,6.00
```

**Regras:**
- ? Header obrigatório: `date,title,amount`
- ? Data no formato: `YYYY-MM-DD`
- ? Valores negativos são ignorados (ex: pagamentos recebidos)
- ? Categorização automática baseada no `title`

---

## ?? Exemplos de Uso Real

### **Cenário 1: Importar fatura do Nubank**

1. Receber email do Nubank com CSV
2. Baixar arquivo `Nubank_2026-03.csv`
3. Fazer upload via Postman:
   ```
   POST /api/creditcard/1/import-csv
   Form-data: file = [selecionar arquivo]
   ```
4. Pronto! 56 despesas categorizadas em 2 segundos

### **Cenário 2: Ver onde está gastando mais**

```sh
GET /api/creditcard/1/by-category?month=3&year=2026
```

**Resultado:**
- Alimentação: R$ 450 (21%)
- Transporte: R$ 380 (17%)
- Saúde: R$ 220 (10%)
- **Insight:** "Você está gastando muito com alimentação!"

### **Cenário 3: Criar despesa manual**

```sh
POST /api/creditcard/1/expenses
{
  "purchaseDate": "2026-04-10",
  "description": "Restaurante Japonês",
  "amount": 150.00,
  "status": "PENDENTE"
}
```

**Sistema categoriza automaticamente** ? "Alimentação"

---

## ?? Filtros Disponíveis

### **Filtrar despesas:**

```sh
# Todas as despesas de um cartão
GET /api/creditcard/1/expenses

# Despesas de um mês específico
GET /api/creditcard/1/expenses?month=3&year=2026

# Despesas de uma categoria
GET /api/creditcard/1/expenses?category=Alimentação

# Combinar filtros
GET /api/creditcard/1/expenses?month=3&year=2026&category=Transporte
```

---

## ?? Tabelas Criadas

### **CreditCards**
```sql
Id INT PRIMARY KEY
Name NVARCHAR(100)
Brand NVARCHAR(50)
CardLimit DECIMAL(18,2)
ClosingDay INT
DueDay INT
IsActive BIT
CreatedAt DATETIME
UpdatedAt DATETIME
```

### **CreditCardExpenses**
```sql
Id INT PRIMARY KEY
CreditCardId INT (FK)
PurchaseDate DATE
Description NVARCHAR(500)
Amount DECIMAL(18,2)
Category NVARCHAR(100)
Month INT
Year INT
Status NVARCHAR(50)
CreatedAt DATETIME
```

---

## ??? Tecnologias

- ? .NET 8 ASP.NET Core
- ? Dapper (consultas SQL)
- ? CsvHelper (parse de CSV)
- ? SQL Server (banco de dados)
- ? Stored Procedures

---

## ?? Stored Procedures Criadas

1. `sp_CreateCreditCard` - Criar cartão
2. `sp_UpdateCreditCard` - Atualizar cartão
3. `sp_DeleteCreditCard` - Deletar cartão
4. `sp_GetCreditCardById` - Buscar por ID
5. `sp_GetAllCreditCards` - Listar todos
6. `sp_CreateCreditCardExpense` - Criar despesa
7. `sp_UpdateCreditCardExpense` - Atualizar despesa
8. `sp_DeleteCreditCardExpense` - Deletar despesa
9. `sp_GetCreditCardExpenseById` - Buscar despesa
10. `sp_GetCreditCardExpensesByCard` - Listar despesas
11. `sp_GetCreditCardStatement` - Fatura mensal
12. `sp_GetCreditCardExpensesByCategory` - Análise por categoria

---

## ? Próximos Passos

1. ? Executar SQL script
2. ? Parar e reiniciar aplicação
3. ? Criar um cartão de crédito
4. ? Importar CSV do Nubank
5. ? Ver fatura e análises

---

## ?? Pronto para Usar!

O CRUD está 100% funcional com:
- ? Importação automática de CSV
- ? Categorização inteligente
- ? Relatórios detalhados
- ? Filtros avançados
- ? Análise por categoria

**Desenvolvido com ?? usando .NET 8**
