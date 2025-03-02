FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder

# Setup working directory for the project
WORKDIR /src

COPY ./src/BuildingBlocks/micro-bootstrap/src/*.csproj ./BuildingBlocks/micro-bootstrap/src/
COPY ./src/Services/ECommerce.Services.Customers/ECommerce.Services.Customers/ECommerce.Services.Customers.csproj ./Services/ECommerce.Services.Customers/ECommerce.Services.Customers/
COPY ./src/Services/ECommerce.Services.Customers/ECommerce.Services.Customers.Api/ECommerce.Services.Customers.Api.csproj ./Services/ECommerce.Services.Customers/ECommerce.Services.Customers.Api/

# Restore nuget packages
RUN dotnet restore ./Services/ECommerce.Services.Customers/src/ECommerce.Services.Customers.Api/ECommerce.Services.Customers.Api.csproj

# Copy project files
COPY ./src/BuildingBlocks/micro-bootstrap/src/ ./BuildingBlocks/micro-bootstrap/src/
COPY ./src/Services/ECommerce.Services.Customers/ECommerce.Services.Customers.Api/  ./Services/ECommerce.Services.Customers/ECommerce.Services.Customers.Api/
COPY ./src/Services/ECommerce.Services.Customers/ECommerce.Services.Customers/  ./Services/ECommerce.Services.Customers/ECommerce.Services.Customers/

# Build project with Release configuration
# and no restore, as we did it already
RUN dotnet build -c Release --no-restore  ./Services/ECommerce.Services.Customers/ECommerce.Services.Customers.Api/ECommerce.Services.Customers.Api.csproj

WORKDIR /src/Services/ECommerce.Services.Customers/ECommerce.Services.Customers.Api/
# Publish project to output folder
# and no build, as we did it already
RUN ls
RUN dotnet publish -c Release --no-build -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
# Setup working directory for the project
WORKDIR /app
COPY --from=builder /src/Services/ECommerce.Services.Customers/ECommerce.Services.Customers.Api/out  .
EXPOSE 80

ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker
ENTRYPOINT ["dotnet", "ECommerce.Services.Customers.Api.dll"]
