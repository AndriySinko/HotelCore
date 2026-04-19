import type { ReactNode } from 'react';

type AppShellProps = {
  children: ReactNode;
};

export default function AppShell({ children }: AppShellProps) {
  return (
    <div className="min-h-screen bg-slate-50 text-slate-900">
      <header className="mx-auto flex w-full max-w-[1160px] items-start justify-between px-8 py-8">
        <button type="button" className="text-sm font-medium text-slate-700 hover:text-slate-900">
          ← Back to Dashboard
        </button>
        <div className="text-right leading-tight">
          <div className="text-sm text-slate-500">Guest</div>
          <div className="text-base font-medium text-slate-900">John Smith</div>
        </div>
      </header>

      <main className="mx-auto w-full max-w-[1160px] px-8 pb-10">{children}</main>
    </div>
  );
}