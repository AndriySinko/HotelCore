import React, { useCallback, useEffect, useState } from 'react';
import {
  View,
  Text,
  FlatList,
  StyleSheet,
  ActivityIndicator,
  TouchableOpacity,
  Alert,
} from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { Ionicons } from '@expo/vector-icons';
import { RestaurantScreenProps } from '../../types/navigation';
import { Category, MenuItem } from '../../types/menu';
import { menuApi } from '../../api/menuApi';
import { useCartStore } from '../../store/cartStore';
import { CategoryFilter } from '../../components/restaurant/CategoryFilter';
import { MenuItemCard } from '../../components/restaurant/MenuItemCard';
import { Colors, FontSizes, FontWeights, Spacing, Radius, Shadow } from '../../theme';

export function RestaurantScreen({ navigation }: RestaurantScreenProps) {
  const [categories, setCategories]             = useState<Category[]>([]);
  const [items, setItems]                       = useState<MenuItem[]>([]);
  const [selectedCategory, setSelectedCategory] = useState('all');
  const [isLoading, setIsLoading]               = useState(true);

  const addItem        = useCartStore((s) => s.addItem);
  const cartItems      = useCartStore((s) => s.items);
  const totalItemCount = useCartStore((s) => s.getTotalItems());

  const loadData = useCallback(async () => {
    setIsLoading(true);
    try {
      const cats = await menuApi.getCategories();
      setCategories(cats);
      setItems(await menuApi.getMenuItems());
    } catch {
      Alert.alert('Error', 'Failed to load menu. Please try again.');
    } finally {
      setIsLoading(false);
    }
  }, []);

  const handleCategoryChange = useCallback(async (id: string) => {
    setSelectedCategory(id);
    setItems(await menuApi.getMenuItems(id === 'all' ? undefined : id));
  }, []);

  useEffect(() => { loadData(); }, [loadData]);

  const handleAddToCart = useCallback((item: MenuItem) => { addItem(item); }, [addItem]);
  const getCartQty = useCallback(
    (id: string) => cartItems.find((c) => c.menuItem.id === id)?.quantity ?? 0,
    [cartItems],
  );

  return (
    <SafeAreaView style={styles.safe} edges={['top']}>
      <View style={styles.header}>
        <View>
          <Text style={styles.brandTag}>Hotel</Text>
          <Text style={styles.headerTitle}>Restaurant</Text>
        </View>
        <TouchableOpacity
          style={[styles.cartBtn, totalItemCount > 0 && styles.cartBtnActive]}
          onPress={() => navigation.navigate('Cart')}
          activeOpacity={0.8}
        >
          <Ionicons name="cart" size={20} color={totalItemCount > 0 ? Colors.text.onPrimary : Colors.text.secondary} />
          {totalItemCount > 0 && <Text style={styles.cartCount}>{totalItemCount}</Text>}
        </TouchableOpacity>
      </View>

      {isLoading ? (
        <View style={styles.loader}>
          <ActivityIndicator size="large" color={Colors.brand} />
          <Text style={styles.loaderText}>Loading menu…</Text>
        </View>
      ) : (
        <FlatList
          data={items}
          keyExtractor={(item) => item.id}
          contentContainerStyle={styles.list}
          showsVerticalScrollIndicator={false}
          ListHeaderComponent={
            <View style={styles.listHeader}>
              <Text style={styles.sectionLabel}>CATEGORIES</Text>
              <CategoryFilter categories={categories} selectedId={selectedCategory} onSelect={handleCategoryChange} />
              <Text style={styles.countLabel}>{items.length} item{items.length !== 1 ? 's' : ''}</Text>
            </View>
          }
          renderItem={({ item }) => (
            <MenuItemCard item={item} onAddToCart={handleAddToCart} cartQuantity={getCartQty(item.id)} />
          )}
          ItemSeparatorComponent={() => <View style={{ height: 2 }} />}
          ListEmptyComponent={
            <View style={styles.empty}>
              <Text style={styles.emptyIcon}>🍴</Text>
              <Text style={styles.emptyText}>No items in this category</Text>
            </View>
          }
        />
      )}

      {totalItemCount > 0 && !isLoading && (
        <View style={styles.floatingBar}>
          <TouchableOpacity style={styles.floatingBtn} onPress={() => navigation.navigate('Cart')} activeOpacity={0.88}>
            <View style={styles.floatingLeft}>
              <View style={styles.floatingBadge}>
                <Text style={styles.floatingBadgeText}>{totalItemCount}</Text>
              </View>
              <Text style={styles.floatingLabel}>View Cart</Text>
            </View>
            <Ionicons name="arrow-forward" size={18} color={Colors.text.onPrimary} />
          </TouchableOpacity>
        </View>
      )}
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safe: { flex: 1, backgroundColor: Colors.background },
  header: {
    flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between',
    paddingHorizontal: Spacing.lg, paddingVertical: Spacing.md,
    backgroundColor: Colors.surface,
    borderBottomWidth: StyleSheet.hairlineWidth, borderBottomColor: Colors.border,
  },
  brandTag:    { fontSize: FontSizes.sm, fontWeight: FontWeights.bold, color: Colors.brand, letterSpacing: 0.4 },
  headerTitle: { fontSize: FontSizes.xxl, fontWeight: FontWeights.heavy, color: Colors.text.primary, letterSpacing: -0.5, marginTop: 1 },
  cartBtn: {
    flexDirection: 'row', alignItems: 'center', gap: Spacing.xs,
    backgroundColor: Colors.buttonSecondary, borderRadius: Radius.full,
    paddingHorizontal: Spacing.md, paddingVertical: Spacing.xs + 2,
  },
  cartBtnActive: { backgroundColor: Colors.brand },
  cartCount:     { fontSize: FontSizes.sm, fontWeight: FontWeights.bold, color: Colors.text.onPrimary },
  loader:        { flex: 1, alignItems: 'center', justifyContent: 'center', gap: Spacing.md },
  loaderText:    { fontSize: FontSizes.sm, color: Colors.text.secondary },
  list:          { paddingTop: Spacing.sm, paddingBottom: 100 },
  listHeader:    { paddingBottom: Spacing.md },
  sectionLabel:  { fontSize: FontSizes.xs, fontWeight: FontWeights.semibold, color: Colors.text.tertiary, letterSpacing: 0.8, paddingHorizontal: Spacing.lg, marginTop: Spacing.md, marginBottom: Spacing.xs },
  countLabel:    { fontSize: FontSizes.xs, color: Colors.text.tertiary, paddingHorizontal: Spacing.lg, marginTop: Spacing.sm },
  empty:         { alignItems: 'center', paddingVertical: 60, gap: Spacing.md },
  emptyIcon:     { fontSize: 40 },
  emptyText:     { fontSize: FontSizes.md, color: Colors.text.secondary },
  floatingBar: {
    position: 'absolute', bottom: 0, left: 0, right: 0,
    paddingHorizontal: Spacing.lg, paddingVertical: Spacing.md, paddingBottom: Spacing.lg,
    backgroundColor: Colors.surface,
    borderTopWidth: StyleSheet.hairlineWidth, borderTopColor: Colors.border,
  },
  floatingBtn: {
    backgroundColor: Colors.brand, borderRadius: Radius.md, height: 52,
    flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between',
    paddingHorizontal: Spacing.lg, ...Shadow.elevated,
  },
  floatingLeft:      { flexDirection: 'row', alignItems: 'center', gap: Spacing.sm },
  floatingBadge:     { backgroundColor: 'rgba(255,255,255,0.28)', borderRadius: Radius.full, minWidth: 24, height: 24, alignItems: 'center', justifyContent: 'center', paddingHorizontal: Spacing.xs },
  floatingBadgeText: { fontSize: FontSizes.sm, fontWeight: FontWeights.bold, color: Colors.text.onPrimary },
  floatingLabel:     { fontSize: FontSizes.md, fontWeight: FontWeights.semibold, color: Colors.text.onPrimary },
});
