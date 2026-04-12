# ?? Financial Control UI - Blazor WebAssembly

Frontend mobile-first desenvolvido em Blazor WebAssembly (.NET 8).

---

## ?? **COMO RODAR:**

### **Terminal 1 - Backend (API):**
```powershell
cd "C:\Users\Lucas Alves\Documents\Lucas\.net\WebApplicationAPI"
dotnet run
```

### **Terminal 2 - Frontend (Blazor):**
```powershell
cd "C:\Users\Lucas Alves\Documents\Lucas\.net\FinancialControlUI"
dotnet run
```

### **Acessar:**
```
Desktop: https://localhost:5001
```

---

## ?? **ACESSAR NO CELULAR:**

### **1. Descubra seu IP:**
```powershell
ipconfig
# IPv4 Address: 192.168.1.10
```

### **2. Configure HTTPS no projeto:**

Edite `Properties/launchSettings.json`:
```json
{
  "applicationUrl": "https://0.0.0.0:5001;http://0.0.0.0:5000"
}
```

### **3. No celular:**
```
https://192.168.1.10:5001
```

**OU use HTTP (mais fácil):**
```
http://192.168.1.10:5000
```

---

## ?? **FUNCIONALIDADES:**

### **1. Home (Dashboard)**
- ? Resumo financeiro mensal
- ? Lista de cartões
- ? Navegação rápida

### **2. Simulador de Compra** ?
- ? Selecionar cartão
- ? Definir valor e parcelas
- ? Análise mês a mês
- ? Recomendação visual
- ? Confirmar compra

---

## ??? **TECNOLOGIAS:**

- ? .NET 8
- ? Blazor WebAssembly
- ? Bootstrap 5
- ? HttpClient
- ? PWA Ready

---

## ?? **MOBILE-FIRST:**

- ? Responsivo (100% mobile)
- ? Touch-friendly
- ? Botões grandes
- ? Layout adaptativo
- ? Funciona offline (após primeiro carregamento)

---

## ?? **INTEGRAÇÃO:**

**API Base:** `http://localhost:5296/api/`

**Endpoints:**
- GET `/creditcard` - Lista cartões
- POST `/creditcard/{id}/simulate-purchase` - Simula
- POST `/creditcard/{id}/confirm-purchase` - Confirma
- GET `/v1/monthly-financial` - Controle mensal

---

## ? **PRONTO PARA USAR!**

Execute os 2 terminais e acesse!
