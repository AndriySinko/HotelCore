import type { ButtonHTMLAttributes, ReactNode } from 'react';

type ButtonVariant = 'primary' | 'secondary';

type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: ButtonVariant;
  children: ReactNode;
};

export default function Button({ variant = 'primary', className = '', children, ...props }: ButtonProps) {
  const baseClasses = 'inline-flex h-12 items-center justify-center rounded-lg px-6 text-sm font-medium transition-colors';
  const variantClasses =
    variant === 'primary'
      ? 'bg-black text-white hover:bg-slate-800'
      : 'border border-slate-300 bg-white text-slate-900 hover:bg-slate-100';

  return (
    <button type="button" className={`${baseClasses} ${variantClasses} ${className}`.trim()} {...props}>
      {children}
    </button>
  );
}