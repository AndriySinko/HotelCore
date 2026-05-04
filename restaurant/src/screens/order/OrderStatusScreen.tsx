import React, { useCallback, useEffect } from 'react';
import {
  View,
  Text,
  ScrollView,
  StyleSheet,
  TouchableOpacity,
  Alert,
  RefreshControl,
} from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { Ionicons } from '@expo/vector-icons';
import { OrderStatusScreenProps } from '../../types/navigation';
import { useOrderStore } from '../../store/orderStore';
import { OrderStatus, ORDER_STATUS_LABEL } from '../../types/order';
import { Colors, FontSizes, FontWeights, Spacing, Radius, Shadow } from '../../theme';

const STEPS: { key: OrderStatus; label: string; icon: string }[] = [
  { key: 'received',   label: 'Received',   icon: 'checkmark-circle' },
  { key: 'preparing',  label: 'Preparing',  icon: 'flame' },
  { key: 'on_the_way', label: 'On the Way', icon: 'bicycle' },
  { key: 'delivered',  label: 'Delivered',  icon: 'home' },
];

export function OrderStatusScreen({ navigation, route }: OrderStatusScreenProps) {
  const { orderId } = route.params;
  const { activeOrder, isLoading, refreshOrder, cancelOrder } = useOrderStore();

  useEffect(() => { refreshOrder(orderId); }, [orderId, refreshOrder]);

  const handleCancel = useCallback(() => {
    Alert.alert('Cancel Order', 'Are you sure you want to cancel this order?', [
      { text: 'Keep Order', style: 'cancel' },
      {
        text: 'Cancel Order',
        style: 'destructive',
        onPress: async () => {
          try { await cancelOrder(orderId); }
          catch (e: unknown) {
            Alert.alert('Cannot Cancel', e instanceof Error ? e.message : 'Cannot cancel at this stage.');
          }
        },
      },
    ]);
  }, [orderId, cancelOrder]);

  const handleRefresh = useCallback(() => { refreshOrder(orderId); }, [orderId, refreshOrder]);

  if (!activeOrder) {
    return (
      <SafeAreaView style={styles.safe} edges={['top']}>
        <View style={styles.header}>
          <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backBtn} activeOpacity={0.7}>
            <Ionicons name="chevron-back" size={24} color={Colors.text.primary} />
          </TouchableOpacity>
          <Text style={styles.headerTitle}>Order</Text>
          <View style={{ width: 40 }} />
        </View>
        <View style={styles.loadingState}>
          <Text style={styles.loadingText}>Loading order…</Text>
        </View>
      </SafeAreaView>
    );
  }

  const isCancellable = activeOrder.status === 'received';
  const isTerminal    = activeOrder.status === 'delivered' || activeOrder.status === 'cancelled';
  const isCancelled   = activeOrder.status === 'cancelled';
  const stepIndex     = isCancelled ? -1 : STEPS.findIndex((s) => s.key === activeOrder.status);
  const paymentLabel  = activeOrder.paymentMethod === 'room_bill' ? 'Room bill' : 'Online payment';

  return (
    <SafeAreaView style={styles.safe} edges={['top', 'bottom']}>
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backBtn} activeOpacity={0.7}>
          <Ionicons name="chevron-back" size={24} color={Colors.text.primary} />
        </TouchableOpacity>
        <Text style={styles.headerTitle} numberOfLines={1}>{activeOrder.id}</Text>
        <TouchableOpacity onPress={handleRefresh} style={styles.refreshBtn} activeOpacity={0.7}>
          <Ionicons name="refresh-outline" size={20} color={Colors.text.secondary} />
        </TouchableOpacity>
      </View>

      <ScrollView
        style={styles.scroll}
        contentContainerStyle={styles.content}
        showsVerticalScrollIndicator={false}
        refreshControl={<RefreshControl refreshing={isLoading} onRefresh={handleRefresh} tintColor={Colors.brand} />}
      >
        {!isCancelled ? (
          <View style={styles.progressCard}>
            <Text style={styles.progressTitle}>{ORDER_STATUS_LABEL[activeOrder.status]}</Text>
            <View style={styles.tracker}>
              {STEPS.map((step, i) => {
                const done    = i <= stepIndex;
                const current = i === stepIndex;
                return (
                  <React.Fragment key={step.key}>
                    <View style={styles.trackerStep}>
                      <View style={[styles.trackerDot, done && styles.trackerDotDone, current && styles.trackerDotCurrent]}>
                        {done
                          ? <Ionicons name={step.icon as any} size={current ? 15 : 13} color={Colors.text.onPrimary} />
                          : <View style={styles.trackerDotInner} />
                        }
                      </View>
                      <Text style={[styles.trackerLabel, done && styles.trackerLabelDone]}>{step.label}</Text>
                    </View>
                    {i < STEPS.length - 1 && (
                      <View style={[styles.trackerLine, i < stepIndex && styles.trackerLineDone]} />
                    )}
                  </React.Fragment>
                );
              })}
            </View>
          </View>
        ) : (
          <View style={[styles.progressCard, styles.cancelledCard]}>
            <Ionicons name="close-circle" size={36} color={Colors.error} />
            <Text style={styles.cancelledTitle}>Order Cancelled</Text>
            <Text style={styles.cancelledNote}>A refund will be processed within 3–5 business days.</Text>
          </View>
        )}

        <View style={styles.card}>
          <Text style={styles.sectionTitle}>Items</Text>
          {activeOrder.items.map((item, idx) => (
            <View key={`${item.id}-${idx}`} style={styles.summaryRow}>
              <View style={styles.summaryLeft}>
                <Text style={styles.summaryName}>{item.quantity}× {item.productName}</Text>
                {item.specialRequest && item.specialRequest.trim() !== '' && (
                  <Text style={styles.summaryNote}>Note: {item.specialRequest}</Text>
                )}
              </View>
              <Text style={styles.summaryPrice}>€{(item.pricePerUnit * item.quantity).toFixed(2)}</Text>
            </View>
          ))}
          <View style={styles.divider} />
          <View style={styles.totalRow}>
            <Text style={styles.totalLabel}>Total</Text>
            <Text style={styles.totalAmount}>€{activeOrder.totalPrice.toFixed(2)}</Text>
          </View>
        </View>

        <View style={styles.card}>
          <View style={styles.rowIcon}>
            <Ionicons name="card-outline" size={16} color={Colors.brand} />
            <Text style={styles.cardLabel}>Payment</Text>
          </View>
          <Text style={styles.paymentValue}>{paymentLabel}</Text>
        </View>
      </ScrollView>

      {isCancellable && (
        <View style={styles.footer}>
          <TouchableOpacity style={styles.cancelBtn} onPress={handleCancel} activeOpacity={0.85}>
            <Text style={styles.cancelLabel}>Cancel Order</Text>
          </TouchableOpacity>
        </View>
      )}
      {isTerminal && (
        <View style={styles.footer}>
          <TouchableOpacity style={styles.homeBtn} onPress={() => navigation.navigate('Tabs')} activeOpacity={0.85}>
            <Text style={styles.homeBtnLabel}>Back to Home</Text>
          </TouchableOpacity>
        </View>
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
  backBtn:     { padding: Spacing.xs, marginLeft: -Spacing.xs },
  refreshBtn:  { padding: Spacing.xs },
  headerTitle: { fontSize: FontSizes.sm, fontWeight: FontWeights.semibold, color: Colors.text.secondary, letterSpacing: 0.3, flex: 1, textAlign: 'center' },
  loadingState: { flex: 1, alignItems: 'center', justifyContent: 'center' },
  loadingText:  { fontSize: FontSizes.md, color: Colors.text.secondary },
  scroll:  { flex: 1 },
  content: { padding: Spacing.lg, gap: Spacing.md, paddingBottom: Spacing.xxxl },

  progressCard: {
    backgroundColor: Colors.surface, borderRadius: Radius.lg, padding: Spacing.xl,
    gap: Spacing.md, alignItems: 'center', ...Shadow.card,
  },
  progressTitle: { fontSize: FontSizes.xl, fontWeight: FontWeights.heavy, color: Colors.text.primary },
  tracker: {
    flexDirection: 'row', alignItems: 'flex-start', width: '100%',
    marginTop: Spacing.md,
  },
  trackerStep:  { alignItems: 'center', flex: 1, gap: Spacing.xs },
  trackerDot:   {
    width: 32, height: 32, borderRadius: 16,
    backgroundColor: Colors.separator, alignItems: 'center', justifyContent: 'center',
    borderWidth: 2, borderColor: Colors.border,
  },
  trackerDotDone:    { backgroundColor: Colors.brand, borderColor: Colors.brand },
  trackerDotCurrent: { width: 36, height: 36, borderRadius: 18, borderColor: Colors.brand, backgroundColor: Colors.brand, shadowColor: Colors.brand, shadowOpacity: 0.4, shadowRadius: 8, elevation: 4 },
  trackerDotInner:   { width: 8, height: 8, borderRadius: 4, backgroundColor: Colors.border },
  trackerLabel:      { fontSize: FontSizes.xs, color: Colors.text.tertiary, textAlign: 'center', maxWidth: 60 },
  trackerLabelDone:  { color: Colors.brand, fontWeight: FontWeights.semibold },
  trackerLine: {
    flex: 1, height: 2, backgroundColor: Colors.border,
    marginTop: 17, marginHorizontal: -Spacing.xs,
  },
  trackerLineDone: { backgroundColor: Colors.brand },

  cancelledCard: { alignItems: 'center', gap: Spacing.sm },
  cancelledTitle:{ fontSize: FontSizes.lg, fontWeight: FontWeights.bold, color: Colors.error },
  cancelledNote: { fontSize: FontSizes.sm, color: Colors.text.secondary, textAlign: 'center', lineHeight: 20 },

  card:    { backgroundColor: Colors.surface, borderRadius: Radius.lg, padding: Spacing.lg, gap: Spacing.sm, ...Shadow.card },
  rowIcon: { flexDirection: 'row', alignItems: 'center', gap: Spacing.xs },
  cardLabel:    { fontSize: FontSizes.sm, color: Colors.text.secondary },
  paymentValue: { fontSize: FontSizes.md, fontWeight: FontWeights.medium, color: Colors.text.primary },
  sectionTitle: { fontSize: FontSizes.md, fontWeight: FontWeights.semibold, color: Colors.text.primary, marginBottom: Spacing.xs },
  summaryRow:   { flexDirection: 'row', alignItems: 'flex-start', justifyContent: 'space-between', gap: Spacing.sm },
  summaryLeft:  { flex: 1, gap: 2 },
  summaryName:  { fontSize: FontSizes.sm, fontWeight: FontWeights.medium, color: Colors.text.primary },
  summaryNote:  { fontSize: FontSizes.xs, color: Colors.text.secondary, fontStyle: 'italic' },
  summaryPrice: { fontSize: FontSizes.sm, fontWeight: FontWeights.semibold, color: Colors.text.primary },
  divider:    { height: StyleSheet.hairlineWidth, backgroundColor: Colors.border, marginVertical: Spacing.sm },
  totalRow:   { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between' },
  totalLabel: { fontSize: FontSizes.md, color: Colors.text.secondary },
  totalAmount:{ fontSize: FontSizes.xl, fontWeight: FontWeights.heavy, color: Colors.brand },

  footer: {
    paddingHorizontal: Spacing.lg, paddingVertical: Spacing.md,
    backgroundColor: Colors.surface,
    borderTopWidth: StyleSheet.hairlineWidth, borderTopColor: Colors.border,
  },
  cancelBtn: {
    backgroundColor: Colors.surface, borderRadius: Radius.md, height: 52,
    alignItems: 'center', justifyContent: 'center',
    borderWidth: 1.5, borderColor: Colors.error,
  },
  cancelLabel: { fontSize: FontSizes.md, fontWeight: FontWeights.semibold, color: Colors.error },
  homeBtn: {
    backgroundColor: Colors.brand, borderRadius: Radius.md, height: 52,
    alignItems: 'center', justifyContent: 'center', ...Shadow.elevated,
  },
  homeBtnLabel: { fontSize: FontSizes.md, fontWeight: FontWeights.semibold, color: Colors.text.onPrimary },
});
