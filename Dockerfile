# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy and restore project files
COPY src/UrlShortener.Api/UrlShortener.Api.csproj ./src/UrlShortener.Api/
COPY src/UrlShortener.Application/UrlShortener.Application.csproj ./src/UrlShortener.Application/
RUN dotnet restore src/UrlShortener.Api/UrlShortener.Api.csproj

# Copy the rest of the source code
COPY . ./

# Build and publish
RUN dotnet publish src/UrlShortener.Api/UrlShortener.Api.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final-env
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "UrlShortener.Api.dll"]