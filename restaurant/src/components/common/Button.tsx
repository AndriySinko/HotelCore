import React from 'react';
import {
  TouchableOpacity,
  Text,
  StyleSheet,
  ActivityIndicator,
  TouchableOpacityProps,
  ViewStyle,
  TextStyle,
} from 'react-native';
import { Colors, Radius, Spacing, FontSizes, FontWeights } from '../../theme';

type ButtonVariant = 'primary' | 'secondary' | 'outline' | 'ghost' | 'destructive';

interface ButtonProps extends TouchableOpacityProps {
  label: string;
  variant?: ButtonVariant;
  isLoading?: boolean;
  fullWidth?: boolean;
  style?: ViewStyle;
  labelStyle?: TextStyle;
}

export function Button({
  label,
  variant = 'primary',
  isLoading = false,
  fullWidth = false,
  style,
  labelStyle,
  disabled,
  ...rest
}: ButtonProps) {
  const isDisabled = disabled || isLoading;

  return (
    <TouchableOpacity
      activeOpacity={0.78}
      disabled={isDisabled}
      style={[
        styles.base,
        styles[variant],
        fullWidth && styles.fullWidth,
        isDisabled && styles.disabled,
        style,
      ]}
      {...rest}
    >
      {isLoading ? (
        <ActivityIndicator
          color={variant === 'primary' || variant === 'destructive' ? Colors.text.onPrimary : Colors.brand}
          size="small"
        />
      ) : (
        <Text style={[styles.label, styles[`${variant}Label`], labelStyle]}>{label}</Text>
      )}
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  base: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingHorizontal: Spacing.xl,
    paddingVertical: Spacing.md,
    borderRadius: Radius.md,
    minHeight: 46,
  },
  fullWidth: { alignSelf: 'stretch' },
  disabled: { opacity: 0.42 },

  primary:     { backgroundColor: Colors.brand },
  secondary:   { backgroundColor: Colors.buttonSecondary },
  outline:     { backgroundColor: 'transparent', borderWidth: 1.5, borderColor: Colors.brand },
  ghost:       { backgroundColor: 'transparent' },
  destructive: { backgroundColor: Colors.error },

  label:           { fontSize: FontSizes.sm, fontWeight: FontWeights.semibold, letterSpacing: 0.2 },
  primaryLabel:     { color: Colors.text.onPrimary },
  secondaryLabel:   { color: Colors.text.primary },
  outlineLabel:     { color: Colors.brand },
  ghostLabel:       { color: Colors.brand },
  destructiveLabel: { color: Colors.text.onPrimary },
});
