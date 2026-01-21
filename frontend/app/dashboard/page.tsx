'use client';

import { ProtectedRoute } from '@/components/ProtectedRoute';
import { useAuth } from '@/contexts/AuthContext';

export default function DashboardPage() {
  const { user } = useAuth();

  return (
    <ProtectedRoute>
      <div className="container mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold text-black dark:text-white">
          Welcome, {user?.username}!
        </h1>
        <p className="mt-4 text-gray-600 dark:text-gray-400">
          Your subscription tier: {user?.subscriptionTier}
        </p>
      </div>
    </ProtectedRoute>
  );
}
