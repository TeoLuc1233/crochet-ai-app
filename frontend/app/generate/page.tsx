'use client';

import { useState } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { ProtectedRoute } from '@/components/ProtectedRoute';
import { apiClient } from '@/lib/api/client';

interface GeneratePatternResponse {
  patternId: number;
  title: string;
  description: string;
  difficulty: string;
  category: string;
  materials: string[];
  instructions: string;
  notes?: string;
}

export default function GeneratePage() {
  const { user } = useAuth();
  const [image, setImage] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const [uploading, setUploading] = useState(false);
  const [generating, setGenerating] = useState(false);
  const [pattern, setPattern] = useState<GeneratePatternResponse | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [progress, setProgress] = useState(0);

  const handleImageSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setImage(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    const file = e.dataTransfer.files[0];
    if (file && file.type.startsWith('image/')) {
      setImage(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleGenerate = async () => {
    if (!image) return;

    setError(null);
    setPattern(null);
    setUploading(true);
    setProgress(10);

    try {
      // Upload image
      const formData = new FormData();
      formData.append('file', image);

      const uploadResponse = await apiClient.post<{ blobName: string; temporaryUrl: string }>(
        '/api/images/upload',
        formData,
        {
          headers: { 'Content-Type': 'multipart/form-data' }
        }
      );

      setProgress(50);
      setUploading(false);
      setGenerating(true);

      // Generate pattern
      const generateResponse = await apiClient.post<GeneratePatternResponse>(
        '/api/ai/generate-pattern',
        {
          imageUrl: uploadResponse.temporaryUrl
        }
      );

      setProgress(100);
      setPattern(generateResponse);
    } catch (err: any) {
      setError(err.message || 'Failed to generate pattern');
    } finally {
      setUploading(false);
      setGenerating(false);
      setProgress(0);
    }
  };

  return (
    <ProtectedRoute>
      <div className="container mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold text-black dark:text-white mb-8">AI Pattern Generator</h1>

        {!pattern ? (
          <div className="max-w-2xl mx-auto">
            <div
              className="border-2 border-dashed border-gray-300 dark:border-gray-600 rounded-lg p-12 text-center cursor-pointer hover:border-indigo-500 transition-colors"
              onDragOver={handleDragOver}
              onDrop={handleDrop}
              onClick={() => document.getElementById('image-upload')?.click()}
            >
              <input
                id="image-upload"
                type="file"
                accept="image/*"
                className="hidden"
                onChange={handleImageSelect}
              />
              {imagePreview ? (
                <div>
                  <img
                    src={imagePreview}
                    alt="Preview"
                    className="max-w-full max-h-96 mx-auto mb-4 rounded"
                  />
                  <p className="text-gray-600 dark:text-gray-400">
                    {image?.name} ({(image?.size || 0) / 1024} KB)
                  </p>
                  <button
                    onClick={(e) => {
                      e.stopPropagation();
                      setImage(null);
                      setImagePreview(null);
                    }}
                    className="mt-2 text-red-600 hover:text-red-700"
                  >
                    Remove
                  </button>
                </div>
              ) : (
                <div>
                  <p className="text-gray-600 dark:text-gray-400 mb-2">
                    Drag and drop an image here, or click to select
                  </p>
                  <p className="text-sm text-gray-500 dark:text-gray-500">
                    Supported formats: JPEG, PNG, GIF, WebP (max 10MB)
                  </p>
                </div>
              )}
            </div>

            {(uploading || generating) && (
              <div className="mt-6">
                <div className="w-full bg-gray-200 rounded-full h-2.5 dark:bg-gray-700">
                  <div
                    className="bg-indigo-600 h-2.5 rounded-full transition-all"
                    style={{ width: `${progress}%` }}
                  ></div>
                </div>
                <p className="mt-2 text-sm text-gray-600 dark:text-gray-400">
                  {uploading ? 'Uploading image...' : 'Generating pattern...'}
                </p>
              </div>
            )}

            {error && (
              <div className="mt-4 bg-red-50 dark:bg-red-900 p-4 rounded-lg">
                <p className="text-red-800 dark:text-red-200">{error}</p>
              </div>
            )}

            <button
              onClick={handleGenerate}
              disabled={!image || uploading || generating}
              className="mt-6 w-full bg-indigo-600 text-white px-6 py-3 rounded-lg hover:bg-indigo-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {uploading || generating ? 'Processing...' : 'Generate Pattern'}
            </button>
          </div>
        ) : (
          <div className="max-w-4xl mx-auto">
            <div className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow">
              <h2 className="text-2xl font-bold text-black dark:text-white mb-4">{pattern.title}</h2>
              <p className="text-gray-700 dark:text-gray-300 mb-4">{pattern.description}</p>

              <div className="mb-4">
                <span className="bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200 px-3 py-1 rounded mr-2">
                  {pattern.difficulty}
                </span>
                <span className="text-gray-600 dark:text-gray-400">{pattern.category}</span>
              </div>

              <div className="mb-4">
                <h3 className="font-semibold text-black dark:text-white mb-2">Materials</h3>
                <ul className="list-disc list-inside text-gray-700 dark:text-gray-300">
                  {pattern.materials.map((material, index) => (
                    <li key={index}>{material}</li>
                  ))}
                </ul>
              </div>

              <div className="mb-4">
                <h3 className="font-semibold text-black dark:text-white mb-2">Instructions</h3>
                <pre className="whitespace-pre-wrap text-gray-700 dark:text-gray-300 bg-gray-50 dark:bg-gray-900 p-4 rounded">
                  {pattern.instructions}
                </pre>
              </div>

              {pattern.notes && (
                <div className="mb-4">
                  <h3 className="font-semibold text-black dark:text-white mb-2">Notes</h3>
                  <p className="text-gray-700 dark:text-gray-300">{pattern.notes}</p>
                </div>
              )}

              <div className="mt-6 flex gap-4">
                <button
                  onClick={() => {
                    setPattern(null);
                    setImage(null);
                    setImagePreview(null);
                  }}
                  className="bg-gray-200 dark:bg-gray-700 text-gray-800 dark:text-gray-200 px-4 py-2 rounded hover:bg-gray-300 dark:hover:bg-gray-600"
                >
                  Generate Another
                </button>
                <a
                  href={`/patterns/${pattern.patternId}`}
                  className="bg-indigo-600 text-white px-4 py-2 rounded hover:bg-indigo-700"
                >
                  View Full Pattern
                </a>
              </div>
            </div>
          </div>
        )}
      </div>
    </ProtectedRoute>
  );
}
