# 🎯 CROCHET AI - RALPH SETUP GUIDE

## 📚 TABLE OF CONTENTS

1. [What is Ralph?](#what-is-ralph)
2. [What You Have](#what-you-have)
3. [Installation Options](#installation-options)
4. [Setup Instructions](#setup-instructions)
5. [Running Ralph](#running-ralph)
6. [Monitoring Progress](#monitoring-progress)
7. [Troubleshooting](#troubleshooting)
8. [FAQ](#faq)

---

## 🤖 WHAT IS RALPH?

**Ralph** (Ralph Wiggum Loop) is an **autonomous AI development technique**, not a framework or tool you install.

### How It Works:

```
1. You give Claude Code a detailed prompt (PROMPT.md)
2. Claude Code reads the prompt and specs
3. Claude Code writes code, runs tests, commits to git
4. The prompt repeats → Claude Code reads git history + specs again
5. Continue until all tasks are done
```

**Key Insight**: The "loop" is just calling Claude Code repeatedly with the same prompt. Progress is saved in **files and git commits**, not in Claude's memory.

### Why It's Effective:

- Fresh context every iteration (no context pollution)
- Git history = memory
- Test-driven (catches errors early)
- Autonomous (works while you sleep)

---

## 📦 WHAT YOU HAVE

I've created a complete **autonomous development package** for your Crochet AI app:

```
crochet-ai-app/
├── PROMPT.md                  ← Main prompt Ralph/Claude Code will use
├── @fix_plan.md              ← 73 prioritized tasks (Ralph's checklist)
├── AGENT.md                  ← Build/run/test commands
├── ARCHITECTURE.md           ← All technical decisions already made
├── plan.md                   ← High-level project overview
├── specs/                    ← Detailed specifications
│   ├── backend/
│   │   ├── 06-auth-endpoints.md (example)
│   │   └── ... (you'll create more as needed)
│   ├── frontend/
│   ├── ai/
│   │   └── 02-claude-vision.md (example)
│   ├── security/
│   ├── infrastructure/
│   └── documentation/
└── README.md                 ← This file
```

**What's Special**:
- All architectural decisions are ALREADY MADE (security-first stack)
- 73 tasks broken down in detail
- Example specs show the level of detail needed
- Claude Code can work autonomously with this

---

## 💻 INSTALLATION OPTIONS

You have **3 options** for running Ralph:

### Option 1: Manual Ralph (Simplest) ✅ RECOMMENDED FOR YOU

**What it is**: You manually call Claude Code repeatedly  
**Pros**: No installation, you control everything  
**Cons**: You have to trigger each iteration manually

**How it works**:
```bash
# Iteration 1
claude-code "Read PROMPT.md and start on the first task in @fix_plan.md"

# Wait for it to finish, then...

# Iteration 2
claude-code "Read PROMPT.md and continue with the next task"

# Repeat until all tasks done
```

### Option 2: Ralph Script (Automated)

**What it is**: A bash script that loops Claude Code automatically  
**Pros**: Fully autonomous  
**Cons**: Need to install the script

**Installation**:
```bash
# Clone Ralph implementation
git clone https://github.com/frankbria/ralph-claude-code.git
cd ralph-claude-code
chmod +x ralph.sh

# Run in your project
cd /path/to/crochet-ai-app
/path/to/ralph.sh
```

### Option 3: Ralph Framework (Most Advanced)

**What it is**: Full Ralph orchestrator with subagents  
**Pros**: Parallel work, specialized agents, monitoring  
**Cons**: More complex setup

**Installation**: See https://github.com/DG1001/ralph-framework

---

## 🚀 SETUP INSTRUCTIONS

### Step 1: Get Claude Code

**Option A: Claude Code CLI (Recommended)**

1. Go to https://claude.ai
2. Navigate to Settings → Developer
3. Install Claude Code CLI: `npm install -g @anthropic-ai/claude-code`
4. Authenticate: `claude-code auth`

**Option B: Use Claude Code in VS Code**

1. Install VS Code extension: "Claude Code"
2. Authenticate with your Anthropic account

**Important**: Ensure you have a **Claude Pro or API subscription** - Ralph is token-intensive!

### Step 2: Initialize Your Project

```bash
# Create a new directory
mkdir crochet-ai-project
cd crochet-ai-project

# Initialize git (CRITICAL - Ralph uses git as memory)
git init
git config user.name "Your Name"
git config user.email "your@email.com"

# Copy all the files I created into this directory
# (PROMPT.md, @fix_plan.md, AGENT.md, ARCHITECTURE.md, plan.md, specs/)

# Make initial commit
git add .
git commit -m "Initial commit: Ralph setup"
```

### Step 3: Install Prerequisites

**On your machine, install:**

```bash
# .NET 8 SDK
# Download from: https://dotnet.microsoft.com/download/dotnet/8.0

# Node.js 18+
# Download from: https://nodejs.org/

# PostgreSQL 16
# Download from: https://www.postgresql.org/download/
# OR use Docker: docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=postgres postgres:16

# Redis
# Download from: https://redis.io/download
# OR use Docker: docker run -d -p 6379:6379 redis:7

# Docker (optional but recommended)
# Download from: https://www.docker.com/products/docker-desktop
```

### Step 4: Configure Environment

Create a `.env` file in project root:

```bash
# .env (DO NOT COMMIT THIS FILE)
ANTHROPIC_API_KEY=sk-ant-your-key-here
OPENAI_API_KEY=sk-your-openai-key-here (optional, for fallback)
DATABASE_URL=postgresql://postgres:postgres@localhost:5432/crochetai
REDIS_URL=localhost:6379
JWT_SECRET=your-super-secret-key-here-minimum-32-chars
AZURE_STORAGE_CONNECTION_STRING=your-azure-storage-here
STRIPE_SECRET_KEY=sk_test_your-stripe-key
```

Add to `.gitignore`:
```bash
echo ".env" >> .gitignore
echo "*.env" >> .gitignore
```

---

## 🎬 RUNNING RALPH

### Manual Mode (Recommended for First Time)

```bash
# Start iteration 1
claude-code "Read PROMPT.md and execute the first uncompleted task from @fix_plan.md. Follow all instructions exactly."

# Claude Code will:
# 1. Read PROMPT.md
# 2. Check @fix_plan.md for next task (TASK-001)
# 3. Read relevant specs
# 4. Implement the task
# 5. Write tests
# 6. Run tests
# 7. Mark task [DONE] in @fix_plan.md
# 8. Git commit

# When it's done, review the changes:
git log -1
git diff HEAD~1

# If good, continue:
claude-code "Read PROMPT.md and continue with the next task"

# Repeat until EXIT_SIGNAL appears
```

### Automated Mode (Bash Loop)

Create `ralph.sh`:

```bash
#!/bin/bash

MAX_ITERATIONS=73  # One per task
ITERATION=0

while [ $ITERATION -lt $MAX_ITERATIONS ]; do
    echo "=== ITERATION $((ITERATION + 1)) / $MAX_ITERATIONS ==="
    
    # Run Claude Code
    claude-code "Read PROMPT.md and execute the next task from @fix_plan.md"
    
    # Check for exit signal
    if git log -1 --pretty=%B | grep -q "EXIT_SIGNAL"; then
        echo "✅ PROJECT COMPLETE!"
        exit 0
    fi
    
    # Check if @fix_plan.md still has uncompleted tasks
    if ! grep -q "\[ \]" @fix_plan.md; then
        echo "✅ ALL TASKS COMPLETE!"
        exit 0
    fi
    
    ITERATION=$((ITERATION + 1))
    
    # Small delay to avoid rate limits
    sleep 5
done

echo "⚠️  Reached max iterations. Check @fix_plan.md for remaining tasks."
```

Run it:
```bash
chmod +x ralph.sh
./ralph.sh
```

---

## 📊 MONITORING PROGRESS

### Check Current Task

```bash
# See which task Ralph is working on
grep -n "\[IN PROGRESS\]" @fix_plan.md

# See remaining tasks
grep -c "\[ \]" @fix_plan.md
```

### View Recent Changes

```bash
# Last 5 commits
git log --oneline -5

# See what files changed
git diff HEAD~1 --stat

# See actual code changes
git diff HEAD~1
```

### Check If Tests Pass

```bash
# Backend tests
cd backend && dotnet test

# Frontend tests
cd frontend && npm test

# Integration tests
./scripts/run-integration-tests.sh
```

### Monitor API Costs

Ralph will use Claude Code which consumes Anthropic API credits.

**Estimated costs**:
- 73 tasks × ~$0.50-2 per task = **$40-150 total**
- Depends heavily on task complexity
- Can be reduced by using Claude Haiku for simple tasks

**To monitor**:
1. Check your Anthropic usage: https://console.anthropic.com/usage
2. Set up billing alerts
3. Ralph logs costs in git commits

---

## 🛠️ TROUBLESHOOTING

### Issue: Claude Code Gets Stuck

**Symptoms**: Same task attempted multiple times, no progress

**Solution**:
```bash
# Check what's failing
git log -3 --oneline

# Read the error
cd backend && dotnet test
# OR
cd frontend && npm test

# Fix manually or guide Claude:
claude-code "The tests are failing because X. Please fix by doing Y."
```

### Issue: Tests Keep Failing

**Symptoms**: Task marked [DONE] but tests fail

**Solution**:
```bash
# Mark task back to [ ] in @fix_plan.md
# Manually edit @fix_plan.md:
# Change: [DONE] TASK-010: Setup ASP.NET Identity
# To:     [ ] TASK-010: Setup ASP.NET Identity

git add @fix_plan.md
git commit -m "Reset TASK-010 due to test failures"

# Re-run Ralph
claude-code "Read PROMPT.md and fix TASK-010 tests"
```

### Issue: Ralph Ignores Specs

**Symptoms**: Implementation doesn't match spec

**Solution**:
```bash
# Be more explicit in prompt:
claude-code "Read PROMPT.md, then read specs/backend/06-auth-endpoints.md CAREFULLY and implement it EXACTLY as specified. Do not deviate."
```

### Issue: Dependencies Not Installed

**Symptoms**: Build fails with "package not found"

**Solution**:
```bash
# Backend
cd backend && dotnet restore

# Frontend
cd frontend && rm -rf node_modules && npm install

# Then continue Ralph
```

### Issue: Rate Limited by API

**Symptoms**: "Rate limit exceeded" error

**Solution**:
- Wait 1 hour
- Upgrade to higher API tier
- Add delays between iterations (`sleep 60` in ralph.sh)

---

## ❓ FAQ

### Q: Do I need Claude Pro?

**A**: Yes, or API access. Ralph is token-intensive. Free tier won't work.

### Q: Can Ralph really build this entire app alone?

**A**: 70-80% yes, 20-30% you'll need to intervene. It's excellent at:
- Boilerplate
- Standard patterns
- Following specs exactly

It struggles with:
- Novel algorithms
- Complex business logic
- Debugging weird edge cases

**Expect to spend 20-30 hours guiding/fixing**, but Ralph does the heavy lifting.

### Q: What if I want to change the tech stack?

**A**: Edit `ARCHITECTURE.md` before starting. For example, replace Next.js with SvelteKit. Ralph will use the new stack. BUT: make the decision BEFORE iteration 1, not halfway through.

### Q: How do I add more specs?

**A**: Copy the example specs (`specs/backend/06-auth-endpoints.md`, `specs/ai/02-claude-vision.md`) and create new ones following the same structure. Ralph will read them when it gets to that task.

### Q: Can I use Ralph with Visual Studio Code instead of CLI?

**A**: Yes! Use the Claude Code VS Code extension. Instead of running `claude-code "..."` in terminal, you:
1. Open PROMPT.md in VS Code
2. Select all text
3. Send to Claude Code via extension
4. Claude Code works in your workspace

**Downside**: Less automated, more manual, but same concept.

### Q: What if Ralph builds something I don't like?

**A**: 
```bash
# Revert last commit
git revert HEAD

# Or reset to previous state
git reset --hard HEAD~1

# Then guide Claude explicitly
claude-code "Redo TASK-X but this time use approach Y instead of Z"
```

### Q: How long will this take?

**A**: 
- **Optimistic**: 3-5 days (if Ralph runs 24/7 smoothly)
- **Realistic**: 1-2 weeks (with interventions and fixes)
- **Pessimistic**: 3-4 weeks (if lots of debugging needed)

Remember: 73 tasks × 30 mins each = 36 hours of work. But you're not doing it manually!

### Q: What should I do while Ralph works?

**A**:
- Monitor progress every few hours
- Review git commits
- Test the app manually
- Fix things that break
- **Sleep!** Ralph works overnight

---

## 🎯 NEXT STEPS FOR YOU

### Right Now:

1. ✅ Read this entire README
2. ✅ Install prerequisites (Step 3)
3. ✅ Set up environment variables (Step 4)
4. ✅ Run first iteration manually to see how it works

### This Week:

1. Create remaining spec files (follow the examples)
2. Let Ralph run through first 10-15 tasks
3. Review and test
4. Fix any issues
5. Continue

### This Month:

1. Complete all 73 tasks
2. Deploy MVP
3. Test with real users
4. Iterate based on feedback

---

## 📞 SUPPORT

**If you get stuck:**

1. Check git log to see what Ralph did
2. Read error messages carefully
3. Check the specific spec file for that task
4. Ask Claude (me!) for help with specific errors

**Example**:
> "Ralph tried to implement TASK-023 (pattern endpoints) but got error: 'PatternRepository not found'. What should I do?"

---

## 🎉 GOOD LUCK!

You have everything you need. Ralph will do the heavy lifting, but YOU are the project manager. Guide it, review its work, and make decisions.

**Remember**: This is autonomous AI development, not magic. Expect to be involved, but expect to save 70%+ of coding time.

**Start small**: Run 1-2 iterations manually first to get comfortable, then automate if you want.

---

**Ready? Let's build this app! 🚀**

```bash
git add .
git commit -m "Ralph setup complete, ready to start"
claude-code "Read PROMPT.md and begin with TASK-001"
```
