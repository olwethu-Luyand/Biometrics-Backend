import React from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { motion } from 'framer-motion';
import { SignInSchema, type SignInSchemaType } from '../../types/auth';

export const SignInForm: React.FC = () => {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<SignInSchemaType>({
    resolver: zodResolver(SignInSchema),
  });

  const onSubmit = (data: SignInSchemaType) => {
    console.log('Sign In Submitted:', data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div>
        <input
          {...register('email')}
          type="email"
          placeholder="Email Address"
          className="w-full rounded-xl border border-primeoak-gray-border p-3 text-sm focus:border-primeoak-blue focus:outline-none focus:ring-1 focus:ring-primeoak-blue transition-shadow duration-300"
        />
        {errors.email && <p className="mt-1 text-xs text-red-500">{errors.email.message}</p>}
      </div>

      <div>
        <input
          {...register('password')}
          type="password"
          placeholder="Password"
          className="w-full rounded-xl border border-primeoak-gray-border p-3 text-sm focus:border-primeoak-blue focus:outline-none focus:ring-1 focus:ring-primeoak-blue transition-shadow duration-300"
        />
        {errors.password && <p className="mt-1 text-xs text-red-500">{errors.password.message}</p>}
      </div>

      <div className="flex justify-end">
        <a href="#" className="text-sm text-primeoak-blue hover:underline">
          Forgot password?
        </a>
      </div>

      <motion.button
        type="submit"
        disabled={isSubmitting}
        whileHover={{ scale: 1.02, boxShadow: "0 0 20px rgba(0, 98, 173, 0.5)" }}
        whileTap={{ scale: 0.98 }}
        className="w-full rounded-xl bg-primeoak-blue py-3.5 text-sm font-semibold text-white transition-colors disabled:opacity-50"
      >
        {isSubmitting ? 'Signing in...' : 'Sign in'}
      </motion.button>
    </form>
  );
};
