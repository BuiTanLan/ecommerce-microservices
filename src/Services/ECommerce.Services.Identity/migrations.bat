
IF "%1"=="init-context" dotnet ef migrations add InitialIdentityServerMigration -o src\ECommerce.Services.Identity\Shared\Data\Migrations\Identity --project .\src\ECommerce.Services.Identity\ECommerce.Services.Identity.csproj -c IdentityContext --verbose & goto exit
IF "%1"=="update-context" dotnet ef database update -c IdentityContext --verbose --project .\src\ECommerce.Services.Identity\ECommerce.Services.Identity.csproj & goto exit 

IF "%1"=="init-outbox" dotnet ef migrations add InitialOutboxMigration -o src\ECommerce.Services.Identity\Shared\Data\Migrations\Outbox --project .\src\ECommerce.Services.Identity\ECommerce.Services.Identity.csproj -c OutboxDataContext & goto exit
IF "%1"=="update-outbox" dotnet ef database update -c OutboxDataContext --project .\src\ECommerce.Services.Identity\ECommerce.Services.Identity.csproj --verbose & goto exit 

:exit