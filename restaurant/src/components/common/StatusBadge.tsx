import React from 'react';
import { View, Text, StyleSheet, ViewStyle } from 'react-native';
import { Colors, FontSizes, FontWeights } from '../../theme';
import { OrderStatus, ORDER_STATUS_LABEL } from '../../types/order';

const STATUS_COLORS: Record<OrderStatus, string> = {
  in_progress: Colors.statusReceived,
  received:    Colors.statusReceived,
  preparing:   Colors.statusPreparing,
  on_the_way:  Colors.statusOnTheWay,
  delivered:   Colors.statusDelivered,
  cancelled:   Colors.statusCancelled,
};

interface StatusBadgeProps {
  status: OrderStatus;
  style?: ViewStyle;
}

export function StatusBadge({ status, style }: StatusBadgeProps) {
  const color = STATUS_COLORS[status];
  return (
    <View style={[styles.badge, { backgroundColor: `${color}18`, borderColor: `${color}50` }, style]}>
      <View style={[styles.dot, { backgroundColor: color }]} />
      <Text style={[styles.label, { color }]}>{ORDER_STATUS_LABEL[status]}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  badge: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 5,
    paddingHorizontal: 10,
    paddingVertical: 5,
    borderRadius: 20,
    borderWidth: 1,
    alignSelf: 'flex-start',
  },
  dot: { width: 6, height: 6, borderRadius: 3 },
  label: {
    fontSize: FontSizes.xs,
    fontWeight: FontWeights.semibold,
    letterSpacing: 0.3,
  },
});
