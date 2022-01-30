#### Migration Scripts

```bash
dotnet ef migrations add InitialIdentityServerMigration -o Shared\Data\Migrations\Identity -c IdentityContext
dotnet ef database update -c IdentityContext

dotnet ef migrations add InitialOutboxMigration -o Shared\Data\Migrations\Outbox -c OutboxDataContext
dotnet ef database update -c OutboxDataContext
```
