import React from 'react';
import type { AuthMode } from '../../types/auth';

interface AuthTabsProps {
  mode: AuthMode;
  onModeChange: (mode: AuthMode) => void;
}

export const AuthTabs: React.FC<AuthTabsProps> = ({ mode, onModeChange }) => {
  return (
    <div className="flex bg-primeoak-gray-tab p-1.5 rounded-2xl w-full">
      <button
        type="button"
        onClick={() => onModeChange('signin')}
        className={`flex-1 py-2.5 text-sm font-semibold rounded-xl transition-all duration-200 ${
          mode === 'signin'
            ? 'bg-white text-primeoak-dark shadow-sm'
            : 'text-primeoak-gray-text hover:text-primeoak-dark'
        }`}
      >
        Sign in
      </button>
      <button
        type="button"
        onClick={() => onModeChange('signup')}
        className={`flex-1 py-2.5 text-sm font-semibold rounded-xl transition-all duration-200 ${
          mode === 'signup'
            ? 'bg-white text-primeoak-dark shadow-sm'
            : 'text-primeoak-gray-text hover:text-primeoak-dark'
        }`}
      >
        Sign up
      </button>
    </div>
  );
};
