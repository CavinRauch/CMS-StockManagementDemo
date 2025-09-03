import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { PagedResult, StockDetailDto, StockListItemDto, CreateStockRequest, UpdateStockRequest } from '../shared/models/stock.models';

@Injectable({ providedIn: 'root' })
export class StockApi {
  private http = inject(HttpClient);
  private base = `${environment.api}/stock`;

  list(opts: { page?: number; pageSize?: number; q?: string; minYear?: number; maxPrice?: number; sort?: string; }) {
    let params = new HttpParams();
    for (const [k, v] of Object.entries(opts)) if (v !== undefined && v !== null && v !== '') params = params.set(k, String(v));
    return this.http.get<PagedResult<StockListItemDto>>(this.base, { params });
  }

  get(id: number) { 
    return this.http.get<StockDetailDto>(`${this.base}/${id}`); 
  }

  create(body: CreateStockRequest) { 
    return this.http.post<{ id: number }>(this.base, body); 
  }

  update(id: number, body: UpdateStockRequest) { 
    return this.http.put<void>(`${this.base}/${id}`, body); 
  }
  
  remove(id: number) { 
    return this.http.delete<void>(`${this.base}/${id}`); 
  }

  uploadImage(id: number, file: File, name: string, isPrimary: boolean) {
    const form = new FormData();
    form.append('file', file);
    form.append('name', name);
    form.append('isPrimary', String(isPrimary));
    return this.http.post<{ imageId: number }>(`${this.base}/${id}/images`, form);
  }

  imageUrl(id: number, imageId: number) { 
    return `${this.base}/${id}/images/${imageId}`; 
  }
  
  primaryImageUrl(id: number) { 
    return `${this.base}/${id}/primary-image`; 
  }

  setPrimaryImage(stockId: number, imageId: number) {
    return this.http.put<void>(`${this.base}/${stockId}/images/${imageId}/primary`, {});
  }

  deleteImage(stockId: number, imageId: number) { 
    return this.http.delete<void>(`${this.base}/${stockId}/images/${imageId}`); 
  }
}