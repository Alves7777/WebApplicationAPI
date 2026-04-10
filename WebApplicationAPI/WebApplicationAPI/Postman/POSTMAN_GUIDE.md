# ?? Postman Collection - CreditCard API

Guia completo para importar e usar a Collection do Postman para a API de Cartões de Crédito.

---

## ?? Como Importar no Postman

### **Opção 1: Importar pelo arquivo**

1. Abra o Postman
2. Clique em **"Import"** (canto superior esquerdo)
3. Clique em **"Upload Files"**
4. Selecione o arquivo: `CreditCard_API_Collection.json`
5. Clique em **"Import"**

### **Opção 2: Importar pelo código**

1. Abra o Postman
2. Clique em **"Import"**
3. Clique em **"Raw text"**
4. Cole todo o conteúdo do arquivo `CreditCard_API_Collection.json`
5. Clique em **"Continue"** ? **"Import"**

---

## ?? Configurar Variável de Ambiente

A collection usa a variável `{{baseUrl}}` que está configurada como `http://localhost:5296`.

Se sua API estiver rodando em outra porta:

1. No Postman, vá em **Collections**
2. Clique nos **3 pontinhos** ao lado de **"CreditCard API - Financial Control"**
3. Clique em **"Edit"**
4. Vá na aba **"Variables"**
5. Altere o valor de `baseUrl` para sua porta
6. Clique em **"Save"**

---

## ?? Estrutura da Collection

A collection está organizada em 6 pastas:

```
?? CreditCard API - Financial Control
?
??? ?? Credit Cards (5 requests)
?   ??? Create Credit Card
?   ??? Get All Credit Cards
?   ??? Get Credit Card by ID
?   ??? Update Credit Card
?   ??? Delete Credit Card
?
??? ?? Credit Card Expenses (7 requests)
?   ??? Create Expense
?   ??? Get All Expenses by Card
?   ??? Get Expenses by Card (Filtered by Month)
?   ??? Get Expenses by Card (Filtered by Category)
?   ??? Get Expense by ID
?   ??? Update Expense
?   ??? Delete Expense
?
??? ?? CSV Import (1 request)
?   ??? Import CSV (Nubank)
?
??? ?? Analytics & Reports (2 requests)
?   ??? Get Statement (Fatura Mensal)
?   ??? Get Expenses by Category
?
??? ?? Examples - Multiple Cards (3 requests)
?   ??? Create Nubank Card
?   ??? Create Credicard
?   ??? Create Inter Card
?
??? ?? Examples - Manual Expenses (3 requests)
    ??? Add Alimentação Expense
    ??? Add Transporte Expense
    ??? Add Saúde Expense
```

---

## ?? Guia de Uso Rápido

### **1. Criar um Cartão de Crédito**

**Request:** `POST /api/creditcard`

**Pasta:** Credit Cards ? Create Credit Card

**Body:**
```json
{
  "name": "Nubank",
  "brand": "Mastercard",
  "cardLimit": 5000.00,
  "closingDay": 10,
  "dueDay": 17,
  "isActive": true
}
```

**Response (201 Created):**
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

**?? Anote o `id` retornado!**

---

### **2. Importar CSV do Nubank** ?

**Request:** `POST /api/creditcard/1/import-csv`

**Pasta:** CSV Import ? Import CSV (Nubank)

**Como fazer:**

1. Abra a request no Postman
2. Vá na aba **"Body"**
3. Selecione **"form-data"**
4. Na linha `file`:
   - Clique no dropdown à direita
   - Selecione **"File"**
   - Clique em **"Select Files"**
   - Escolha o arquivo `Nubank_2026-03.csv`
5. Clique em **"Send"**

**Response (200 OK):**
```json
{
  "totalRecords": 58,
  "importedRecords": 56,
  "errorCount": 2,
  "errors": [
    "Data inválida: invalid-date"
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

---

### **3. Ver Fatura do Mês**

**Request:** `GET /api/creditcard/1/statement?month=3&year=2026`

**Pasta:** Analytics & Reports ? Get Statement

**Query Params:**
- `month`: 3
- `year`: 2026

**Response (200 OK):**
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
  "expenses": [
    {
      "id": 1,
      "creditCardId": 1,
      "purchaseDate": "2026-03-27",
      "description": "Mercado901ltda",
      "amount": 9.69,
      "category": "Alimentação",
      "month": 3,
      "year": 2026,
      "status": "PAGO",
      "createdAt": "2026-04-10T14:30:00Z"
    },
    ...
  ]
}
```

---

### **4. Análise por Categoria**

**Request:** `GET /api/creditcard/1/by-category?month=3&year=2026`

**Pasta:** Analytics & Reports ? Get Expenses by Category

**Query Params:**
- `month`: 3
- `year`: 2026

