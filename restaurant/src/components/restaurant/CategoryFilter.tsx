import React from 'react';
import { ScrollView, StyleSheet } from 'react-native';
import { Category } from '../../types/menu';
import { CategoryChip } from './CategoryChip';
import { Spacing } from '../../theme';

const ALL_CATEGORY: Category = { id: 'all', name: 'All' };

interface CategoryFilterProps {
  categories: Category[];
  selectedId: string;
  onSelect: (id: string) => void;
}

export function CategoryFilter({ categories, selectedId, onSelect }: CategoryFilterProps) {
  const allCats = [ALL_CATEGORY, ...categories];

  return (
    <ScrollView
      horizontal
      showsHorizontalScrollIndicator={false}
      contentContainerStyle={styles.content}
      style={styles.scroll}
    >
      {allCats.map((cat) => (
        <CategoryChip
          key={cat.id}
          label={cat.name}
          selected={selectedId === cat.id}
          onPress={() => onSelect(cat.id)}
        />
      ))}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  scroll: { flexGrow: 0 },
  content: {
    paddingHorizontal: Spacing.lg,
    paddingVertical: Spacing.sm,
    gap: Spacing.sm,
    flexDirection: 'row',
    alignItems: 'center',
  },
});
