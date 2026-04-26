import React, { useCallback } from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  ActivityIndicator,
  Alert,
} from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { Ionicons } from '@expo/vector-icons';
import { LoginScreenProps } from '../../types/navigation';
import { useAuthStore } from '../../store/authStore';
import { Colors, FontSizes, FontWeights, Spacing, Radius, Shadow } from '../../theme';

export function LoginScreen(_props: LoginScreenProps) {
  const { loginWithQrCode, isLoading, error, clearError } = useAuthStore();

  const handleDemoLogin = useCallback(async () => {
    clearError();
    await loginWithQrCode('demo');
  }, [loginWithQrCode, clearError]);

  React.useEffect(() => {
    if (error) {
      Alert.alert('Login Failed', error, [{ text: 'OK', onPress: clearError }]);
    }
  }, [error, clearError]);

  return (
    <SafeAreaView style={styles.container}>
      <View style={styles.brandArea}>
        <View style={styles.logoCircle}>
          <Ionicons name="restaurant-outline" size={36} color={Colors.text.onPrimary} />
        </View>
        <Text style={styles.brandName}>Restaurant</Text>
        <Text style={styles.tagline}>Order food from your room</Text>
      </View>

      <View style={styles.scanSection}>
        <View style={styles.scanCard}>
          <View style={styles.qrFrame}>
            <View style={[styles.corner, styles.cornerTL]} />
            <View style={[styles.corner, styles.cornerTR]} />
            <View style={[styles.corner, styles.cornerBL]} />
            <View style={[styles.corner, styles.cornerBR]} />
            <View style={styles.qrInner}>
              <Ionicons name="qr-code-outline" size={52} color={Colors.text.tertiary} />
            </View>
          </View>
          <Text style={styles.scanTitle}>Scan your table QR code</Text>
          <Text style={styles.scanSub}>
            A QR code is provided at your table or room.{`\n`}Point your camera at the code to begin ordering.
          </Text>
        </View>
      </View>

      <View style={styles.footer}>
        <View style={styles.dividerRow}>
          <View style={styles.dividerLine} />
          <Text style={styles.dividerText}>or</Text>
          <View style={styles.dividerLine} />
        </View>

        <TouchableOpacity
          style={[styles.demoBtn, isLoading && styles.demoBtnLoading]}
          onPress={handleDemoLogin}
          disabled={isLoading}
          activeOpacity={0.82}
        >
          {isLoading ? (
            <ActivityIndicator color={Colors.text.onPrimary} size="small" />
          ) : (
            <>
              <Ionicons name="flash-outline" size={18} color={Colors.text.onPrimary} style={{ marginRight: 8 }} />
              <Text style={styles.demoBtnLabel}>Continue as Demo Guest</Text>
            </>
          )}
        </TouchableOpacity>

        <Text style={styles.demoNote}>Room 404 · George Sladkovsky · Development mode</Text>
      </View>
    </SafeAreaView>
  );
}

const CORNER = 22;
const THICKNESS = 3;

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
    paddingHorizontal: Spacing.xl,
    justifyContent: 'space-between',
    paddingVertical: Spacing.xxxl,
  },
  brandArea: { alignItems: 'center', gap: Spacing.sm, paddingTop: Spacing.xl },
  logoCircle: {
    width: 72,
    height: 72,
    borderRadius: 36,
    backgroundColor: Colors.brand,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: Spacing.sm,
    ...Shadow.elevated,
  },
  brandName: { fontSize: FontSizes.xxxl, fontWeight: FontWeights.heavy, color: Colors.text.primary, letterSpacing: -0.5 },
  tagline: { fontSize: FontSizes.md, color: Colors.text.secondary, letterSpacing: 0.2 },
  scanSection: { alignItems: 'center' },
  scanCard: {
    backgroundColor: Colors.surface,
    borderRadius: Radius.xl,
    padding: Spacing.xxl,
    alignItems: 'center',
    gap: Spacing.md,
    width: '100%',
    ...Shadow.card,
  },
  qrFrame: {
    width: 190,
    height: 190,
    backgroundColor: Colors.background,
    borderRadius: Radius.lg,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: Spacing.sm,
  },
  qrInner: { alignItems: 'center', justifyContent: 'center' },
  corner:   { position: 'absolute', width: CORNER, height: CORNER, borderColor: Colors.brand },
  cornerTL: { top: 14, left: 14, borderTopWidth: THICKNESS, borderLeftWidth: THICKNESS, borderTopLeftRadius: 6 },
  cornerTR: { top: 14, right: 14, borderTopWidth: THICKNESS, borderRightWidth: THICKNESS, borderTopRightRadius: 6 },
  cornerBL: { bottom: 14, left: 14, borderBottomWidth: THICKNESS, borderLeftWidth: THICKNESS, borderBottomLeftRadius: 6 },
  cornerBR: { bottom: 14, right: 14, borderBottomWidth: THICKNESS, borderRightWidth: THICKNESS, borderBottomRightRadius: 6 },
  scanTitle: { fontSize: FontSizes.lg, fontWeight: FontWeights.bold, color: Colors.text.primary },
  scanSub:   { fontSize: FontSizes.sm, color: Colors.text.secondary, textAlign: 'center', lineHeight: 20 },
  footer: { gap: Spacing.md },
  dividerRow: { flexDirection: 'row', alignItems: 'center', gap: Spacing.md },
  dividerLine: { flex: 1, height: StyleSheet.hairlineWidth, backgroundColor: Colors.border },
  dividerText: { fontSize: FontSizes.sm, color: Colors.text.tertiary },
  demoBtn: {
    backgroundColor: Colors.brand,
    height: 54,
    borderRadius: Radius.md,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    ...Shadow.elevated,
  },
  demoBtnLoading: { opacity: 0.7 },
  demoBtnLabel: { fontSize: FontSizes.md, fontWeight: FontWeights.semibold, color: Colors.text.onPrimary, letterSpacing: 0.2 },
  demoNote: { fontSize: FontSizes.xs, color: Colors.text.placeholder, textAlign: 'center' },
});
