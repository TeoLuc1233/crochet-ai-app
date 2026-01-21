# @fix_plan.md - Prioritized Task List

**Ralph Instructions**: Work on tasks in order. Mark `[DONE]` when complete with all tests passing.

---

## 🏗️ PHASE 1: PROJECT FOUNDATION

### 1.1 Project Structure Setup

- [DONE] **TASK-001**: Create backend project structure
  - ASP.NET Core 8 Web API project
  - Standard folder structure (Controllers, Models, Services, etc.)
  - appsettings.json configuration
  - Spec: `specs/backend/01-project-structure.md`

- [DONE] **TASK-002**: Create frontend project structure
  - Next.js 14 with TypeScript
  - App router structure
  - TailwindCSS + shadcn/ui setup
  - Spec: `specs/frontend/01-project-structure.md`

- [DONE] **TASK-003**: Setup Docker Compose
  - PostgreSQL container
  - Redis container
  - Backend container
  - Frontend container
  - Spec: `specs/infrastructure/01-docker-compose.md`

- [DONE] **TASK-004**: Create .env templates
  - backend/.env.example
  - frontend/.env.example
  - Clear documentation of all variables
  - Spec: `specs/infrastructure/02-environment-variables.md`

---

## 🗄️ PHASE 2: DATABASE & CORE MODELS

### 2.1 Database Design

- [DONE] **TASK-005**: Create Entity Framework models
  - User, Pattern, Project, Subscription, AIGeneration
  - Proper relationships and indexes
  - Spec: `specs/backend/02-database-models.md`

- [DONE] **TASK-006**: Create initial EF migration
  - `dotnet ef migrations add InitialCreate`
  - Verify migration SQL
  - Spec: `specs/backend/03-migrations.md`

- [DONE] **TASK-007**: Setup database seeding
  - Test users
  - Sample patterns
  - Seed script in Program.cs
  - Spec: `specs/backend/04-database-seeding.md`

- [DONE] **TASK-008**: Write repository interfaces
  - IUserRepository, IPatternRepository, etc.
  - Generic repository pattern
  - Spec: `specs/backend/05-repositories.md`

- [DONE] **TASK-009**: Implement repositories
  - UserRepository, PatternRepository, etc.
  - EF Core queries with proper includes
  - Unit tests for each repository
  - Spec: `specs/backend/05-repositories.md`

---

## 🔐 PHASE 3: AUTHENTICATION & SECURITY

### 3.1 Backend Authentication

- [DONE] **TASK-010**: Setup ASP.NET Identity
  - Configure Identity with custom User model
  - Password requirements
  - Spec: `specs/security/01-authentication.md`

- [DONE] **TASK-011**: Implement JWT authentication
  - Token generation service
  - Refresh token mechanism
  - Token validation middleware
  - Spec: `specs/security/02-jwt-tokens.md`

- [DONE] **TASK-012**: Create auth endpoints
  - POST /api/auth/register
  - POST /api/auth/login
  - POST /api/auth/refresh-token
  - POST /api/auth/logout
  - Spec: `specs/backend/06-auth-endpoints.md`

- [DONE] **TASK-013**: Write auth integration tests
  - Test registration flow
  - Test login flow
  - Test invalid credentials
  - Test token refresh
  - Spec: `specs/backend/06-auth-endpoints.md`

### 3.2 Frontend Authentication

- [DONE] **TASK-014**: Create auth context
  - React context for auth state
  - Token storage (httpOnly cookies)
  - Auto-refresh mechanism
  - Spec: `specs/frontend/02-authentication.md`

- [DONE] **TASK-015**: Create auth pages
  - /login page
  - /register page
  - /forgot-password page (UI only for now)
  - Spec: `specs/frontend/03-auth-pages.md`

- [DONE] **TASK-016**: Create protected route wrapper
  - Redirect to /login if not authenticated
  - Handle token expiration
  - Spec: `specs/frontend/04-protected-routes.md`

---

## 🔒 PHASE 4: SECURITY HARDENING

### 4.1 Rate Limiting

- [DONE] **TASK-017**: Setup Redis connection
  - Redis client configuration
  - Connection health check
  - Spec: `specs/security/03-redis-setup.md`

