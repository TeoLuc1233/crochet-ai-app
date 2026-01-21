# ARCHITECTURE.md - Technical Decisions & Guidelines

**Purpose**: This document contains ALL architectural decisions. Ralph should follow these without questioning. Security is the #1 priority.

---

## 🏗️ SYSTEM ARCHITECTURE

### High-Level Architecture

```
┌─────────────┐
│   Client    │ (Browser/PWA)
│  Next.js 14 │
└──────┬──────┘
       │ HTTPS
       │
┌──────▼──────────────────────────┐
│     Azure Front Door            │ (CDN, WAF, DDoS protection)
└──────┬──────────────────────────┘
       │
┌──────▼──────────────────────────┐
│   ASP.NET Core 8 API            │ (Azure App Service)
│   - Authentication              │
│   - Rate Limiting               │
│   - Business Logic              │
└──┬────┬────┬────┬───────────────┘
   │    │    │    │
   │    │    │    └───────┐
   │    │    │            │
┌──▼────▼────▼────▼─────┐ │
│  PostgreSQL 16        │ │
│  - User data          │ │
│  - Patterns           │ │
│  - Subscriptions      │ │
└───────────────────────┘ │
                          │
┌─────────────────────────▼─┐
│  Azure Blob Storage       │
│  - User uploaded images   │
│  - Pattern images         │
└─────────────┬─────────────┘
              │
┌─────────────▼─────────────┐
│  Redis Cache              │
│  - Rate limiting          │
│  - Session data           │
│  - AI response cache      │
└───────────────────────────┘
              │
┌─────────────▼─────────────┐
│  External APIs            │
│  - Anthropic Claude       │
│  - OpenAI (fallback)      │
│  - Stripe                 │
└───────────────────────────┘
```

---

## 🔧 TECHNOLOGY STACK

### Backend

**Framework**: ASP.NET Core 8
- **Why**: Type-safe, performant, excellent security features, familiar to developer
- **Pattern**: Clean Architecture (Controllers → Services → Repositories)
- **API Style**: RESTful with minimal API endpoints where appropriate

**Database**: PostgreSQL 16
- **Why**: ACID compliant, excellent JSON support, mature, open-source
- **ORM**: Entity Framework Core 8
- **Migrations**: Code-first approach
- **Connection Pooling**: Npgsql with connection pooling enabled

**Caching**: Redis 7
- **Why**: Fast, reliable, supports advanced data structures
- **Use Cases**:
  - Rate limiting (per-IP, per-user)
  - Session storage
  - AI response caching
  - Feature flags

**Storage**: Azure Blob Storage
- **Why**: Scalable, cheap, integrates well with Azure
- **Structure**:
  - `user-uploads/`: User-uploaded images for AI
  - `pattern-images/`: Official pattern images
  - `generated-patterns/`: AI-generated pattern PDFs
- **Access**: SAS tokens with expiration (max 1 hour)

**Authentication**: ASP.NET Identity + JWT
- **Why**: Battle-tested, OWASP compliant
- **Token Lifetime**:
  - Access token: 15 minutes
  - Refresh token: 7 days
- **Storage**: Refresh tokens in PostgreSQL, access tokens in httpOnly cookies

### Frontend

**Framework**: Next.js 14
- **Why**: SSR/SSG for SEO, excellent performance, React ecosystem
- **Rendering**: Hybrid (SSR for pages, CSR for dynamic components)
- **Router**: App Router (not Pages Router)

**Language**: TypeScript 5
- **Why**: Type safety, better DX, catches bugs early
- **Strict Mode**: Enabled
- **Config**: Strict null checks, no implicit any

**Styling**: TailwindCSS 3 + shadcn/ui
- **Why**: Utility-first, fast development, consistent design
- **Components**: shadcn/ui for base components (accessible, customizable)
- **Theme**: Custom theme in `tailwind.config.ts`

**State Management**: React Context + hooks
- **Why**: Simple, built-in, no external dependency for basic state
- **For Complex State**: Zustand if needed (lightweight, no boilerplate)

