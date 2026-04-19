import type { ButtonHTMLAttributes, ReactNode } from 'react';

type ButtonVariant = 'primary' | 'secondary';

type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: ButtonVariant;
  children: ReactNode;
};

export default function Button({ variant = 'primary', className = '', children, ...props }: ButtonProps) {
  const baseClasses =
    'inline-flex h-12 items-center justify-center rounded-lg px-6 text-sm font-medium transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-500 disabled:cursor-not-allowed disabled:opacity-60';
  const variantClasses =
    variant === 'primary'
      ? 'bg-black text-white hover:bg-slate-800 disabled:hover:bg-black'
      : 'border border-slate-300 bg-white text-slate-900 hover:bg-slate-100 disabled:hover:bg-white';

  return (
    <button type="button" className={`${baseClasses} ${variantClasses} ${className}`.trim()} {...props}>
      {children}
    </button>
  );
}