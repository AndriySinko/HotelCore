import React from 'react';
import { Platform, StyleSheet } from 'react-native';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { Ionicons } from '@expo/vector-icons';
import { TabParamList, AppStackParamList } from '../types/navigation';
import { Colors, Spacing } from '../theme';
import { HomeScreen } from '../screens/home/HomeScreen';
import { RestaurantScreen } from '../screens/restaurant/RestaurantScreen';
import { SettingsScreen } from '../screens/settings/SettingsScreen';
import { OrderHistoryScreen } from '../screens/orders/OrderHistoryScreen';
import { CartScreen } from '../screens/cart/CartScreen';
import { ConfirmOrderScreen } from '../screens/order/ConfirmOrderScreen';
import { OrderStatusScreen } from '../screens/order/OrderStatusScreen';
import { useCartStore } from '../store/cartStore';

const Tab = createBottomTabNavigator<TabParamList>();
const AppStack = createNativeStackNavigator<AppStackParamList>();

function TabNavigator() {
  const totalItems = useCartStore((s) => s.getTotalItems());

  return (
    <Tab.Navigator
      screenOptions={({ route }) => ({
        headerShown: false,
        tabBarStyle: styles.tabBar,
        tabBarActiveTintColor: Colors.tabBarActive,
        tabBarInactiveTintColor: Colors.tabBarInactive,
        tabBarLabelStyle: styles.tabLabel,
        tabBarIcon: ({ color, size, focused }) => {
          let iconName: keyof typeof Ionicons.glyphMap;
          if (route.name === 'Home') {
            iconName = focused ? 'home' : 'home-outline';
          } else if (route.name === 'Restaurant') {
            iconName = focused ? 'restaurant' : 'restaurant-outline';
          } else if (route.name === 'Orders') {
            iconName = focused ? 'receipt' : 'receipt-outline';
          } else {
            iconName = focused ? 'settings' : 'settings-outline';
          }
          return <Ionicons name={iconName} size={size} color={color} />;
        },
      })}
    >
      <Tab.Screen name="Home" component={HomeScreen} options={{ title: 'Home' }} />
      <Tab.Screen
        name="Restaurant"
        component={RestaurantScreen}
        options={{
          title: 'Menu',
          tabBarBadge: totalItems > 0 ? totalItems : undefined,
          tabBarBadgeStyle: styles.badge,
        }}
      />
      <Tab.Screen name="Orders" component={OrderHistoryScreen} options={{ title: 'Orders' }} />
      <Tab.Screen name="Settings" component={SettingsScreen} options={{ title: 'Settings' }} />
    </Tab.Navigator>
  );
}

export function AppNavigator() {
  return (
    <AppStack.Navigator screenOptions={{ headerShown: false, animation: 'slide_from_right' }}>
      <AppStack.Screen name="Tabs" component={TabNavigator} />
      <AppStack.Screen name="Cart" component={CartScreen} options={{ animation: 'slide_from_bottom' }} />
      <AppStack.Screen name="ConfirmOrder" component={ConfirmOrderScreen} />
      <AppStack.Screen name="OrderStatus" component={OrderStatusScreen} />
    </AppStack.Navigator>
  );
}

const styles = StyleSheet.create({
  tabBar: {
    backgroundColor: Colors.tabBar,
    borderTopColor: Colors.tabBarBorder,
    borderTopWidth: StyleSheet.hairlineWidth,
    height: Platform.OS === 'ios' ? 83 : 60,
    paddingBottom: Platform.OS === 'ios' ? 28 : Spacing.sm,
    paddingTop: Spacing.sm,
  },
  tabLabel: { fontSize: 11, fontWeight: '500' },
  badge: {
    backgroundColor: Colors.primary,
    color: Colors.text.onPrimary,
    fontSize: 10,
  },
});