**Animations**: Framer Motion 11
- **Why**: Declarative, performant, great DX
- **Use Cases**: Page transitions, micro-interactions, loading states

**Forms**: React Hook Form + Zod
- **Why**: Performant, type-safe validation
- **Validation**: Zod schemas shared between frontend and backend

**HTTP Client**: Fetch API with custom wrapper
- **Why**: Native, no external dependency
- **Features**: Auto-retry, timeout, token refresh

### AI/ML

**Primary**: Anthropic Claude Sonnet 4
- **Why**: Best vision model, great at following instructions, reliable
- **Model**: `claude-sonnet-4-20250514`
- **Use Cases**: Image analysis, pattern generation

**Fallback**: OpenAI GPT-4o
- **Why**: Backup if Claude is down or rate-limited
- **Model**: `gpt-4o`
- **Switch Logic**: Auto-switch on 3 consecutive failures

**Caching Strategy**:
- Hash image (SHA256) + user preferences
- Cache pattern generation results for 24 hours
- Store in Redis with TTL

---

## 🔒 SECURITY ARCHITECTURE

### Threat Model

**Primary Threats**:
1. Unauthorized access to premium content
2. API abuse (rate limit exhaustion, cost attacks)
3. SQL injection
4. XSS attacks
5. CSRF attacks
6. File upload attacks (malware, XXE, etc.)
7. Account takeover
8. Payment fraud

### Defense Layers

#### Layer 1: Network Security

**Azure Front Door**:
- Web Application Firewall (WAF)
- DDoS protection
- Geo-filtering (block known bad actors)
- Rate limiting at edge

**HTTPS Only**:
```csharp
// In Program.cs
app.UseHttpsRedirection();
app.UseHsts(); // HTTP Strict Transport Security
```

**Security Headers**:
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:;");
    await next();
});
```

#### Layer 2: Authentication & Authorization

**Password Requirements**:
- Minimum 8 characters
- At least 1 uppercase, 1 lowercase, 1 number, 1 special char
- No common passwords (use zxcvbn-like check)
- Password hashing: PBKDF2 with 10,000 iterations (ASP.NET Identity default)

**JWT Security**:
```csharp
// JWT Configuration
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
            ClockSkew = TimeSpan.Zero // No clock skew
        };
    });
```

**Refresh Token Storage**:
- Store in PostgreSQL `RefreshTokens` table
- Hash token before storage (SHA256)
- Include: UserId, TokenHash, ExpiresAt, CreatedAt, RevokedAt
- Automatic cleanup of expired tokens (background job)

**Authorization Policies**:
```csharp
services.AddAuthorization(options =>
{
    options.AddPolicy("FreeUser", policy => 
        policy.RequireClaim("SubscriptionTier", "Free"));
    options.AddPolicy("PremiumUser", policy => 
        policy.RequireClaim("SubscriptionTier", "Premium", "Pro"));
    options.AddPolicy("ProUser", policy => 
        policy.RequireClaim("SubscriptionTier", "Pro"));
});
```

#### Layer 3: Rate Limiting

**Multi-Tier Rate Limiting**:

1. **Global API Rate Limit** (per IP):
   - 1000 requests/hour
   - 100 requests/minute

2. **Authentication Endpoints** (per IP):
   - 5 requests/minute (login, register)
   - 10 requests/hour

3. **AI Generation** (per user):
   - Free: 1/month
   - Premium: 20/month
   - Pro: 100/month

4. **File Upload** (per user):
   - 10 files/hour
   - 50 MB/hour total

**Implementation**:
```csharp
// Use AspNetCoreRateLimit
services.AddMemoryCache();
services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
services.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
services.AddInMemoryRateLimiting();
services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// In appsettings.json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1h",
        "Limit": 1000
      },
      {
        "Endpoint": "*/api/auth/*",
        "Period": "1m",
        "Limit": 5
      }
    ]
  }
}
```

#### Layer 4: Input Validation

**FluentValidation for All DTOs**:
```csharp
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Password must contain uppercase")
            .Matches(@"[a-z]").WithMessage("Password must contain lowercase")
            .Matches(@"[0-9]").WithMessage("Password must contain number")
            .Matches(@"[\W]").WithMessage("Password must contain special char");

        RuleFor(x => x.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50)
            .Matches(@"^[a-zA-Z0-9_]+$")
                .WithMessage("Username can only contain letters, numbers, underscores");
    }
}
```

**SQL Injection Prevention**:
- **NEVER** use string concatenation for queries
- **ALWAYS** use parameterized queries (EF Core does this automatically)
- **NEVER** use `.FromSqlRaw()` with user input

**XSS Prevention**:
- All user input sanitized on frontend (DOMPurify)
- All output escaped on backend (ASP.NET Core does this by default)
- Content-Security-Policy header active

**CSRF Prevention**:
```csharp
// Add anti-forgery tokens
services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Validate on all state-changing endpoints
[ValidateAntiForgeryToken]
public async Task<IActionResult> CreatePattern([FromBody] CreatePatternRequest request)
{
    // ...
}
```

#### Layer 5: File Upload Security

**File Upload Validation**:
```csharp
public class ImageUploadValidator
{
    private static readonly byte[][] ImageMagicNumbers = 
    {
        new byte[] { 0xFF, 0xD8, 0xFF }, // JPEG
        new byte[] { 0x89, 0x50, 0x4E, 0x47 }, // PNG
        new byte[] { 0x47, 0x49, 0x46 }, // GIF
        new byte[] { 0x42, 0x4D }, // BMP
    };

