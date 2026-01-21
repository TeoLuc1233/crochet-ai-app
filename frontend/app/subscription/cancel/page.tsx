'use client';

import Link from 'next/link';

export default function SubscriptionCancelPage() {
  return (
    <div className="container mx-auto px-4 py-16">
      <div className="max-w-2xl mx-auto text-center">
        <h1 className="text-3xl font-bold text-black dark:text-white mb-4">
          Subscription Cancelled
        </h1>
        <p className="text-lg text-gray-600 dark:text-gray-400 mb-8">
          Your subscription was not completed. No charges were made.
        </p>

        <div className="space-y-4">
          <Link
            href="/pricing"
            className="inline-block bg-indigo-600 text-white px-6 py-3 rounded-lg hover:bg-indigo-700"
          >
            Try Again
          </Link>
          <Link
            href="/dashboard"
            className="inline-block ml-4 bg-gray-200 dark:bg-gray-700 text-gray-800 dark:text-gray-200 px-6 py-3 rounded-lg hover:bg-gray-300 dark:hover:bg-gray-600"
          >
            Go to Dashboard
          </Link>
        </div>
      </div>
    </div>
  );
}
