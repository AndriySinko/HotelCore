import React, { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  TextInput,
  Alert,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { CartItem } from '../../types/cart';
import { Colors, FontSizes, FontWeights, Radius, Spacing, Shadow } from '../../theme';

interface CartItemRowProps {
  item: CartItem;
  onIncrease: () => void;
  onDecrease: () => void;
  onRemove: () => void;
  onNoteChange: (note: string) => void;
}

export function CartItemRow({ item, onIncrease, onDecrease, onRemove, onNoteChange }: CartItemRowProps) {
  const [editingNote, setEditingNote] = useState(false);
  const [draftNote, setDraftNote] = useState(item.note);
  const lineTotal = (item.menuItem.price * item.quantity).toFixed(2);

  const handleSaveNote = () => { onNoteChange(draftNote); setEditingNote(false); };
  const handleCancelNote = () => { setDraftNote(item.note); setEditingNote(false); };
  const handleRemove = () => {
    Alert.alert('Remove item', `Remove ${item.menuItem.name} from cart?`, [
      { text: 'Cancel', style: 'cancel' },
      { text: 'Remove', style: 'destructive', onPress: onRemove },
    ]);
  };

  return (
    <View style={styles.card}>
      <View style={styles.topRow}>
        <View style={styles.nameBlock}>
          <Text style={styles.name}>{item.menuItem.name}</Text>
          <Text style={styles.unitPrice}>€{item.menuItem.price.toFixed(2)} each</Text>
        </View>
        <TouchableOpacity onPress={handleRemove} style={styles.removeBtn} activeOpacity={0.7}>
          <Ionicons name="trash-outline" size={18} color={Colors.text.tertiary} />
        </TouchableOpacity>
      </View>

      <View style={styles.qtyRow}>
        <View style={styles.stepper}>
          <TouchableOpacity onPress={onDecrease} style={styles.stepBtn} activeOpacity={0.75}>
            <Ionicons name="remove" size={14} color={Colors.text.primary} />
          </TouchableOpacity>
          <Text style={styles.qtyText}>{item.quantity}</Text>
          <TouchableOpacity onPress={onIncrease} style={styles.stepBtn} activeOpacity={0.75}>
            <Ionicons name="add" size={14} color={Colors.text.primary} />
          </TouchableOpacity>
        </View>
        <Text style={styles.lineTotal}>€{lineTotal}</Text>
      </View>

      <View style={styles.noteSep} />
      <View style={styles.noteSection}>
        <View style={styles.noteLabelRow}>
          <Ionicons name="chatbubble-ellipses-outline" size={14} color={Colors.text.tertiary} />
          <Text style={styles.noteLabel}>Special request</Text>
          {!editingNote && (
            <TouchableOpacity onPress={() => setEditingNote(true)} activeOpacity={0.7}
              hitSlop={{ top: 8, bottom: 8, left: 8, right: 8 }}>
              <Text style={styles.editText}>Edit</Text>
            </TouchableOpacity>
          )}
        </View>
        {editingNote ? (
          <View style={styles.noteEditBlock}>
            <TextInput
              style={styles.noteInput}
              value={draftNote}
              onChangeText={setDraftNote}
              placeholder="e.g. no onions, extra sauce…"
              placeholderTextColor={Colors.text.placeholder}
              multiline
              autoFocus
              maxLength={200}
            />
            <View style={styles.noteActions}>
              <TouchableOpacity onPress={handleSaveNote} style={styles.saveBtn} activeOpacity={0.8}>
                <Text style={styles.saveBtnLabel}>Save</Text>
              </TouchableOpacity>
              <TouchableOpacity onPress={handleCancelNote} style={styles.cancelBtn} activeOpacity={0.8}>
                <Text style={styles.cancelBtnLabel}>Cancel</Text>
              </TouchableOpacity>
            </View>
          </View>
        ) : (
          <Text style={styles.noteValue}>{item.note.trim() ? item.note : 'None'}</Text>
        )}
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  card: { backgroundColor: Colors.surface, borderRadius: Radius.lg, padding: Spacing.lg, gap: Spacing.sm, ...Shadow.card },
  topRow: { flexDirection: 'row', alignItems: 'flex-start', gap: Spacing.sm },
  nameBlock: { flex: 1, gap: 3 },
  name: { fontSize: FontSizes.md, fontWeight: FontWeights.semibold, color: Colors.text.primary },
  unitPrice: { fontSize: FontSizes.sm, color: Colors.text.secondary },
  removeBtn: { padding: 4, marginTop: -4, marginRight: -4 },
  qtyRow: { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between' },
  stepper: { flexDirection: 'row', alignItems: 'center', gap: Spacing.md, backgroundColor: Colors.background, borderRadius: Radius.full, paddingHorizontal: Spacing.sm, paddingVertical: 5 },
  stepBtn: { padding: Spacing.xs },
  qtyText: { fontSize: FontSizes.md, fontWeight: FontWeights.bold, color: Colors.text.primary, minWidth: 22, textAlign: 'center' },
  lineTotal: { fontSize: FontSizes.md, fontWeight: FontWeights.bold, color: Colors.text.primary },
  noteSep: { height: StyleSheet.hairlineWidth, backgroundColor: Colors.separator },
  noteSection: { gap: Spacing.xs },
  noteLabelRow: { flexDirection: 'row', alignItems: 'center', gap: Spacing.xs },
  noteLabel: { flex: 1, fontSize: FontSizes.xs, fontWeight: FontWeights.semibold, color: Colors.text.secondary, textTransform: 'uppercase', letterSpacing: 0.4 },
  editText: { fontSize: FontSizes.sm, color: Colors.brand, fontWeight: FontWeights.medium },
  noteValue: { fontSize: FontSizes.sm, color: Colors.text.secondary },
  noteEditBlock: { gap: Spacing.sm },
  noteInput: { borderWidth: 1, borderColor: Colors.border, borderRadius: Radius.sm, padding: Spacing.md, fontSize: FontSizes.sm, color: Colors.text.primary, minHeight: 68, textAlignVertical: 'top', backgroundColor: Colors.background },
  noteActions: { flexDirection: 'row', gap: Spacing.sm },
  saveBtn: { backgroundColor: Colors.brand, borderRadius: Radius.sm, paddingHorizontal: Spacing.lg, paddingVertical: Spacing.sm },
  saveBtnLabel: { fontSize: FontSizes.sm, fontWeight: FontWeights.semibold, color: Colors.text.onPrimary },
  cancelBtn: { backgroundColor: Colors.buttonSecondary, borderRadius: Radius.sm, paddingHorizontal: Spacing.lg, paddingVertical: Spacing.sm },
  cancelBtnLabel: { fontSize: FontSizes.sm, fontWeight: FontWeights.medium, color: Colors.text.secondary },
});