    public async Task<(bool isValid, string error)> ValidateImageAsync(IFormFile file)
    {
        // 1. Check file size (max 10MB)
        if (file.Length > 10 * 1024 * 1024)
            return (false, "File too large (max 10MB)");

        // 2. Check file extension
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" }.Contains(ext))
            return (false, "Invalid file type");

        // 3. Verify magic bytes (prevents extension spoofing)
        using var stream = file.OpenReadStream();
        var buffer = new byte[8];
        await stream.ReadAsync(buffer, 0, 8);
        
        bool validMagicBytes = ImageMagicNumbers.Any(magic => 
            buffer.Take(magic.Length).SequenceEqual(magic));
        
        if (!validMagicBytes)
            return (false, "Invalid file content");

        // 4. Scan with antivirus (in production, use Azure Defender)
        // await _antivirusService.ScanAsync(stream);

        return (true, null);
    }
}
```

**File Storage Security**:
- Generate random filename (GUID)
- Store in isolated blob container
- Use SAS tokens with short expiration
- Never allow direct file execution

#### Layer 6: API Security

**Audit Logging**:
```csharp
public class AuditLog
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Action { get; set; } // "Login", "PatternView", "AIGeneration", etc.
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
    public string Details { get; set; } // JSON
}

// Log all sensitive actions
await _auditService.LogAsync(new AuditLog
{
    UserId = user.Id,
    Action = "AIPatternGeneration",
    IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
    UserAgent = Request.Headers["User-Agent"],
    Timestamp = DateTime.UtcNow,
    Details = JsonSerializer.Serialize(new { ImageId = imageId, PatternId = result.Id })
});
```

**Error Handling**:
- **NEVER** expose stack traces in production
- Log detailed errors server-side
- Return generic error messages to client
```csharp
app.UseExceptionHandler("/error");

