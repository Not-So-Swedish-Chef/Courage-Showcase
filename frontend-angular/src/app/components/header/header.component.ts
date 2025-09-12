import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { UserType } from '../../models/user';
import { Router } from '@angular/router';

interface NavItem {
  label: string;
  link: string;
}

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
})
export class HeaderComponent {
  user$ = this.auth.user$;

  memberNav: NavItem[] = [{ label: 'Events', link: '/events' }];
  hostNav: NavItem[] = [
    { label: 'Events', link: '/events' },
    { label: 'Create Event', link: '/events/create' }, 
  ];

  constructor(private auth: AuthService, private router: Router) {}

  navFor(type: UserType | null | undefined): NavItem[] {
    if (type === 1) return this.hostNav; // 1 = Host
    if (type === 2) return this.memberNav; // 2 = Member
    return [];
  }

  logout() {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }
}
