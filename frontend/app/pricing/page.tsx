'use client';

import { useAuth } from '@/contexts/AuthContext';
import { apiClient } from '@/lib/api/client';
import { useState } from 'react';

export default function PricingPage() {
  const { user } = useAuth();
  const [loading, setLoading] = useState<string | null>(null);

  const handleSubscribe = async (priceId: string) => {
    setLoading(priceId);
    try {
      const response = await apiClient.post<{ checkoutUrl: string }>(
        '/api/subscriptions/create-checkout',
        { priceId }
      );
      window.location.href = response.checkoutUrl;
    } catch (error) {
      console.error('Failed to create checkout:', error);
      setLoading(null);
    }
  };

  const plans = [
    {
      name: 'Free',
      price: '$0',
      period: 'forever',
      features: [
        'Browse free patterns',
        'Create up to 3 projects',
        'Basic pattern search',
        'Community support'
      ],
      priceId: '',
      current: user?.subscriptionTier === 'Free'
    },
    {
      name: 'Premium',
      price: '$9.99',
      period: 'per month',
      features: [
        'All free features',
        'Access to premium patterns',
        'Unlimited projects',
        'AI pattern generation (10/month)',
        'Priority support'
      ],
      priceId: 'price_premium', // Replace with actual Stripe price ID
      current: user?.subscriptionTier === 'Premium'
    },
    {
      name: 'Pro',
      price: '$19.99',
      period: 'per month',
      features: [
        'All premium features',
        'Unlimited AI generations',
        'Advanced pattern editor',
        'Export to PDF',
        'Early access to new features',
        'Dedicated support'
      ],
      priceId: 'price_pro', // Replace with actual Stripe price ID
      current: user?.subscriptionTier === 'Pro'
    }
  ];

  return (
    <div className="container mx-auto px-4 py-16">
      <div className="text-center mb-12">
        <h1 className="text-4xl font-bold text-black dark:text-white mb-4">
          Choose Your Plan
        </h1>
        <p className="text-xl text-gray-600 dark:text-gray-400">
          Select the perfect plan for your crochet journey
        </p>
      </div>

      <div className="grid md:grid-cols-3 gap-8 max-w-6xl mx-auto">
        {plans.map((plan) => (
          <div
            key={plan.name}
            className={`bg-white dark:bg-gray-800 rounded-lg shadow-lg p-8 ${
              plan.current ? 'ring-2 ring-indigo-500' : ''
            }`}
          >
            <div className="text-center mb-6">
              <h3 className="text-2xl font-bold text-black dark:text-white mb-2">
                {plan.name}
              </h3>
              <div className="mb-4">
                <span className="text-4xl font-bold text-black dark:text-white">
                  {plan.price}
                </span>
                {plan.period !== 'forever' && (
                  <span className="text-gray-600 dark:text-gray-400">
                    {' '}/ {plan.period}
                  </span>
                )}
              </div>
              {plan.current && (
                <span className="inline-block bg-indigo-100 dark:bg-indigo-900 text-indigo-800 dark:text-indigo-200 px-3 py-1 rounded-full text-sm">
                  Current Plan
                </span>
              )}
            </div>

            <ul className="space-y-3 mb-8">
              {plan.features.map((feature, index) => (
                <li key={index} className="flex items-start">
                  <svg
                    className="w-5 h-5 text-green-500 mr-2 flex-shrink-0 mt-0.5"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M5 13l4 4L19 7"
                    />
                  </svg>
                  <span className="text-gray-700 dark:text-gray-300">{feature}</span>
                </li>
              ))}
            </ul>

            {plan.priceId ? (
              <button
                onClick={() => handleSubscribe(plan.priceId)}
                disabled={plan.current || loading !== null}
                className="w-full bg-indigo-600 text-white px-6 py-3 rounded-lg hover:bg-indigo-700 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {loading === plan.priceId
                  ? 'Processing...'
                  : plan.current
                  ? 'Current Plan'
                  : 'Subscribe'}
              </button>
            ) : (
              <button
                disabled
                className="w-full bg-gray-200 dark:bg-gray-700 text-gray-500 dark:text-gray-400 px-6 py-3 rounded-lg cursor-not-allowed"
              >
                Current Plan
              </button>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}
