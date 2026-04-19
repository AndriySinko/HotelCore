import type { ReactNode } from 'react';
import Card from './Card';

type RadioCardProps = {
  title: string;
  description?: string;
  selected?: boolean;
  children?: ReactNode;
  className?: string;
};

export default function RadioCard({ title, description, selected = false, children, className = '' }: RadioCardProps) {
  return (
    <Card className={`p-5 ${selected ? 'border-blue-500 ring-1 ring-blue-500' : ''} ${className}`.trim()}>
      <div className="flex items-start justify-between gap-4">
        <div>
          <div className="text-base font-medium text-slate-900">{title}</div>
          {description ? <p className="mt-2 text-sm leading-6 text-slate-500">{description}</p> : null}
        </div>
        <span className={`mt-1 inline-flex h-5 w-5 rounded-full border-2 ${selected ? 'border-blue-600 bg-blue-600' : 'border-slate-300 bg-white'}`} />
      </div>
      {children ? <div className="mt-4">{children}</div> : null}
    </Card>
  );
}