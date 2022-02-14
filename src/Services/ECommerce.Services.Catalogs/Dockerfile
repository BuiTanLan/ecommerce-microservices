# https://devblogs.microsoft.com/nuget/microsoft-author-signing-certificate-update/
# https://github.com/NuGet/Home/issues/10491
# https://devblogs.microsoft.com/nuget/net-5-nuget-restore-failures-on-linux-distributions-using-nss-or-ca-certificates/
# https://github.com/dotnet/announcements/issues/180
# https://github.com/NuGet/Announcements/issues/56
# https://event-driven.io/en/how_to_buid_an_optimal_docker_image_for_your_application/

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 4000
ENV ASPNETCORE_URLS http://*:4000
ENV ASPNETCORE_ENVIRONMENT docker

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS builder

# Setup working directory for the project
WORKDIR /src
RUN curl -o /usr/local/share/ca-certificates/verisign.crt -SsL https://crt.sh/?d=1039083 && update-ca-certificates
COPY ./src/BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj ./BuildingBlocks/BuildingBlocks/
COPY ./src/Services/ECommerce.Services.Catalogs/src/ECommerce.Services.Catalogs/ECommerce.Services.Catalogs.csproj ./Services/ECommerce.Services.Catalogs/src/ECommerce.Services.Catalogs/
COPY ./src/Services/ECommerce.Services.Catalogs/src/ECommerce.Services.Catalogs.Api/ECommerce.Services.Catalogs.Api.csproj ./Services/ECommerce.Services.Catalogs/src/ECommerce.Services.Catalogs.Api/
COPY ./src/Services/ECommerce.Services.Catalogs/nuget.config ./Services/ECommerce.Services.Catalogs/src

# Restore nuget packages
RUN dotnet restore ./Services/ECommerce.Services.Catalogs/src/ECommerce.Services.Catalogs.Api/ECommerce.Services.Catalogs.Api.csproj

# Copy project files
COPY ./src/BuildingBlocks/BuildingBlocks ./BuildingBlocks/BuildingBlocks/
COPY ./src/Services/ECommerce.Services.Catalogs/src/ECommerce.Services.Catalogs  ./Services/ECommerce.Services.Catalogs/src/ECommerce.Services.Catalogs/
COPY ./src/Services/ECommerce.Services.Catalogs/src/ECommerce.Services.Catalogs.Api ./Services/ECommerce.Services.Catalogs/src/ECommerce.Services.Catalogs.Api/

WORKDIR /src/Services/ECommerce.Services.Catalogs/src/ECommerce.Services.Catalogs.Api
# Build project with Release configuration
# and no restore, as we did it already
RUN dotnet build  -c Release --no-restore -o /app/build

FROM build AS publish
# Publish project to output folder
# and no build, as we did it already
RUN dotnet publish -c Release --no-build -o /app/publish

# second, final, lighter image
FROM base AS final
# Setup working directory for the project
WORKDIR /app
COPY --from=builder /app/publish  .
ENTRYPOINT ["dotnet", "ECommerce.Services.Catalogs.Api.dll"]
