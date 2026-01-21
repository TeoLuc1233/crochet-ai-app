'use client';

interface PatternCardProps {
  id: number;
  title: string;
  description: string;
  difficulty: string;
  category: string;
  imageUrl?: string;
  isPremium: boolean;
}

export function PatternCard({
  id,
  title,
  description,
  difficulty,
  category,
  imageUrl,
  isPremium
}: PatternCardProps) {
  return (
    <div className="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden hover:shadow-lg transition-shadow">
      {imageUrl && (
        <img
          src={imageUrl}
          alt={title}
          className="w-full h-48 object-cover"
        />
      )}
      <div className="p-4">
        <div className="flex items-center justify-between mb-2">
          <h3 className="text-lg font-semibold text-black dark:text-white line-clamp-1">
            {title}
          </h3>
          {isPremium && (
            <span className="bg-yellow-500 text-white text-xs px-2 py-1 rounded flex-shrink-0">
              Premium
            </span>
          )}
        </div>
        <p className="text-sm text-gray-600 dark:text-gray-400 mb-2 line-clamp-2">
          {description}
        </p>
        <div className="flex items-center gap-2 text-sm mb-3">
          <span className="bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200 px-2 py-1 rounded">
            {difficulty}
          </span>
          <span className="text-gray-500 dark:text-gray-400">{category}</span>
        </div>
        <a
          href={`/patterns/${id}`}
          className="inline-block text-indigo-600 hover:text-indigo-700 dark:text-indigo-400 font-medium"
        >
          View Pattern →
        </a>
      </div>
    </div>
  );
}