- [DONE] **TASK-018**: Implement rate limiting middleware
  - AspNetCoreRateLimit configuration
  - Per-endpoint limits
  - IP-based + user-based limits
  - Spec: `specs/security/04-rate-limiting.md`

- [DONE] **TASK-019**: Test rate limiting
  - Unit tests for rate limit service
  - Integration tests for API endpoints
  - Spec: `specs/security/04-rate-limiting.md`

### 4.2 Input Validation & Security Headers

- [DONE] **TASK-020**: Setup FluentValidation
  - Install FluentValidation
  - Create validators for all DTOs
  - Register in DI container
  - Spec: `specs/security/05-input-validation.md`

- [DONE] **TASK-021**: Configure security headers
  - HSTS
  - CSP (Content Security Policy)
  - X-Content-Type-Options
  - X-Frame-Options
  - Spec: `specs/security/06-security-headers.md`

- [DONE] **TASK-022**: Implement CSRF protection
  - Anti-forgery tokens
  - Validation in API
  - Spec: `specs/security/07-csrf-protection.md`

---

## 🎨 PHASE 5: PATTERN LIBRARY (MVP)

### 5.1 Pattern Backend

- [DONE] **TASK-023**: Create pattern endpoints
  - GET /api/patterns (list with pagination)
  - GET /api/patterns/{id} (single pattern)
  - GET /api/patterns/search (search with filters)
  - Spec: `specs/backend/07-pattern-endpoints.md`

- [DONE] **TASK-024**: Implement pattern filtering
  - Filter by difficulty, category, materials
  - Sort by popularity, date, difficulty
  - Spec: `specs/backend/08-pattern-filtering.md`

- [DONE] **TASK-025**: Write pattern endpoint tests
  - Test pagination
  - Test filtering
  - Test permissions (free vs premium)
  - Spec: `specs/backend/07-pattern-endpoints.md`

### 5.2 Pattern Frontend

- [DONE] **TASK-026**: Create pattern list page
  - /patterns route
  - Grid layout (responsive)
  - Filter sidebar
  - Pagination controls
  - Spec: `specs/frontend/05-pattern-list.md`

- [ ] **TASK-027**: Create pattern detail page
  - /patterns/[id] route
  - Pattern display (materials, instructions, difficulty)
  - "Save to Projects" button
  - Premium lock for free users
  - Spec: `specs/frontend/06-pattern-detail.md`

- [ ] **TASK-028**: Create pattern card component
  - Reusable pattern card
  - Image, title, difficulty badge
  - Premium indicator
  - Spec: `specs/frontend/07-pattern-card.md`

---

## 🤖 PHASE 6: AI PATTERN GENERATOR (KILLER FEATURE)

### 6.1 Image Upload & Storage

- [ ] **TASK-029**: Setup Azure Blob Storage
  - BlobServiceClient configuration
  - Container creation
  - SAS token generation
  - Spec: `specs/backend/09-blob-storage.md`

- [ ] **TASK-030**: Create image upload endpoint
  - POST /api/images/upload
  - File validation (size, type, magic bytes)
  - Return temporary URL
  - Spec: `specs/backend/10-image-upload.md`

- [ ] **TASK-031**: Write image upload tests
  - Test valid images
  - Test invalid file types
  - Test file size limits
  - Test malicious files
  - Spec: `specs/backend/10-image-upload.md`

### 6.2 AI Integration

- [ ] **TASK-032**: Create AI service interface
  - IClaudeVisionService
  - IPatternGeneratorService
  - Spec: `specs/ai/01-service-interfaces.md`

- [ ] **TASK-033**: Implement Claude Vision service
  - Anthropic SDK integration
  - Image analysis method
  - Error handling & retries
  - Spec: `specs/ai/02-claude-vision.md`

- [ ] **TASK-034**: Implement pattern generator service
  - Takes analysis → generates pattern text
  - Structured output (materials, instructions, notes)
  - Caching similar results
  - Spec: `specs/ai/03-pattern-generator.md`

- [ ] **TASK-035**: Create pattern generation endpoint
  - POST /api/ai/generate-pattern
  - Rate limiting (per user tier)
  - Usage tracking
  - Spec: `specs/backend/11-ai-endpoints.md`

- [ ] **TASK-036**: Write AI service tests
  - Mock Anthropic API responses
  - Test error handling
  - Test caching logic
  - Spec: `specs/ai/04-testing.md`

