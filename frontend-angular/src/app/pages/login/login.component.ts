import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  email = '';
  password = '';
  errorMsg = '';

  constructor(private auth: AuthService, private router: Router) {}

  async submit() {
    this.errorMsg = '';
    const ok = await this.auth.login({
      email: this.email,
      password: this.password,
    });

    if (ok) {
      this.router.navigateByUrl('/');
    } else {
      this.errorMsg = 'Invalid email or password.';
    }
  }
}
