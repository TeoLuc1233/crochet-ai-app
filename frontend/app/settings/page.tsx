'use client';

import { useState, useEffect } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { ProtectedRoute } from '@/components/ProtectedRoute';
import { apiClient } from '@/lib/api/client';

export default function SettingsPage() {
  const { user, logout } = useAuth();
  const [subscription, setSubscription] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadSubscription();
  }, []);

  const loadSubscription = async () => {
    try {
      // In a real app, fetch subscription from API
      setSubscription({
        tier: user?.subscriptionTier || 'Free',
        status: 'active'
      });
    } catch (error) {
      console.error('Failed to load subscription:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleCancelSubscription = async () => {
    if (confirm('Are you sure you want to cancel your subscription?')) {
      try {
        await apiClient.post('/api/subscriptions/cancel');
        setSubscription({ tier: 'Free', status: 'canceled' });
      } catch (error) {
        console.error('Failed to cancel subscription:', error);
      }
    }
  };

  return (
    <ProtectedRoute>
      <div className="container mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold text-black dark:text-white mb-8">Account Settings</h1>

        <div className="max-w-4xl mx-auto space-y-6">
          {/* Current Plan */}
          <div className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow">
            <h2 className="text-xl font-semibold text-black dark:text-white mb-4">Current Plan</h2>
            {loading ? (
              <p className="text-gray-600 dark:text-gray-400">Loading...</p>
            ) : (
              <div>
                <p className="text-lg text-black dark:text-white mb-2">
                  {subscription?.tier || 'Free'}
                </p>
                <p className="text-gray-600 dark:text-gray-400 mb-4">
                  Status: {subscription?.status || 'Active'}
                </p>
                {subscription?.tier !== 'Free' && (
                  <div className="space-x-4">
                    <a
                      href="/pricing"
                      className="inline-block bg-indigo-600 text-white px-4 py-2 rounded hover:bg-indigo-700"
                    >
                      Change Plan
                    </a>
                    <button
                      onClick={handleCancelSubscription}
                      className="bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700"
                    >
                      Cancel Subscription
                    </button>
                  </div>
                )}
              </div>
            )}
          </div>

          {/* Account Info */}
          <div className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow">
            <h2 className="text-xl font-semibold text-black dark:text-white mb-4">Account Information</h2>
            <div className="space-y-2">
              <p className="text-gray-700 dark:text-gray-300">
                <span className="font-medium">Username:</span> {user?.username}
              </p>
              <p className="text-gray-700 dark:text-gray-300">
                <span className="font-medium">Email:</span> {user?.email}
              </p>
              <p className="text-gray-700 dark:text-gray-300">
                <span className="font-medium">Member since:</span>{' '}
                {user?.createdAt ? new Date(user.createdAt).toLocaleDateString() : 'N/A'}
              </p>
            </div>
          </div>

          {/* Danger Zone */}
          <div className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow border-2 border-red-200 dark:border-red-900">
            <h2 className="text-xl font-semibold text-red-600 dark:text-red-400 mb-4">Danger Zone</h2>
            <button
              onClick={logout}
              className="bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700"
            >
              Logout
            </button>
          </div>
        </div>
      </div>
    </ProtectedRoute>
  );
}
