# ?? API Response Pattern - Padrão de Respostas

Todos os endpoints da API agora seguem um padrão unificado de resposta.

---

## ? **SUCCESS (200, 201)**

### **Estrutura:**
```json
{
  "status": "success",
  "message": "Mensagem opcional",
  "data": { ... }
}
```

### **Exemplos:**

**GET - Listar todos:**
```json
{
  "status": "success",
  "message": null,
  "data": [
    {
      "id": 1,
      "name": "Nubank",
      "cardLimit": 5000.00
    }
  ]
}
```

**POST - Criar:**
```json
{
  "status": "success",
  "message": "Cartão criado com sucesso",
  "data": {
    "id": 1,
    "name": "Nubank",
    "cardLimit": 5000.00
  }
}
```

**DELETE - Deletar:**
```json
{
  "status": "success",
  "message": "Cartão deletado com sucesso",
  "data": null
}
```

---

## ? **FAIL (400, 404)**

### **Estrutura:**
```json
{
  "status": "fail",
  "message": "Mensagem de erro",
  "data": null
}
```

### **Exemplos:**

**404 - Not Found:**
```json
{
  "status": "fail",
  "message": "Cartão não encontrado",
  "data": null
}
```

**400 - Bad Request:**
```json
{
  "status": "fail",
  "message": "Já existe um registro para 5/2025",
  "data": null
}
```

**400 - Validation Error:**
```json
{
  "status": "fail",
  "message": "Arquivo não fornecido ou vazio",
  "data": null
}
```

---

## ?? **ERROR (500)**

### **Estrutura:**
```json
{
  "status": "error",
  "message": "Mensagem de erro interno",
  "data": null
}
```

### **Exemplo:**

```json
{
  "status": "error",
  "message": "Erro interno: Cannot connect to database",
  "data": null
}
```

---

## ?? **Mapeamento de Status HTTP:**

| Status HTTP | Status Response | Quando usar |
|-------------|-----------------|-------------|
| 200 OK | `success` | GET, PUT com sucesso |
| 201 Created | `success` | POST com sucesso |
| 204 No Content | N/A | Não usado mais (agora 200) |
| 400 Bad Request | `fail` | Validação, regra de negócio |
| 401 Unauthorized | `fail` | Não autenticado |
| 403 Forbidden | `fail` | Não autorizado |
| 404 Not Found | `fail` | Recurso não encontrado |
| 500 Internal Server Error | `error` | Erro do servidor |

---

## ?? **Exemplos por Endpoint:**

### **POST /api/creditcard**

**Success (201):**
```json
{
  "status": "success",
  "message": "Cartão criado com sucesso",
  "data": {
    "id": 1,
    "name": "Nubank",
    "brand": "Mastercard",
    "cardLimit": 5000.00,
    "closingDay": 10,
    "dueDay": 17,
    "isActive": true
  }
}
```

**Fail (400):**
```json
{
  "status": "fail",
  "message": "Nome do cartão é obrigatório",
  "data": null
}
```

---

### **POST /api/creditcard/1/import-csv**

**Success (200):**
```json
{
  "status": "success",
  "message": "56 despesas importadas com sucesso",
  "data": {
    "totalRecords": 58,
    "importedRecords": 56,
    "errorCount": 2,
    "errors": ["Data inválida: ..."],
    "categoriesCount": {
      "Alimentação": 15,
      "Transporte": 12
    }
  }
}
```

**Fail (400):**
```json
{
  "status": "fail",
  "message": "Cartão não encontrado",
  "data": null
}
```

**Fail (400) - Arquivo inválido:**
```json
{
  "status": "fail",
  "message": "Apenas arquivos CSV são permitidos",
  "data": null
}
```

---

### **GET /api/creditcard/1/statement?month=3&year=2026**

**Success (200):**
```json
{
  "status": "success",
  "message": null,
  "data": {
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
}
```

---

### **DELETE /api/creditcard/1**

**Success (200):**
```json
{
  "status": "success",
  "message": "Cartão deletado com sucesso",
  "data": null
}
```

**Fail (404):**
```json
{
  "status": "fail",
  "message": "Cartão não encontrado",
  "data": null
}
```

---

## ?? **Controllers Atualizados:**

? **CreditCardController** - 100% atualizado  
? **ExpenseController** - 100% atualizado  
? **MonthlyFinancialController** - 100% atualizado  
? **CategoryController** - 100% atualizado  

---

## ?? **Vantagens do Padrão:**

? **Consistência** - Todas as respostas seguem o mesmo formato  
? **Previsibilidade** - Frontend sabe sempre o que esperar  
? **Facilita tratamento de erros** - `if (response.status === "success")`  
? **Mensagens claras** - Sempre tem um `message` explicativo  
? **Type-safe** - TypeScript pode tipar facilmente  

---

## ?? **Como usar no Frontend:**

### **JavaScript/TypeScript:**

```typescript
const response = await fetch('/api/creditcard');
const json = await response.json();

if (json.status === 'success') {
  console.log('Dados:', json.data);
} else if (json.status === 'fail') {
  alert('Erro: ' + json.message);
} else {
  console.error('Erro do servidor:', json.message);
}
```

### **React:**

```tsx
const handleImportCsv = async (file: File) => {
  const formData = new FormData();
  formData.append('file', file);

  const response = await fetch('/api/creditcard/1/import-csv', {
    method: 'POST',
    body: formData
  });

  const json = await response.json();

  if (json.status === 'success') {
    toast.success(json.message);
    console.log('Importados:', json.data.importedRecords);
  } else {
    toast.error(json.message);
  }
};
```

---

**Padrão implementado com sucesso! ??**
