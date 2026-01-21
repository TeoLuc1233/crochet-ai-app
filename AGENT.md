# AGENT.md - Build, Run, Test Instructions

## 🏗️ PROJECT SETUP

### First Time Setup

```bash
# Backend setup
cd backend
dotnet restore
dotnet ef database update

# Frontend setup
cd ../frontend
npm install

# Environment variables
cp backend/.env.example backend/.env
cp frontend/.env.example frontend/.env

# Edit .env files with your actual values
```

### Required Environment Variables

**backend/.env**
```
DATABASE_URL=postgresql://localhost:5432/crochetai
REDIS_URL=localhost:6379
JWT_SECRET=<generate-secure-random-string>
ANTHROPIC_API_KEY=<your-anthropic-key>
OPENAI_API_KEY=<your-openai-key>
AZURE_STORAGE_CONNECTION_STRING=<your-azure-storage>
AZURE_STORAGE_CONTAINER=crochet-images
```

**frontend/.env.local**
```
NEXT_PUBLIC_API_URL=http://localhost:5000
```

## 🚀 RUNNING THE APPLICATION

### Development Mode

**Terminal 1 - Backend:**
```bash
cd backend
dotnet watch run
# API runs on http://localhost:5000
```

**Terminal 2 - Frontend:**
```bash
cd frontend
npm run dev
# Frontend runs on http://localhost:3000
```

**Terminal 3 - Redis (if not already running):**
```bash
redis-server
```

### Docker Compose (Recommended)

```bash
docker-compose up -d
# Everything runs together
# Frontend: http://localhost:3000
# Backend: http://localhost:5000
# PostgreSQL: localhost:5432
# Redis: localhost:6379
```

## 🧪 TESTING

### Backend Tests

```bash
cd backend

# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverageReportFormat=opencover

# Run specific test project
dotnet test tests/CrochetAI.Api.Tests

# Run tests with filter
dotnet test --filter "Category=Integration"
dotnet test --filter "FullyQualifiedName~Security"
```

### Frontend Tests

```bash
cd frontend

# Run all tests
npm test

# Run tests in watch mode
npm test -- --watch

# Run tests with coverage
npm test -- --coverage

# Run E2E tests (Playwright)
npm run test:e2e

# Run E2E in UI mode
npm run test:e2e:ui
```

### Integration Tests

```bash
# Run full integration test suite
./scripts/run-integration-tests.sh

# This starts test containers, runs all tests, then cleans up
```

## 🔍 CODE QUALITY

### Linting

```bash
# Backend (Roslyn analyzers run automatically on build)
cd backend
dotnet build /p:TreatWarningsAsErrors=true

# Frontend
cd frontend
npm run lint
npm run lint:fix
```

### Formatting

```bash
# Backend
cd backend
dotnet format

# Frontend
cd frontend
npm run format
npm run format:check
```

### Security Scanning

```bash
# Backend - Check for vulnerable packages
cd backend
dotnet list package --vulnerable

# Frontend - Check for vulnerable packages
cd frontend
npm audit
npm audit fix
```

## 📊 MONITORING & LOGS

### View Logs

```bash
# Backend logs
cd backend
dotnet run | tee logs/app.log

# Frontend logs
cd frontend
npm run dev 2>&1 | tee logs/frontend.log

# Docker logs
docker-compose logs -f backend
docker-compose logs -f frontend
```

### Health Checks

```bash
# Backend health check
curl http://localhost:5000/health

# Expected output:
# {"status":"Healthy","checks":{"database":"Healthy","redis":"Healthy","storage":"Healthy"}}

# Frontend health check
curl http://localhost:3000/api/health
```

## 🐛 DEBUGGING

### Backend Debugging

```bash
# Run with debug logging
cd backend
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Attach debugger in VS Code (use launch.json)
# F5 to start debugging
```

### Frontend Debugging

```bash
# Run with debug output
cd frontend
DEBUG=* npm run dev

# Open Chrome DevTools
# Network tab for API calls
# Console for errors
```

## 📦 DATABASE MANAGEMENT

### Migrations

```bash
cd backend

# Create new migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigrationName

# Remove last migration
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script > migration.sql
```

### Seed Data

```bash
cd backend

# Run seed data
dotnet run --seed

# Or via CLI
dotnet run -- --seed
```

## 🚢 BUILD FOR PRODUCTION

### Backend Production Build

```bash
cd backend
dotnet publish -c Release -o out
```

### Frontend Production Build

```bash
cd frontend
npm run build

# Test production build locally
npm run start
```

### Docker Production Images

```bash
# Build backend image
docker build -t crochetai-backend:latest -f backend/Dockerfile .

# Build frontend image
docker build -t crochetai-frontend:latest -f frontend/Dockerfile .

# Run production containers
docker-compose -f docker-compose.prod.yml up -d
```

## ✅ VALIDATION CHECKLIST

Before marking a task as DONE, verify:

```bash
# 1. All tests pass
cd backend && dotnet test
cd ../frontend && npm test

# 2. No linting errors
cd backend && dotnet build /p:TreatWarningsAsErrors=true
cd ../frontend && npm run lint

# 3. No security vulnerabilities
cd backend && dotnet list package --vulnerable
cd ../frontend && npm audit

# 4. App runs without errors
docker-compose up -d
# Check http://localhost:3000
# Check http://localhost:5000/health

# 5. Clean git state
git status
# Should see only expected changes
```

## 🆘 TROUBLESHOOTING

### "Database connection failed"
```bash
# Check PostgreSQL is running
docker ps | grep postgres

# Or start it
docker-compose up -d postgres

# Verify connection
psql postgresql://localhost:5432/crochetai
```

### "Redis connection failed"
```bash
# Check Redis is running
docker ps | grep redis

# Or start it
docker-compose up -d redis

# Test connection
redis-cli ping
# Should return PONG
```

### "Port already in use"
```bash
# Find process using port
lsof -i :5000  # Backend
lsof -i :3000  # Frontend

# Kill process
kill -9 <PID>
```

### "npm install fails"
```bash
# Clear cache and reinstall
cd frontend
rm -rf node_modules package-lock.json
npm cache clean --force
npm install
```

### "EF migrations fail"
```bash
# Reset database (CAUTION: deletes all data)
cd backend
dotnet ef database drop -f
dotnet ef database update
```

## 📝 COMMIT STANDARDS

```bash
# Format: <type>: <description>

# Types:
# feat: New feature
# fix: Bug fix
# test: Adding tests
# refactor: Code refactoring
# docs: Documentation
# style: Formatting
# perf: Performance improvement
# security: Security fix

# Examples:
git commit -m "feat: add AI pattern generator endpoint"
git commit -m "test: add security tests for image upload"
git commit -m "fix: resolve CSRF token validation issue"
git commit -m "security: implement rate limiting on AI endpoints"
```

## 🎯 QUICK REFERENCE

```bash
# Start everything
docker-compose up -d

# Run all tests
cd backend && dotnet test && cd ../frontend && npm test

# Check health
curl http://localhost:5000/health && curl http://localhost:3000/api/health

# View logs
docker-compose logs -f

# Stop everything
docker-compose down
```

---

**Always run tests before committing. Always check health endpoints after changes.**
