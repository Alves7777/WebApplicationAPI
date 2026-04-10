# ?? Extrato por Período - Statement Period

Endpoint para buscar extrato de despesas por intervalo de datas com validação de máximo 30 dias.

---

## ?? **Endpoint**

```
GET /api/creditcard/{id}/statement-period?startDate={startDate}&endDate={endDate}
```

---

## ?? **Parâmetros**

| Parâmetro | Tipo | Obrigatório | Descrição |
|-----------|------|-------------|-----------|
| `id` | int | ? Sim | ID do cartão de crédito |
| `startDate` | DateTime | ? Sim | Data inicial (formato: yyyy-MM-dd) |
| `endDate` | DateTime | ? Sim | Data final (formato: yyyy-MM-dd) |

---

## ? **Validações**

### **1. Período máximo de 30 dias**
```
Diferença entre endDate e startDate <= 30 dias
```

**Erro se exceder:**
```json
{
  "status": "fail",
  "message": "O período não pode exceder 30 dias",
  "data": null
}
```

### **2. Data final não pode ser anterior à inicial**
```
endDate >= startDate
```

**Erro se inverter:**
```json
{
  "status": "fail",
  "message": "A data final não pode ser anterior à data inicial",
  "data": null
}
```

### **3. Cartão deve existir**
```json
{
  "status": "fail",
  "message": "Cartão não encontrado",
  "data": null
}
```

---

## ?? **Exemplos de Uso**

### **Exemplo 1: Período de 13 dias (28/03 a 09/04)**

**Request:**
```
GET /api/creditcard/1/statement-period?startDate=2026-03-28&endDate=2026-04-09
```

**Response (200 OK):**
```json
{
  "status": "success",
  "message": "Extrato de 28/03/2026 até 09/04/2026",
  "data": {
    "cardName": "Nubank",
    "brand": "Mastercard",
    "month": 3,
    "year": 2026,
    "totalAmount": 1245.67,
    "totalTransactions": 23,
    "cardLimit": 5000.00,
    "availableLimit": 3754.33,
    "usagePercentage": 24.91,
    "expenses": [
      {
        "id": 45,
        "creditCardId": 1,
        "purchaseDate": "2026-04-09",
        "description": "Supermercado Cometa",
        "amount": 68.87,
        "category": "Alimentação",
        "month": 4,
        "year": 2026,
        "status": "PAGO",
        "createdAt": "2026-04-10T02:30:00Z"
      },
      {
        "id": 44,
        "creditCardId": 1,
        "purchaseDate": "2026-04-08",
        "description": "Uber Trip",
        "amount": 25.50,
        "category": "Transporte",
        "month": 4,
        "year": 2026,
        "status": "PAGO",
        "createdAt": "2026-04-10T02:30:00Z"
      },
      ...
    ]
  }
}
```

---

### **Exemplo 2: Período de 30 dias (01/03 a 30/03)**

**Request:**
```
GET /api/creditcard/1/statement-period?startDate=2026-03-01&endDate=2026-03-30
```

**Response (200 OK):**
```json
{
  "status": "success",
  "message": "Extrato de 01/03/2026 até 30/03/2026",
  "data": {
    "cardName": "Nubank",
    "totalAmount": 2145.67,
    "totalTransactions": 56,
    "expenses": [...]
  }
}
```

---

### **Exemplo 3: Erro - Período maior que 30 dias**

**Request:**
```
GET /api/creditcard/1/statement-period?startDate=2026-03-01&endDate=2026-05-01
```

**Response (400 Bad Request):**
```json
{
  "status": "fail",
  "message": "O período não pode exceder 30 dias",
  "data": null
}
```

**Cálculo:**
```
01/03/2026 até 01/05/2026 = 61 dias ? (máximo 30)
```

---

### **Exemplo 4: Erro - Data invertida**

**Request:**
```
GET /api/creditcard/1/statement-period?startDate=2026-04-09&endDate=2026-03-28
```

**Response (400 Bad Request):**
```json
{
  "status": "fail",
  "message": "A data final não pode ser anterior à data inicial",
  "data": null
}
```

---

## ?? **Cálculos no Response**

O endpoint retorna automaticamente:

```json
{
  "totalAmount": 1245.67,              // Soma de todas as despesas
  "totalTransactions": 23,             // Quantidade de despesas
  "cardLimit": 5000.00,                // Limite do cartão
  "availableLimit": 3754.33,           // Calculado: cardLimit - totalAmount
  "usagePercentage": 24.91             // Calculado: (totalAmount / cardLimit) * 100
}
```

---

## ?? **Cenários de Uso**

### **Cenário 1: Ver gastos entre duas faturas**
```
# Ver despesas entre 15/03 e 14/04
GET /api/creditcard/1/statement-period?startDate=2026-03-15&endDate=2026-04-14
```

### **Cenário 2: Gastos da última semana**
```
# 01/04 a 07/04 (7 dias)
GET /api/creditcard/1/statement-period?startDate=2026-04-01&endDate=2026-04-07
```

### **Cenário 3: Gastos do mês corrente até hoje**
```
# 01/04 a 10/04 (10 dias)
GET /api/creditcard/1/statement-period?startDate=2026-04-01&endDate=2026-04-10
```

### **Cenário 4: Período personalizado**
```
# Qualquer período de até 30 dias
GET /api/creditcard/1/statement-period?startDate=2026-03-20&endDate=2026-04-18
```

---

## ?? **Formatos de Data Aceitos**

### **Formato Recomendado (ISO 8601):**
```
yyyy-MM-dd
```

**Exemplos:**
- `2026-03-28` ?
- `2026-04-09` ?
- `2026-12-25` ?

### **Outros formatos aceitos:**
```
yyyy/MM/dd
dd-MM-yyyy
dd/MM/yyyy
```

**Exemplos:**
- `2026/03/28` ?
- `28-03-2026` ?
- `28/03/2026` ?

---

## ?? **Diferenças entre Endpoints**

| Endpoint | Filtro | Validação | Uso |
|----------|--------|-----------|-----|
| `/statement?month=3&year=2026` | Mês inteiro | Nenhuma | Fatura mensal |
| `/statement-period?startDate=...&endDate=...` | Período customizado | Máx 30 dias | Extrato flexível |
| `/expenses?month=3&year=2026` | Mês inteiro | Nenhuma | Lista simples |

---

## ?? **Como Testar**

### **PowerShell:**
```powershell
Invoke-RestMethod -Uri "http://localhost:5296/api/creditcard/1/statement-period?startDate=2026-03-28&endDate=2026-04-09" -Method Get
```

### **CURL:**
```bash
curl "http://localhost:5296/api/creditcard/1/statement-period?startDate=2026-03-28&endDate=2026-04-09"
```

### **Postman:**
```
GET http://localhost:5296/api/creditcard/1/statement-period
Params:
  - startDate: 2026-03-28
  - endDate: 2026-04-09
```

---

## ? **Implementado!**

Agora você tem:
- ? Extrato por período customizado
- ? Validação de máximo 30 dias
- ? Validação de datas invertidas
- ? Totais calculados automaticamente
- ? Despesas ordenadas por data (mais recente primeiro)

**Reinicia a API e testa! ??**

---

**Endpoint criado com sucesso!** ??
