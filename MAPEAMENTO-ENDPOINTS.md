# Mapeamento de Endpoints x Páginas

## ? Endpoints Implementados

### 1. Category (Categorias)
- ? `GET /api/Category` - Página: **Categories.razor**
- ? `POST /api/Category` - Página: **Categories.razor** (formulário de criação)
- ? `GET /api/Category/{id}` - Usado internamente
- ?? Endpoints não implementados na UI:
  - Não há edição/exclusão na página atual

### 2. CreditCard (Cartões de Crédito)
- ? `GET /api/creditcard` - Página: **CreditCards.razor**
- ? `POST /api/creditcard` - Página: **CreditCards.razor** (formulário de criação)
- ? `GET /api/creditcard/{id}` - Usado internamente
- ? `PUT /api/creditcard/{id}` - Página: **CreditCards.razor** (edição)
- ? `DELETE /api/creditcard/{id}` - Página: **CreditCards.razor** (botão deletar)
- ? `POST /api/creditcard/{id}/simulate-purchase` - Página: **Simulator.razor**
- ? `POST /api/creditcard/{id}/confirm-purchase` - Página: **Simulator.razor**
- ?? Endpoints não implementados na UI:
  - `POST /api/creditcard/{id}/expenses` - Criar despesa do cartão
  - `GET /api/creditcard/{id}/expenses` - Listar despesas do cartão
  - `GET /api/creditcard/expenses/{expenseId}` - Ver despesa específica
  - `PUT /api/creditcard/expenses/{expenseId}` - Editar despesa
  - `DELETE /api/creditcard/expenses/{expenseId}` - Deletar despesa
  - `POST /api/creditcard/{id}/import-csv` - Importar CSV
  - `GET /api/creditcard/{id}/statement` - Ver fatura
  - `GET /api/creditcard/{id}/by-category` - Análise por categoria
  - `GET /api/creditcard/{id}/statement-period` - Fatura por período
  - `GET /api/creditcard/{id}/installment-purchases` - Ver compras parceladas

### 3. Expense (Despesas)
- ? `GET /api/Expense` - Página: **Expenses.razor**
- ? `POST /api/Expense` - Página: **Expenses.razor** (formulário de criação)
- ? `PUT /api/Expense/{id}` - Página: **Expenses.razor** (edição)
- ? `PATCH /api/Expense/{id}` - Usado para atualizações parciais
- ? `DELETE /api/Expense/{id}` - Página: **Expenses.razor** (botão deletar)
- ?? Endpoints não implementados na UI:
  - `GET /api/Expense/report` - Relatório de despesas

### 4. MonthlyFinancial (Controle Mensal)
- ? `GET /api/v1/monthly-financial` - Página: **MonthlyFinancialPage.razor**
- ? `POST /api/v1/monthly-financial` - Página: **MonthlyFinancialPage.razor** (formulário de criação)
- ? `GET /api/v1/monthly-financial/{id}` - Usado internamente
- ? `PUT /api/v1/monthly-financial/{id}` - Página: **MonthlyFinancialPage.razor** (edição)
- ? `DELETE /api/v1/monthly-financial/{id}` - Página: **MonthlyFinancialPage.razor** (botão deletar)

### 5. Summary (Resumo/Dashboard)
- ? `GET /api/Summary` - Página: **Dashboard.razor**

### 6. User (Usuários)
- ? `GET /api/User` - Página: **Users.razor**
- ? `POST /api/User` - Página: **Users.razor** (formulário de criação)
- ? `GET /api/User/{id}` - Usado internamente
- ? `PUT /api/User/{id}` - Página: **Users.razor** (edição)
- ? `DELETE /api/User/{id}` - Página: **Users.razor** (botão deletar)

## ?? Estatísticas
- **Total de endpoints da API**: ~35
- **Endpoints com interface implementada**: ~20
- **Cobertura**: ~57%

## ?? Sugestões de Implementação Futura

### Alta Prioridade
1. **Página de Fatura do Cartão** - Para visualizar extratos mensais
2. **Página de Despesas do Cartão** - Gerenciar despesas específicas de cada cartão
3. **Página de Relatórios** - Exibir relatórios detalhados de despesas

### Média Prioridade
1. **Importação CSV** - Adicionar botão na página de cartões
2. **Análise por Categoria** - Widget no dashboard
3. **Compras Parceladas** - Visualização das parcelas

## ?? Mudanças Recentes
- ? Atualizado IP da API de `192.168.1.114` para `192.168.0.178`
- ? Adicionado componente `BackButton` em todas as páginas
- ? Corrigido link de "Cartões" no menu de navegação
