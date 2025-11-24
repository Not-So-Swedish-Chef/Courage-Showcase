import { Component, OnInit } from '@angular/core';
import { AdminUserService } from '../../services/admin-user.service';
import { User, UserStatus } from '../../models/user';

@Component({
  selector: 'app-admin-users',
  templateUrl: './admin-users.component.html',
  styleUrls: ['./admin-users.component.css'],
})
export class AdminUsersComponent implements OnInit {
  users: User[] = [];
  loading = true;

  suspendTarget: User | null = null;
  suspendDays: number = 1; // number of days to suspend

  constructor(private adminUserService: AdminUserService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  private loadUsers(): void {
    this.loading = true;
    this.adminUserService.getUsers().subscribe({
      next: (res) => {
        this.users = res;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading users:', error);
        this.loading = false;
      }
    });
  }

  banUser(id?: number) {
    if (!id) return;
    this.adminUserService.banUser(id).subscribe({
      next: (message) => {
        console.log(message);
        this.loadUsers();
      },
      error: (error) => {
        console.error('Error banning user:', error);
      }
    });
  }

  openSuspend(user: User) {
    this.suspendTarget = user;
    this.suspendDays = 1;
  }

  confirmSuspend() {
    if (!this.suspendTarget || !this.suspendDays) return;

    this.adminUserService.suspendUser(this.suspendTarget.id!, this.suspendDays).subscribe({
      next: (message) => {
        console.log(message);
        this.suspendTarget = null;
        this.loadUsers();
      },
      error: (error) => {
        console.error('Error suspending user:', error);
      }
    });
  }

  unsuspendUser(id?: number) {
    if (!id) return;
    this.adminUserService.unsuspendUser(id).subscribe({
      next: (message) => {
        console.log(message);
        this.loadUsers();
      },
      error: (error) => {
        console.error('Error unsuspending user:', error);
      }
    });
  }

  unbanUser(id?: number) {
    if (!id) return;
    this.adminUserService.unbanUser(id).subscribe({
      next: (message) => {
        console.log(message);
        this.loadUsers();
      },
      error: (error) => {
        console.error('Error unbanning user:', error);
      }
    });
  }

  cancelSuspend() {
    this.suspendTarget = null;
  }

  isSuspended(user: User): boolean {
    return user.status === 1 && !!user.suspensionEndDate && new Date(user.suspensionEndDate) > new Date();
  }

  isBanned(user: User): boolean {
    return user.status === 2;
  }

  getUserStatusText(user: User): string {
    switch (user.status) {
      case 2: return 'Banned';
      case 1: return 'Suspended';
      case 0:
      default: return 'Active';
    }
  }

  getUserStatusClass(user: User): string {
    switch (user.status) {
      case 2: return 'banned-status';
      case 1: return 'suspended-status';
      case 0:
      default: return 'active-status';
    }
  }

  getStatusDetails(user: User): string {
    if (this.isBanned(user)) {
      return 'Permanently banned';
    }
    if (this.isSuspended(user)) {
      if (user.suspensionEndDate) {
        const suspendDate = new Date(user.suspensionEndDate);
        const now = new Date();
        const daysRemaining = Math.ceil((suspendDate.getTime() - now.getTime()) / (1000 * 60 * 60 * 24));
        const formattedDate = suspendDate.toLocaleDateString();
        
        if (daysRemaining === 1) {
          return `Suspended until ${formattedDate} (1 day remaining)`;
        } else if (daysRemaining > 1) {
          return `Suspended until ${formattedDate} (${daysRemaining} days remaining)`;
        } else {
          return `Suspended until ${formattedDate} (expires today)`;
        }
      }
      return 'Suspended (end date unknown)';
    }
    return 'Account is active';
  }

  getUserTypeLabel(type: number): string {
    return type === 0 ? 'Admin' : type === 1 ? 'Host' : 'Member';
  }

  canSuspend(user: User): boolean {
    return user.status === 0; // Only active users can be suspended
  }

  canBan(user: User): boolean {
    return user.status !== 2; // Active and suspended users can be banned
  }

  canUnsuspend(user: User): boolean {
    return user.status === 1; // Only suspended users can be unsuspended
  }

  canUnban(user: User): boolean {
    return user.status === 2; // Only banned users can be unbanned
  }
}
