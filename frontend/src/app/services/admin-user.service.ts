import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root',
})
export class AdminUserService {
  // (replace with API later)
  private readonly _users$ = new BehaviorSubject<User[]>([
    {
      id: 1,
      firstName: 'Alice',
      lastName: 'Smith',
      email: 'alice@mail.com',
      userType: 1,
    },
    {
      id: 2,
      firstName: 'Bob',
      lastName: 'Chen',
      email: 'bob@mail.com',
      userType: 2,
    },
    {
      id: 3,
      firstName: 'Eve',
      lastName: 'Brown',
      email: 'eve@mail.com',
      userType: 0,
    },
  ]);

  // constructor(private http: HttpClient) {}
  // private readonly BASE_URL = `${environment.apiBaseUrl}/users`; // e.g. /users

  /** Get all users */
  getUsers(): Observable<User[]> {
    // Later: return this.http.get<User[]>(this.BASE_URL);
    return this._users$.asObservable();
  }

  /** Delete a user by id */
  deleteUser(id: number): Observable<void> {
    // Later: return this.http.delete<void>(`${this.BASE_URL}/${id}`);
    const next = this._users$.value.filter((u) => u.id !== id);
    this._users$.next(next);
    return of(void 0);
  }

  /** (Optional) Upsert/replace entire list — handy for seeding/testing */
  setUsers(users: User[]): void {
    this._users$.next(users);
  }

  suspendUser(id: number, suspendUntil: Date): void {
    const updated = this._users$.value.map((user) =>
      user.id === id ? { ...user, suspendUntil } : user
    );
    this._users$.next(updated);
  }
}
