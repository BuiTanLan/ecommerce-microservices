#### Migration Scripts

```bash
dotnet ef migrations add InitialIdentityServerMigration -o Shared\Data\Migrations\Identity -c IdentityContext
dotnet ef database update -c IdentityContext

dotnet ef migrations add InitialOutboxMigration -o Shared\Data\Migrations\Outbox -c OutboxDataContext
dotnet ef database update -c OutboxDataContext

dotnet ef migrations add InitialInternalMessagesMigration -o Shared\Data\Migrations\InternalMessages -c InternalMessageDbContext
dotnet ef database update -c InternalMessageDbContext
```