**Response (200 OK):**
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
  {
    "category": "Saúde",
    "totalAmount": 220.00,
    "transactionCount": 8,
    "averageAmount": 27.50,
    "percentage": 10.25
  },
  ...
]
```

---

### **5. Criar Despesa Manual**

**Request:** `POST /api/creditcard/1/expenses`

**Pasta:** Credit Card Expenses ? Create Expense

**Body:**
```json
{
  "purchaseDate": "2026-04-10",
  "description": "Restaurante Japonês",
  "amount": 150.00,
  "status": "PENDENTE"
}
```

**?? Note que NÃO precisa enviar `category`!**

O sistema categoriza automaticamente:
- "Restaurante" ? Categoria: **"Alimentação"**

**Response (201 Created):**
```json
{
  "id": 57,
  "creditCardId": 1,
  "purchaseDate": "2026-04-10",
  "description": "Restaurante Japonês",
  "amount": 150.00,
  "category": "Alimentação",
  "month": 4,
  "year": 2026,
  "status": "PENDENTE",
  "createdAt": "2026-04-10T15:00:00Z"
}
```

---

## ?? Cenários de Teste

### **Cenário 1: Setup Completo**

Execute na ordem:

1. **Create Nubank Card** (Examples - Multiple Cards)
2. **Import CSV (Nubank)** (CSV Import)
3. **Get Statement** (Analytics & Reports)
4. **Get Expenses by Category** (Analytics & Reports)

### **Cenário 2: Múltiplos Cartões**

Execute na ordem:

1. **Create Nubank Card** (Examples - Multiple Cards)
2. **Create Credicard** (Examples - Multiple Cards)
3. **Create Inter Card** (Examples - Multiple Cards)
4. **Get All Credit Cards** (Credit Cards)

### **Cenário 3: Categorização Automática**

Execute na ordem:

1. **Add Alimentação Expense** (Examples - Manual Expenses)
2. **Add Transporte Expense** (Examples - Manual Expenses)
3. **Add Saúde Expense** (Examples - Manual Expenses)
4. **Get All Expenses by Card** (Credit Card Expenses)

---

## ?? Dicas de Uso

### **1. Salvar Respostas**

Após criar um cartão ou despesa, clique em **"Save Response"** para guardar o exemplo.

### **2. Usar Variáveis**

Você pode criar variáveis para IDs:

1. Após criar um cartão, clique nos **3 pontinhos** da response
2. Selecione **"Set as environment variable"**
3. Nomeie como `cardId`
4. Use `{{cardId}}` nas próximas requests

### **3. Executar Collection Inteira**

1. Clique nos **3 pontinhos** da collection
2. Selecione **"Run collection"**
3. Escolha as requests que deseja executar
4. Clique em **"Run CreditCard API"**

### **4. Testar Filtros**

Na pasta **Credit Card Expenses**, há 3 requests de exemplo com filtros:
- Por mês/ano
- Por categoria
- Combinado

---

## ?? Exemplos de Query Params

### **Filtrar por mês:**
```
GET /api/creditcard/1/expenses?month=3&year=2026
```

### **Filtrar por categoria:**
```
GET /api/creditcard/1/expenses?category=Alimentação
```

### **Combinar filtros:**
```
GET /api/creditcard/1/expenses?month=3&year=2026&category=Transporte
```

---

## ?? Categorias Automáticas

Ao criar despesas manualmente, o sistema categoriza automaticamente:

| Descrição | Categoria |
|-----------|-----------|
| "Mercado", "Supermercado" | Alimentação |
| "Uber", "Posto", "Gasolina" | Transporte |
| "Farmacia", "Pague Menos" | Saúde |
| "Pet Shop" | Pets |
| "Barbearia", "Cabelo" | Beleza |
| "Amazon", "Mercadolivre" | Compras Online |
| "Claro", "Vivo", "Tim" | Telefonia |
| "Cinema", "Hotel" | Lazer |
| Outros casos | Outros |

---

## ? Erros Comuns

### **Error 404 - Not Found**

```json
{
  "message": "Cartão não encontrado"
}
```

**Solução:** Verifique se o ID do cartão existe.

### **Error 400 - Bad Request**

```json
{
  "message": "Arquivo não fornecido ou vazio"
}
```

**Solução:** Certifique-se de selecionar um arquivo CSV válido.

### **Error 500 - Internal Server Error**

```json
{
  "message": "Erro ao importar CSV",
  "error": "..."
}
```

**Solução:** Verifique se o formato do CSV está correto (date, title, amount).

---

## ?? Testes Automatizados (Opcional)

Você pode adicionar **Tests** nas requests do Postman:

```javascript
// Exemplo: Verificar se criou com sucesso
pm.test("Status code is 201", function () {
    pm.response.to.have.status(201);
});

pm.test("Response has id", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('id');
});

pm.test("Card name is Nubank", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.name).to.eql("Nubank");
});
```

---

## ?? Notas Importantes

1. ? **Sempre execute o SQL script primeiro** antes de testar
2. ? **Anote os IDs** retornados para usar em outras requests
3. ? **Use filtros** para encontrar despesas específicas
4. ? **Importe CSV** para testar categorização automática
5. ? **Verifique análises** para entender gastos

---

## ?? Pronto para Usar!

A Collection está completa com:
- ? 21 requests prontas
- ? Exemplos pré-configurados
- ? Documentação inline
- ? Variáveis configuradas
- ? Corpo das requests preenchido

**Importe no Postman e comece a testar!** ??

---

**Desenvolvido com ?? para facilitar seus testes**
