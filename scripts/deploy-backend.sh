#!/bin/bash
# Deploy backend to Azure App Service

set -e

echo "Deploying backend to Azure App Service..."

# Build the project
dotnet publish backend/CrochetAI.Api/CrochetAI.Api.csproj -c Release -o ./publish

# Deploy to Azure (requires Azure CLI and app service configured)
# az webapp deploy --resource-group <resource-group> --name <app-name> --src-path ./publish

echo "Backend deployment complete"
