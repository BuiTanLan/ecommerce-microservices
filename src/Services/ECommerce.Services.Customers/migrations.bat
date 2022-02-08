
IF "%1"=="init-context" dotnet ef migrations add InitialCustomersMigration -o src\ECommerce.Services.Customer\Shared\Data\Migrations\Customer --project .\src\ECommerce.Services.Customers\ECommerce.Services.Customers.csproj -c CustomersDbContext --verbose & goto exit
IF "%1"=="update-context" dotnet ef database update -c CustomersDbContext --verbose --project .\src\ECommerce.Services.Customers\ECommerce.Services.Customers.csproj & goto exit 

IF "%1"=="init-outbox" dotnet ef migrations add InitialOutboxMigration -o src\ECommerce.Services.Customer\Shared\Data\Migrations\Outbox --project .\src\ECommerce.Services.Customers\ECommerce.Services.Customers.csproj -c OutboxDataContext & goto exit
IF "%1"=="update-outbox" dotnet ef database update -c OutboxDataContext --project .\src\ECommerce.Services.Customers\ECommerce.Services.Customers.csproj --verbose & goto exit 

:exit