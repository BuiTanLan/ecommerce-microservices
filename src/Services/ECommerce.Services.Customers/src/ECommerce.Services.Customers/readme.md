#### Migration Scripts

```bash
dotnet ef migrations add InitialCustomersMigration -o Shared\Data\Migrations\Customer -c CustomersDbContext
dotnet ef database update -c CustomersDbContext

dotnet ef migrations add InitialOutboxMigration -o Shared\Data\Migrations\Outbox -c OutboxDataContext
dotnet ef database update -c OutboxDataContext
```
