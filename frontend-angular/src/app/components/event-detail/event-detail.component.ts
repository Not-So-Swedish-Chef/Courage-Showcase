import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../../services/event.service';
import { EventDetails } from '../../models/EventDetails';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-event-detail',
  templateUrl: './event-detail.component.html',
  styleUrls: ['./event-detail.component.css'],
})
export class EventDetailComponent implements OnInit {
  event?: EventDetails;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.eventService.getEventById(id).subscribe({
        next: (data) => {
          this.event = data;
          this.loading = false;
        },
        error: (err) => {
          console.error('Failed to load event', err);
          this.loading = false;
        },
      });
    }
  }

  goBack() {
    this.router.navigate(['/events']);
  }

  updateEvent() {
    if (this.event) {
      this.router.navigate(['/events', this.event.id, 'update']);
    }
  }

  deleteEvent() {
    if (this.event) {
      if (confirm('Are you sure you want to delete this event?')) {
        this.eventService.deleteEvent(this.event.id).subscribe({
          next: () => {
            alert('Event deleted');
            this.router.navigate(['/events']);
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
  }
}
