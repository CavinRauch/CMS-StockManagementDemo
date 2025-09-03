export interface PagedResult<T> { items: T[]; total: number; page: number; pageSize: number; }

export interface StockListItemDto { id: number; regNo: string; make: string; model: string; modelYear: number; kms: number; colour: string; retailPrice: number; primaryImageId?: number | null; }

export interface StockAccessoryDto { id: number; name: string; description?: string | null; }

export interface StockImageDto { id: number; name: string; isPrimary: boolean; contentType: string; bytesLength: number; }

export interface StockDetailDto extends StockListItemDto {
  vin: string; costPrice: number; dtCreated: string; dtUpdated: string;
  accessories: StockAccessoryDto[]; images: StockImageDto[];
}
export interface CreateAccessory { name: string; description?: string | null; }

export interface CreateStockRequest {
  regNo: string; make: string; model: string; modelYear: number; kms: number; colour: string;
  vin: string; retailPrice: number; costPrice: number; accessories: CreateAccessory[];
}
export interface UpdateStockRequest extends CreateStockRequest {}

export interface LoginResponse { accessToken: string; expiresAtUtc: string; roles: string[]; }

export interface StockQuery { page?: number; pageSize?: number; q?: string; minYear?: number; maxPrice?: number; sort?: string; }