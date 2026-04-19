import type { HTMLAttributes, ReactNode } from 'react';

type CardProps = HTMLAttributes<HTMLDivElement> & {
  children: ReactNode;
};

export default function Card({ className = '', children, ...props }: CardProps) {
  return (
    <div className={`rounded border border-slate-200 bg-white shadow-sm ${className}`.trim()} {...props}>
      {children}
    </div>
  );
}