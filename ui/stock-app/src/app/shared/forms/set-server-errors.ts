import { AbstractControl, FormArray, FormGroup } from '@angular/forms';

/**
 * Applies ASP.NET/FluentValidation errors to an Angular form.
 * Supports keys like "RegNo", "Accessories[0].Name".
 */
export function setServerErrors(
  form: FormGroup,
  errors: Record<string, string[]>
) {
  // clear previous server errors
  clearServerErrors(form);

  for (const [key, messages] of Object.entries(errors)) {
    const path = key
      .replaceAll('$.', '')             // if keys come as "$.RegNo"
      .replaceAll('[', '.')
      .replaceAll(']', '')
      .split('.')
      .filter(Boolean);

    let control: AbstractControl | null = form;
    for (const seg of path) {
      if (!control) break;
      if (control instanceof FormArray) {
        const idx = Number(seg);
        control = control.at(idx) ?? null;
      } else if (control instanceof FormGroup) {
        control = control.get(seg);
      } else {
        control = null;
      }
    }

    if (control) {
      const existing = control.getError('server');
      const next = Array.isArray(existing) ? [...existing, ...messages] : messages;
      control.setErrors({ ...(control.errors || {}), server: next });
      control.markAsTouched();
    } else {
      // Form-level error when control isn't found
      form.setErrors({ ...(form.errors || {}), server: (form.errors?.['server'] ?? []).concat(messages) });
    }
  }
}

export function clearServerErrors(ctrl: AbstractControl) {
  if (ctrl instanceof FormGroup || ctrl instanceof FormArray) {
    Object.values(ctrl.controls).forEach(clearServerErrors);
  }
  if (ctrl.errors?.['server']) {
    const { server, ...rest } = ctrl.errors!;
    ctrl.setErrors(Object.keys(rest).length ? rest : null);
  }
}
