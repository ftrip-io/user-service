FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /build

ARG CONFIGURATION=Release

# Restore dependencies
COPY ["ftrip.io.user-service.contracts/*.csproj", "ftrip.io.user-service.contracts/package*.json", "./ftrip.io.user-service.contracts/"]
RUN dotnet restore /p:Configuration=$CONFIGURATION ftrip.io.user-service.contracts/ftrip.io.user-service.contracts.csproj

COPY ["ftrip.io.user-service/*.csproj", "ftrip.io.user-service/package*.json", "./ftrip.io.user-service/"]
RUN dotnet restore /p:Configuration=$CONFIGURATION ftrip.io.user-service/ftrip.io.user-service.csproj

# Publish
COPY . .

RUN dotnet publish -c $CONFIGURATION -o out ftrip.io.user-service/ftrip.io.user-service.csproj
 
# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS final
WORKDIR /app

RUN addgroup --system ftrip-io && adduser --system user-service

COPY --from=build --chown=user-service:ftrip-io --chmod=500 /build/out .

EXPOSE 80

USER user-service

ENTRYPOINT ["dotnet", "ftrip.io.user-service.dll"]