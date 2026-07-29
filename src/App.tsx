import { useState, useEffect } from 'react';
import { AuthLayout } from './components/layout/AuthLayout';
import { AuthTabs } from './components/auth/AuthTabs';
import { SignInForm } from './components/auth/SignInForm';
import { SignUpForm } from './components/auth/SignUpForm';
import type { AuthMode } from './types/auth';

export default function App() {
  const [authMode, setAuthMode] = useState<AuthMode>('signup');

  // Update browser tab title dynamically
  useEffect(() => {
    document.title = authMode === 'signin'
      ? 'Sign In | PrimeOak Solutions'
      : 'Sign Up | PrimeOak Solutions';
  }, [authMode]);

  return (
    <AuthLayout>
      <div className="space-y-6">
        <div className="space-y-1">
          <h2 className="text-2xl font-bold tracking-tight text-primeoak-dark">
            {authMode === 'signin' ? 'Welcome Back' : 'Create an account'}
          </h2>
          <p className="text-sm text-primeoak-gray-text">Please enter your details</p>
        </div>

        <AuthTabs mode={authMode} onModeChange={setAuthMode} />
        {authMode === 'signin' ? <SignInForm /> : <SignUpForm />}
      </div>
    </AuthLayout>
  );
}
