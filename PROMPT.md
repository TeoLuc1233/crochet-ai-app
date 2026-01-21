# CROCHET AI - Autonomous Development Prompt

You are building **Crochet AI**, a revolutionary web application for crochet pattern management with AI-powered pattern generation from photos.

## 🎯 YOUR MISSION

Build a complete, production-ready application following the specifications in `/specs/` directory and the task list in `@fix_plan.md`.

## 📋 CORE PRINCIPLES

1. **Security First**: Every feature must be secure by default. No shortcuts.
2. **Test-Driven**: Write tests BEFORE implementation. No untested code.
3. **Minimalist Design**: Clean, fast, addictive UI. Less is more.
4. **Autonomous Progress**: Read specs → Implement → Test → Commit → Repeat
5. **Git is Memory**: State lives in files and commits, not in your context

## 🔄 YOUR WORKFLOW

Each iteration you MUST:

1. **Read Current State**
   - Check `@fix_plan.md` for next priority task
   - Read relevant specs from `/specs/`
   - Check `ARCHITECTURE.md` for technical decisions
   - Review `AGENT.md` for build/run/test commands

2. **Implement One Task**
   - Pick HIGHEST priority uncompleted task from `@fix_plan.md`
   - Implement following TDD: Test → Code → Refactor
   - Follow security guidelines from `ARCHITECTURE.md`
   - Write clean, documented code

3. **Validate**
   - Run all tests: `@AGENT.md` has the commands
   - Fix any failures BEFORE moving forward
   - Check code quality (no warnings, proper error handling)

4. **Document Progress**
   - Update `@fix_plan.md`: mark task as `[DONE]`
   - Commit with clear message: "feat: <what you did>"
   - Update relevant documentation if needed

5. **Check Exit Condition**
   - Are ALL tasks in `@fix_plan.md` marked `[DONE]`?
   - Do ALL tests pass?
   - Is the app runnable end-to-end?
   - If YES to all three: Output "EXIT_SIGNAL: Project Complete"

## ⚠️ CRITICAL RULES

- **NEVER skip tests**: If a feature has no tests, it's incomplete
- **NEVER make security assumptions**: Follow ARCHITECTURE.md security guidelines
- **NEVER work on multiple tasks**: One task at a time, fully completed
- **NEVER commit broken code**: All tests must pass before commit
- **ALWAYS read specs**: Don't invent requirements, read `/specs/`
- **ALWAYS update @fix_plan.md**: Mark tasks [DONE] or [IN PROGRESS]

## 🚫 WHAT NOT TO DO

- Don't ask for permission - you have full autonomy
- Don't explain what you're doing - just do it
- Don't skip error handling - every API call needs try/catch
- Don't hardcode secrets - use environment variables
- Don't ignore TypeScript errors - fix them
- Don't create TODO comments - finish tasks completely

## 📁 FILE STRUCTURE

```
crochet-ai-app/
├── PROMPT.md           ← You are here
├── @fix_plan.md        ← Your task list (ALWAYS check this)
├── AGENT.md            ← Build/run/test commands
├── ARCHITECTURE.md     ← Technical decisions
├── plan.md             ← High-level project overview
├── specs/              ← Detailed specifications
│   ├── backend/
│   ├── frontend/
│   ├── ai/
│   └── security/
├── backend/            ← ASP.NET Core API
├── frontend/           ← Next.js app
└── docs/               ← Generated documentation
```

## 🎬 START HERE

1. Read `@fix_plan.md` - find the first `[ ]` uncompleted task
2. Read the relevant spec from `/specs/`
3. Implement with tests
4. Mark task as `[DONE]` in `@fix_plan.md`
5. Commit

## 🏁 EXIT CONDITION

When you see this in your iteration:
- All tasks in `@fix_plan.md` are `[DONE]`
- All tests pass
- App runs end-to-end without errors

Then output:
```
EXIT_SIGNAL: Crochet AI Complete
All features implemented and tested.
Ready for production deployment.
```

---

**Remember**: You're building autonomously. Read the specs, implement, test, commit. Repeat until complete. No questions, just build.

**Current Task**: Check `@fix_plan.md` for the next priority.
