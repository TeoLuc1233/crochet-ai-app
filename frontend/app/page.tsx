export default function Home() {
  return (
    <div className="min-h-screen flex flex-col items-center justify-center p-8">
      <main className="flex flex-col gap-8 items-center text-center max-w-2xl">
        <h1 className="text-4xl font-bold">Crochet AI</h1>
        <p className="text-lg text-muted-foreground">
          Generate crochet patterns from photos using AI. Browse premium patterns and manage your projects.
        </p>
        <div className="flex gap-4">
          <a
            href="/patterns"
            className="rounded-md bg-primary text-primary-foreground px-4 py-2 hover:bg-primary/90 transition-colors"
          >
            Browse Patterns
          </a>
          <a
            href="/generate"
            className="rounded-md border border-input bg-background px-4 py-2 hover:bg-accent transition-colors"
          >
            Generate Pattern
          </a>
        </div>
      </main>
    </div>
  );
}