app.Map("/error", (HttpContext context) =>
{
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    
    // Log with full details
    _logger.LogError(exception, "Unhandled exception");
    
    // Return generic error to client
    return Results.Problem(
        title: "An error occurred",
        statusCode: 500,
        detail: context.RequestServices.GetRequiredService<IHostEnvironment>()
            .IsDevelopment() ? exception?.Message : null
    );
});
```

---

## 📊 DATABASE SCHEMA

### Core Tables

```sql
-- Users (managed by ASP.NET Identity)
CREATE TABLE AspNetUsers (
    Id VARCHAR(255) PRIMARY KEY,
    UserName VARCHAR(256) NOT NULL,
    Email VARCHAR(256) NOT NULL,
    EmailConfirmed BOOLEAN NOT NULL,
    PasswordHash TEXT,
    SecurityStamp TEXT,
    ConcurrencyStamp TEXT,
    PhoneNumber VARCHAR(50),
    PhoneNumberConfirmed BOOLEAN,
    TwoFactorEnabled BOOLEAN,
    LockoutEnd TIMESTAMPTZ,
    LockoutEnabled BOOLEAN,
    AccessFailedCount INT,
    SubscriptionTier VARCHAR(20) NOT NULL DEFAULT 'Free', -- Custom column
    CreatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Subscriptions
CREATE TABLE Subscriptions (
    Id SERIAL PRIMARY KEY,
    UserId VARCHAR(255) NOT NULL REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    StripeCustomerId VARCHAR(255) NOT NULL,
    StripeSubscriptionId VARCHAR(255) UNIQUE,
    Tier VARCHAR(20) NOT NULL, -- 'Free', 'Premium', 'Pro'
    Status VARCHAR(20) NOT NULL, -- 'Active', 'Canceled', 'PastDue'
    CurrentPeriodStart TIMESTAMPTZ NOT NULL,
    CurrentPeriodEnd TIMESTAMPTZ NOT NULL,
    CanceledAt TIMESTAMPTZ,
    CreatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Patterns
CREATE TABLE Patterns (
    Id SERIAL PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Description TEXT,
    Difficulty VARCHAR(20) NOT NULL, -- 'Beginner', 'Intermediate', 'Advanced'
    Category VARCHAR(50) NOT NULL, -- 'Amigurumi', 'Clothing', 'Home', etc.
    Materials JSONB NOT NULL, -- { "yarn": "...", "hook": "...", "other": [...] }
    Instructions TEXT NOT NULL,
    ImageUrl VARCHAR(500),
    IsPremium BOOLEAN NOT NULL DEFAULT FALSE,
    ViewCount INT NOT NULL DEFAULT 0,
    CreatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    INDEX idx_difficulty (Difficulty),
    INDEX idx_category (Category),
    INDEX idx_premium (IsPremium)
);

-- Projects (user's work)
CREATE TABLE Projects (
    Id SERIAL PRIMARY KEY,
    UserId VARCHAR(255) NOT NULL REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    PatternId INT REFERENCES Patterns(Id) ON DELETE SET NULL,
    Title VARCHAR(255) NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'InProgress', -- 'NotStarted', 'InProgress', 'Completed'
    Progress JSONB, -- { "currentRow": 10, "totalRows": 50, "notes": "..." }
    StartedAt TIMESTAMPTZ,
    CompletedAt TIMESTAMPTZ,
    CreatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    INDEX idx_user_status (UserId, Status)
);

-- AI Generations (track usage)
CREATE TABLE AIGenerations (
    Id SERIAL PRIMARY KEY,
    UserId VARCHAR(255) NOT NULL REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    ImageUrl VARCHAR(500) NOT NULL,
    ImageHash VARCHAR(64) NOT NULL, -- SHA256 for caching
    AnalysisResult JSONB NOT NULL,
    GeneratedPattern TEXT NOT NULL,
    CachedResponse BOOLEAN NOT NULL DEFAULT FALSE,
    GenerationTimeMs INT NOT NULL,
    CreatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    INDEX idx_user_month (UserId, (DATE_TRUNC('month', CreatedAt))),
    INDEX idx_image_hash (ImageHash)
);

-- Refresh Tokens
CREATE TABLE RefreshTokens (
    Id SERIAL PRIMARY KEY,
    UserId VARCHAR(255) NOT NULL REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    TokenHash VARCHAR(64) NOT NULL UNIQUE,
    ExpiresAt TIMESTAMPTZ NOT NULL,
    CreatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    RevokedAt TIMESTAMPTZ,
    INDEX idx_user_active (UserId, ExpiresAt) WHERE RevokedAt IS NULL
);

-- Audit Logs
CREATE TABLE AuditLogs (
    Id BIGSERIAL PRIMARY KEY,
    UserId VARCHAR(255) REFERENCES AspNetUsers(Id) ON DELETE SET NULL,
    Action VARCHAR(100) NOT NULL,
    IpAddress VARCHAR(45) NOT NULL,
    UserAgent TEXT,
    Details JSONB,
    Timestamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    INDEX idx_user_action (UserId, Action),
    INDEX idx_timestamp (Timestamp)
);
```

---

## 🎨 FRONTEND ARCHITECTURE

### Folder Structure

```
frontend/
├── app/                    # Next.js 14 App Router
│   ├── (auth)/             # Auth group routes
│   │   ├── login/
│   │   ├── register/
│   │   └── layout.tsx
│   ├── (dashboard)/        # Protected routes
│   │   ├── patterns/
│   │   ├── projects/
│   │   ├── generate/
│   │   ├── settings/
│   │   └── layout.tsx
│   ├── pricing/
│   ├── layout.tsx          # Root layout
│   └── page.tsx            # Home page
├── components/
│   ├── ui/                 # shadcn/ui components
│   ├── patterns/           # Pattern-specific components
│   ├── ai/                 # AI generator components
│   └── layout/             # Layout components
├── lib/
│   ├── api/                # API client
│   ├── auth/               # Auth context & hooks
│   ├── utils/              # Utility functions
│   └── validations/        # Zod schemas
├── public/
│   ├── images/
│   └── fonts/
├── styles/
│   └── globals.css
└── types/
    └── index.ts            # TypeScript types
```

### Component Guidelines

**Component Structure**:
```tsx
// components/patterns/PatternCard.tsx
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { motion } from "framer-motion";

interface PatternCardProps {
  id: number;
  title: string;
  difficulty: "Beginner" | "Intermediate" | "Advanced";
  imageUrl: string;
  isPremium: boolean;
}

export function PatternCard({ 
  id, title, difficulty, imageUrl, isPremium 
}: PatternCardProps) {
  return (
    <motion.div
      whileHover={{ scale: 1.02 }}
      transition={{ duration: 0.2 }}
    >
      <Card className="overflow-hidden cursor-pointer">
        <CardHeader className="p-0">
          <img 
            src={imageUrl} 
            alt={title}
            className="w-full h-48 object-cover"
          />
        </CardHeader>
        <CardContent className="p-4">
          <h3 className="font-semibold text-lg">{title}</h3>
          <div className="flex gap-2 mt-2">
            <Badge variant="secondary">{difficulty}</Badge>
            {isPremium && <Badge variant="default">Premium</Badge>}
          </div>
        </CardContent>
      </Card>
    </motion.div>
  );
}
```

**Error Boundaries**:
```tsx
// components/ErrorBoundary.tsx
'use client';

import { Component, ReactNode } from 'react';

interface Props {
  children: ReactNode;
  fallback?: ReactNode;
}

interface State {
  hasError: boolean;
  error?: Error;
}

export class ErrorBoundary extends Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = { hasError: false };
  }

  static getDerivedStateFromError(error: Error): State {
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, errorInfo: any) {
    console.error('ErrorBoundary caught:', error, errorInfo);
  }

  render() {
    if (this.state.hasError) {
      return this.props.fallback || (
        <div className="p-4 text-center">
          <h2>Something went wrong</h2>
          <button onClick={() => this.setState({ hasError: false })}>
            Try again
          </button>
        </div>
      );
    }

    return this.props.children;
  }
}
```

---

## 🚀 PERFORMANCE OPTIMIZATION

### Backend Performance

**Database Optimization**:
- Use proper indexes (see schema above)
- Implement pagination (skip/take)
- Use `.AsNoTracking()` for read-only queries
- Batch operations where possible

**Caching Strategy**:
```csharp
// Cache pattern list for 5 minutes
public async Task<List<Pattern>> GetPatternsAsync()
{
    var cacheKey = "patterns:all";
    var cached = await _cache.GetStringAsync(cacheKey);
    
    if (cached != null)
        return JsonSerializer.Deserialize<List<Pattern>>(cached);
    
    var patterns = await _context.Patterns.ToListAsync();
    
    await _cache.SetStringAsync(cacheKey, 
        JsonSerializer.Serialize(patterns),
        new DistributedCacheEntryOptions 
        { 
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) 
        });
    
    return patterns;
}
```

### Frontend Performance

**Code Splitting**:
```tsx
// Lazy load AI generator (heavy component)
import dynamic from 'next/dynamic';

