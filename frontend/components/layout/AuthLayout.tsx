import React from 'react';
import HeroPanel from './HeroPanel';

interface AuthLayoutProps {
  children: React.ReactNode;
}

export const AuthLayout: React.FC<AuthLayoutProps> = ({ children }) => {
  return (
    <div className="min-h-screen w-full grid grid-cols-1 lg:grid-cols-2">
      <HeroPanel />
      <div className="flex items-center justify-center p-6 md:p-12 bg-primeoak-gray-bg">
        <div className="w-full max-w-md space-y-6">
          {children}
        </div>
      </div>
    </div>
  );
};
