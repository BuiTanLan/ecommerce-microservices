#### Migration Scripts

```bash
dotnet ef migrations add InitialCatalogMigration -o Infrastructure\Data\Migrations\Catalog -c CatalogDbContext
dotnet ef database update -c CatalogDbContext

dotnet ef migrations add InitialOutboxMigration -o Infrastructure\Data\Migrations\Outbox -c OutboxDataContext
dotnet ef database update -c OutboxDataContext
```
