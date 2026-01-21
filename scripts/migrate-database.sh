#!/bin/bash
# Database migration script

set -e

echo "Running database migrations..."

cd backend/CrochetAI.Api
dotnet ef database update

echo "Database migrations complete"
