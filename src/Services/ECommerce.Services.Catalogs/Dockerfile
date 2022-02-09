FROM mcr.microsoft.com/dotnet/core/sdk:6 AS build
WORKDIR /src
ARG Version

COPY . .

WORKDIR src/ECommerce.Services.Catalogs.Api
RUN dotnet restore

RUN dotnet publish /p:Version=$Version --no-restore -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:6 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", ECommerce.Services.Catalogs.Api.dll"]
