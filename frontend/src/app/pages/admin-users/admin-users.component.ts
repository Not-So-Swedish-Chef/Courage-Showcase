import { Component, OnInit } from '@angular/core';
import { AdminUserService } from '../../services/admin-user.service';
import { User } from '../../models/user';

@Component({
  selector: 'app-admin-users',
  templateUrl: './admin-users.component.html',
  styleUrls: ['./admin-users.component.css'],
})
export class AdminUsersComponent implements OnInit {
  users: User[] = [];
  loading = true;

  suspendTarget: User | null = null;
  suspendUntil: string = ''; // date input value (YYYY-MM-DD)

  constructor(private adminUserService: AdminUserService) {}

  ngOnInit(): void {
    this.adminUserService.getUsers().subscribe((res) => {
      this.users = res;
      this.loading = false;
    });
  }

  deleteUser(id?: number) {
    if (!id) return;
    this.adminUserService.deleteUser(id).subscribe(() => {});
  }

  openSuspend(user: User) {
    this.suspendTarget = user;
    this.suspendUntil = '';
  }

  confirmSuspend() {
    if (!this.suspendTarget || !this.suspendUntil) return;

    const date = new Date(this.suspendUntil);
    this.adminUserService.suspendUser(this.suspendTarget.id!, date);

    this.suspendTarget = null;
  }

  cancelSuspend() {
    this.suspendTarget = null;
  }

  isSuspended(user: User): boolean {
    return !!user.suspendUntil && new Date(user.suspendUntil) > new Date();
  }

  getUserTypeLabel(type: number): string {
    return type === 0 ? 'Admin' : type === 1 ? 'Host' : 'Member';
  }
}
