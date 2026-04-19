import { webcrypto } from 'node:crypto';

Object.defineProperty(globalThis, 'crypto', {
  value: webcrypto,
  configurable: true,
});

const { defineConfig } = await import('vite');
const { default: react } = await import('@vitejs/plugin-react');

export default defineConfig({
  plugins: [react()],
});