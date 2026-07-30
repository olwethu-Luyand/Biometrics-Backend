import { z } from 'zod';

// Sign In Schema
export const SignInSchema = z.object({
  email: z.string().min(1, 'Email is required').email('Invalid email address'),
  password: z.string().min(1, 'Password is required'),
});

export type SignInSchemaType = z.infer<typeof SignInSchema>;

// Sign Up Schema
export const SignUpSchema = z.object({
  name: z.string().min(1, 'Name is required'),
  surname: z.string().min(1, 'Surname is required'),
  idNumber: z.string().min(1, 'ID Number is required'),
  role: z.string().min(1, 'Role is required'),
  email: z.string().min(1, 'Email is required').email('Invalid email address'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
  confirmPassword: z.string().min(1, 'Please confirm your password'),
  agreeToTerms: z.boolean().refine((val) => val === true, {
    message: 'You must agree to the terms and conditions',
  }),
}).refine((data) => data.password === data.confirmPassword, {
  message: "Passwords don't match",
  path: ['confirmPassword'],
});

export type SignUpSchemaType = z.infer<typeof SignUpSchema>;

export type AuthMode = 'signin' | 'signup';