const AIGenerator = dynamic(() => import('@/components/ai/AIGenerator'), {
  loading: () => <LoadingSpinner />,
  ssr: false // Client-side only
});
```

**Image Optimization**:
```tsx
import Image from 'next/image';

<Image
  src={pattern.imageUrl}
  alt={pattern.title}
  width={400}
  height={300}
  placeholder="blur"
  blurDataURL={pattern.blurHash}
/>
```

**Debouncing/Throttling**:
```tsx
import { useDebouncedCallback } from 'use-debounce';

const handleSearch = useDebouncedCallback((query: string) => {
  // API call
}, 300);
```

---

## 🧪 TESTING STRATEGY

### Backend Testing

**Unit Tests** (xUnit):
```csharp
public class PatternServiceTests
{
    [Fact]
    public async Task GetPatternById_ExistingId_ReturnsPattern()
    {
        // Arrange
        var mockRepo = new Mock<IPatternRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Pattern { Id = 1, Title = "Test" });
        var service = new PatternService(mockRepo.Object);

        // Act
        var result = await service.GetPatternByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.Title);
    }
}
```

**Integration Tests** (WebApplicationFactory):
```csharp
public class PatternEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PatternEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPatterns_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/patterns");
        response.EnsureSuccessStatusCode();
    }
}
```

### Frontend Testing

**Component Tests** (React Testing Library):
```tsx
import { render, screen } from '@testing-library/react';
import { PatternCard } from './PatternCard';

