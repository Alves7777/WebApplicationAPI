# ?? Proteção contra Duplicidade - CSV Import

Sistema de verificação de duplicidade ao importar CSV do Nubank.

---

## ? **PROBLEMA RESOLVIDO**

Agora ao importar o mesmo CSV múltiplas vezes, o sistema:
- ? **NÃO duplica** despesas já existentes
- ? **Informa quantos** foram duplicados
- ? **Lista os duplicados** ignorados

---

## ?? **Como Funciona**

### **Critério de Duplicidade:**

Uma despesa é considerada **duplicada** se já existir com:
1. ? Mesmo **CartãoId**
2. ? Mesma **Data** (PurchaseDate)
3. ? Mesma **Descrição** (Description)
4. ? Mesmo **Valor** (Amount)

**Exemplo:**
```
2026-03-27, Mercado901ltda, 9.69 ? JÁ EXISTE
2026-03-27, Mercado901ltda, 9.69 ? DUPLICADO (ignorado)
2026-03-27, Mercado901ltda, 10.00 ? NOVO (valor diferente)
```

---

## ?? **Exemplo de Resposta**

### **Primeira Importação:**

```json
{
  "status": "success",
  "message": "56 despesas importadas com sucesso",
  "data": {
    "totalRecords": 58,
    "importedRecords": 56,
    "duplicateCount": 0,
    "errorCount": 2,
    "errors": [
      "Data inválida: invalid-date"
    ],
    "categoriesCount": {
      "Alimentação": 15,
      "Transporte": 12,
      "Saúde": 8
    }
  }
}
```

### **Segunda Importação (mesmo CSV):**

```json
{
  "status": "success",
  "message": "0 despesas importadas com sucesso",
  "data": {
    "totalRecords": 58,
    "importedRecords": 0,
    "duplicateCount": 56,
    "errorCount": 2,
    "errors": [
      "Duplicado (ignorado): Mercado901ltda - R$ 9,69",
      "Duplicado (ignorado): Sobral e Palacio - Fin - R$ 14,60",
      "Duplicado (ignorado): Mc Donalds Tzm - R$ 6,00",
      ... (56 itens)
    ],
    "categoriesCount": {}
  }
}
```

### **Importação Parcial (CSV atualizado):**

Se você baixar o CSV no meio do mês (10 despesas) e depois no final (60 despesas):

**Primeira importação (dia 15):**
```json
{
  "totalRecords": 10,
  "importedRecords": 10,
  "duplicateCount": 0
}
```

**Segunda importação (dia 31):**
```json
{
  "totalRecords": 60,
  "importedRecords": 50,
  "duplicateCount": 10,
  "errorCount": 0
}
```

? **Apenas as 50 novas despesas foram importadas!**

---

## ?? **Cenários de Uso**

### **Cenário 1: Importar 2 vezes o mesmo CSV**

```bash
# Primeira vez
POST /api/creditcard/1/import-csv ? 56 importados

# Segunda vez (mesmo arquivo)
POST /api/creditcard/1/import-csv ? 0 importados, 56 duplicados
```

? **Resultado:** Sem duplicatas

---

### **Cenário 2: CSV atualizado no meio do mês**

```bash
# Dia 15 - Baixar CSV (30 despesas)
POST /api/creditcard/1/import-csv ? 30 importados

# Dia 31 - Baixar CSV novamente (60 despesas)
POST /api/creditcard/1/import-csv ? 30 importados, 30 duplicados
```

? **Resultado:** Apenas as 30 novas foram importadas

---

### **Cenário 3: Múltiplos cartões**

```bash
# Importar Nubank
POST /api/creditcard/1/import-csv ? Nubank_2026-03.csv

# Importar Credicard  
POST /api/creditcard/2/import-csv ? Credicard_2026-03.csv
```

? **Resultado:** Sem conflito (cartões diferentes)

---

## ?? **Proteções Implementadas**

### **1. Verificação de Duplicidade**
```sql
SELECT COUNT(1) FROM CreditCardExpenses
WHERE CreditCardId = @CreditCardId
  AND PurchaseDate = @PurchaseDate
  AND Description = @Description
  AND Amount = @Amount
```

### **2. Valores Negativos Ignorados**
```
Pagamento recebido, -2909.15 ? Ignorado (não é despesa)
```

### **3. Datas Inválidas**
```
"invalid-date" ? Erro reportado
```

### **4. Valores Inválidos**
```
"ABC" ? Erro reportado
```

---

## ?? **Vantagens**

? **Seguro** - Pode importar o mesmo CSV quantas vezes quiser  
? **Inteligente** - Só importa o que é novo  
? **Transparente** - Informa exatamente o que aconteceu  
? **Rastreável** - Lista todos os duplicados encontrados  
? **Performance** - Verifica antes de inserir (evita rollback)  

---

## ?? **Exemplo Real**

### **Situação:**
- Dia 15/03: Baixei CSV e importei (30 despesas)
- Dia 31/03: Baixei CSV novamente (60 despesas)

### **O que acontece:**

```
Total no CSV: 60
??? 30 já existem (duplicados) ? Ignorados
??? 30 são novas ? Importadas
```

### **Resposta:**
```json
{
  "totalRecords": 60,
  "importedRecords": 30,
  "duplicateCount": 30,
  "errorCount": 0,
  "errors": [],
  "categoriesCount": {
    "Alimentação": 8,
    "Transporte": 6,
    "Saúde": 5,
    "Outros": 11
  }
}
```

---

## ?? **Código Implementado**

### **Repository:**
```csharp
public async Task<bool> ExpenseExistsAsync(
    int creditCardId, 
    DateTime purchaseDate, 
    string description, 
    decimal amount)
{
    // Verifica se já existe no banco
}
```

### **Service:**
```csharp
var exists = await _repository.ExpenseExistsAsync(
    creditCardId,
    purchaseDate,
    record.Title,
    amount
);

if (exists)
{
    result.DuplicateCount++;
    errors.Add($"Duplicado (ignorado): {record.Title}");
    continue; // Pula para próxima linha
}
```

---

## ?? **Como Testar**

### **1. Importar CSV primeira vez:**
```
POST /api/creditcard/1/import-csv
File: Nubank_2026-03.csv

Resultado: 56 importados, 0 duplicados
```

### **2. Importar mesmo CSV novamente:**
```
POST /api/creditcard/1/import-csv
File: Nubank_2026-03.csv (mesmo arquivo)

Resultado: 0 importados, 56 duplicados
```

### **3. Verificar banco:**
```sql
SELECT COUNT(*) FROM CreditCardExpenses
WHERE CreditCardId = 1 AND Month = 3 AND Year = 2026

-- Resultado: 56 (não duplicou!)
```

---

## ? **Implementado com Sucesso!**

Agora você pode:
- ? Importar o CSV quantas vezes quiser
- ? Não vai duplicar despesas
- ? Recebe feedback claro de duplicados
- ? Mantém dados íntegros

**Reinicia a API e testa! ??**
