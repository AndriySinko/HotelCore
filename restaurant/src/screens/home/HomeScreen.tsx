import React, { useCallback } from 'react';
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
import { HomeScreenProps } from '../../types/navigation';
import { useAuthStore } from '../../store/authStore';
import { useOrderStore } from '../../store/orderStore';
import { StatusBadge } from '../../components/common/StatusBadge';
import { Colors, FontSizes, FontWeights, Spacing, Radius, Shadow } from '../../theme';

export function HomeScreen({ navigation }: HomeScreenProps) {
  const user        = useAuthStore((s) => s.user);
  const logout      = useAuthStore((s) => s.logout);
  const activeOrder = useOrderStore((s) => s.activeOrder);

  const handleCreateOrder = useCallback(() => navigation.navigate('Restaurant'), [navigation]);
  const handleViewOrder   = useCallback(() => {
    if (activeOrder) navigation.navigate('OrderStatus', { orderId: activeOrder.id });
  }, [navigation, activeOrder]);
  const handleLogout = useCallback(() => {
    Alert.alert('Sign out', 'Are you sure you want to sign out?', [
      { text: 'Cancel', style: 'cancel' },
      { text: 'Sign out', style: 'destructive', onPress: logout },
    ]);
  }, [logout]);

  if (!user) return null;

  const firstName = user.name.split(' ')[0];
  const showActiveOrder =
    activeOrder &&
    activeOrder.status !== 'delivered' &&
    activeOrder.status !== 'cancelled';

  return (
    <SafeAreaView style={styles.safe} edges={['top']}>
      <View style={styles.header}>
        <Text style={styles.brand}>Hotel</Text>
        <TouchableOpacity onPress={handleLogout} style={styles.iconBtn} activeOpacity={0.7}>
          <Ionicons name="log-out-outline" size={22} color={Colors.text.secondary} />
        </TouchableOpacity>
      </View>

      <ScrollView
        style={{ flex: 1 }}
        contentContainerStyle={styles.content}
        showsVerticalScrollIndicator={false}
      >
        <View style={styles.hero}>
          <Text style={styles.greeting}>Good day, {firstName}! 👋</Text>
          {user.roomNumber && (
            <Text style={styles.greetingSub}>Room {user.roomNumber}</Text>
          )}
        </View>

        <View style={styles.featureCard}>
          <View style={styles.featureHeader}>
            <View style={styles.featureIconWrap}>
              <Ionicons name="restaurant" size={22} color={Colors.brand} />
            </View>
            <View style={{ flex: 1 }}>
              <Text style={styles.featureTitle}>In-Room Delivery</Text>
              <Text style={styles.featureSub}>Delivered straight to your door</Text>
            </View>
          </View>

          <View style={styles.infoGrid}>
            <View style={styles.infoItem}>
              <Ionicons name="time-outline" size={14} color={Colors.text.tertiary} />
              <Text style={styles.infoText}>10:00 – 22:00</Text>
            </View>
            <View style={styles.infoItem}>
              <Ionicons name="bicycle-outline" size={14} color={Colors.text.tertiary} />
              <Text style={styles.infoText}>20 – 30 min avg.</Text>
            </View>
          </View>

          <TouchableOpacity style={styles.primaryBtn} onPress={handleCreateOrder} activeOpacity={0.85}>
            <Ionicons name="add-circle-outline" size={18} color={Colors.text.onPrimary} />
            <Text style={styles.primaryBtnLabel}>Create Order</Text>
          </TouchableOpacity>
        </View>

        <View style={styles.quickRow}>
          <TouchableOpacity
            style={styles.quickCard}
            onPress={() => navigation.navigate('Restaurant')}
            activeOpacity={0.8}
          >
            <Ionicons name="list-outline" size={24} color={Colors.brand} />
            <Text style={styles.quickLabel}>Menu</Text>
          </TouchableOpacity>
          <TouchableOpacity
            style={styles.quickCard}
            onPress={() => navigation.navigate('Orders')}
            activeOpacity={0.8}
          >
            <Ionicons name="receipt-outline" size={24} color={Colors.brand} />
            <Text style={styles.quickLabel}>Orders</Text>
          </TouchableOpacity>
          <TouchableOpacity
            style={styles.quickCard}
            onPress={() => navigation.navigate('Settings')}
            activeOpacity={0.8}
          >
            <Ionicons name="settings-outline" size={24} color={Colors.brand} />
            <Text style={styles.quickLabel}>Settings</Text>
          </TouchableOpacity>
        </View>

        {showActiveOrder && (
          <TouchableOpacity style={styles.orderCard} onPress={handleViewOrder} activeOpacity={0.85}>
            <View style={styles.orderCardHeader}>
              <Text style={styles.orderCardTitle}>Active Order</Text>
              <Ionicons name="chevron-forward" size={16} color={Colors.text.tertiary} />
            </View>
            <Text style={styles.orderId}>{activeOrder!.id}</Text>
            <StatusBadge status={activeOrder!.status} style={{ marginTop: Spacing.xs }} />
          </TouchableOpacity>
        )}
      </ScrollView>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safe:    { flex: 1, backgroundColor: Colors.background },
  header:  {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: Spacing.lg,
    paddingVertical: Spacing.md,
    backgroundColor: Colors.surface,
    borderBottomWidth: StyleSheet.hairlineWidth,
    borderBottomColor: Colors.border,
  },
  brand:   { fontSize: FontSizes.lg, fontWeight: FontWeights.heavy, color: Colors.brand, letterSpacing: -0.3 },
  iconBtn: { padding: Spacing.xs },

  content: { padding: Spacing.lg, gap: Spacing.md, paddingBottom: Spacing.xxxl },

  hero:        { gap: 4, marginBottom: Spacing.xs },
  greeting:    { fontSize: FontSizes.xxl, fontWeight: FontWeights.heavy, color: Colors.text.primary, letterSpacing: -0.3 },
  greetingSub: { fontSize: FontSizes.sm, color: Colors.text.secondary },

  featureCard: {
    backgroundColor: Colors.surface,
    borderRadius: Radius.xl,
    padding: Spacing.lg,
    gap: Spacing.md,
    ...Shadow.card,
  },
  featureHeader:  { flexDirection: 'row', alignItems: 'center', gap: Spacing.md },
  featureIconWrap: {
    width: 44, height: 44, borderRadius: 22,
    backgroundColor: Colors.brandLight,
    alignItems: 'center', justifyContent: 'center',
  },
  featureTitle: { fontSize: FontSizes.md, fontWeight: FontWeights.bold, color: Colors.text.primary },
  featureSub:   { fontSize: FontSizes.sm, color: Colors.text.secondary },
  infoGrid:     { flexDirection: 'row', gap: Spacing.xl },
  infoItem:     { flexDirection: 'row', alignItems: 'center', gap: Spacing.xs },
  infoText:     { fontSize: FontSizes.sm, color: Colors.text.secondary },

  primaryBtn: {
    backgroundColor: Colors.brand,
    borderRadius: Radius.md, height: 48,
    flexDirection: 'row', alignItems: 'center', justifyContent: 'center',
    gap: Spacing.xs,
  },
  primaryBtnLabel: { fontSize: FontSizes.md, fontWeight: FontWeights.semibold, color: Colors.text.onPrimary },

  quickRow:  { flexDirection: 'row', gap: Spacing.sm },
  quickCard: {
    flex: 1, backgroundColor: Colors.surface,
    borderRadius: Radius.lg, paddingVertical: Spacing.lg,
    alignItems: 'center', gap: Spacing.xs,
    ...Shadow.card,
  },
  quickLabel: { fontSize: FontSizes.xs, fontWeight: FontWeights.semibold, color: Colors.text.secondary },

  orderCard: {
    backgroundColor: Colors.surface,
    borderRadius: Radius.xl, padding: Spacing.lg, gap: Spacing.xs,
    borderLeftWidth: 4, borderLeftColor: Colors.brand,
    ...Shadow.card,
  },
  orderCardHeader: { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between' },
  orderCardTitle:  { fontSize: FontSizes.sm, fontWeight: FontWeights.semibold, color: Colors.text.secondary, textTransform: 'uppercase', letterSpacing: 0.5 },
  orderId:         { fontSize: FontSizes.lg, fontWeight: FontWeights.bold, color: Colors.text.primary },
});
