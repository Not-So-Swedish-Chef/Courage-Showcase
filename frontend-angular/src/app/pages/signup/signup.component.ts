import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css'],
})
export class SignupComponent {
  firstName = '';
  lastName = '';
  email = '';
  password = '';
  userType: 1 | 2 = 2; // default Member
  errorMsg = '';

  constructor(private auth: AuthService, private router: Router) {}

  async submit() {
    this.errorMsg = '';

    const result = await this.auth.register({
      email: this.email,
      password: this.password,
      firstName: this.firstName,
      lastName: this.lastName,
      userType: this.userType,
    });

    if (result.ok) {
      this.router.navigateByUrl('/');
    } else {
      if (Array.isArray(result.errors)) {
        this.errorMsg = result.errors.map((e: any) => e.description).join(', ');
      } else {
        this.errorMsg = 'Registration failed. Please try again.';
      }
    }
  }
}
