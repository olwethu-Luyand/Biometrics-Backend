import React from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { motion } from 'framer-motion';
import { SignUpSchema, type SignUpSchemaType } from '../../types/auth';

export const SignUpForm: React.FC = () => {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<SignUpSchemaType>({
    resolver: zodResolver(SignUpSchema),
    defaultValues: {
      agreeToTerms: false,
    },
  });

  const onSubmit = (data: SignUpSchemaType) => {
    console.log('Sign Up Submitted:', data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <input
            {...register('name')}
            type="text"
            placeholder="Name"
            className="w-full rounded-xl border border-primeoak-gray-border p-3 text-sm focus:border-primeoak-blue focus:outline-none focus:ring-1 focus:ring-primeoak-blue transition-shadow duration-300"
          />
          {errors.name && <p className="mt-1 text-xs text-red-500">{errors.name.message}</p>}
        </div>
        <div>
          <input
            {...register('surname')}
            type="text"
            placeholder="Surname"
            className="w-full rounded-xl border border-primeoak-gray-border p-3 text-sm focus:border-primeoak-blue focus:outline-none focus:ring-1 focus:ring-primeoak-blue transition-shadow duration-300"
          />
          {errors.surname && <p className="mt-1 text-xs text-red-500">{errors.surname.message}</p>}
        </div>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <input
            {...register('idNumber')}
            type="text"
            placeholder="ID Number"
            className="w-full rounded-xl border border-primeoak-gray-border p-3 text-sm focus:border-primeoak-blue focus:outline-none focus:ring-1 focus:ring-primeoak-blue transition-shadow duration-300"
          />
          {errors.idNumber && <p className="mt-1 text-xs text-red-500">{errors.idNumber.message}</p>}
        </div>
        <div>
          <input
            {...register('role')}
            type="text"
            placeholder="Role"
            className="w-full rounded-xl border border-primeoak-gray-border p-3 text-sm focus:border-primeoak-blue focus:outline-none focus:ring-1 focus:ring-primeoak-blue transition-shadow duration-300"
          />
          {errors.role && <p className="mt-1 text-xs text-red-500">{errors.role.message}</p>}
        </div>
      </div>

      <div>
        <input
          {...register('email')}
          type="email"
          placeholder="Email Address"
          className="w-full rounded-xl border border-primeoak-gray-border p-3 text-sm focus:border-primeoak-blue focus:outline-none focus:ring-1 focus:ring-primeoak-blue transition-shadow duration-300"
        />
        {errors.email && <p className="mt-1 text-xs text-red-500">{errors.email.message}</p>}
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div>
          <input
            {...register('password')}
            type="password"
            placeholder="Password"
            className="w-full rounded-xl border border-primeoak-gray-border p-3 text-sm focus:border-primeoak-blue focus:outline-none focus:ring-1 focus:ring-primeoak-blue transition-shadow duration-300"
          />
          {errors.password && <p className="mt-1 text-xs text-red-500">{errors.password.message}</p>}
        </div>
        <div>
          <input
            {...register('confirmPassword')}
            type="password"
            placeholder="Confirm Password"
            className="w-full rounded-xl border border-primeoak-gray-border p-3 text-sm focus:border-primeoak-blue focus:outline-none focus:ring-1 focus:ring-primeoak-blue transition-shadow duration-300"
          />
          {errors.confirmPassword && (
            <p className="mt-1 text-xs text-red-500">{errors.confirmPassword.message}</p>
          )}
        </div>
      </div>

      <div>
        <div className="flex items-center gap-2 pt-2">
          <input
            {...register('agreeToTerms')}
            id="terms"
            type="checkbox"
            className="h-4 w-4 rounded border-primeoak-gray-border text-primeoak-blue focus:ring-primeoak-blue"
          />
          <label htmlFor="terms" className="text-xs text-primeoak-gray-text">
            By signing up, you agree to our{' '}
            <a href="#" className="font-semibold text-primeoak-blue hover:underline">
              Terms and Conditions
            </a>
            .
          </label>
        </div>
        {errors.agreeToTerms && (
          <p className="mt-1 text-xs text-red-500">{errors.agreeToTerms.message}</p>
        )}
      </div>

      <motion.button
        type="submit"
        disabled={isSubmitting}
        whileHover={{ scale: 1.02, boxShadow: "0 0 20px rgba(0, 98, 173, 0.5)" }}
        whileTap={{ scale: 0.98 }}
        className="w-full rounded-xl bg-primeoak-blue py-3.5 text-sm font-semibold text-white transition-colors disabled:opacity-50"
      >
        {isSubmitting ? 'Creating account...' : 'Sign up'}
      </motion.button>
    </form>
  );
};
