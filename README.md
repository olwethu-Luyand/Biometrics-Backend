# PrimeOak Auth

Authentication UI built with Vite, React, TypeScript, and Tailwind CSS.

## Project Structure

```
primeoak-auth/
├── public/                      # Static assets served as-is
│   ├── favicon.svg              # Browser tab icon
│   ├── icons.svg                # SVG icon sprite
│   └── images/
│       └── primeoak-logo.svg    # PrimeOak Solutions logo
│
├── src/
│   ├── assets/                  # Imported asset files
│   │   ├── logo.jpeg            # Company logo (navbar/tab)
│   │   └── prime_oak.jpeg       # Hero panel background image
│   │
│   ├── components/
│   │   ├── auth/                # Authentication-specific components
│   │   │   ├── AuthTabs.tsx     # Segmented pill tab (Sign In / Sign Up)
│   │   │   ├── SignInForm.tsx   # Sign in form with react-hook-form + Zod
│   │   │   └── SignUpForm.tsx   # Sign up form with 2-column grid layout
│   │   │
│   │   ├── common/              # Reusable UI primitives
│   │   │   ├── Button.tsx       # Styled button (primary/secondary/outline)
│   │   │   ├── Checkbox.tsx     # Custom checkbox input
│   │   │   └── Input.tsx        # Label + input + error display
│   │   │
│   │   ├── layout/              # Page layout components
│   │   │   ├── AuthLayout.tsx   # Split grid: hero left, form right
│   │   │   └── HeroPanel.tsx    # Animated background panel with glows
│   │   │
│   │   └── ui/                  # shadcn-style UI components
│   │       ├── lamp.tsx         # Framer-motion lamp animation container
│   │       └── demo.tsx         # Lamp demo usage example
│   │
│   ├── lib/
│   │   └── utils.ts             # cn() utility (clsx + tailwind-merge)
│   │
│   ├── types/
│   │   └── auth.ts              # Zod schemas & inferred TypeScript types
│   │
│   ├── App.tsx                  # Root app with mode state & dynamic title
│   ├── index.css                # Tailwind directives
│   └── main.tsx                 # React DOM mount entry point
│
├── index.html                   # HTML entry with root div
├── tailwind.config.js           # Tailwind config with brand colors
├── vite.config.ts               # Vite config with @ path alias
├── tsconfig.json                # TS project references
├── tsconfig.app.json            # TS config for src/ with path aliases
└── tsconfig.node.json           # TS config for Node tooling
```

## Tech Stack

- **Vite** — dev server & bundler
- **React 19** — UI library
- **TypeScript 6** — type safety
- **Tailwind CSS 3** — utility-first styling
- **react-hook-form** — form state management
- **Zod** — schema validation
- **framer-motion** — animations

## Getting Started

```bash
npm install
npm run dev
```

## Scripts

| Command          | Description                  |
|------------------|------------------------------|
| `npm run dev`    | Start dev server             |
| `npm run build`  | TypeScript check + Vite build|
| `npm run preview`| Preview production build     |
| `npm run lint`   | Run Oxlint                   |
