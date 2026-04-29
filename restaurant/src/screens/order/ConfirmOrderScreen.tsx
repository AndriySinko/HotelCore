import React, { useCallback, useState } from 'react';
import {
  View,
  Text,
  ScrollView,
  StyleSheet,
  TouchableOpacity,
  Alert,
} from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { Ionicons } from '@expo/vector-icons';
import { ConfirmOrderScreenProps } from '../../types/navigation';
import { useCartStore } from '../../store/cartStore';
import { useAuthStore } from '../../store/authStore';
import { useOrderStore } from '../../store/orderStore';
import { PaymentMethod } from '../../types/order';
import { Colors, FontSizes, FontWeights, Spacing, Radius, Shadow } from '../../theme';

const PAYMENT_OPTIONS: { id: PaymentMethod; icon: string; title: string; sub: (room: string | null) => string }[] = [
  { id: 'room_bill',       icon: 'receipt-outline', title: 'Charge to Room Bill', sub: (r) => r ? `Room ${r}` : 'Room charge' },
  { id: 'online_payment',  icon: 'card-outline',    title: 'Pay Online',          sub: () => 'Credit or debit card' },
];

export function ConfirmOrderScreen({ navigation }: ConfirmOrderScreenProps) {
  const cartItems     = useCartStore((s) => s.items);
  const getTotalPrice = useCartStore((s) => s.getTotalPrice);
  const clearCart     = useCartStore((s) => s.clearCart);
  const user          = useAuthStore((s) => s.user);
  const { placeOrder, isLoading } = useOrderStore();
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>('room_bill');

  const handleConfirm = useCallback(async () => {
    try {
      const order = await placeOrder(cartItems, paymentMethod);
      clearCart();
      navigation.replace('OrderStatus', { orderId: order.id });
    } catch {
      Alert.alert('Order Failed', 'Unable to place your order. Please try again.');
    }
  }, [cartItems, paymentMethod, placeOrder, clearCart, navigation]);

  const total = getTotalPrice();

  return (
    <SafeAreaView style={styles.safe} edges={['top', 'bottom']}>
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backBtn} activeOpacity={0.7}>
          <Ionicons name="chevron-back" size={24} color={Colors.text.primary} />
        </TouchableOpacity>
        <Text style={styles.title}>Confirm Order</Text>
        <View style={{ width: 40 }} />
      </View>

      <ScrollView style={styles.scroll} contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
        {user?.roomNumber && (
          <View style={styles.card}>
            <View style={styles.rowIcon}>
              <Ionicons name="location-outline" size={18} color={Colors.brand} />
              <Text style={styles.cardLabel}>Delivering to</Text>
            </View>
            <Text style={styles.bigValue}>Room {user.roomNumber}</Text>
          </View>
        )}

        <View style={styles.card}>
          <Text style={styles.sectionTitle}>Order Summary</Text>
          {cartItems.map((ci) => (
            <View key={ci.menuItem.id} style={styles.summaryRow}>
              <View style={styles.summaryLeft}>
                <Text style={styles.summaryName}>{ci.quantity}× {ci.menuItem.name}</Text>
                {ci.note.trim() !== '' && <Text style={styles.summaryNote}>Note: {ci.note}</Text>}
              </View>
              <Text style={styles.summaryPrice}>€{(ci.menuItem.price * ci.quantity).toFixed(2)}</Text>
            </View>
          ))}
          <View style={styles.divider} />
          <View style={styles.totalRow}>
            <Text style={styles.totalLabel}>Total</Text>
            <Text style={styles.totalAmount}>€{total.toFixed(2)}</Text>
          </View>
        </View>

        <View style={styles.card}>
          <Text style={styles.sectionTitle}>Payment Method</Text>
          {PAYMENT_OPTIONS.map((opt) => {
            const active = paymentMethod === opt.id;
            return (
              <TouchableOpacity
                key={opt.id}
                style={[styles.payOption, active && styles.payOptionActive]}
                onPress={() => setPaymentMethod(opt.id)}
                activeOpacity={0.8}
              >
                <View style={styles.payOptionLeft}>
                  <View style={[styles.payOptIcon, active && styles.payOptIconActive]}>
                    <Ionicons name={opt.icon as any} size={18} color={active ? Colors.text.onPrimary : Colors.text.secondary} />
                  </View>
                  <View>
                    <Text style={[styles.payOptTitle, active && styles.payOptTitleActive]}>{opt.title}</Text>
                    <Text style={[styles.payOptSub, active && styles.payOptSubActive]}>{opt.sub(user?.roomNumber ?? null)}</Text>
                  </View>
                </View>
                {active && <Ionicons name="checkmark-circle" size={20} color={Colors.brand} />}
              </TouchableOpacity>
            );
          })}
        </View>
      </ScrollView>

      <View style={styles.footer}>
        <TouchableOpacity
          style={[styles.confirmBtn, isLoading && styles.confirmBtnDisabled]}
          onPress={handleConfirm}
          disabled={isLoading}
          activeOpacity={0.85}
        >
          <Text style={styles.confirmLabel}>{isLoading ? 'Placing Order…' : 'Confirm Order'}</Text>
          {!isLoading && <Text style={styles.confirmSub}>€{total.toFixed(2)}</Text>}
        </TouchableOpacity>
      </View>
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
  backBtn: { padding: Spacing.xs, marginLeft: -Spacing.xs },
  title:   { fontSize: FontSizes.lg, fontWeight: FontWeights.semibold, color: Colors.text.primary },
  scroll:  { flex: 1 },
  content: { padding: Spacing.lg, gap: Spacing.md, paddingBottom: Spacing.xxxl },
  card:    { backgroundColor: Colors.surface, borderRadius: Radius.lg, padding: Spacing.lg, gap: Spacing.sm, ...Shadow.card },
  rowIcon: { flexDirection: 'row', alignItems: 'center', gap: Spacing.xs },
  cardLabel:  { fontSize: FontSizes.sm, fontWeight: FontWeights.medium, color: Colors.text.secondary },
  bigValue:   { fontSize: FontSizes.xl, fontWeight: FontWeights.heavy, color: Colors.text.primary },
  sectionTitle: { fontSize: FontSizes.md, fontWeight: FontWeights.semibold, color: Colors.text.primary, marginBottom: Spacing.xs },
  summaryRow: { flexDirection: 'row', alignItems: 'flex-start', justifyContent: 'space-between', gap: Spacing.sm },
  summaryLeft: { flex: 1, gap: 2 },
  summaryName:  { fontSize: FontSizes.sm, fontWeight: FontWeights.medium, color: Colors.text.primary },
  summaryNote:  { fontSize: FontSizes.xs, color: Colors.text.secondary, fontStyle: 'italic' },
  summaryPrice: { fontSize: FontSizes.sm, fontWeight: FontWeights.semibold, color: Colors.text.primary },
  divider: { height: StyleSheet.hairlineWidth, backgroundColor: Colors.border, marginVertical: Spacing.sm },
  totalRow:    { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between' },
  totalLabel:  { fontSize: FontSizes.md, color: Colors.text.secondary },
  totalAmount: { fontSize: FontSizes.xl, fontWeight: FontWeights.heavy, color: Colors.brand },
  payOption: {
    flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between',
    borderWidth: 1.5, borderColor: Colors.border, borderRadius: Radius.md,
    padding: Spacing.md, gap: Spacing.sm,
  },
  payOptionActive: { borderColor: Colors.brand, backgroundColor: Colors.brandLight },
  payOptionLeft:   { flexDirection: 'row', alignItems: 'center', gap: Spacing.md, flex: 1 },
  payOptIcon:      { width: 36, height: 36, borderRadius: Radius.sm, backgroundColor: Colors.buttonSecondary, alignItems: 'center', justifyContent: 'center' },
  payOptIconActive:{ backgroundColor: Colors.brand },
  payOptTitle:      { fontSize: FontSizes.sm, fontWeight: FontWeights.semibold, color: Colors.text.primary },
  payOptTitleActive:{ color: Colors.text.primary },
  payOptSub:        { fontSize: FontSizes.xs, color: Colors.text.secondary },
  payOptSubActive:  { color: Colors.text.secondary },
  footer: {
    paddingHorizontal: Spacing.lg, paddingVertical: Spacing.md,
    backgroundColor: Colors.surface,
    borderTopWidth: StyleSheet.hairlineWidth, borderTopColor: Colors.border,
  },
  confirmBtn: {
    backgroundColor: Colors.brand, borderRadius: Radius.md, height: 56,
    flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between',
    paddingHorizontal: Spacing.xl, ...Shadow.elevated,
  },
  confirmBtnDisabled: { opacity: 0.6 },
  confirmLabel: { fontSize: FontSizes.md, fontWeight: FontWeights.semibold, color: Colors.text.onPrimary },
  confirmSub:   { fontSize: FontSizes.md, fontWeight: FontWeights.heavy, color: 'rgba(255,255,255,0.85)' },
});
