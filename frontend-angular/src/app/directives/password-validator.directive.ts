import { Directive } from '@angular/core';
import {
  AbstractControl,
  NG_VALIDATORS,
  ValidationErrors,
  Validator,
} from '@angular/forms';

@Directive({
  selector: '[appPasswordValidator]',
  providers: [
    {
      provide: NG_VALIDATORS,
      useExisting: PasswordValidatorDirective,
      multi: true,
    },
  ],
})
export class PasswordValidatorDirective implements Validator {
  validate(control: AbstractControl): ValidationErrors | null {
    const value = control.value || '';
    const errors: any = {};

    if (!/[A-Z]/.test(value)) {
      errors.uppercase = true;
    }
    if (!/\d/.test(value)) {
      errors.digit = true;
    }
    if (!/[^a-zA-Z0-9]/.test(value)) {
      errors.special = true;
    }

    return Object.keys(errors).length ? errors : null;
  }
}
