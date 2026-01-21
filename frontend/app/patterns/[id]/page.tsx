'use client';

import { useState, useEffect } from 'react';
import { useParams } from 'next/navigation';
import { apiClient } from '@/lib/api/client';
import { useAuth } from '@/contexts/AuthContext';
import { ProtectedRoute } from '@/components/ProtectedRoute';

interface Pattern {
  id: number;
  title: string;
  description: string;
  difficulty: string;
  category: string;
  materials: string[];
  instructions: string;
  imageUrl?: string;
  isPremium: boolean;
  createdAt: string;
  updatedAt: string;
}

export default function PatternDetailPage() {
  const params = useParams();
  const { user } = useAuth();
  const [pattern, setPattern] = useState<Pattern | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (params.id) {
      loadPattern();
    }
  }, [params.id]);

  const loadPattern = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await apiClient.get<Pattern>(`/api/patterns/${params.id}`, { requireAuth: false });
      setPattern(response);
    } catch (err: any) {
      if (err.message?.includes('403') || err.message?.includes('Premium')) {
        setError('This pattern requires a premium subscription.');
      } else {
        setError('Failed to load pattern.');
      }
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center">
          <div className="inline-block h-8 w-8 animate-spin rounded-full border-4 border-solid border-indigo-600 border-r-transparent"></div>
          <p className="mt-4 text-gray-600 dark:text-gray-400">Loading pattern...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="bg-red-50 dark:bg-red-900 p-6 rounded-lg">
          <h2 className="text-xl font-semibold text-red-800 dark:text-red-200 mb-2">Error</h2>
          <p className="text-red-600 dark:text-red-300">{error}</p>
          {error.includes('premium') && (
            <a
              href="/pricing"
              className="mt-4 inline-block bg-indigo-600 text-white px-4 py-2 rounded hover:bg-indigo-700"
            >
              Upgrade to Premium
            </a>
          )}
        </div>
      </div>
    );
  }

  if (!pattern) {
    return (
      <div className="container mx-auto px-4 py-8">
        <p className="text-gray-600 dark:text-gray-400">Pattern not found.</p>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="max-w-4xl mx-auto">
        {pattern.imageUrl && (
          <img
            src={pattern.imageUrl}
            alt={pattern.title}
            className="w-full h-96 object-cover rounded-lg mb-6"
          />
        )}

        <div className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow">
          <div className="flex items-center justify-between mb-4">
            <h1 className="text-3xl font-bold text-black dark:text-white">{pattern.title}</h1>
            {pattern.isPremium && (
              <span className="bg-yellow-500 text-white text-xs px-3 py-1 rounded">
                Premium
              </span>
            )}
          </div>

          <div className="flex items-center gap-4 mb-4">
            <span className="bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200 px-3 py-1 rounded">
              {pattern.difficulty}
            </span>
            <span className="text-gray-600 dark:text-gray-400">{pattern.category}</span>
          </div>

          {pattern.description && (
            <p className="text-gray-700 dark:text-gray-300 mb-6">{pattern.description}</p>
          )}

          <div className="mb-6">
            <h2 className="text-xl font-semibold mb-3 text-black dark:text-white">Materials</h2>
            <ul className="list-disc list-inside space-y-1 text-gray-700 dark:text-gray-300">
              {pattern.materials.map((material, index) => (
                <li key={index}>{material}</li>
              ))}
            </ul>
          </div>

          <div className="mb-6">
            <h2 className="text-xl font-semibold mb-3 text-black dark:text-white">Instructions</h2>
            <div className="prose dark:prose-invert max-w-none">
              <pre className="whitespace-pre-wrap text-gray-700 dark:text-gray-300 bg-gray-50 dark:bg-gray-900 p-4 rounded">
                {pattern.instructions}
              </pre>
            </div>
          </div>

          {user && (
            <div className="mt-6">
              <button className="bg-indigo-600 text-white px-6 py-2 rounded hover:bg-indigo-700">
                Save to Projects
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
