import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { User, UserType } from '../models/user';

const LS_KEY = 'eh_user_info';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private _user$ = new BehaviorSubject<User | null>(this.readFromStorage());
  user$ = this._user$.asObservable();

  get user(): User | null {
    return this._user$.value;
  }

  async register(payload: {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    userType: 1 | 2;
  }): Promise<{ ok: boolean; errors?: any }> {
    try {
      const res = await fetch('http://localhost:5000/api/User/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      });

      if (!res.ok) {
        const errorData = await res.json();
        return { ok: false, errors: errorData };
      }

      return { ok: true };
    } catch (e) {
      console.error('Register failed', e);
      return { ok: false, errors: e };
    }
  }

  logout() {
    localStorage.removeItem(LS_KEY);
    this._user$.next(null);
  }

  private readFromStorage(): User | null {
    try {
      const raw = localStorage.getItem(LS_KEY);
      return raw ? (JSON.parse(raw) as User) : null;
    } catch {
      return null;
    }
  }

  private writeToStorage(user: User) {
    localStorage.setItem(LS_KEY, JSON.stringify(user));
  }

  async login(payload: { email: string; password: string }): Promise<boolean> {
    try {
      const res = await fetch('http://localhost:5000/api/User/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Accept: 'application/json',
        },
        body: JSON.stringify(payload),
        // credentials: 'include',
      });

      if (!res.ok) {
        console.error('Login failed:', res.status);
        return false;
      }

      let data: any;
      const contentType = res.headers.get('content-type');
      if (contentType && contentType.includes('application/json')) {
        data = await res.json();
      } else {
        const text = await res.text();
        data = JSON.parse(text);
      }

      //  { token, signedInUser }
      const user = {
        ...data.signedInUser,
        token: data.token,
      };

      this.writeToStorage(user);
      this._user$.next(user);
      return true;
    } catch (e) {
      console.error('Login error:', e);
      return false;
    }
  }
}
