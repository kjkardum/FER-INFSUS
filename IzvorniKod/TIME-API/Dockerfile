# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY TIME-API.sln ./
COPY FER.InfSus.Time.Api/FER.InfSus.Time.Api.csproj FER.InfSus.Time.Api/
COPY FER.InfSus.Time.Application/FER.InfSus.Time.Application.csproj FER.InfSus.Time.Application/
COPY FER.InfSus.Time.Domain/FER.InfSus.Time.Domain.csproj FER.InfSus.Time.Domain/
COPY FER.InfSus.Time.Infrastructure/FER.InfSus.Time.Infrastructure.csproj FER.InfSus.Time.Infrastructure/
COPY Tests/FER.InfSus.Time.Api.Tests/FER.InfSus.Time.Api.Tests.csproj Tests/FER.InfSus.Time.Api.Tests/
COPY Tests/FER.InfSus.Time.Application.Tests/FER.InfSus.Time.Application.Tests.csproj Tests/FER.InfSus.Time.Application.Tests/
COPY Tests/FER.InfSus.Time.Infrastructure.Tests/FER.InfSus.Time.Infrastructure.Tests.csproj Tests/FER.InfSus.Time.Infrastructure.Tests/

RUN dotnet restore

# Copy everything else and build
COPY . .

RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
WORKDIR /app
COPY --from=build-env /app/out .

RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

EXPOSE 80
ENTRYPOINT ["dotnet", "FER.InfSus.Time.Api.dll"]
