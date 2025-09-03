import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators, FormArray, FormGroup } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { finalize } from 'rxjs/operators';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../shared/ui/confirm-dialog.component';
import { StockApi } from '../stock.api';
import { StockDetailDto, CreateAccessory, UpdateStockRequest } from '../../shared/models/stock.models';
import { setServerErrors } from '../../shared/forms/set-server-errors';

@Component({
  standalone: true,
  selector: 'app-stock-detail',
  imports: [
    CommonModule, ReactiveFormsModule,
    MatCardModule, MatFormFieldModule, MatInputModule,
    MatButtonModule, MatIconModule, MatDividerModule, MatSnackBarModule
  ],
  template: `
  <div class="page">
    <mat-card class="card" *ngIf="vm(); else loading" appearance="outlined">
      <mat-card-header class="card-header">
        <mat-card-title>
          <h2>{{ isCreate() ? 'New stock item' :  vm()!.make + ' ' + vm()!.model + ' ' + vm()!.modelYear}}</h2>
        </mat-card-title>
      </mat-card-header>

      <!-- Main form -->
      <mat-card-content>
        <form [formGroup]="form" class="grid" *ngIf="vm() as m">
          <mat-form-field appearance="outline">
            <mat-label>Registration</mat-label>
            <input matInput formControlName="regNo">
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Make</mat-label>
            <input matInput formControlName="make">
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Model</mat-label>
            <input matInput formControlName="model">
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Model year</mat-label>
            <input matInput type="number" formControlName="modelYear">
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Kilometers</mat-label>
            <input matInput type="number" formControlName="kms">
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Colour</mat-label>
            <input matInput formControlName="colour">
          </mat-form-field>

          <mat-form-field appearance="outline" class="span-2">
            <mat-label>VIN</mat-label>
            <input matInput formControlName="vin">
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Retail price</mat-label>
            <input matInput type="number" formControlName="retailPrice">
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>Cost price</mat-label>
            <input matInput type="number" formControlName="costPrice">
          </mat-form-field>

          <!-- Accessories -->
          <div class="span-2">
            <h3>Accessories</h3>
            <div class="acc-list" formArrayName="accessories">
              <div class="acc-row" *ngFor="let _ of acc.controls; let i = index" [formGroupName]="i">
                <mat-form-field appearance="fill" class="flex">
                  <mat-label>Name</mat-label>
                  <input matInput formControlName="name">
                  <mat-error *ngIf="acc.at(i).get('name')?.hasError('server')">
                    {{ acc.at(i).get('name')?.getError('server')[0] }}
                  </mat-error>
                </mat-form-field>

                <mat-form-field appearance="fill" class="flex">
                  <mat-label>Description</mat-label>
                  <input matInput formControlName="description" >
                  <mat-error *ngIf="acc.at(i).get('description')?.hasError('server')">
                    {{ acc.at(i).get('description')?.getError('server')[0] }}
                  </mat-error>
                </mat-form-field>

                <button mat-icon-button color="warn" (click)="removeAccessory(i)" aria-label="Remove">
                  <mat-icon>delete</mat-icon>
                </button>
              </div>
              
              <ng-template [ngIf]="!acc.length">
                <p class="muted">No accessories.</p>
              </ng-template>
            </div>

            <button mat-stroked-button (click)="addAccessory()">
              <mat-icon>add</mat-icon> Add accessory
            </button>
          </div>
        </form>

        <!-- Images -->
        <div class="section images" *ngIf="vm() as m">
          <h3>Images</h3>

          <div class="thumbs">
            <figure class="thumb" *ngFor="let img of m.images">
              <img [src]="api.imageUrl(m.id, img.id)" [alt]="img.name" loading="lazy">
              <figcaption>
                <span [class.primary]="img.isPrimary">{{ img.name }}</span>
                <span class="size">{{ img.bytesLength | number }} B</span>
              </figcaption>
              <div class="thumb-actions">
                <button mat-stroked-button color="primary"
                        (click)="markPrimary(img.id)"
                        [disabled]="busy() || img.isPrimary">
                  <mat-icon>star</mat-icon> {{ img.isPrimary ? 'Primary' : 'Make primary' }}
                </button>

                <button mat-stroked-button color="warn"
                        (click)="deleteImage(img.id)"
                        [disabled]="busy()">
                  <mat-icon>delete</mat-icon> Remove
                </button>
              </div>
            </figure>
          </div>

          <div class="upload">
            <input type="file" (change)="upload($event)" [disabled]="busy()">
            <button mat-stroked-button (click)="togglePrimaryNext()" [disabled]="busy()">
              <mat-icon>star</mat-icon>
              Next upload is primary: {{ markPrimaryNext() ? 'Yes' : 'No' }}
            </button>
          </div>
        </div>
      </mat-card-content>

      <mat-card-footer align="end" class="form-actions">
        <button mat-raised-button color="warn" *ngIf="!isCreate()" (click)="delete()" [disabled]="busy()">Delete</button>
        <button mat-raised-button color="primary" (click)="save()" [disabled]="form.invalid || busy()">Save</button>
      </mat-card-footer>
    </mat-card>

    <ng-template #loading>
      <mat-card class="card">
        <p>Loading…</p>
      </mat-card>
    </ng-template>
  </div>
  `,
  styles: [`
    .page { display:grid; place-items:start center; padding:24px; }
    .card { width:100%; max-width:1100px; padding:16px; padding-top:0; }
    .card-header { margin-bottom:12px; }
    .header { display:flex; align-items:center; gap:12px; }
    .header h2 { flex:1; }
    .actions { display:flex; gap:8px; }
    .grid {
      display:grid;
      gap:12px;
      grid-template-columns: repeat(2, minmax(260px, 1fr));
    }
    .span-2 { grid-column: span 2; }
    .section { padding:16px 0; }
    .acc-list { display:flex; flex-direction:column; gap:10px; margin-bottom:8px; }
    .acc-row { display:flex; gap:10px; align-items:center; }
    .flex { flex:1; }
    .muted { opacity:.7; }
    .images .thumbs { display:flex; flex-wrap:wrap; gap:12px; }
    .thumb { width:180px; }
    .thumb img { width:180px; height:120px; object-fit:cover; border-radius:8px; background:rgba(0,0,0,.06); }
    .thumb figcaption { display:flex; justify-content:space-between; font-size:.85rem; margin-top:4px; }
    .thumb-actions { display: flex; gap: 8px; margin-top: 6px; }
    .primary { font-weight:600; }
    .upload { display:flex; align-items:center; gap:12px; margin-top:12px; }
    .form-actions button {  margin-left:10px; }
  `]
})
export class StockDetailComponent {
  api = inject(StockApi);
  private dialog = inject(MatDialog);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private snack = inject(MatSnackBar);

