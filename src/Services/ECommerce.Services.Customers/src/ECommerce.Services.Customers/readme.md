#### Migration Scripts

```bash
dotnet ef migrations add InitialCustomersMigration -o Shared\Data\Migrations\Customer -c CustomersDbContext
dotnet ef database update -c CustomersDbContext

dotnet ef migrations add InitialOutboxMigration -o Shared\Data\Migrations\Outbox -c OutboxDataContext
dotnet ef database update -c OutboxDataContext

dotnet ef migrations add InitialInternalMessagesMigration -o Shared\Data\Migrations\InternalMessages -c InternalMessageDbContext
dotnet ef database update -c InternalMessageDbContext
```
