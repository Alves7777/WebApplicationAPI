# Scripts de Banco de Dados

## Ordem de Execução

Execute os scripts na seguinte ordem para configurar o banco de dados:

### 1. Criar Tabelas
Execute na ordem:
1. `DB\Migration\V0_create_expenses_table.sql` - Cria a tabela Expenses
2. `DB\Migration\V1_create_users.sql` - Cria a tabela Users

### 2. Criar Stored Procedures
Execute na ordem:
1. `SQL\ExpenseStoredProcedures.sql` - Cria as stored procedures para Expenses
2. `SQL\StoredProcedures.sql` - Cria as stored procedures para Users

## Como Executar

### Usando SQL Server Management Studio (SSMS)
1. Conecte-se ao seu servidor SQL Server
2. Abra cada arquivo SQL
3. Execute o script (F5)

### Usando Azure Data Studio
1. Conecte-se ao seu servidor SQL Server
2. Abra cada arquivo SQL
3. Execute o script (F5 ou clique em "Run")

### Usando linha de comando (sqlcmd)
```bash
sqlcmd -S localhost -d WebApplicationAPI -i "DB\Migration\V0_create_expenses_table.sql"
sqlcmd -S localhost -d WebApplicationAPI -i "DB\Migration\V1_create_users.sql"
sqlcmd -S localhost -d WebApplicationAPI -i "SQL\ExpenseStoredProcedures.sql"
sqlcmd -S localhost -d WebApplicationAPI -i "SQL\StoredProcedures.sql"
```

## Verificar se as tabelas foram criadas
```sql
-- Listar todas as tabelas
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Listar todas as stored procedures
SELECT name 
FROM sys.procedures 
ORDER BY name;
```

## String de Conexão
Certifique-se de que o arquivo `appsettings.json` contém a string de conexão correta:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WebApplicationAPI;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```
