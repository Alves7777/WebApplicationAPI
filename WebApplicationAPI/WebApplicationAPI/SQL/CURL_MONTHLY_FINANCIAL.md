# ?? CURL Commands - Monthly Financial Control API

Base URL: `http://localhost:5296/api/v1/monthly-financial`

---

## ?? Índice

1. [CREATE - Criar novo registro](#1-create---criar-novo-registro)
2. [GET ALL - Listar todos os registros](#2-get-all---listar-todos-os-registros)
3. [GET BY ID - Buscar por ID](#3-get-by-id---buscar-por-id)
4. [UPDATE - Atualizar registro](#4-update---atualizar-registro)
5. [DELETE - Deletar registro](#5-delete---deletar-registro)

---

## 1. CREATE - Criar novo registro

### ? Criar Janeiro/2025

```bash
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{
    "year": 2025,
    "month": 1,
    "money": 6000.00,
    "rv": 2000.00,
    "debit": 300.00,
    "others": 500.00,
    "reserve": 1500.00
  }'
```

### ? Criar Fevereiro/2025

```bash
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{
    "year": 2025,
    "month": 2,
    "money": 6500.00,
    "rv": 1800.00,
    "debit": 250.00,
    "others": 400.00,
    "reserve": 1200.00
  }'
```

### ? Criar Março/2025

```bash
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{
    "year": 2025,
    "month": 3,
    "money": 6200.00,
    "rv": 2200.00,
    "debit": 280.00,
    "others": 450.00,
    "reserve": 1300.00
  }'
```

### ? Criar Maio/2025

```bash
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{
    "year": 2025,
    "month": 5,
    "money": 5000.00,
    "rv": 1500.00,
    "debit": 200.00,
    "others": 300.00,
    "reserve": 1000.00
  }'
```

### ? Criar Dezembro/2025

```bash
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{
    "year": 2025,
    "month": 12,
    "money": 7000.00,
    "rv": 3000.00,
    "debit": 400.00,
    "others": 600.00,
    "reserve": 2000.00
  }'
```

### ? Teste de Erro - Duplicidade

```bash
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{
    "year": 2025,
    "month": 1,
    "money": 5000.00,
    "rv": 1000.00,
    "debit": 100.00,
    "others": 200.00,
    "reserve": 800.00
  }'
```

**Resposta esperada (400):**
```json
{
  "message": "Já existe um registro para 1/2025"
}
```

---

## 2. GET ALL - Listar todos os registros

### ? Listar todos

```bash
curl -X GET http://localhost:5296/api/v1/monthly-financial
```

**Formato simplificado (pretty print):**

```bash
curl -X GET http://localhost:5296/api/v1/monthly-financial | json_pp
```

**Com verbose (debug):**

```bash
curl -v -X GET http://localhost:5296/api/v1/monthly-financial
```

---

## 3. GET BY ID - Buscar por ID

### ? Buscar ID 1

```bash
curl -X GET http://localhost:5296/api/v1/monthly-financial/1
```

### ? Buscar ID 2

```bash
curl -X GET http://localhost:5296/api/v1/monthly-financial/2
```

### ? Buscar ID 5

```bash
curl -X GET http://localhost:5296/api/v1/monthly-financial/5
```

### ? Teste de Erro - ID inexistente

```bash
curl -X GET http://localhost:5296/api/v1/monthly-financial/999
```

**Resposta esperada (404):**
```json
{
  "message": "Registro não encontrado"
}
```

---

## 4. UPDATE - Atualizar registro

### ? Atualizar Janeiro/2025 (ID 1) - Aumentar salário

```bash
curl -X PUT http://localhost:5296/api/v1/monthly-financial/1 \
  -H "Content-Type: application/json" \
  -d '{
    "year": 2025,
    "month": 1,
    "money": 6500.00,
    "rv": 2000.00,
    "debit": 300.00,
    "others": 500.00,
    "reserve": 1500.00
  }'
```

### ? Atualizar Maio/2025 - Aumentar reserva

```bash
curl -X PUT http://localhost:5296/api/v1/monthly-financial/4 \
  -H "Content-Type: application/json" \
  -d '{
    "year": 2025,
    "month": 5,
    "money": 5000.00,
    "rv": 1500.00,
    "debit": 200.00,
    "others": 300.00,
    "reserve": 1500.00
  }'
```

### ? Atualizar Dezembro/2025 - Modificar RV

```bash
curl -X PUT http://localhost:5296/api/v1/monthly-financial/5 \
  -H "Content-Type: application/json" \
  -d '{
    "year": 2025,
    "month": 12,
    "money": 7000.00,
    "rv": 3500.00,
    "debit": 400.00,
    "others": 600.00,
    "reserve": 2000.00
  }'
```

### ? Teste de Erro - Atualizar para mês já existente

```bash
curl -X PUT http://localhost:5296/api/v1/monthly-financial/2 \
  -H "Content-Type: application/json" \
  -d '{
    "year": 2025,
    "month": 1,
    "money": 5000.00,
    "rv": 1500.00,
    "debit": 200.00,
    "others": 300.00,
    "reserve": 1000.00
  }'
```

**Resposta esperada (400):**
```json
{
  "message": "Já existe outro registro para 1/2025"
}
```

---

## 5. DELETE - Deletar registro

### ? Deletar Dezembro/2025 (ID 5)

```bash
curl -X DELETE http://localhost:5296/api/v1/monthly-financial/5
```

**Resposta esperada (204 No Content)**

### ? Deletar Maio/2025 (ID 4)

```bash
curl -X DELETE http://localhost:5296/api/v1/monthly-financial/4
```

### ? Teste de Erro - Deletar ID inexistente

```bash
curl -X DELETE http://localhost:5296/api/v1/monthly-financial/999
```

**Resposta esperada (404):**
```json
{
  "message": "Registro não encontrado"
}
```

---

## ?? Cenários de Teste Completos

### Cenário 1: Ciclo Completo (Create ? Read ? Update ? Delete)

```bash
# 1. Criar
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{
    "year": 2025,
    "month": 4,
    "money": 5500.00,
    "rv": 1700.00,
    "debit": 250.00,
    "others": 350.00,
    "reserve": 1100.00
  }'

# 2. Listar todos
curl -X GET http://localhost:5296/api/v1/monthly-financial

# 3. Buscar o criado (assumindo ID 6)
curl -X GET http://localhost:5296/api/v1/monthly-financial/6

# 4. Atualizar
curl -X PUT http://localhost:5296/api/v1/monthly-financial/6 \
  -H "Content-Type: application/json" \
  -d '{
    "year": 2025,
    "month": 4,
    "money": 6000.00,
    "rv": 1700.00,
    "debit": 250.00,
    "others": 350.00,
    "reserve": 1200.00
  }'

# 5. Deletar
curl -X DELETE http://localhost:5296/api/v1/monthly-financial/6
```

---

### Cenário 2: Criar Ano Completo (12 meses)

```bash
# Janeiro
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{"year": 2026, "month": 1, "money": 6000.00, "rv": 2000.00, "debit": 300.00, "others": 500.00, "reserve": 1500.00}'

# Fevereiro
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{"year": 2026, "month": 2, "money": 6000.00, "rv": 2000.00, "debit": 300.00, "others": 500.00, "reserve": 1500.00}'

# Março
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{"year": 2026, "month": 3, "money": 6000.00, "rv": 2000.00, "debit": 300.00, "others": 500.00, "reserve": 1500.00}'

# Abril
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{"year": 2026, "month": 4, "money": 6000.00, "rv": 2000.00, "debit": 300.00, "others": 500.00, "reserve": 1500.00}'

# Maio
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{"year": 2026, "month": 5, "money": 6000.00, "rv": 2000.00, "debit": 300.00, "others": 500.00, "reserve": 1500.00}'

# Junho
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{"year": 2026, "month": 6, "money": 6000.00, "rv": 2000.00, "debit": 300.00, "others": 500.00, "reserve": 1500.00}'

# Julho
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{"year": 2026, "month": 7, "money": 6000.00, "rv": 2000.00, "debit": 300.00, "others": 500.00, "reserve": 1500.00}'

# Agosto
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{"year": 2026, "month": 8, "money": 6000.00, "rv": 2000.00, "debit": 300.00, "others": 500.00, "reserve": 1500.00}'

# Setembro
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{"year": 2026, "month": 9, "money": 6000.00, "rv": 2000.00, "debit": 300.00, "others": 500.00, "reserve": 1500.00}'

# Outubro
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{"year": 2026, "month": 10, "money": 6000.00, "rv": 2000.00, "debit": 300.00, "others": 500.00, "reserve": 1500.00}'

# Novembro
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{"year": 2026, "month": 11, "money": 6000.00, "rv": 2000.00, "debit": 300.00, "others": 500.00, "reserve": 1500.00}'

# Dezembro
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{"year": 2026, "month": 12, "money": 6000.00, "rv": 2000.00, "debit": 300.00, "others": 500.00, "reserve": 1500.00}'
```

---

## ?? Verificar Cálculos Automáticos

### Exemplo com valores para verificação

```bash
curl -X POST http://localhost:5296/api/v1/monthly-financial \
  -H "Content-Type: application/json" \
  -d '{
    "year": 2025,
    "month": 6,
    "money": 5000.00,
    "rv": 1500.00,
    "debit": 200.00,
    "others": 300.00,
    "reserve": 1000.00
  }'
```

**Cálculos esperados na resposta:**
```
SalaryTotal = 5000 + 1500 + 200 + 300 = 7000.00
ExpensesTotal = (soma das despesas de Junho/2025 no banco)
Balance = 7000.00 - ExpensesTotal
CanSpend = Balance - 1000.00
```

---

## ?? Comandos Úteis

### Salvar resposta em arquivo

```bash
curl -X GET http://localhost:5296/api/v1/monthly-financial > response.json
```

### Ver apenas status HTTP

```bash
curl -s -o /dev/null -w "%{http_code}" http://localhost:5296/api/v1/monthly-financial/1
```

### Timing da requisição

```bash
curl -w "@-" -o /dev/null -s http://localhost:5296/api/v1/monthly-financial <<'EOF'
   time_namelookup:  %{time_namelookup}\n
      time_connect:  %{time_connect}\n
   time_appconnect:  %{time_appconnect}\n
  time_pretransfer:  %{time_pretransfer}\n
     time_redirect:  %{time_redirect}\n
time_starttransfer:  %{time_starttransfer}\n
                   ----------\n
        time_total:  %{time_total}\n
EOF
```

### Headers incluídos

```bash
curl -i -X GET http://localhost:5296/api/v1/monthly-financial/1
```

---

## ?? PowerShell (Windows)

Se estiver usando PowerShell no Windows, use `Invoke-RestMethod`:

### GET
```powershell
Invoke-RestMethod -Uri "http://localhost:5296/api/v1/monthly-financial" -Method Get
```

### POST
```powershell
$body = @{
    year = 2025
    month = 1
    money = 6000.00
    rv = 2000.00
    debit = 300.00
    others = 500.00
    reserve = 1500.00
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5296/api/v1/monthly-financial" -Method Post -Body $body -ContentType "application/json"
```

### PUT
```powershell
$body = @{
    year = 2025
    month = 1
    money = 6500.00
    rv = 2000.00
    debit = 300.00
    others = 500.00
    reserve = 1500.00
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5296/api/v1/monthly-financial/1" -Method Put -Body $body -ContentType "application/json"
```

### DELETE
```powershell
Invoke-RestMethod -Uri "http://localhost:5296/api/v1/monthly-financial/1" -Method Delete
```

---

## ?? Notas

- Substitua `localhost:5296` pela URL correta se estiver em outro ambiente
- Os IDs podem variar dependendo da ordem de criação
- Todos os valores decimais devem ter duas casas decimais
- Year válido: 2020-2100
- Month válido: 1-12

---

**Pronto para testar! ??**