test('renders pattern card with title', () => {
  render(
    <PatternCard
      id={1}
      title="Test Pattern"
      difficulty="Beginner"
      imageUrl="/test.jpg"
      isPremium={false}
    />
  );
  
  expect(screen.getByText('Test Pattern')).toBeInTheDocument();
  expect(screen.getByText('Beginner')).toBeInTheDocument();
});
```

**E2E Tests** (Playwright):
```typescript
import { test, expect } from '@playwright/test';

test('user can generate AI pattern', async ({ page }) => {
  await page.goto('/generate');
  await page.setInputFiles('input[type="file"]', './test-image.jpg');
  await page.click('button:has-text("Generate Pattern")');
  await expect(page.locator('.pattern-result')).toBeVisible({ timeout: 30000 });
});
```

---

## 📝 CODE STYLE & CONVENTIONS

### C# (Backend)

- **Naming**: PascalCase for public members, camelCase for private
- **Async**: All I/O operations async, suffix methods with `Async`
- **Nullability**: Enable nullable reference types
- **Error Handling**: Use specific exceptions, never catch `Exception`

### TypeScript (Frontend)

- **Naming**: camelCase for variables/functions, PascalCase for components/types
- **Strict Mode**: Enabled, no `any` types
- **Functional Components**: Use hooks, avoid class components
- **Imports**: Absolute imports with `@/` prefix

---

## 🎯 SUCCESS CRITERIA

**When all of these are true, the project is complete**:

1. ✅ All 73 tasks in `@fix_plan.md` are `[DONE]`
2. ✅ All tests pass (backend + frontend)
3. ✅ Code coverage > 80%
4. ✅ No security vulnerabilities in dependencies
5. ✅ App runs end-to-end without errors
6. ✅ API response times < 200ms (p95)
7. ✅ Frontend loads in < 2s
8. ✅ AI generation works in < 30s
9. ✅ Deployed and accessible via URL
10. ✅ Health checks passing

---

**Remember**: Security is not optional. Every feature must be secure by default. When in doubt, read this document.
