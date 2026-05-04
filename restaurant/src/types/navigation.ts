import type { NativeStackScreenProps } from '@react-navigation/native-stack';
import type { BottomTabScreenProps } from '@react-navigation/bottom-tabs';
import type { CompositeScreenProps } from '@react-navigation/native';

export type RootStackParamList = {
  Auth: undefined;
  App: undefined;
};

export type AuthStackParamList = {
  Login: undefined;
};

export type AppStackParamList = {
  Tabs: undefined;
  Cart: undefined;
  ConfirmOrder: undefined;
  OrderStatus: { orderId: string };
};

export type TabParamList = {
  Home: undefined;
  Restaurant: undefined;
  Orders: undefined;
  Settings: undefined;
};

export type LoginScreenProps         = NativeStackScreenProps<AuthStackParamList, 'Login'>;
export type CartScreenProps          = NativeStackScreenProps<AppStackParamList, 'Cart'>;
export type ConfirmOrderScreenProps  = NativeStackScreenProps<AppStackParamList, 'ConfirmOrder'>;
export type OrderStatusScreenProps   = NativeStackScreenProps<AppStackParamList, 'OrderStatus'>;

export type HomeScreenProps = CompositeScreenProps<
  BottomTabScreenProps<TabParamList, 'Home'>,
  NativeStackScreenProps<AppStackParamList>
>;

export type RestaurantScreenProps = CompositeScreenProps<
  BottomTabScreenProps<TabParamList, 'Restaurant'>,
  NativeStackScreenProps<AppStackParamList>
>;

export type OrderHistoryScreenProps = CompositeScreenProps<
  BottomTabScreenProps<TabParamList, 'Orders'>,
  NativeStackScreenProps<AppStackParamList>
>;
