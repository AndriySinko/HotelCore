import React from 'react';
import { TouchableOpacity, Text, StyleSheet } from 'react-native';
import { Colors, FontSizes, FontWeights, Radius, Spacing } from '../../theme';

interface CategoryChipProps {
  label: string;
  selected: boolean;
  onPress: () => void;
}

export function CategoryChip({ label, selected, onPress }: CategoryChipProps) {
  return (
    <TouchableOpacity
      onPress={onPress}
      activeOpacity={0.75}
      style={[styles.chip, selected ? styles.chipSelected : styles.chipUnselected]}
    >
      <Text style={[styles.label, selected ? styles.labelSelected : styles.labelUnselected]}>
        {label}
      </Text>
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  chip: {
    paddingHorizontal: Spacing.lg,
    paddingVertical: Spacing.sm - 1,
    borderRadius: Radius.full,
    borderWidth: 1.5,
    justifyContent: 'center',
    alignItems: 'center',
  },
  chipSelected:    { backgroundColor: Colors.brand, borderColor: Colors.brand },
  chipUnselected:  { backgroundColor: Colors.surface, borderColor: Colors.border },
  label:           { fontSize: FontSizes.sm, fontWeight: FontWeights.semibold, letterSpacing: 0.2 },
  labelSelected:   { color: Colors.text.onPrimary },
  labelUnselected: { color: Colors.text.secondary },
});
