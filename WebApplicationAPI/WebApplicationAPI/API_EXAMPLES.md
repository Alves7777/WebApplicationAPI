# Exemplos de Requisições - API de Despesas

## ?? Endpoint Base
```
http://localhost:5296/api/expense
```

## ? 1. Criar Nova Despesa (POST)
**URL:** `POST http://localhost:5296/api/expense`

**Headers:**
```
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "month": 1,
  "year": 2025,
  "description": "Compra no supermercado",
  "amount": 250.50,
  "category": "Alimentação",
  "status": "Pago",
  "paymentMethod": "Cartão de Crédito"
}
```

### Outros Exemplos de Body:

**Despesa de Transporte:**
```json
{
  "month": 1,
  "year": 2025,
  "description": "Combustível",
  "amount": 180.00,
  "category": "Transporte",
  "status": "Pago",
  "paymentMethod": "PIX"
}
```

**Despesa de Moradia:**
```json
{
  "month": 1,
  "year": 2025,
  "description": "Aluguel",
  "amount": 1500.00,
  "category": "Moradia",
  "status": "Pendente",
  "paymentMethod": "Transferência Bancária"
}
```

**Despesa de Lazer:**
```json
{
  "month": 1,
  "year": 2025,
  "description": "Cinema",
  "amount": 45.00,
  "category": "Lazer",
  "status": "Pago",
  "paymentMethod": "Cartão de Débito"
}
```

---

## ?? 2. Buscar Todas as Despesas (GET)
**URL:** `GET http://localhost:5296/api/expense`

**Headers:**
```
Accept: application/json
```

**Sem body necessário**

---

## ?? 3. Buscar Despesas com Filtros (GET)
**URL:** `GET http://localhost:5296/api/expense?month=1&year=2025&category=Alimentação`

**Query Parameters:**
- `month` (opcional): Número do mês (1-12)
- `year` (opcional): Ano (ex: 2025)
- `category` (opcional): Categoria da despesa
- `status` (opcional): Status (Pago, Pendente, etc.)
- `paymentMethod` (opcional): Método de pagamento

**Exemplos de URLs com filtros:**
```
GET http://localhost:5296/api/expense?month=1&year=2025
GET http://localhost:5296/api/expense?category=Transporte
GET http://localhost:5296/api/expense?status=Pago
GET http://localhost:5296/api/expense?month=1&year=2025&category=Alimentação&status=Pago
```

---

## ?? 4. Atualizar Despesa (PUT)
**URL:** `PUT http://localhost:5296/api/expense/{id}`

*Substitua `{id}` pelo ID real da despesa (ex: `1`)*

**Headers:**
```
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "month": 1,
  "year": 2025,
  "description": "Compra no supermercado - Atualizado",
  "amount": 300.00,
  "category": "Alimentação",
  "status": "Pago",
  "paymentMethod": "Dinheiro"
}
```

**Exemplo de URL:**
```
PUT http://localhost:5296/api/expense/1
```

---

## ? 5. Deletar Despesa (DELETE)
**URL:** `DELETE http://localhost:5296/api/expense/{id}`

*Substitua `{id}` pelo ID real da despesa (ex: `1`)*

**Sem body necessário**

**Exemplo de URL:**
```
DELETE http://localhost:5296/api/expense/1
```

---

## ?? 6. Buscar Resumo Financeiro (GET)
**URL:** `GET http://localhost:5296/api/summary?month=1&year=2025&monthlyIncome=5000`

**Query Parameters:**
- `month` (obrigatório): Número do mês (1-12)
- `year` (obrigatório): Ano (ex: 2025)
- `monthlyIncome` (obrigatório): Renda mensal

**Exemplo de URL:**
```
GET http://localhost:5296/api/summary?month=1&year=2025&monthlyIncome=5000
```

---

## ?? Campos Obrigatórios

### CreateExpenseRequest:
- ? `month` (int): 1-12
- ? `year` (int): maior que 2000
- ? `description` (string): não vazio
- ? `amount` (decimal): maior que 0
- ? `category` (string): não vazio
- ? `status` (string): não vazio
- ? `paymentMethod` (string): não vazio

### Sugestões de Valores:

**Categories (Categorias):**
- Alimentação
- Transporte
- Moradia
- Lazer
- Saúde
- Educação
- Vestuário
- Serviços
- Outros

**Status:**
- Pago
- Pendente
- Cancelado

**Payment Methods (Métodos de Pagamento):**
- Dinheiro
- Cartão de Crédito
- Cartão de Débito
- PIX
- Transferência Bancária
- Boleto
- Cheque

---

## ?? Testando no Postman

1. Crie uma nova requisição
2. Selecione o método HTTP (POST, GET, PUT, DELETE)
3. Cole a URL
4. Se necessário, vá em "Body" ? "raw" ? selecione "JSON"
5. Cole o JSON de exemplo
6. Clique em "Send"

---

## ?? Troubleshooting

### Erro 400 - Validation Error
Certifique-se de que todos os campos obrigatórios estão presentes no JSON.

### Erro 404 - Not Found
Verifique se a URL está correta e se o servidor está rodando na porta 5296.

### Erro 500 - Internal Server Error
Verifique se o banco de dados está configurado e se as tabelas foram criadas.
