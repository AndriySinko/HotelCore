import { apiClient } from './apiClient';
import { Category, MenuItem } from '../types/menu';

interface CategoryDto {
  id: string;
  name: string;
}

interface ProductDto {
  id: string;
  name: string;
  description: string;
  price: number;
  isAvailable: boolean;
  imageUrl: string | null;
  categoryId: string;
  categoryName: string;
}

export const menuApi = {
  async getCategories(): Promise<Category[]> {
    const res = await apiClient.get<CategoryDto[]>('/restaurant/categories');
    return res.data;
  },

  async getMenuItems(categoryId?: string | null): Promise<MenuItem[]> {
    const params = categoryId ? { categoryId } : {};
    const res = await apiClient.get<ProductDto[]>('/restaurant/products', { params });
    return res.data.map((p) => ({
      id: p.id,
      categoryId: p.categoryId,
      categoryName: p.categoryName,
      name: p.name,
      description: p.description,
      price: p.price,
      imageUrl: p.imageUrl ?? undefined,
      isAvailable: p.isAvailable,
    }));
  },

  isRestaurantOpen(): Promise<boolean> {
    return Promise.resolve(true);
  },
};