### 6.3 AI Frontend

- [ ] **TASK-037**: Create AI generator page
  - /generate route
  - Image upload UI (drag-drop)
  - Loading state with progress
  - Result display
  - Spec: `specs/frontend/08-ai-generator.md`

- [ ] **TASK-038**: Create pattern editor component
  - Display generated pattern
  - Inline editing (text only)
  - Save to projects
  - Export options (PDF, text)
  - Spec: `specs/frontend/09-pattern-editor.md`

---

## 💳 PHASE 7: SUBSCRIPTION & PAYMENTS

### 7.1 Stripe Integration

- [ ] **TASK-039**: Setup Stripe
  - Install Stripe.NET
  - Configure API keys
  - Create webhook endpoint
  - Spec: `specs/backend/12-stripe-setup.md`

- [ ] **TASK-040**: Create subscription service
  - Create/cancel subscription
  - Handle plan upgrades/downgrades
  - Sync with database
  - Spec: `specs/backend/13-subscription-service.md`

- [ ] **TASK-041**: Create checkout endpoint
  - POST /api/subscriptions/create-checkout
  - Handle Stripe Checkout Session
  - Spec: `specs/backend/14-checkout-endpoint.md`

- [ ] **TASK-042**: Create webhook handler
  - POST /api/webhooks/stripe
  - Handle subscription events
  - Update user tier in database
  - Spec: `specs/backend/15-stripe-webhooks.md`

- [ ] **TASK-043**: Write payment tests
  - Mock Stripe API
  - Test subscription creation
  - Test webhook handling
  - Spec: `specs/backend/16-payment-tests.md`

### 7.2 Subscription Frontend

- [ ] **TASK-044**: Create pricing page
  - /pricing route
  - Pricing cards (Free, Premium, Pro)
  - CTA buttons
  - Spec: `specs/frontend/10-pricing-page.md`

- [ ] **TASK-045**: Create checkout flow
  - Redirect to Stripe Checkout
  - Success/cancel pages
  - Update UI on successful payment
  - Spec: `specs/frontend/11-checkout-flow.md`

- [ ] **TASK-046**: Create account settings page
  - /settings route
  - Display current plan
  - Manage subscription (upgrade/cancel)
  - Billing history
  - Spec: `specs/frontend/12-account-settings.md`

---

## 🎯 PHASE 8: PROJECT TRACKING (BASIC)

### 8.1 Project Management Backend

