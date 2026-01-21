# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY CrochetAI.Api.csproj .
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

EXPOSE 5000

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CrochetAI.Api.dll"]
