import { createRequire } from 'node:module';
import { webcrypto } from 'node:crypto';

const require = createRequire(import.meta.url);
const crypto = require('node:crypto');

crypto.getRandomValues = webcrypto.getRandomValues.bind(webcrypto);
Object.defineProperty(globalThis, 'crypto', {
  value: crypto,
  configurable: true,
});

const { build } = await import('vite');

await build();