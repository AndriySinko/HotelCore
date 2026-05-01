import React, { useCallback } from 'react';
import {
  View,
  Text,
  FlatList,
  StyleSheet,
  TouchableOpacity,
} from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { OrderHistoryScreenProps } from '../../types/navigation';
import { useOrderStore } from '../../store/orderStore';
import { Order } from '../../types/order';
import { StatusBadge } from '../../components/common/StatusBadge';
import { Colors, FontSizes, FontWeights, Spacing, Radius, Shadow } from '../../theme';

function formatDate(iso: string): string {
  try {
    return new Date(iso).toLocaleDateString(undefined, { month: 'short', day: 'numeric', year: 'numeric' });
  } catch {
    return iso;
  }
}

function itemsSummary(order: Order): string {
  const names = order.items.map((i) => i.productName);
  if (names.length <= 2) return names.join(', ');
  return `${names[0]}, ${names[1]} +${names.length - 2} more`;
}

export function OrderHistoryScreen({ navigation }: OrderHistoryScreenProps) {
  const orderHistory = useOrderStore((s) => s.orderHistory);

  const handlePress = useCallback(
    (orderId: string) => { navigation.navigate('OrderStatus', { orderId }); },
    [navigation],
  );

  return (
    <SafeAreaView style={styles.safe} edges={['top']}>
      <View style={styles.header}>
        <Text style={styles.brand}>Hotel</Text>
        <Text style={styles.headerTitle}>Orders</Text>
      </View>

      <FlatList
        data={orderHistory}
        keyExtractor={(item) => item.id}
        contentContainerStyle={styles.list}
        showsVerticalScrollIndicator={false}
        ItemSeparatorComponent={() => <View style={{ height: Spacing.sm }} />}
        ListEmptyComponent={
          <View style={styles.empty}>
            <Text style={styles.emptyIcon}>🧾</Text>
            <Text style={styles.emptyTitle}>No orders yet</Text>
            <Text style={styles.emptySubtitle}>
              Your order history will appear here after you place your first order.
            </Text>
            <TouchableOpacity
              style={styles.menuBtn}
              onPress={() => navigation.navigate('Restaurant')}
              activeOpacity={0.8}
            >
              <Text style={styles.menuBtnLabel}>Browse Menu</Text>
            </TouchableOpacity>
          </View>
        }
        renderItem={({ item }: { item: Order }) => (
          <TouchableOpacity
            style={styles.card}
            onPress={() => handlePress(item.id)}
            activeOpacity={0.82}
          >
            <View style={styles.cardTop}>
              <View style={styles.cardLeft}>
                <Text style={styles.orderId}>{item.id}</Text>
                <Text style={styles.orderDate}>{formatDate(item.createdAt)}</Text>
              </View>
              <StatusBadge status={item.status} />
            </View>

            <Text style={styles.itemsSummary} numberOfLines={1}>
              {itemsSummary(item)}
            </Text>

            <View style={styles.cardBottom}>
              <Text style={styles.total}>€{item.totalPrice.toFixed(2)}</Text>
            </View>
          </TouchableOpacity>
        )}
      />
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safe:   { flex: 1, backgroundColor: Colors.background },
  header: {
    paddingHorizontal: Spacing.lg, paddingVertical: Spacing.md,
    backgroundColor: Colors.surface,
    borderBottomWidth: StyleSheet.hairlineWidth, borderBottomColor: Colors.border,
  },
  brand:       { fontSize: FontSizes.sm, fontWeight: FontWeights.bold, color: Colors.brand, letterSpacing: 0.4 },
  headerTitle: { fontSize: FontSizes.xxl, fontWeight: FontWeights.heavy, color: Colors.text.primary, letterSpacing: -0.5, marginTop: 1 },

  list: { padding: Spacing.lg, paddingBottom: Spacing.xxxl },

  card: {
    backgroundColor: Colors.surface, borderRadius: Radius.lg,
    padding: Spacing.lg, gap: Spacing.sm, ...Shadow.card,
  },
  cardTop:  { flexDirection: 'row', alignItems: 'flex-start', justifyContent: 'space-between' },
  cardLeft: { gap: 2 },
  orderId:  { fontSize: FontSizes.sm, fontWeight: FontWeights.semibold, color: Colors.text.secondary, letterSpacing: 0.3 },
  orderDate:{ fontSize: FontSizes.xs, color: Colors.text.tertiary },
  itemsSummary: { fontSize: FontSizes.sm, color: Colors.text.primary, lineHeight: 20 },
  cardBottom:   { flexDirection: 'row', alignItems: 'center', justifyContent: 'flex-end', marginTop: 2 },
  total:        { fontSize: FontSizes.md, fontWeight: FontWeights.heavy, color: Colors.brand },

  empty: {
    flex: 1, alignItems: 'center', justifyContent: 'center',
    paddingHorizontal: Spacing.xl, paddingTop: 80, gap: Spacing.md,
  },
  emptyIcon:     { fontSize: 52 },
  emptyTitle:    { fontSize: FontSizes.xl, fontWeight: FontWeights.semibold, color: Colors.text.primary },
  emptySubtitle: { fontSize: FontSizes.md, color: Colors.text.secondary, textAlign: 'center', lineHeight: 22 },
  menuBtn:      { backgroundColor: Colors.brand, borderRadius: Radius.md, paddingHorizontal: Spacing.xxl, paddingVertical: Spacing.md, marginTop: Spacing.sm },
  menuBtnLabel: { fontSize: FontSizes.md, fontWeight: FontWeights.semibold, color: Colors.text.onPrimary },
});
