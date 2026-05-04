import { Platform } from 'react-native';

export const Fonts = {
  body: undefined as string | undefined,
  heading: undefined as string | undefined,
  mono: Platform.select({
    ios: 'Courier New',
    android: 'monospace',
    default: 'monospace',
  }),
  brand: undefined as string | undefined,
};

export const FontSizes = {
  xs: 11,
  sm: 13,
  md: 15,
  lg: 18,
  xl: 22,
  xxl: 28,
  xxxl: 34,
};

export const FontWeights = {
  regular: '400' as const,
  medium:  '500' as const,
  semibold:'600' as const,
  bold:    '700' as const,
  heavy:   '800' as const,
};

export const LineHeights = {
  tight:   18,
  normal:  22,
  relaxed: 28,
};
