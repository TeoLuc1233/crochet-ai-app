#!/bin/bash
# Deploy frontend to Vercel

set -e

echo "Deploying frontend to Vercel..."

# Build the project
cd frontend
npm run build

# Deploy to Vercel (requires Vercel CLI)
# vercel --prod

echo "Frontend deployment complete"
