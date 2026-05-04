import React, { useCallback } from 'react';
import {
  View,
  Text,
  FlatList,
  StyleSheet,
  TouchableOpacity,
  Alert,
} from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { Ionicons } from '@expo/vector-icons';
import { CartScreenProps } from '../../types/navigation';
import { useCartStore } from '../../store/cartStore';
import { CartItem } from '../../types/cart';
import { CartItemRow } from '../../components/cart/CartItemRow';
import { Colors, FontSizes, FontWeights, Spacing, Radius, Shadow } from '../../theme';

export function CartScreen({ navigation }: CartScreenProps) {
  const items          = useCartStore((s) => s.items);
  const removeItem     = useCartStore((s) => s.removeItem);
  const updateQuantity = useCartStore((s) => s.updateQuantity);
  const updateNote     = useCartStore((s) => s.updateNote);
  const getTotalPrice  = useCartStore((s) => s.getTotalPrice);

  const handleClearCart = useCallback(() => {
    Alert.alert('Clear cart', 'Remove all items from cart?', [
      { text: 'Cancel', style: 'cancel' },
      { text: 'Clear', style: 'destructive', onPress: () => useCartStore.getState().clearCart() },
    ]);
  }, []);

  return (
    <SafeAreaView style={styles.safe} edges={['top', 'bottom']}>
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backBtn} activeOpacity={0.7}>
          <Ionicons name="chevron-back" size={24} color={Colors.text.primary} />
        </TouchableOpacity>
        <Text style={styles.title}>Cart</Text>
        {items.length > 0 ? (
          <TouchableOpacity onPress={handleClearCart} activeOpacity={0.7}>
            <Text style={styles.clearText}>Clear</Text>
          </TouchableOpacity>
        ) : (
          <View style={{ width: 40 }} />
        )}
      </View>

      {items.length === 0 ? (
        <View style={styles.empty}>
          <Text style={styles.emptyIcon}>🛒</Text>
          <Text style={styles.emptyTitle}>Your cart is empty</Text>
          <Text style={styles.emptySubtitle}>Browse the menu and add items to get started</Text>
          <TouchableOpacity style={styles.browseBtn} onPress={() => navigation.goBack()} activeOpacity={0.8}>
            <Text style={styles.browseBtnLabel}>Browse Menu</Text>
          </TouchableOpacity>
        </View>
      ) : (
        <>
          <FlatList
            data={items}
            keyExtractor={(item) => item.menuItem.id}
            contentContainerStyle={styles.list}
            showsVerticalScrollIndicator={false}
            renderItem={({ item }: { item: CartItem }) => (
              <CartItemRow
                item={item}
                onIncrease={() => updateQuantity(item.menuItem.id, item.quantity + 1)}
                onDecrease={() => updateQuantity(item.menuItem.id, item.quantity - 1)}
                onRemove={() => removeItem(item.menuItem.id)}
                onNoteChange={(note) => updateNote(item.menuItem.id, note)}
              />
            )}
            ItemSeparatorComponent={() => <View style={{ height: Spacing.sm }} />}
            ListFooterComponent={
              <View style={styles.totalCard}>
                <Text style={styles.totalLabel}>Order Total</Text>
                <Text style={styles.totalAmount}>€{getTotalPrice().toFixed(2)}</Text>
              </View>
            }
          />
          <View style={styles.footer}>
            <TouchableOpacity
              style={styles.proceedBtn}
              onPress={() => navigation.navigate('ConfirmOrder')}
              activeOpacity={0.85}
            >
              <Text style={styles.proceedLabel}>Proceed to Order</Text>
              <Text style={styles.proceedSub}>€{getTotalPrice().toFixed(2)}</Text>
            </TouchableOpacity>
          </View>
        </>
      )}
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safe:   { flex: 1, backgroundColor: Colors.background },
  header: {
    flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between',
    paddingHorizontal: Spacing.lg, paddingVertical: Spacing.md,
    backgroundColor: Colors.surface,
    borderBottomWidth: StyleSheet.hairlineWidth, borderBottomColor: Colors.border,
  },
  backBtn:   { padding: Spacing.xs, marginLeft: -Spacing.xs },
  title:     { fontSize: FontSizes.lg, fontWeight: FontWeights.semibold, color: Colors.text.primary },
  clearText: { fontSize: FontSizes.sm, color: Colors.error, fontWeight: FontWeights.medium, padding: Spacing.xs },
  list:      { padding: Spacing.lg, paddingBottom: Spacing.xxxl },
  totalCard: {
    flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between',
    backgroundColor: Colors.surface, borderRadius: Radius.lg, padding: Spacing.lg,
    marginTop: Spacing.md, ...Shadow.card,
  },
  totalLabel:  { fontSize: FontSizes.md, color: Colors.text.secondary },
  totalAmount: { fontSize: FontSizes.xl, fontWeight: FontWeights.heavy, color: Colors.brand },
  footer: {
    paddingHorizontal: Spacing.lg, paddingVertical: Spacing.md,
    backgroundColor: Colors.surface,
    borderTopWidth: StyleSheet.hairlineWidth, borderTopColor: Colors.border,
  },
  proceedBtn: {
    backgroundColor: Colors.brand, borderRadius: Radius.md, height: 56,
    flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between',
    paddingHorizontal: Spacing.xl, ...Shadow.elevated,
  },
  proceedLabel: { fontSize: FontSizes.md, fontWeight: FontWeights.semibold, color: Colors.text.onPrimary },
  proceedSub:   { fontSize: FontSizes.md, fontWeight: FontWeights.heavy, color: 'rgba(255,255,255,0.85)' },
  empty: {
    flex: 1, alignItems: 'center', justifyContent: 'center',
    gap: Spacing.md, paddingHorizontal: Spacing.xl,
  },
  emptyIcon:     { fontSize: 56 },
  emptyTitle:    { fontSize: FontSizes.xl, fontWeight: FontWeights.semibold, color: Colors.text.primary },
  emptySubtitle: { fontSize: FontSizes.md, color: Colors.text.secondary, textAlign: 'center' },
  browseBtn:      { backgroundColor: Colors.brand, borderRadius: Radius.md, paddingHorizontal: Spacing.xxl, paddingVertical: Spacing.md, marginTop: Spacing.sm },
  browseBtnLabel: { fontSize: FontSizes.md, fontWeight: FontWeights.semibold, color: Colors.text.onPrimary },
});