- [ ] **TASK-047**: Create project endpoints
  - POST /api/projects (create)
  - GET /api/projects (list user's projects)
  - GET /api/projects/{id} (single)
  - PUT /api/projects/{id} (update progress)
  - DELETE /api/projects/{id}
  - Spec: `specs/backend/17-project-endpoints.md`

- [ ] **TASK-048**: Write project endpoint tests
  - Test CRUD operations
  - Test permissions
  - Spec: `specs/backend/17-project-endpoints.md`

### 8.2 Project Management Frontend

- [ ] **TASK-049**: Create projects page
  - /projects route
  - List of user's projects
  - Progress indicators
  - Spec: `specs/frontend/13-projects-page.md`

- [ ] **TASK-050**: Create project detail page
  - /projects/[id] route
  - Pattern reference
  - Progress tracker (checkboxes for rows/rounds)
  - Notes section
  - Spec: `specs/frontend/14-project-detail.md`

---

## 🎨 PHASE 9: UI/UX POLISH

### 9.1 Core Components

- [ ] **TASK-051**: Create design system
  - Colors, typography, spacing tokens
  - shadcn/ui theme configuration
  - Spec: `specs/frontend/15-design-system.md`

- [ ] **TASK-052**: Create layout components
  - Header with navigation
  - Footer
  - Sidebar (for filters)
  - Spec: `specs/frontend/16-layouts.md`

- [ ] **TASK-053**: Add loading states
  - Skeleton loaders
  - Spinner for async operations
  - Progress bars for uploads
  - Spec: `specs/frontend/17-loading-states.md`

- [ ] **TASK-054**: Add animations
  - Framer Motion setup
  - Page transitions
  - Hover effects
  - Micro-interactions
  - Spec: `specs/frontend/18-animations.md`

### 9.2 Responsive Design

- [ ] **TASK-055**: Mobile optimization
  - Test on mobile viewports
  - Touch-friendly buttons
  - Mobile menu
  - Spec: `specs/frontend/19-mobile-responsive.md`

- [ ] **TASK-056**: Tablet optimization
  - Test on tablet viewports
  - Adjusted layouts
  - Spec: `specs/frontend/19-mobile-responsive.md`

---

## 🧪 PHASE 10: TESTING & QUALITY

### 10.1 Backend Testing

- [ ] **TASK-057**: Increase unit test coverage
  - Target: 80%+ coverage
  - Test all services
  - Test all repositories
  - Spec: `specs/backend/18-unit-tests.md`

- [ ] **TASK-058**: Write integration tests
  - Test end-to-end flows
  - Test with real database (test container)
  - Spec: `specs/backend/19-integration-tests.md`

- [ ] **TASK-059**: Security audit tests
  - SQL injection tests
  - XSS tests
  - CSRF tests
  - Rate limiting tests
  - Spec: `specs/security/08-security-testing.md`

### 10.2 Frontend Testing

- [ ] **TASK-060**: Write component tests
  - Test all UI components
  - Test user interactions
  - Spec: `specs/frontend/20-component-tests.md`

- [ ] **TASK-061**: Write E2E tests (Playwright)
  - Test auth flow
  - Test pattern browsing
  - Test AI generation
  - Test subscription flow
  - Spec: `specs/frontend/21-e2e-tests.md`

---

## 🚀 PHASE 11: DEPLOYMENT

### 11.1 CI/CD Pipeline

- [ ] **TASK-062**: Setup GitHub Actions
  - Run tests on PR
  - Build Docker images
  - Deploy to staging on merge to main
  - Spec: `specs/infrastructure/03-ci-cd.md`

- [ ] **TASK-063**: Create deployment scripts
  - Deploy backend to Azure App Service
  - Deploy frontend to Vercel
  - Database migration script
  - Spec: `specs/infrastructure/04-deployment.md`

### 11.2 Monitoring & Logging

- [ ] **TASK-064**: Setup Serilog
  - Structured logging
  - Log to file + console
  - Log levels configuration
  - Spec: `specs/backend/20-logging.md`

- [ ] **TASK-065**: Create health check endpoint
  - GET /health
  - Check database, Redis, blob storage
  - Return status JSON
  - Spec: `specs/backend/21-health-checks.md`

- [ ] **TASK-066**: Setup Application Insights
  - Azure Application Insights integration
  - Track API performance
  - Track exceptions
  - Spec: `specs/infrastructure/05-monitoring.md`

---

## 📚 PHASE 12: DOCUMENTATION

### 12.1 API Documentation

- [ ] **TASK-067**: Setup Swagger/OpenAPI
  - Install Swashbuckle
  - Document all endpoints
  - Add examples
  - Spec: `specs/backend/22-swagger.md`

### 12.2 User Documentation

- [ ] **TASK-068**: Create README.md
  - Project overview
  - Setup instructions
  - Contribution guidelines
  - Spec: `specs/documentation/01-readme.md`

- [ ] **TASK-069**: Create user guide
  - How to use AI generator
  - How to browse patterns
  - How to manage projects
  - Spec: `specs/documentation/02-user-guide.md`

---

## ✅ FINAL VALIDATION

- [ ] **TASK-070**: End-to-end smoke test
  - Register new user
  - Browse patterns
  - Generate AI pattern
  - Subscribe to premium
  - Create project
  - All flows work without errors

- [ ] **TASK-071**: Security final check
  - Run security scanner
  - Check for vulnerable dependencies
  - Verify all security measures active

- [ ] **TASK-072**: Performance check
  - API response times < 200ms
  - Frontend loads < 2s
  - AI generation < 30s

- [ ] **TASK-073**: Browser compatibility
  - Test on Chrome, Firefox, Safari, Edge
  - Fix any UI issues

---

## 🎉 PROJECT COMPLETE

When all tasks above are `[DONE]` and all tests pass:

**OUTPUT**: `EXIT_SIGNAL: Crochet AI Complete`

---

**Current Status**: Starting TASK-001
**Last Updated**: [Ralph will update this]
