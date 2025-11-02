import { Component, OnInit } from '@angular/core';
import { AdminUserService } from '../../services/admin-user.service';
import { User } from '../../models/user';

@Component({
  selector: 'app-admin-users',
  templateUrl: './admin-users.component.html',
  styleUrl: './admin-users.component.css',
})
export class AdminUsersComponent implements OnInit {
  users: User[] = [];
  loading = true;

  constructor(private adminUserService: AdminUserService) {}

  ngOnInit(): void {
    this.adminUserService.getUsers().subscribe((data) => {
      this.users = data;
      this.loading = false;
    });
  }

  deleteUser(id?: number) {
    if (!id) return;
    if (!confirm('Are you sure you want to delete this user?')) return;

    this.adminUserService.deleteUser(id).subscribe(() => {
      this.users = this.users.filter((u) => u.id !== id);
    });
  }

  getUserTypeLabel(type: 0 | 1 | 2): string {
    return type === 0 ? 'Admin' : type === 1 ? 'Host' : 'Member';
  }
}