  vm = signal<StockDetailDto | null>(null);
  busy = signal(false);
  isCreate = signal(false);
  markPrimaryNext = signal(false);

  form = this.fb.group({
    regNo: ['', [Validators.required, Validators.maxLength(32)]],
    make: ['', [Validators.required, Validators.maxLength(64)]],
    model: ['', [Validators.required, Validators.maxLength(64)]],
    modelYear: [2020, [Validators.required, Validators.min(1950), Validators.max(new Date().getFullYear() + 1)]],
    kms: [0, [Validators.required, Validators.min(0)]],
    colour: ['', [Validators.required, Validators.maxLength(32)]],
    vin: ['', [Validators.required, Validators.maxLength(32)]],
    retailPrice: [0, [Validators.required, Validators.min(0)]],
    costPrice: [0, [Validators.required, Validators.min(0)]],
    accessories: this.fb.array<FormGroup>([])   // ✅ proper type
  });

  get acc(): FormArray<FormGroup> {
    return this.form.get('accessories') as FormArray<FormGroup>;
  }

  constructor() {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (!id) { 
      this.isCreate.set(true)
      this.busy.set(false);
      this.vm.set(this.form.value as StockDetailDto);
      return;
    }
        
    this.load(id); 
  }

  private load(id: number) {
    this.busy.set(true);
    this.api.get(id).pipe(finalize(() => this.busy.set(false)))
      .subscribe({
        next: dto => { 
          this.vm.set(dto); 
          this.patchForm(dto); 
          this.isCreate.set(false);
        },
        error: () => this.snack.open('Failed to load stock item', 'Dismiss', { duration: 2500 })
      });
  }

  private patchForm(m: StockDetailDto) {
    // Reset accessories
    while (this.acc.length) this.acc.removeAt(0);
    for (const a of m.accessories) {
      this.acc.push(this.fb.group({
        name: [a.name, [Validators.required, Validators.maxLength(64)]],
        description: [a.description ?? '', [Validators.maxLength(256)]]
      }));
    }
    this.form.patchValue({
      regNo: m.regNo, make: m.make, model: m.model, modelYear: m.modelYear,
      kms: m.kms, colour: m.colour, vin: m.vin,
      retailPrice: m.retailPrice, costPrice: m.costPrice
    }, { emitEvent: false });
  }

