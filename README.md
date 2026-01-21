# Crochet AI

AI-powered crochet pattern generator that creates patterns from photos using Claude Sonnet 4.

## Features

- 🎨 **AI Pattern Generation**: Upload a photo and generate a complete crochet pattern
- 📚 **Pattern Library**: Browse and search through a collection of crochet patterns
- 📊 **Project Tracking**: Track your crochet projects with progress indicators
- 💳 **Subscription Plans**: Free, Premium, and Pro tiers with different features
- 🔐 **Secure Authentication**: JWT-based authentication with refresh tokens
- 🖼️ **Image Storage**: Azure Blob Storage for pattern images
- ⚡ **Fast & Responsive**: Built with Next.js 14 and ASP.NET Core 8

## Tech Stack

### Backend
- ASP.NET Core 8 Web API
- PostgreSQL 16
- Redis 7
- Entity Framework Core 8
- ASP.NET Identity
- JWT Authentication
- Azure Blob Storage
- Stripe.NET
- Serilog
- Application Insights

### Frontend
- Next.js 14 (App Router)
- TypeScript 5
- TailwindCSS 3
- shadcn/ui
- Framer Motion
- React Hook Form + Zod

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 20+
- PostgreSQL 16
- Redis 7
- Docker & Docker Compose (optional)

### Setup

1. Clone the repository:
```bash
git clone <repository-url>
cd crochet-ai-app
```

2. Backend setup:
```bash
cd backend/CrochetAI.Api
dotnet restore
dotnet ef database update
dotnet run
```

3. Frontend setup:
```bash
cd frontend
npm install
npm run dev
```

4. Environment variables:
- Copy `.env.example` files and configure your settings
- Set up Azure Blob Storage connection string
- Configure Anthropic API key
- Configure Stripe API keys

### Docker Compose

```bash
docker-compose up -d
```

## Project Structure

```
crochet-ai-app/
├── backend/
│   ├── CrochetAI.Api/          # Main API project
│   ├── CrochetAI.Api.Tests/    # Unit tests
│   └── CrochetAI.IntegrationTests/  # Integration tests
├── frontend/                    # Next.js application
├── scripts/                     # Deployment scripts
└── docker-compose.yml          # Docker configuration
```

## API Endpoints

- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh-token` - Refresh access token
- `GET /api/patterns` - List patterns
- `GET /api/patterns/{id}` - Get pattern details
- `POST /api/ai/generate-pattern` - Generate pattern from image
- `POST /api/images/upload` - Upload image
- `GET /api/projects` - List user projects
- `POST /api/subscriptions/create-checkout` - Create Stripe checkout

## Testing

```bash
# Backend tests
cd backend
dotnet test

# Frontend tests
cd frontend
npm test
```

## Deployment

See `scripts/` directory for deployment scripts:
- `deploy-backend.sh` - Deploy to Azure App Service
- `deploy-frontend.sh` - Deploy to Vercel
- `migrate-database.sh` - Run database migrations

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Write tests
5. Submit a pull request

## License

MIT
