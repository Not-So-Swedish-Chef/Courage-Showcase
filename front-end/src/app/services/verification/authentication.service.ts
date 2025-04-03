import { Injectable } from '@angular/core';
import { AngularFireAuth } from '@angular/fire/compat/auth';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  constructor(private angular_fire_auth : AngularFireAuth) { }

  register(email: string, password: string) {
    return this.angular_fire_auth.createUserWithEmailAndPassword(email, password);
  }

  login(email: string, password: string) {
    return this.angular_fire_auth.signInWithEmailAndPassword(email, password);
  }

  logout() {
    return this.angular_fire_auth.signOut();
  }

  getCurrentUser() {
    return this.angular_fire_auth.authState;
  }
}
