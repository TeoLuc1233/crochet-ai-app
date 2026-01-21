# Crochet AI - Project Plan

## 🎯 Project Vision

**Crochet AI** is a revolutionary web application that combines traditional crochet pattern management with cutting-edge AI technology. The app allows users to browse premium patterns, manage their projects, and—uniquely—generate custom patterns from photos using computer vision AI.

## 💡 Core Value Propositions

1. **AI Pattern Generation**: Upload a photo of any crochet work → Get a detailed, written pattern (UNIQUE in the market)
2. **Premium Pattern Library**: Curated collection of high-quality, licensed patterns
3. **Project Management**: Track progress, materials, difficulty levels
4. **Minimalist, Addictive UX**: Fast, beautiful, makes users want to come back
5. **Security-First**: Bank-level security for user data and payments

## 🎭 Target Users

- **Primary**: Crochet enthusiasts (intermediate to advanced) aged 25-55
- **Secondary**: Designers wanting to reverse-engineer inspiration
- **Tertiary**: Beginners looking for guided learning

## 💰 Business Model

### Revenue Streams

1. **Freemium Subscription**
   - Free: Browse 5 patterns, 1 AI generation/month
   - Premium: €9.99/month - Unlimited patterns, 20 AI generations/month
   - Pro: €19.99/month - Everything + priority AI, export features

2. **Pattern Marketplace** (Phase 2)
   - Individual pattern purchases: €3-15 each
   - Revenue split: 70% designer, 30% platform

3. **Ad Revenue** (Free tier only)
   - Non-intrusive display ads
   - Target: €0.10 RPM

### Cost Structure

**Initial (Year 1):**
- Development: Your time (free)
- AI API: ~€50-200/month (scales with users)
- Hosting: ~€20/month (Azure/AWS free tier → paid)
- Apple Developer: $99/year
- Google Play: $25 one-time
- Pattern licenses: €500-1000 upfront
- **Total Year 1**: ~€1,500-2,500

**Break-even**: ~50 premium subscribers or 150 free users with ads

## 🏗️ Technical Architecture

### Stack

**Backend:**
- ASP.NET Core 8 (Web API)
- PostgreSQL 16 (primary database)
- Redis (caching, rate limiting)
- Azure Blob Storage (images)
- Entity Framework Core

**Frontend:**
- Next.js 14 (React 18)
- TypeScript
- TailwindCSS + shadcn/ui
- Framer Motion (animations)
- PWA (installable)

**AI:**
- Anthropic Claude Sonnet 4 (vision + text)
- OpenAI GPT-4o (fallback)
- Custom caching layer

**Security:**
- ASP.NET Identity + JWT
- Rate limiting (Redis-based)
- HTTPS/HSTS/CSP
- Input validation (FluentValidation)
- Audit logging

### Infrastructure

**Development:**
- Docker Compose (local dev)
- GitHub Actions (CI/CD)

**Production (Phase 1):**
- Azure App Service (backend)
- Vercel (frontend)
- Azure Database for PostgreSQL
- Azure Redis Cache
- Azure Blob Storage

## 📋 Development Phases

### Phase 1: MVP (6-8 weeks) ← CURRENT FOCUS

**Core Features:**
1. User authentication (email/password)
2. Pattern browsing (20-30 premium patterns)
3. AI pattern generator (photo → text pattern)
4. Basic project tracking
5. Subscription payments (Stripe)
6. Responsive web app

**Success Criteria:**
- End-to-end working app
- AI generates 70%+ accurate patterns
- Load time < 2s
- Mobile responsive
- All security measures in place

### Phase 2: Marketplace (2-3 months)

**Additional Features:**
1. Designer dashboard
2. Pattern upload/sale system
3. Review/rating system
4. Community features
5. Pattern search/filters

### Phase 3: Mobile Apps (3-4 months)

**Features:**
1. Native iOS app
2. Native Android app
3. Offline pattern viewing
4. Screenshot protection
5. Push notifications

### Phase 4: Advanced AI (Ongoing)

