import { ChangeDetectorRef, Component, Input, OnInit } from '@angular/core';
import { EventDetails } from '../../models/EventDetails';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { HttpClient } from '@angular/common/http';
import { EventService } from '../../services/event.service';

@Component({
  selector: 'app-event-card',
  templateUrl: './event-card.component.html',
  styleUrls: ['./event-card.component.css'],
})
export class EventCardComponent implements OnInit {
  @Input() event!: EventDetails;
  @Input() showActions: boolean = false;
  isFav = false;
  private apiUrl = 'http://localhost:5000/api/User';
  isProcessing = false;
  constructor(
    private router: Router,
    private auth: AuthService,
    private http: HttpClient,
    private cdr: ChangeDetectorRef,
    private eventService: EventService
  ) {}

  ngOnInit() {
    this.checkIfSaved();
  }

  // async checkIfSaved() {
  //   try {
  //     const res = await fetch(`http://localhost:5000/api/User/saved`, {
  //       credentials: 'include',
  //     });
  //     if (!res.ok) return;
  //     const savedEvents = await res.json();
  //     this.isFav = savedEvents.some((e: any) => e.id === this.event.id);
  //   } catch (err) {
  //     console.error('Failed to check saved events', err);
  //   }
  // }

  checkIfSaved() {
    if (!this.auth.user?.token) return;

    this.http.get<any[]>(`${this.apiUrl}/saved`).subscribe({
      next: (savedEvents) => {
        this.isFav = savedEvents.some((e) => e.id === this.event.id);
      },
      error: (err) => {
        if (err.status !== 401) console.error('Get saved events error:', err);
      },
    });
  }

  // async toggleFav(event: MouseEvent) {
  //   event.stopPropagation();

  //   if (!this.auth.user) {
  //     alert('Please login first to save events.');
  //     return;
  //   }

  //   const url = `http://localhost:5000/api/User/save/${this.event.id}`;
  //   const method = this.isFav ? 'DELETE' : 'POST';

  //   try {
  //     const res = await fetch(url, {
  //       method,
  //       credentials: 'include',
  //     });

  //     if (res.ok) {
  //       this.isFav = !this.isFav;
  //     } else if (res.status === 401) {
  //       alert('Login required to use favorites.');
  //     } else {
  //       const msg = await res.text();
  //       alert(msg);
  //     }
  //   } catch (err) {
  //     console.error('Error saving event', err);
  //   }
  // }

  toggleFav(event: MouseEvent) {
    event.stopPropagation();

    if (!this.auth.user?.token) {
      alert('Please login first to save events.');
      return;
    }

    const prev = this.isFav;
    this.isFav = !this.isFav; // ✅ 立刻切换 UI
    this.cdr.detectChanges();

    const url = `${this.apiUrl}/save/${this.event.id}`;
    const req = prev ? this.http.delete(url) : this.http.post(url, null);

    req.subscribe({
      next: () => {
        console.log('Favorite toggled successfully.');
      },
      error: (err) => {
        console.error('Toggle favorite failed:', err);
      },
    });
  }

  onUpdate(event: MouseEvent) {
    event.stopPropagation();
    if (this.event) {
      this.router.navigate(['/events/update', this.event.id]);
    }
  }

  // ✅ Delete button handler
  onDelete(event: MouseEvent) {
    event.stopPropagation();

    if (!this.event) return;
    if (confirm('Are you sure you want to delete this event?')) {
      this.eventService.deleteEvent(this.event.id).subscribe({
        next: () => {
          alert('Event deleted');
          // 刷新当前页面（例如 /my-events）
          window.location.reload();
        },
        error: (err) => {
          if (err.status === 403) {
            alert('You are not allowed to delete this event.');
          } else {
            alert('Delete failed.');
          }
          console.error('Delete failed', err);
        },
      });
    }
  }

  goToDetail() {
    this.router.navigate(['/events', this.event.id]);
  }
}
