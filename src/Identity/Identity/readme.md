#### Migration Scripts

```bash
dotnet ef migrations add InitialIdentityServerMigration -o Infrastructure\Data\Migrations
dotnet ef database update
```