  addAccessory() {
    this.acc.push(this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(64)]],
      description: ['', [Validators.maxLength(256)]]
    }));
  }
  removeAccessory(i: number) { this.acc.removeAt(i); }

 delete() {
    const m = this.vm(); if (!m) return;

    const ref = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete stock item',
        message: `Are you sure you want to delete “${m.make} ${m.model} (${m.modelYear})”?`,
        confirmText: 'Delete',
        cancelText: 'Cancel'
      }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (!confirmed) return;

      this.busy.set(true);
      this.api.remove(m.id).subscribe({
        next: () => {
          this.busy.set(false);
          this.snack.open('Deleted', 'OK', { duration: 1500 });
          this.router.navigate(['/stock']);
        },
        error: () => {
          this.busy.set(false);
          this.snack.open('Delete failed', 'Dismiss', { duration: 3000 });
        }
      });
    });
  }

  save() {
    if (this.form.invalid || !this.vm()) return;
    const id = this.vm()!.id;

    const req: UpdateStockRequest = {
      regNo: String(this.form.value.regNo),
      make: String(this.form.value.make),
      model: String(this.form.value.model),
      modelYear: Number(this.form.value.modelYear),
      kms: Number(this.form.value.kms),
      colour: String(this.form.value.colour),
      vin: String(this.form.value.vin,),
      retailPrice: Number(this.form.value.retailPrice),
      costPrice: Number(this.form.value.costPrice),
      accessories: (this.acc.getRawValue() as { name: string; description?: string }[])
          .map(a => ({ name: a.name, description: a.description ?? null })) as CreateAccessory[]
    };    

    this.busy.set(true);
    if (!this.isCreate()) {
      this.api.update(id, req)
        .pipe(finalize(() => this.busy.set(false)))
        .subscribe({
          next: () => { 
            this.snack.open('Edited', 'OK', { duration: 1500 }); 
            this.load(id); 
          },
          error: err => { setServerErrors(this.form, err.validation) }
        });
    }
    else
    {
      this.api.create(req)
        .pipe(finalize(() => this.busy.set(false)))
        .subscribe({
          next: (data) => { 
            this.snack.open('Created', 'OK', { duration: 1500 }); 
            this.router.navigate(['/stock', data.id]);
          },
          error: err => { setServerErrors(this.form, err.validation) }
        });
    }
  }

  // ---- Images ----
  togglePrimaryNext() { this.markPrimaryNext.set(!this.markPrimaryNext()); }

  upload(evt: Event) {
    const input = evt.target as HTMLInputElement;
    if (!input.files || !this.vm()) return;
    const file = input.files[0];
    const id = this.vm()!.id;

    this.busy.set(true);
    this.api.uploadImage(id, file, file.name, this.markPrimaryNext())
      .pipe(finalize(() => { 
        this.busy.set(false); 
        this.markPrimaryNext.set(false); input.value = ''; 
      }))
      .subscribe({ next: () => this.load(id) });
  }
  
  markPrimary(imageId: number) {
    const m = this.vm(); if (!m) return;

    this.busy.set(true);
    this.api.setPrimaryImage(m.id, imageId)
      .pipe(finalize(() => this.busy.set(false)))
      .subscribe({
        next: () => {
          this.snack.open('Primary image updated', 'OK', { duration: 1500 });
          this.load(m.id); // refresh
        },
        error: () => this.snack.open('Failed to update primary image', 'Dismiss', { duration: 3000 })
      });
  }

  deleteImage(imageId: number) {
    const m = this.vm(); if (!m) return;

    const ref = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Remove image',
        message: 'Are you sure you want to remove this image?',
        confirmText: 'Remove',
        cancelText: 'Cancel'
      }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (!confirmed) return;

      this.busy.set(true);
      this.api.deleteImage(m.id, imageId).subscribe({
        next: () => {
          this.busy.set(false);
          this.snack.open('Image removed', 'OK', { duration: 1500 });
          this.load(m.id); // refresh images
        },
        error: () => {
          this.busy.set(false);
          this.snack.open('Failed to remove image', 'Dismiss', { duration: 3000 });
        }
      });
    });
  }
}