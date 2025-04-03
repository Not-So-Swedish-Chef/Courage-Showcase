import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/verification/authentication.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.scss'],
  standalone: false,
})
export class LoginPage implements OnInit {
  email: string = '';
  password: string = '';

  constructor(private auth_service: AuthenticationService) {}

  ngOnInit() {
  }


  login() {
    this.auth_service.login(this.email, this.password)
      .then(() => console.log('Login successful'))
      .catch(err => console.error('Login error:', err));
  }

  register() {
    this.auth_service.register(this.email, this.password)
      .then(() => console.log('Registration successful'))
      .catch(err => console.error('Registration error:', err));
  }

}
