import { Component, ChangeDetectionStrategy, ViewChild, inject, signal, computed, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSort, MatSortModule, Sort } from '@angular/material/sort';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';

import { StockApi } from '../stock.api';
import { PagedResult, StockListItemDto } from '../../shared/models/stock.models';

@Component({
  standalone: true,
  selector: 'app-stock-list',
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule
  ],
  //
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
  <div class="page">
    <mat-card class="card" appearance="outlined">
      <mat-card-header>
        <mat-card-title>Stock</mat-card-title>
        <span class="spacer"></span>
        <section>        
          <button mat-flat-button color="primary" (click)="create()">
            Add new
          </button>          
          <mat-form-field appearance="outline" class="search">
            <mat-label>Search</mat-label>
            <input matInput
                  [value]="q()"
                  (input)="onSearch($any($event.target).value)"
                  placeholder="make, model, reg, VIN">
            <button *ngIf="q()" matSuffix mat-icon-button aria-label="Clear" (click)="clearSearch()">
              <mat-icon>close</mat-icon>
            </button>
          </mat-form-field>
        </section>
      </mat-card-header>

      <div class="table-wrap">
        <table mat-table [dataSource]="rows()" matSort (matSortChange)="onSort($event)" class="mat-elevation-z1">

          <ng-container matColumnDef="make">
            <th mat-header-cell *matHeaderCellDef mat-sort-header="make">Make</th>
            <td mat-cell *matCellDef="let r">{{ r.make }}</td>
          </ng-container>

          <ng-container matColumnDef="model">
            <th mat-header-cell *matHeaderCellDef mat-sort-header="model">Model</th>
            <td mat-cell *matCellDef="let r">{{ r.model }}</td>
          </ng-container>

          <ng-container matColumnDef="modelYear">
            <th mat-header-cell *matHeaderCellDef mat-sort-header="modelYear">Year</th>
            <td mat-cell *matCellDef="let r">{{ r.modelYear }}</td>
          </ng-container>

          <ng-container matColumnDef="price">
            <th mat-header-cell *matHeaderCellDef mat-sort-header="price">Price</th>
            <td mat-cell *matCellDef="let r">{{ r.retailPrice | currency }}</td>
          </ng-container>

          <ng-container matColumnDef="img">
            <th mat-header-cell *matHeaderCellDef class="img-th"></th>
            <td mat-cell *matCellDef="let r" class="img-td">
              <img [src]="api.primaryImageUrl(r.id)" alt="primary image" class="thumb" loading="lazy">
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="cols"></tr>
          <tr mat-row *matRowDef="let row; columns: cols;" (click)="open(row)"></tr>
        </table>
      </div>

      <mat-paginator
        [length]="total()"
        [pageIndex]="page()-1"
        [pageSize]="pageSize()"
        [pageSizeOptions]="pageSizeOptions()"
        (page)="onPage($event)">
      </mat-paginator>
    </mat-card>
  </div>
  `,
  styles: [`
    .page { display: grid; place-items: start center; padding: 24px; }
    .card { width: 100%; max-width: 1100px; padding: 12px 12px 4px; }
    .card-header { display: flex; align-items: center; gap: 16px; padding: 8px 8px 16px; }
    .card-header h2 { margin: 0; flex: 1; font-weight: 600; }
    .search { width: 320px; max-width: 100%; margin-left: 16px; }
    .table-wrap { overflow: auto; border-radius: 8px; }
    table { width: 100%; }
    th.mat-header-cell { white-space: nowrap; }
    .img-th { width: 96px; }
    .img-td { text-align: right; }
    .thumb { width: 96px; height: 72px; object-fit: cover; border-radius: 8px; background: rgba(0,0,0,.06); }
    tr.mat-row { cursor: pointer; }
    .spacer { flex: 1; }
  `]
})
export class StockListComponent {
  // --- signal inputs (configurable from parent) ---
  pageSizeOptions = input<number[]>([5, 10, 15, 20]);
  initialPageSize = input<number>(10);

  // --- services ---
  api = inject(StockApi);
  private router = inject(Router);

  // --- view refs (for firstPage reset) ---
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  // --- state (signals) ---
  q = signal('');
  page = signal(1);
  pageSize = signal(this.initialPageSize());
  sortParam = signal<string | null>(null);

  // data
  private items = signal<StockListItemDto[]>([]);
  private totalCount = signal(0);

  // expose to template
  rows = computed(() => this.items());
  total = computed(() => this.totalCount());

  cols: ReadonlyArray<string> = ['make', 'model', 'modelYear', 'price', 'img'];

  constructor() {
    // initial load
    this.load();
  }

  // --- actions ---
  private load() {
    this.api.list({
      page: this.page(),
      pageSize: this.pageSize(),
      q: this.q() || undefined,
      sort: this.sortParam() || undefined
    }).subscribe((res: PagedResult<StockListItemDto>) => {
      this.items.set(res.items);
      this.totalCount.set(res.total);
    });
  }

  onSearch(value: string) {
    this.q.set(value);
    this.page.set(1);
    this.load();
    this.paginator?.firstPage();
  }

  clearSearch() {
    this.q.set('');
    this.page.set(1);
    this.load();
    this.paginator?.firstPage();
  }

  onPage(e: PageEvent) {
    this.page.set(e.pageIndex + 1);
    this.pageSize.set(e.pageSize);
    this.load();
  }

  onSort(s: Sort) {
    if (!s.active || s.direction === '') {
      this.sortParam.set(null);
    } else {
      const key = s.active === 'modelYear' ? 'modelYear' : (s.active === 'price' ? 'price' : s.active);
      this.sortParam.set(s.direction === 'desc' ? `-${key}` : key);
    }
    this.page.set(1);
    this.load();
    this.paginator?.firstPage();
  }

  open(row: StockListItemDto) {
    this.router.navigate(['/stock', row.id]);
  }

  create() { 
    this.router.navigate(['/stock/new']); 
  }
}