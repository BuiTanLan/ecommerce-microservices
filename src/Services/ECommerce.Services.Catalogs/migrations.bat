
IF "%1"=="init-context" dotnet ef migrations add InitialCatalogMigration -o src\ECommerce.Services.Catalogs\Shared\Data\Migrations\Catalogs --project .\src\ECommerce.Services.Catalogs\ECommerce.Services.Catalogs.csproj -c CatalogDbContext --verbose & goto exit
IF "%1"=="update-context" dotnet ef database update -c CatalogDbContext --verbose --project .\src\ECommerce.Services.Catalogs\ECommerce.Services.Catalogs.csproj & goto exit 

IF "%1"=="init-outbox" dotnet ef migrations add InitialOutboxMigration -o src\ECommerce.Services.Catalogs\Shared\Data\Migrations\Outbox --project .\src\ECommerce.Services.Catalogs\ECommerce.Services.Catalogs.csproj -c OutboxDataContext & goto exit
IF "%1"=="update-outbox" dotnet ef database update -c OutboxDataContext --project .\src\ECommerce.Services.Catalogs\ECommerce.Services.Catalogs.csproj --verbose & goto exit 

:exit