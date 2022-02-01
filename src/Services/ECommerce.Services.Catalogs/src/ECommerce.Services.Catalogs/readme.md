#### Migration Scripts

```bash
dotnet ef migrations add InitialCatalogMigration -o Shared\Data\Migrations\Catalogs -c CatalogDbContext
dotnet ef database update -c CatalogDbContext

dotnet ef migrations add InitialOutboxMigration -o Shared\Data\Migrations\Outbox -c OutboxDataContext
dotnet ef database update -c OutboxDataContext
```
