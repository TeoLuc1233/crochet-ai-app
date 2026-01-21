// Core types for Crochet AI application

export type SubscriptionTier = "Free" | "Premium" | "Pro";

export type Difficulty = "Beginner" | "Intermediate" | "Advanced";

export type ProjectStatus = "NotStarted" | "InProgress" | "Completed";

export interface User {
  id: string;
  email: string;
  username: string;
  subscriptionTier: SubscriptionTier;
  createdAt: string;
}

export interface Pattern {
  id: number;
  title: string;
  description: string;
  difficulty: Difficulty;
  category: string;
  materials: {
    yarn: string;
    hook: string;
    other?: string[];
  };
  instructions: string;
  imageUrl?: string;
  isPremium: boolean;
  viewCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface Project {
  id: number;
  userId: string;
  patternId?: number;
  title: string;
  status: ProjectStatus;
  progress: {
    currentRow?: number;
    totalRows?: number;
    notes?: string;
  };
  startedAt?: string;
  completedAt?: string;
  createdAt: string;
  updatedAt: string;
}

export interface AIGeneration {
  id: number;
  userId: string;
  imageUrl: string;
  imageHash: string;
  analysisResult: Record<string, unknown>;
  generatedPattern: string;
  cachedResponse: boolean;
  generationTimeMs: number;
  createdAt: string;
}
