import React from 'react';
import { View, Text, StyleSheet, TouchableOpacity, Image } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { MenuItem } from '../../types/menu';
import { Colors, FontSizes, FontWeights, Radius, Spacing, Shadow } from '../../theme';

interface MenuItemCardProps {
  item: MenuItem;
  onAddToCart: (item: MenuItem) => void;
  cartQuantity?: number;
}

export function MenuItemCard({ item, onAddToCart, cartQuantity = 0 }: MenuItemCardProps) {
  return (
    <View style={[styles.card, !item.isAvailable && styles.cardUnavailable]}>
      <View style={styles.thumb}>
        {item.imageUrl ? (
          <Image source={{ uri: item.imageUrl }} style={styles.thumbImg} resizeMode="cover" />
        ) : (
          <Text style={styles.thumbEmoji}>🍴</Text>
        )}
        {!item.isAvailable && <View style={styles.thumbOverlay} />}
      </View>

      <View style={styles.info}>
        <View style={styles.infoTop}>
          <Text style={styles.name} numberOfLines={1}>{item.name}</Text>
          <Text style={styles.price}>€{item.price.toFixed(2)}</Text>
        </View>
        <Text style={styles.description} numberOfLines={2}>{item.description}</Text>

        <View style={styles.actions}>
          {!item.isAvailable ? (
            <View style={styles.unavailableTag}>
              <Text style={styles.unavailableText}>Unavailable</Text>
            </View>
          ) : cartQuantity > 0 ? (
            <View style={styles.inCartRow}>
              <View style={styles.inCartBadge}>
                <Text style={styles.inCartText}>{cartQuantity} in cart</Text>
              </View>
              <TouchableOpacity style={styles.addBtn} onPress={() => onAddToCart(item)} activeOpacity={0.8}>
                <Ionicons name="add" size={16} color={Colors.text.onPrimary} />
              </TouchableOpacity>
            </View>
          ) : (
            <TouchableOpacity style={styles.addBtn} onPress={() => onAddToCart(item)} activeOpacity={0.8}>
              <Ionicons name="add" size={18} color={Colors.text.onPrimary} />
            </TouchableOpacity>
          )}
        </View>
      </View>
    </View>
  );
}

const THUMB = 96;

const styles = StyleSheet.create({
  card: {
    backgroundColor: Colors.surface,
    borderRadius: Radius.md,
    marginHorizontal: Spacing.lg,
    marginBottom: Spacing.sm,
    flexDirection: 'column',
    overflow: 'hidden',
    ...Shadow.card,
  },
  cardUnavailable: { opacity: 0.6 },
  thumb: { width: '100%', height: THUMB, backgroundColor: Colors.brandLight, alignItems: 'center', justifyContent: 'center', flexShrink: 0 },
  thumbImg: { width: '100%', height: '100%' },
  thumbEmoji: { fontSize: 36 },
  thumbOverlay: { ...StyleSheet.absoluteFillObject, backgroundColor: 'rgba(255,255,255,0.55)' },
  info: { flex: 1, padding: Spacing.md, justifyContent: 'space-between' },
  infoTop: { flexDirection: 'row', alignItems: 'flex-start', justifyContent: 'space-between', gap: Spacing.xs, marginBottom: Spacing.xs },
  name: { flex: 1, fontSize: FontSizes.md, fontWeight: FontWeights.semibold, color: Colors.text.primary },
  price: { fontSize: FontSizes.sm, fontWeight: FontWeights.bold, color: Colors.brand },
  description: { fontSize: FontSizes.xs, color: Colors.text.secondary, lineHeight: 17 },
  actions: { flexDirection: 'row', alignItems: 'center', justifyContent: 'flex-end', marginTop: Spacing.sm },
  addBtn: { backgroundColor: Colors.brand, width: 30, height: 30, borderRadius: 15, alignItems: 'center', justifyContent: 'center' },
  inCartRow: { flexDirection: 'row', alignItems: 'center', gap: Spacing.xs },
  inCartBadge: { backgroundColor: Colors.brandLight, borderRadius: Radius.full, paddingHorizontal: Spacing.sm, paddingVertical: 3 },
  inCartText: { fontSize: FontSizes.xs, fontWeight: FontWeights.semibold, color: Colors.brand },
  unavailableTag: { backgroundColor: Colors.buttonSecondary, borderRadius: Radius.full, paddingHorizontal: Spacing.sm, paddingVertical: 4 },
  unavailableText: { fontSize: FontSizes.xs, fontWeight: FontWeights.medium, color: Colors.text.tertiary },
});
