import React, { useCallback, useRef, useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  ActivityIndicator,
  Alert,
  Modal,
  StatusBar,
  Dimensions,
} from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { Ionicons } from '@expo/vector-icons';
import { CameraView, useCameraPermissions } from 'expo-camera';
import { LoginScreenProps } from '../../types/navigation';
import { useAuthStore } from '../../store/authStore';
import { Colors, FontSizes, FontWeights, Spacing, Radius, Shadow } from '../../theme';

const SCAN_SIZE = 260;
const { width: SCREEN_W, height: SCREEN_H } = Dimensions.get('window');
const SIDE_W = (SCREEN_W - SCAN_SIZE) / 2;

export function LoginScreen(_props: LoginScreenProps) {
  const { loginWithQrCode, isLoading, error, clearError } = useAuthStore();
  const [scanning, setScanning] = useState(false);
  const [permission, requestPermission] = useCameraPermissions();
  const hasScanned = useRef(false);

  const handleDemoLogin = useCallback(async () => {
    clearError();
    await loginWithQrCode('demo');
  }, [loginWithQrCode, clearError]);

  const handleScanPress = useCallback(async () => {
    if (!permission?.granted) {
      const result = await requestPermission();
      if (!result.granted) {
        Alert.alert(
          'Camera Access Required',
          'Please enable camera access in your device settings to scan QR codes.',
        );
        return;
      }
    }
    hasScanned.current = false;
    setScanning(true);
  }, [permission, requestPermission]);

  const handleBarcodeScanned = useCallback(
    async ({ data }: { data: string }) => {
      if (hasScanned.current) return;
      hasScanned.current = true;
      setScanning(false);
      clearError();
      await loginWithQrCode(data);
    },
    [loginWithQrCode, clearError],
  );

  React.useEffect(() => {
    if (error) {
      Alert.alert('Login Failed', error, [{ text: 'OK', onPress: clearError }]);
    }
  }, [error, clearError]);

  return (
    <>
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

            <TouchableOpacity
              style={[styles.scanBtn, isLoading && styles.scanBtnDisabled]}
              onPress={handleScanPress}
              disabled={isLoading}
              activeOpacity={0.82}
            >
              <Ionicons name="camera-outline" size={20} color={Colors.brand} style={{ marginRight: 8 }} />
              <Text style={styles.scanBtnLabel}>Scan QR Code</Text>
            </TouchableOpacity>
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

      <Modal visible={scanning} animationType="slide" statusBarTranslucent onRequestClose={() => setScanning(false)}>
        <StatusBar barStyle="light-content" backgroundColor="transparent" translucent />
        <View style={styles.cameraContainer}>
          <CameraView
            style={StyleSheet.absoluteFill}
            facing="back"
            barcodeScannerSettings={{ barcodeTypes: ['qr'] }}
            onBarcodeScanned={handleBarcodeScanned}
          />

          {/* Dimmed overlay with transparent scan window */}
          <View style={styles.overlayTop} />
          <View style={styles.overlayMiddle}>
            <View style={styles.overlaySide} />
            <View style={styles.scanWindow}>
              <View style={[styles.camCorner, styles.cornerTL]} />
              <View style={[styles.camCorner, styles.cornerTR]} />
              <View style={[styles.camCorner, styles.cornerBL]} />
              <View style={[styles.camCorner, styles.cornerBR]} />
            </View>
            <View style={styles.overlaySide} />
          </View>
          <View style={styles.overlayBottom}>
            <Text style={styles.scanHint}>Point your camera at a QR code</Text>
            <TouchableOpacity onPress={() => setScanning(false)} style={styles.cancelBtn} activeOpacity={0.8}>
              <Text style={styles.cancelBtnText}>Cancel</Text>
            </TouchableOpacity>
          </View>
        </View>
      </Modal>
    </>
  );
}

const CORNER = 22;
const THICKNESS = 3;
const OVERLAY_COLOR = 'rgba(0,0,0,0.62)';

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
  scanBtn: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    borderWidth: 1.5,
    borderColor: Colors.brand,
    borderRadius: Radius.md,
    height: 48,
    paddingHorizontal: Spacing.xl,
    width: '100%',
    marginTop: Spacing.xs,
  },
  scanBtnDisabled: { opacity: 0.5 },
  scanBtnLabel: { fontSize: FontSizes.md, fontWeight: FontWeights.semibold, color: Colors.brand },
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

  // Camera modal
  cameraContainer: { flex: 1, backgroundColor: '#000' },
  overlayTop: { flex: 1, backgroundColor: OVERLAY_COLOR },
  overlayMiddle: { flexDirection: 'row', height: SCAN_SIZE },
  overlaySide: { width: SIDE_W, backgroundColor: OVERLAY_COLOR },
  scanWindow: { width: SCAN_SIZE, height: SCAN_SIZE },
  overlayBottom: {
    flex: 1,
    backgroundColor: OVERLAY_COLOR,
    alignItems: 'center',
    justifyContent: 'center',
    gap: Spacing.xl,
    paddingBottom: Spacing.xxxl,
  },
  camCorner: { position: 'absolute', width: 32, height: 32, borderColor: '#fff' },
  scanHint: { fontSize: FontSizes.md, color: '#fff', opacity: 0.85 },
  cancelBtn: {
    borderWidth: 1.5,
    borderColor: 'rgba(255,255,255,0.5)',
    borderRadius: Radius.md,
    paddingHorizontal: Spacing.xxl,
    paddingVertical: Spacing.md,
  },
  cancelBtnText: { fontSize: FontSizes.md, fontWeight: FontWeights.semibold, color: '#fff' },
});
