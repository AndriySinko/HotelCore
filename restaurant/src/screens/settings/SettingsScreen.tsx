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
import { useAuthStore } from '../../store/authStore';
import { useOrderStore } from '../../store/orderStore';
import { Colors, FontSizes, FontWeights, Spacing, Radius, Shadow } from '../../theme';

interface InfoRow { icon: string; label: string; value: string; valueStyle?: object }

export function SettingsScreen() {
  const user       = useAuthStore((s) => s.user);
  const logout     = useAuthStore((s) => s.logout);
  const clearOrder = useOrderStore((s) => s.clearOrder);

  const handleLogout = useCallback(() => {
    Alert.alert('Sign out', 'Are you sure you want to sign out?', [
      { text: 'Cancel', style: 'cancel' },
      {
        text: 'Sign out', style: 'destructive',
        onPress: () => { clearOrder(); logout(); },
      },
    ]);
  }, [logout, clearOrder]);

  if (!user) return null;

  const guestRows: InfoRow[] = [
    ...(user.roomNumber ? [{ icon: 'bed-outline', label: 'Room', value: `Room ${user.roomNumber}` }] : []),
  ];

  const appRows: InfoRow[] = [
    { icon: 'information-circle-outline', label: 'Version', value: '1.0.0' },
    { icon: 'server-outline',             label: 'API Mode', value: 'Live', valueStyle: { color: Colors.statusDelivered, fontWeight: FontWeights.semibold as string } },
  ];

  const initials = user.name.split(' ').map((n) => n[0]).join('').toUpperCase();

  return (
    <SafeAreaView style={styles.safe} edges={['top']}>
      <View style={styles.header}>
        <Text style={styles.brand}>Hotel</Text>
      </View>

      <ScrollView style={styles.scroll} contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
        <Text style={styles.pageTitle}>Settings</Text>

        <View style={[styles.card, styles.profileCard]}>
          <View style={styles.avatar}>
            <Text style={styles.avatarText}>{initials}</Text>
          </View>
          <View style={styles.profileInfo}>
            <Text style={styles.profileName}>{user.name}</Text>
            {user.roomNumber && (
              <View style={styles.profileBadge}>
                <Ionicons name="bed-outline" size={12} color={Colors.brand} />
                <Text style={styles.profileBadgeText}>Guest · Room {user.roomNumber}</Text>
              </View>
            )}
          </View>
        </View>

        {guestRows.length > 0 && (
          <View style={styles.card}>
            <Text style={styles.sectionTitle}>Stay Details</Text>
            {guestRows.map((row) => (
              <View key={row.label} style={styles.row}>
                <Ionicons name={row.icon as any} size={16} color={Colors.text.tertiary} />
                <Text style={styles.rowLabel}>{row.label}</Text>
                <Text style={[styles.rowValue, row.valueStyle]}>{row.value}</Text>
              </View>
            ))}
          </View>
        )}

        <View style={styles.card}>
          <Text style={styles.sectionTitle}>App</Text>
          {appRows.map((row) => (
            <View key={row.label} style={styles.row}>
              <Ionicons name={row.icon as any} size={16} color={Colors.text.tertiary} />
              <Text style={styles.rowLabel}>{row.label}</Text>
              <Text style={[styles.rowValue, row.valueStyle]}>{row.value}</Text>
            </View>
          ))}
        </View>

        <TouchableOpacity style={styles.signOutBtn} onPress={handleLogout} activeOpacity={0.8}>
          <Ionicons name="log-out-outline" size={18} color={Colors.error} />
          <Text style={styles.signOutLabel}>Sign Out</Text>
        </TouchableOpacity>
      </ScrollView>
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
  brand:     { fontSize: FontSizes.lg, fontWeight: FontWeights.heavy, color: Colors.brand, letterSpacing: -0.3 },
  scroll:    { flex: 1 },
  content:   { padding: Spacing.lg, gap: Spacing.md, paddingBottom: Spacing.xxxl },
  pageTitle: { fontSize: FontSizes.xxl, fontWeight: FontWeights.heavy, color: Colors.text.primary, marginBottom: Spacing.xs, letterSpacing: -0.5 },
  card:      { backgroundColor: Colors.surface, borderRadius: Radius.lg, padding: Spacing.lg, gap: Spacing.sm, ...Shadow.card },
  profileCard:   { flexDirection: 'row', alignItems: 'center', gap: Spacing.md, paddingVertical: Spacing.xl },
  avatar:        { width: 60, height: 60, borderRadius: 30, backgroundColor: Colors.brand, alignItems: 'center', justifyContent: 'center' },
  avatarText:    { fontSize: FontSizes.lg, fontWeight: FontWeights.heavy, color: Colors.text.onPrimary },
  profileInfo:   { flex: 1, gap: Spacing.xs },
  profileName:   { fontSize: FontSizes.lg, fontWeight: FontWeights.bold, color: Colors.text.primary },
  profileBadge:  { flexDirection: 'row', alignItems: 'center', gap: 4 },
  profileBadgeText: { fontSize: FontSizes.sm, color: Colors.brand, fontWeight: FontWeights.medium },
  sectionTitle: {
    fontSize: FontSizes.xs, fontWeight: FontWeights.semibold,
    color: Colors.text.tertiary, textTransform: 'uppercase', letterSpacing: 0.8,
    marginBottom: Spacing.xs,
  },
  row:      { flexDirection: 'row', alignItems: 'center', gap: Spacing.sm, paddingVertical: 2 },
  rowLabel: { flex: 1, fontSize: FontSizes.sm, color: Colors.text.secondary },
  rowValue: { fontSize: FontSizes.sm, color: Colors.text.primary, fontWeight: FontWeights.medium },
  signOutBtn: {
    flexDirection: 'row', alignItems: 'center', gap: Spacing.sm,
    backgroundColor: Colors.surface, borderRadius: Radius.lg, padding: Spacing.lg,
    borderWidth: 1.5, borderColor: '#FCDEDE',
    ...Shadow.card,
  },
  signOutLabel: { fontSize: FontSizes.md, fontWeight: FontWeights.medium, color: Colors.error },
});
