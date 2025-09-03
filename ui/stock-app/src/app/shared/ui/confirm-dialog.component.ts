import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

export interface ConfirmData { title?: string; message?: string; confirmText?: string; cancelText?: string; }

@Component({
  standalone: true,
  selector: 'app-confirm-dialog',
  imports: [CommonModule, MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>{{ data.title || 'Are you sure?' }}</h2>
    <div mat-dialog-content>{{ data.message || 'This action cannot be undone.' }}</div>
    <div mat-dialog-actions align="end">
      <button mat-button (click)="ref.close(false)">{{ data.cancelText || 'Cancel' }}</button>
      <button mat-raised-button color="warn" (click)="ref.close(true)">{{ data.confirmText || 'Delete' }}</button>
    </div>
  `
})
export class ConfirmDialogComponent {
  ref = inject(MatDialogRef<ConfirmDialogComponent>);
  data = inject<ConfirmData>(MAT_DIALOG_DATA);
}
