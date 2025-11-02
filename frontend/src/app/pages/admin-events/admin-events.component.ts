import { Component } from '@angular/core';
import { EventDetails } from '../../models/EventDetails';
import { EventService } from '../../services/event.service';

@Component({
  selector: 'app-admin-events',
  templateUrl: './admin-events.component.html',
  styleUrl: './admin-events.component.css',
})
export class AdminEventsComponent {
  events: EventDetails[] = [];
  loading = true;

  constructor(private eventService: EventService) {}

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents() {
    this.loading = true;
    this.eventService.getEvents().subscribe({
      next: (data) => {
        this.events = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        alert('Failed to load events');
      },
    });
  }

  deleteEvent(id: number) {
    const confirmed = confirm('Are you sure you want to delete this event?');
    if (!confirmed) return;

    this.eventService.deleteEvent(id).subscribe({
      next: () => {
        this.events = this.events.filter((e) => e.id !== id);
      },
      error: () => {
        alert('Delete failed');
      },
    });
  }
}
