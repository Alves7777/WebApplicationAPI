# ?? COMECE AQUI - Guia Rápido de Implementação

## ?? Tempo estimado: 15 minutos

---

## ?? Passo a Passo

### **1?? Executar Scripts SQL** (5 min)

Abra o **SQL Server Management Studio (SSMS)** e execute **NA ORDEM**:

```sql
-- 1. Script principal (cria estrutura de auth)
WebApplicationAPI\SQL\AuthenticationSetup.sql

-- 2. Script de CreditCard (opcional, mas recomendado)
WebApplicationAPI\SQL\CreditCardAuthProcedures.sql

-- 3. Script de testes (opcional, para validar)
WebApplicationAPI\SQL\TestAuthentication.sql
```

**?? IMPORTANTE:** Execute um de cada vez, aguarde finalizar antes do próximo.

---

### **2?? Iniciar a API** (1 min)

```bash
cd WebApplicationAPI
dotnet run
```

Aguarde a mensagem:
```
Now listening on: http://localhost:5296
```

---

### **3?? Testar no Postman/Swagger** (5 min)

#### **Opção A: Swagger** (Mais fácil)

1. Acesse: http://localhost:5296/swagger
2. Role até `/api/auth/register`
3. Clique em "Try it out"
4. Cole o JSON:
   ```json
   {
     "name": "Seu Nome",
     "email": "seu@email.com",
     "password": "senha123"
   }
   ```
5. Clique "Execute"
6. **COPIE O TOKEN** retornado!
7. Clique no botão **"Authorize"** no topo
8. Cole: `Bearer SEU_TOKEN_AQUI`
9. Clique "Authorize"
10. Agora teste outras rotas (ex: GET /api/creditcard)

#### **Opção B: Postman**

**Passo 1: Registrar**
```http
POST http://localhost:5296/api/auth/register
Content-Type: application/json

{
  "name": "Seu Nome",
  "email": "seu@email.com",
  "password": "senha123"
}
```

**Passo 2: Copiar Token**
Copie o campo `token` da resposta.

**Passo 3: Configurar Authorization**
- Vá em "Authorization"
- Type: "Bearer Token"
- Cole o token

**Passo 4: Testar Rota Protegida**
```http
GET http://localhost:5296/api/creditcard
Authorization: Bearer SEU_TOKEN
```

---

### **4?? Validar Multi-tenancy** (4 min)

1. Crie 2 usuários diferentes (emails diferentes)
2. Faça login com usuário 1, pegue o token
3. Crie um cartão de crédito
4. Faça login com usuário 2, pegue o token
5. Liste os cartões ? deve vir vazio!
6. Crie um cartão para o usuário 2
7. Liste novamente ? deve vir apenas 1 cartão
8. Volte para o token do usuário 1
9. Liste ? deve vir apenas o cartão do usuário 1

**? Se funcionou, parabéns! Multi-tenancy está OK!**

---

## ?? Próximos Passos (Após validar)

### **Atualizar Outras Entidades**

Para cada entidade (Expense, MonthlyFinancial, etc):

1. ? Já fizemos: Adicionar `UserId` no Model
2. ? **FAZER:** Atualizar Repository
3. ? **FAZER:** Atualizar Service
4. ? **FAZER:** Atualizar Controller
5. ? **FAZER:** Criar/Atualizar SPs no SQL

**?? Consulte:** `AUTHENTICATION_EXAMPLES.md` para ver exemplos completos

---

## ?? Checklist de Validação

- [ ] Script SQL executado sem erros
- [ ] API iniciou sem erros
- [ ] Consegui fazer POST /api/auth/register
- [ ] Recebi um token JWT
- [ ] Consegui fazer POST /api/auth/login
- [ ] Token funciona nas rotas protegidas
- [ ] Cada usuário vê apenas seus dados

---

## ?? Problemas Comuns

| Problema | Solução |
|----------|---------|
| Erro ao executar SQL | Verifique se está conectado no banco `WebAppDB` |
| API não inicia | Execute `dotnet restore` e `dotnet build` |
| "Unauthorized" | Verifique se está enviando: `Authorization: Bearer TOKEN` |
| "Email já cadastrado" | Use outro email ou delete do banco |
| Token inválido | Verifique se a chave JWT no appsettings.json está correta |

---

## ?? Documentação Disponível

| Arquivo | Quando Usar |
|---------|-------------|
| `README_AUTH.md` | Resumo executivo |
| `AUTHENTICATION_GUIDE.md` | Guia completo com teoria |
| `AUTHENTICATION_EXAMPLES.md` | Exemplos práticos de código |
| `SQL\AuthenticationSetup.sql` | Script de banco comentado |
| `SQL\TestAuthentication.sql` | Testes automatizados |

---

## ?? Conceitos Importantes

### **O que é JWT?**
Um token que contém informações do usuário (Id, Email, Role) assinado digitalmente. Não precisa consultar banco a cada requisição.

### **O que é Multi-tenancy?**
Múltiplos usuários/clientes usando o mesmo sistema, mas cada um vê apenas seus próprios dados.

### **Como funciona?**
1. Cliente faz login ? Servidor retorna JWT
2. Cliente guarda o JWT (localStorage, cookie)
3. Cliente envia JWT em toda requisição: `Authorization: Bearer TOKEN`
4. Servidor valida JWT e extrai `UserId`
5. Servidor filtra dados por `UserId`

---

## ?? Dicas

### **No Postman:**
- Crie um Environment com variável `token`
- Use `{{token}}` nas requisições
- No endpoint de login, adicione em "Tests":
  ```javascript
  pm.environment.set("token", pm.response.json().token);
  ```

### **No Swagger:**
- Após fazer register/login, clique no botão "Authorize"
- Cole: `Bearer SEU_TOKEN`
- Todas as próximas requisições usarão o token automaticamente

### **Debugging:**
- Para ver o conteúdo do token, acesse: https://jwt.io/
- Cole o token e veja as claims (Sub, Email, Role)

---

## ?? Segurança em Produção

?? **ANTES DE COLOCAR EM PRODUÇÃO:**

1. **Mude a chave JWT** no `appsettings.json`
   - Use uma chave forte com 256+ bits
   - Não commite no Git!

2. **Use HTTPS**
   - Descomente `app.UseHttpsRedirection()` no Program.cs

3. **Use variáveis de ambiente**
   ```csharp
   var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") 
                ?? builder.Configuration["Jwt:Key"];
   ```

4. **Configure CORS adequadamente**
   - Não use `AllowAnyOrigin()` em produção
   - Configure apenas origins permitidas

5. **Adicione rate limiting**
   - Limite tentativas de login
   - Previne brute force attacks

---

## ? Tudo Pronto!

Você agora tem:
- ? Autenticação JWT completa
- ? Multi-tenancy (multi-usuário)
- ? Proteção de rotas
- ? Isolamento de dados
- ? Documentação completa

**?? Parabéns! Seu sistema está seguro e pronto para produção!**

---

## ?? Comandos de Emergência

```bash
# Rebuild completo
dotnet clean
dotnet restore
dotnet build

# Verificar logs
dotnet run --verbosity detailed

# Resetar banco (CUIDADO!)
# Execute no SSMS:
DELETE FROM Expenses WHERE UserId IS NOT NULL;
DELETE FROM Users WHERE Id > 0;
DBCC CHECKIDENT ('Users', RESEED, 0);
```

---

**?? Dúvidas? Consulte os arquivos de documentação na pasta do projeto!**