**Enhancements:**
1. Diagram generation (SVG)
2. Video tutorial integration
3. AR preview (see pattern on real objects)
4. Custom AI models (fine-tuned)

## 📊 Success Metrics

### Technical KPIs
- Uptime: >99.5%
- API response time: <200ms (p95)
- AI generation time: <30s
- Page load: <2s
- Test coverage: >80%

### Business KPIs
- User registrations: 500 in first 3 months
- Conversion to premium: 10-15%
- AI pattern accuracy (user-rated): >70%
- Churn rate: <5% monthly
- NPS score: >50

### User Experience KPIs
- Session duration: >5 min average
- Return rate: >40% weekly
- Pattern completion rate: >60%
- AI feature usage: >30% of users

## 🎨 Design Philosophy

### Visual Identity
- **Colors**: Warm pastels (pink, mint, cream) + black accents
- **Typography**: Modern sans-serif (Inter/Geist)
- **Imagery**: High-quality crochet photography
- **Animations**: Subtle, delightful (Framer Motion)

### UX Principles
1. **Speed**: Everything loads instantly
2. **Simplicity**: One primary action per screen
3. **Delight**: Micro-interactions that surprise
4. **Trust**: Clear communication, no hidden costs
5. **Accessibility**: WCAG 2.1 AA compliant

## 🚧 Known Challenges

### Technical
1. **AI accuracy**: Pattern generation won't be perfect initially
   - *Mitigation*: Clear disclaimers, user editing features
2. **Cost scaling**: AI API costs grow with users
   - *Mitigation*: Aggressive caching, rate limits, tiered pricing
3. **Image quality**: User photos vary widely
   - *Mitigation*: Pre-upload validation, quality guidelines

### Business
1. **Pattern licensing**: Acquiring quality content upfront
   - *Mitigation*: Start with 20-30 patterns, add monthly
2. **User acquisition**: Niche market
   - *Mitigation*: Instagram/Pinterest marketing, influencer partnerships
3. **Competition**: Ravelry dominates
   - *Mitigation*: AI feature is unique, focus on that USP

### Legal
1. **Copyright**: User-uploaded photos might contain copyrighted work
   - *Mitigation*: Clear ToS, DMCA compliance, watermarking
2. **Pattern accuracy**: AI might generate incorrect patterns
   - *Mitigation*: Legal disclaimers, "community verification" system

## 🎯 Immediate Next Steps

Ralph will execute these in order (see `@fix_plan.md`):

1. ✅ Set up project structure
2. ✅ Backend: Database schema + models
3. ✅ Backend: Authentication system
4. ✅ Backend: AI integration (Claude Vision API)
5. ✅ Frontend: Core UI components
6. ✅ Frontend: Authentication flows
7. ✅ Integration: Pattern generator flow
8. ✅ Testing: Unit + integration tests
9. ✅ Security: Hardening + auditing
10. ✅ Deployment: CI/CD pipeline

## 📚 Resources

### Documentation
- ASP.NET Core: https://docs.microsoft.com/aspnet/core
- Next.js: https://nextjs.org/docs
- Claude API: https://docs.anthropic.com
- Stripe: https://stripe.com/docs

### Design Inspiration
- Dribbble: "crochet app", "craft app"
- Behance: Minimalist mobile apps
- Pinterest: Modern pattern layouts

### Market Research
- Ravelry: Features, UX, community
- LoveCrafts: E-commerce flow
- Amigurumi Today: Content strategy

## 🏁 Definition of Done (MVP)

The MVP is complete when:

- [ ] User can sign up, log in, log out
- [ ] User can browse 20+ premium patterns
- [ ] User can upload photo → receive AI-generated pattern
- [ ] User can subscribe via Stripe
- [ ] Free tier has AI usage limits enforced
- [ ] App is responsive on mobile/tablet/desktop
- [ ] All security measures are in place
- [ ] 80%+ test coverage
- [ ] App deployed and accessible via URL
- [ ] Health monitoring active

**Target Launch Date**: 8 weeks from start

---

**Philosophy**: Ship fast, iterate based on real user feedback. The AI feature is the differentiator—make it amazing, even if other features are basic v1.
