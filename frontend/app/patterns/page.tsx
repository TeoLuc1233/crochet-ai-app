'use client';

import { useState, useEffect } from 'react';
import { apiClient } from '@/lib/api/client';

interface Pattern {
  id: number;
  title: string;
  description: string;
  difficulty: string;
  category: string;
  imageUrl?: string;
  isPremium: boolean;
}

interface PatternListResponse {
  patterns: Pattern[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export default function PatternsPage() {
  const [patterns, setPatterns] = useState<Pattern[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [filters, setFilters] = useState({
    difficulty: '',
    category: '',
    material: '',
    sortBy: 'createdAt',
    sortOrder: 'desc'
  });

  useEffect(() => {
    loadPatterns();
  }, [page, filters]);

  const loadPatterns = async () => {
    setLoading(true);
    try {
      const params = new URLSearchParams({
        page: page.toString(),
        pageSize: '20',
        ...(filters.difficulty && { difficulty: filters.difficulty }),
        ...(filters.category && { category: filters.category }),
        ...(filters.material && { material: filters.material }),
        sortBy: filters.sortBy,
        sortOrder: filters.sortOrder
      });

      const response = await apiClient.get<PatternListResponse>(`/api/patterns?${params}`, { requireAuth: false });
      setPatterns(response.patterns);
      setTotalPages(response.totalPages);
    } catch (error) {
      console.error('Failed to load patterns:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-black dark:text-white mb-8">Pattern Library</h1>

      <div className="flex flex-col lg:flex-row gap-8">
        {/* Filter Sidebar */}
        <aside className="w-full lg:w-64">
          <div className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow">
            <h2 className="text-xl font-semibold mb-4 text-black dark:text-white">Filters</h2>

            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Difficulty
                </label>
                <select
                  value={filters.difficulty}
                  onChange={(e) => setFilters({ ...filters, difficulty: e.target.value })}
                  className="w-full rounded-md border border-gray-300 px-3 py-2 dark:border-gray-600 dark:bg-gray-700 dark:text-white"
                >
                  <option value="">All</option>
                  <option value="Beginner">Beginner</option>
                  <option value="Intermediate">Intermediate</option>
                  <option value="Advanced">Advanced</option>
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Category
                </label>
                <select
                  value={filters.category}
                  onChange={(e) => setFilters({ ...filters, category: e.target.value })}
                  className="w-full rounded-md border border-gray-300 px-3 py-2 dark:border-gray-600 dark:bg-gray-700 dark:text-white"
                >
                  <option value="">All</option>
                  <option value="Amigurumi">Amigurumi</option>
                  <option value="Clothing">Clothing</option>
                  <option value="Home">Home</option>
                  <option value="Accessories">Accessories</option>
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                  Sort By
                </label>
                <select
                  value={filters.sortBy}
                  onChange={(e) => setFilters({ ...filters, sortBy: e.target.value })}
                  className="w-full rounded-md border border-gray-300 px-3 py-2 dark:border-gray-600 dark:bg-gray-700 dark:text-white"
                >
                  <option value="createdAt">Date</option>
                  <option value="popularity">Popularity</option>
                  <option value="difficulty">Difficulty</option>
                  <option value="title">Title</option>
                </select>
              </div>
            </div>
          </div>
        </aside>

        {/* Pattern Grid */}
        <main className="flex-1">
          {loading ? (
            <div className="text-center py-12">
              <div className="inline-block h-8 w-8 animate-spin rounded-full border-4 border-solid border-indigo-600 border-r-transparent"></div>
              <p className="mt-4 text-gray-600 dark:text-gray-400">Loading patterns...</p>
            </div>
          ) : patterns.length === 0 ? (
            <div className="text-center py-12">
              <p className="text-gray-600 dark:text-gray-400">No patterns found.</p>
            </div>
          ) : (
            <>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {patterns.map((pattern) => (
                  <div
                    key={pattern.id}
                    className="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden hover:shadow-lg transition-shadow"
                  >
                    {pattern.imageUrl && (
                      <img
                        src={pattern.imageUrl}
                        alt={pattern.title}
                        className="w-full h-48 object-cover"
                      />
                    )}
                    <div className="p-4">
                      <div className="flex items-center justify-between mb-2">
                        <h3 className="text-lg font-semibold text-black dark:text-white">
                          {pattern.title}
                        </h3>
                        {pattern.isPremium && (
                          <span className="bg-yellow-500 text-white text-xs px-2 py-1 rounded">
                            Premium
                          </span>
                        )}
                      </div>
                      <p className="text-sm text-gray-600 dark:text-gray-400 mb-2 line-clamp-2">
                        {pattern.description}
                      </p>
                      <div className="flex items-center gap-2 text-sm">
                        <span className="bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200 px-2 py-1 rounded">
                          {pattern.difficulty}
                        </span>
                        <span className="text-gray-500 dark:text-gray-400">{pattern.category}</span>
                      </div>
                      <a
                        href={`/patterns/${pattern.id}`}
                        className="mt-4 inline-block text-indigo-600 hover:text-indigo-700 dark:text-indigo-400 font-medium"
                      >
                        View Pattern →
                      </a>
                    </div>
                  </div>
                ))}
              </div>

              {/* Pagination */}
              {totalPages > 1 && (
                <div className="mt-8 flex justify-center gap-2">
                  <button
                    onClick={() => setPage(p => Math.max(1, p - 1))}
                    disabled={page === 1}
                    className="px-4 py-2 rounded-md border border-gray-300 dark:border-gray-600 disabled:opacity-50"
                  >
                    Previous
                  </button>
                  <span className="px-4 py-2 text-gray-700 dark:text-gray-300">
                    Page {page} of {totalPages}
                  </span>
                  <button
                    onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                    disabled={page === totalPages}
                    className="px-4 py-2 rounded-md border border-gray-300 dark:border-gray-600 disabled:opacity-50"
                  >
                    Next
                  </button>
                </div>
              )}
            </>
          )}
        </main>
      </div>
    </div>
  );
}
