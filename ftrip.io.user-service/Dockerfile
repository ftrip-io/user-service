FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /build

# Copy everything
COPY . .

# Build and publish a release
RUN dotnet publish --source=/build/.nuget/packages -c Release -o out
 
# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS final
WORKDIR /app
COPY --from=build /build/out .

ENTRYPOINT ["dotnet", "ftrip.io.user-service.dll"]