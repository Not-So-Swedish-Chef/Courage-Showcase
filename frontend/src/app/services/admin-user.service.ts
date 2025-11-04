import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, tap } from 'rxjs';
import { User, UserStatus } from '../models/user';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AdminUserService {
  private readonly _users$ = new BehaviorSubject<User[]>([]);
  private readonly BASE_URL = `${environment.apiBaseUrl}/Admin`;

  constructor(private http: HttpClient) {}

  /** Get all users from API */
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.BASE_URL}/users`).pipe(
      tap(users => this._users$.next(users))
    );
  }

  /** Get users observable for reactive updates */
  getUsersObservable(): Observable<User[]> {
    return this._users$.asObservable();
  }

  /** Ban a user by id */
  banUser(id: number): Observable<string> {
    return this.http.post(`${this.BASE_URL}/ban`, { userId: id }, { responseType: 'text' }).pipe(
      tap(() => {
        const updated = this._users$.value.map(user =>
          user.id === id 
            ? { ...user, status: 2 as UserStatus, suspensionEndDate: null }
            : user
        );
        this._users$.next(updated);
      })
    );
  }

  /** (Optional) Upsert/replace entire list — handy for seeding/testing */
  setUsers(users: User[]): void {
    this._users$.next(users);
  }

  suspendUser(id: number, days: number): Observable<string> {
    return this.http.post(`${this.BASE_URL}/suspend`, { userId: id, days }, { responseType: 'text' }).pipe(
      tap(() => {
        const suspensionEndDate = new Date();
        suspensionEndDate.setDate(suspensionEndDate.getDate() + days);
        const updated = this._users$.value.map((user) =>
          user.id === id ? { ...user, status: 1 as UserStatus, suspensionEndDate } : user
        );
        this._users$.next(updated);
      })
    );
  }

  /** Unsuspend a user by id */
  unsuspendUser(id: number): Observable<string> {
    return this.http.post(`${this.BASE_URL}/unsuspend`, { userId: id }, { responseType: 'text' }).pipe(
      tap(() => {
        const updated = this._users$.value.map(user =>
          user.id === id 
            ? { ...user, status: 0 as UserStatus, suspensionEndDate: null }
            : user
        );
        this._users$.next(updated);
      })
    );
  }

  /** Unban a user by id */
  unbanUser(id: number): Observable<string> {
    return this.http.post(`${this.BASE_URL}/unban`, { userId: id }, { responseType: 'text' }).pipe(
      tap(() => {
        const updated = this._users$.value.map(user =>
          user.id === id 
            ? { ...user, status: 0 as UserStatus, suspensionEndDate: null }
            : user
        );
        this._users$.next(updated);
      })
    );
  }
}
