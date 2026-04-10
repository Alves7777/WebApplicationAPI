# ?? Financial Control API - Sistema de Controle Financeiro

Sistema completo de controle financeiro desenvolvido em **.NET 8** com arquitetura REST API, seguindo as melhores práticas de Clean Architecture, CQRS, e Repository Pattern.

---

## ?? Índice

- [Visão Geral](#visão-geral)
- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Funcionalidades](#funcionalidades)
- [Instalação](#instalação)
- [Endpoints](#endpoints)
- [Padrões de Resposta](#padrões-de-resposta)
- [Banco de Dados](#banco-de-dados)
- [Próximos Passos](#próximos-passos)

---

## ?? Visão Geral

A **Financial Control API** é uma aplicação completa para gerenciamento financeiro pessoal que permite:

- ? Controlar despesas mensais
- ? Gerenciar múltiplos cartões de crédito
- ? Importar faturas em CSV automaticamente
- ? Categorizar despesas automaticamente (IA básica)
- ? Gerar relatórios e análises financeiras
- ? Visualizar extratos por período
- ? Controlar orçamento mensal

---

## ?? Tecnologias

### **Backend**
- ? **.NET 8** - Framework principal
- ? **ASP.NET Core Web API** - REST API
- ? **C# 12** - Linguagem de programação
- ? **Dapper** - Micro ORM para performance
- ? **Entity Framework Core** - ORM (quando necessário)
- ? **SQL Server** - Banco de dados
- ? **Stored Procedures** - Toda lógica SQL (padrão do projeto)

### **Design Patterns & Arquitetura**
- ? **Clean Architecture** - Separação de camadas
- ? **CQRS (MediatR)** - Command Query Responsibility Segregation
- ? **Repository Pattern** - Abstração de acesso a dados
- ? **Dependency Injection** - Inversão de controle
- ? **DTO Pattern** - Data Transfer Objects
- ? **Response Pattern** - Padronização de respostas (success/fail/error)

### **Bibliotecas**
- ? **FluentValidation** - Validação de dados
- ? **AutoMapper** - Mapeamento de objetos
- ? **MediatR** - Mediator pattern
- ? **CsvHelper** - Importação de CSV
- ? **Serilog** - Logging estruturado (futuro)
- ? **Swagger/OpenAPI** - Documentação automática
---

## ? Funcionalidades

### **1. Gestão de Despesas** ??

- ? CRUD completo de despesas
- ? Filtros avançados (mês, ano, categoria, status, método de pagamento)
- ? Categorização manual
- ? Relatórios de despesas
- ? Análise por categoria
- ? CQRS com MediatR

**Endpoints:**
- `POST /api/expense` - Criar despesa
- `GET /api/expense` - Listar com filtros
- `PUT /api/expense/{id}` - Atualizar
- `PATCH /api/expense/{id}` - Atualizar parcial
- `DELETE /api/expense/{id}` - Deletar
- `GET /api/expense/report` - Relatório detalhado

---

### **2. Controle Mensal** ??

- ? Registro de receitas mensais (salário, RV, débitos, outros)
- ? Cálculo automático de totais
- ? Saldo disponível e reserva
- ? Comparação mês a mês
- ? Filtro por ano/mês

**Endpoints:**
- `POST /api/v1/monthly-financial` - Criar controle mensal
- `GET /api/v1/monthly-financial` - Listar todos
- `GET /api/v1/monthly-financial?year=2026` - Filtrar por ano
- `GET /api/v1/monthly-financial?year=2026&month=4` - Filtrar por mês específico
- `PUT /api/v1/monthly-financial/{id}` - Atualizar
- `DELETE /api/v1/monthly-financial/{id}` - Deletar

**Cálculos Automáticos:**
```
SalaryTotal = Money + RV + Debit + Others
ExpensesTotal = SUM(despesas do mês)
Balance = SalaryTotal - ExpensesTotal
CanSpend = Balance - Reserve
```

---

### **3. Cartões de Crédito** ??

#### **3.1 Gerenciamento de Cartões**

- ? CRUD de cartões
- ? Múltiplos cartões
- ? Controle de limite
- ? Dias de fechamento e vencimento

**Endpoints:**
- `POST /api/creditcard` - Criar cartão
- `GET /api/creditcard` - Listar cartões
- `GET /api/creditcard/{id}` - Buscar por ID
- `PUT /api/creditcard/{id}` - Atualizar
- `DELETE /api/creditcard/{id}` - Deletar

#### **3.2 Despesas do Cartão**

- ? CRUD de despesas
- ? Categorização automática (12 categorias)
- ? Filtros (mês, ano, categoria)

**Endpoints:**
- `POST /api/creditcard/{id}/expenses` - Criar despesa
- `GET /api/creditcard/{id}/expenses` - Listar despesas
- `GET /api/creditcard/{id}/expenses?month=3&year=2026` - Filtrar por mês
- `PUT /api/creditcard/expenses/{id}` - Atualizar
- `DELETE /api/creditcard/expenses/{id}` - Deletar

#### **3.3 Importação de CSV (Nubank)** ??

- ? Importação automática de fatura em CSV
- ? Categorização automática inteligente
- ? Proteção contra duplicidade
- ? Validação de dados
- ? Relatório de importação

**Endpoint:**
```
POST /api/creditcard/{id}/import-csv
Content-Type: multipart/form-data
Body: file (CSV)
```

**Formato CSV:**
```csv
date,title,amount
2026-03-27,Mercado901ltda,9.69
2026-03-25,Uber Trip,15.50
```

**Categorização Automática:**
| Palavra-chave | Categoria |
|---------------|-----------|
| mercado, supermercado | Alimentação |
| uber, posto, gasolina | Transporte |
| farmacia, clinica | Saúde |
| pet shop | Pets |
| barbearia, salao | Beleza |
| amazon, mercadolivre | Compras Online |
| claro, vivo, tim | Telefonia |

**Proteção contra Duplicidade:**
- Verifica se despesa já existe (data + descrição + valor)
- Ignora duplicados automaticamente
- Retorna quantidade de duplicados encontrados

#### **3.4 Relatórios & Analytics** ??

**Fatura Mensal:**
```
GET /api/creditcard/{id}/statement?month=3&year=2026
```

Retorna:
- Total gasto
- Quantidade de transações
- Limite disponível
- Percentual de uso
- Lista de despesas

**Análise por Categoria:**
```
GET /api/creditcard/{id}/by-category?month=3&year=2026
```

Retorna:
- Total por categoria
- Quantidade de transações
- Média de valor
- Percentual do total

**Extrato por Período:**
```
GET /api/creditcard/{id}/statement-period?startDate=2026-03-28&endDate=2026-04-10
```

Validações:
- ? Máximo 30 dias entre datas
- ? Data final não pode ser anterior à inicial

---

### **4. Categorias** ???

- ? Gerenciamento de categorias
- ? Categorias customizadas

**Endpoints:**
- `POST /api/category` - Criar categoria
- `GET /api/category` - Listar categorias
- `GET /api/category/{id}` - Buscar por ID

---

## ?? Instalação

### **Pré-requisitos**

- ? **.NET 8 SDK** instalado
- ? **SQL Server** (LocalDB ou Express)
- ? **Visual Studio 2022** ou **VS Code**
- ? **SQL Server Management Studio (SSMS)** (opcional)

### **Passo a Passo**

1. **Clone o repositório:**
```bash
git clone https://github.com/Alves7777/WebApplicationAPI.git
cd WebApplicationAPI
```

2. **Configure a Connection String:**

Edite `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WebAppDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3. **Execute os scripts SQL:**

Execute na ordem:
```sql
-- 1. Criar banco e tabelas principais
WebApplicationAPI/SQL/ExpenseStoredProcedures.sql

-- 2. Controle mensal
WebApplicationAPI/SQL/MonthlyFinancialStoredProcedures.sql

-- 3. Cartões de crédito
WebApplicationAPI/SQL/CreditCardStoredProcedures.sql
WebApplicationAPI/SQL/sp_CheckCreditCardExpenseExists.sql
WebApplicationAPI/SQL/sp_GetCreditCardExpensesByPeriod.sql
WebApplicationAPI/SQL/sp_GetExpensesTotalByYearAndMonth.sql
```

4. **Rode a aplicação:**
```bash
dotnet run
```

5. **Acesse o Swagger:**
```
https://localhost:5296/swagger
```

---

## ?? Endpoints

### **Base URL**
```
http://localhost:5296
```

### **Resumo de Endpoints**

| Recurso | Método | Endpoint | Descrição |
|---------|--------|----------|-----------|
| **Expenses** | POST | `/api/expense` | Criar despesa |
| | GET | `/api/expense` | Listar com filtros |
| | PUT | `/api/expense/{id}` | Atualizar |
| | DELETE | `/api/expense/{id}` | Deletar |
| | GET | `/api/expense/report` | Relatório |
| **Monthly Financial** | POST | `/api/v1/monthly-financial` | Criar controle |
| | GET | `/api/v1/monthly-financial` | Listar todos |
| | GET | `/api/v1/monthly-financial?year=2026` | Filtrar por ano |
| | PUT | `/api/v1/monthly-financial/{id}` | Atualizar |
| | DELETE | `/api/v1/monthly-financial/{id}` | Deletar |
| **Credit Cards** | POST | `/api/creditcard` | Criar cartão |
| | GET | `/api/creditcard` | Listar cartões |
| | POST | `/api/creditcard/{id}/expenses` | Criar despesa |
| | POST | `/api/creditcard/{id}/import-csv` | Importar CSV |
| | GET | `/api/creditcard/{id}/statement` | Fatura mensal |
| | GET | `/api/creditcard/{id}/statement-period` | Extrato período |
| **Categories** | POST | `/api/category` | Criar categoria |
| | GET | `/api/category` | Listar categorias |

---

## ?? Padrões de Resposta

Todas as respostas seguem o padrão `ApiResponse`:

### **Success (200, 201)**
```json
{
  "status": "success",
  "message": "Operação realizada com sucesso",
  "data": { ... }
}
```

### **Fail (400, 404)**
```json
{
  "status": "fail",
  "message": "Mensagem de erro",
  "data": null
}
```

### **Error (500)**
```json
{
  "status": "error",
  "message": "Erro interno: ...",
  "data": null
}
```

---

## ??? Banco de Dados

### **Tabelas Principais**

```sql
-- Despesas gerais
Expenses (
    Id, Description, Amount, Category, 
    Month, Year, Status, PaymentMethod, 
    CreatedAt, UpdatedAt
)

-- Controle mensal
MonthlyFinancialControl (
    Id, Year, Month, Money, RV, Debit, 
    Others, Reserve, CreatedAt, UpdatedAt
)

-- Cartões de crédito
CreditCards (
    Id, Name, Brand, CardLimit, 
    ClosingDay, DueDay, IsActive, 
    CreatedAt, UpdatedAt
)

-- Despesas do cartão
CreditCardExpenses (
    Id, CreditCardId, PurchaseDate, 
    Description, Amount, Category, 
    Month, Year, Status, CreatedAt
)

-- Categorias
Categories (
    Id, Name, Description, 
    CreatedAt, UpdatedAt
)
```

### **Stored Procedures**

**Total:** 30+ procedures

Todas seguem o padrão `sp_` + `Nome`:
- `sp_CreateExpense`
- `sp_GetAllExpenses`
- `sp_CreateCreditCard`
- `sp_GetCreditCardExpensesByPeriod`
- `sp_CheckCreditCardExpenseExists`
- ...

---

## ?? Exemplos de Uso

### **1. Criar um Cartão de Crédito**

```bash
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

### **2. Importar CSV do Nubank**

```bash
curl -X POST http://localhost:5296/api/creditcard/1/import-csv \
  -F "file=@Nubank_2026-03.csv"
```

### **3. Ver Fatura do Mês**

```bash
curl -X GET "http://localhost:5296/api/creditcard/1/statement?month=3&year=2026"
```

### **4. Extrato por Período**

```bash
curl -X GET "http://localhost:5296/api/creditcard/1/statement-period?startDate=2026-03-28&endDate=2026-04-10"
```

### **5. Criar Controle Mensal**

```bash
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{
    "year": 2026,
    "month": 4,
    "money": 6000.00,
    "rv": 2000.00,
    "debit": 300.00,
    "others": 500.00,
    "reserve": 1500.00
  }'
```

---

## ?? Segurança

### **Implementado:**
- ? Validação de dados (FluentValidation)
- ? Global Exception Handler
- ? Stored Procedures (proteção contra SQL Injection)
- ? DTO Pattern (não expõe entidades diretamente)

### **A Implementar (Futuro):**
- ? Autenticação JWT
- ? Autorização por roles
- ? Rate Limiting
- ? CORS configurado

---

## ?? Performance

- ? **Dapper** para consultas rápidas
- ? **Stored Procedures** otimizadas
- ? **Async/Await** em todas as operações
- ? **Conexões gerenciadas** (using statements)
- ? **Queries otimizadas** com índices

---

## ?? Testes

### **Postman Collection**
Disponível em: `WebApplicationAPI/Postman/CreditCard_API_Collection.json`

### **Como testar:**
1. Importe a collection no Postman
2. Configure `baseUrl` = `http://localhost:5296`
3. Execute os requests na ordem

---

## ?? Documentação

### **Swagger/OpenAPI**
```
https://localhost:5296/swagger
```

### **Documentação Adicional:**
- `SQL/README_MONTHLY_FINANCIAL.md` - Controle Mensal
- `SQL/README_CREDITCARD.md` - Cartões de Crédito
- `SQL/DUPLICATE_PROTECTION.md` - Proteção contra Duplicidade
- `SQL/STATEMENT_PERIOD_DOCS.md` - Extrato por Período
- `SQL/STORED_PROCEDURES_SUMMARY.md` - Resumo de Procedures
- `Postman/POSTMAN_GUIDE.md` - Guia Postman
- `Postman/QUICK_TEST_GUIDE.md` - Guia Rápido de Testes

---

## ?? Próximos Passos

### **Microserviços (Próxima Etapa)**

Serão implementados 3 microserviços:

1. **Notification Service** ?
   - Envio de emails (alertas, relatórios)
   - Push notifications
   - Background jobs (Hangfire/Quartz.NET)

2. **Analytics Service** ??
   - Relatórios avançados (PDF, Excel)
   - Machine Learning (previsões)
   - Análises e insights

3. **Integration Service** ??
   - Importação de múltiplos formatos
   - Integração com APIs externas
   - Backup automático (Azure Blob)

### **Frontend**
- React + TypeScript (ou Blazor)
- Dashboard interativo
- Gráficos (Recharts/Chart.js)
- Upload de CSV drag-and-drop

---

## ?? Contribuição

Projeto desenvolvido como sistema de controle financeiro pessoal.

### **Autor**
- **GitHub:** [@Alves7777](https://github.com/Alves7777)
- **Repositório:** [WebApplicationAPI](https://github.com/Alves7777/WebApplicationAPI)

---

## ?? Licença

Este projeto é de código aberto e está disponível sob a licença MIT.

---

## ?? Agradecimentos

Obrigado por usar a **Financial Control API**! 

Para dúvidas ou sugestões, abra uma issue no GitHub.

---

**Desenvolvido com ?? usando .NET 8**

---

## ?? Status do Projeto

```
? Backend API REST - 100% concluído
? CRUD de Despesas - Completo
? Controle Mensal - Completo
? Cartões de Crédito - Completo
? Importação CSV - Completo com proteção de duplicidade
? Stored Procedures - Padrão implementado
? Padrão de Resposta - Unificado
? Documentação - Completa

? Microserviços - Próxima etapa
? Frontend - A desenvolver
? Autenticação JWT - Futuro
? Deploy (Azure) - Futuro
```

---

**Versão:** 1.0.0  
**Última atualização:** 10/04/2026  
**Status:** Produção Ready ??
