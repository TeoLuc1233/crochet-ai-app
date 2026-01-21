'use client';

import { useState } from 'react';

interface PatternEditorProps {
  pattern: {
    id: number;
    title: string;
    description: string;
    instructions: string;
    materials: string[];
  };
  onSave?: (pattern: any) => void;
  onExport?: (format: 'pdf' | 'text') => void;
}

export function PatternEditor({ pattern, onSave, onExport }: PatternEditorProps) {
  const [editedTitle, setEditedTitle] = useState(pattern.title);
  const [editedDescription, setEditedDescription] = useState(pattern.description);
  const [editedInstructions, setEditedInstructions] = useState(pattern.instructions);

  const handleSave = () => {
    if (onSave) {
      onSave({
        ...pattern,
        title: editedTitle,
        description: editedDescription,
        instructions: editedInstructions
      });
    }
  };

  const handleExport = (format: 'pdf' | 'text') => {
    if (onExport) {
      onExport(format);
    } else {
      // Default export behavior
      const content = `
${editedTitle}

${editedDescription}

Materials:
${pattern.materials.map(m => `- ${m}`).join('\n')}

Instructions:
${editedInstructions}
      `.trim();

      if (format === 'text') {
        const blob = new Blob([content], { type: 'text/plain' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `${editedTitle.replace(/\s+/g, '-')}.txt`;
        a.click();
        URL.revokeObjectURL(url);
      }
    }
  };

  return (
    <div className="bg-white dark:bg-gray-800 p-6 rounded-lg shadow">
      <div className="mb-4">
        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
          Title
        </label>
        <input
          type="text"
          value={editedTitle}
          onChange={(e) => setEditedTitle(e.target.value)}
          className="w-full rounded-md border border-gray-300 px-3 py-2 dark:border-gray-600 dark:bg-gray-700 dark:text-white"
        />
      </div>

      <div className="mb-4">
        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
          Description
        </label>
        <textarea
          value={editedDescription}
          onChange={(e) => setEditedDescription(e.target.value)}
          rows={3}
          className="w-full rounded-md border border-gray-300 px-3 py-2 dark:border-gray-600 dark:bg-gray-700 dark:text-white"
        />
      </div>

      <div className="mb-4">
        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
          Instructions
        </label>
        <textarea
          value={editedInstructions}
          onChange={(e) => setEditedInstructions(e.target.value)}
          rows={15}
          className="w-full rounded-md border border-gray-300 px-3 py-2 font-mono text-sm dark:border-gray-600 dark:bg-gray-700 dark:text-white"
        />
      </div>

      <div className="flex gap-4">
        {onSave && (
          <button
            onClick={handleSave}
            className="bg-indigo-600 text-white px-4 py-2 rounded hover:bg-indigo-700"
          >
            Save Changes
          </button>
        )}
        <button
          onClick={() => handleExport('text')}
          className="bg-gray-200 dark:bg-gray-700 text-gray-800 dark:text-gray-200 px-4 py-2 rounded hover:bg-gray-300 dark:hover:bg-gray-600"
        >
          Export as Text
        </button>
      </div>
    </div>
  );
}
