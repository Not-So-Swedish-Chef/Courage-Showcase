import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../../services/event.service';
import { EventDetails } from '../../models/EventDetails';

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
      this.router.navigate(['/events/update', this.event.id]);
    }
  }

  deleteEvent() {
    if (!this.event) return;

    if (confirm('Are you sure you want to delete this event?')) {
      this.eventService.deleteEvent(this.event.id).subscribe({
        next: () => {
          alert('Event deleted');
          this.router.navigate(['/events']);
        },
        error: (err) => {
          if (err.status === 403)
            alert('You are not allowed to delete this event.');
          else alert('Delete failed.');
          console.error(err);
        },
      });
    }
  }

  addToGoogleCalendar() {
    if (!this.event) return;

    const title = encodeURIComponent(this.event.title);
    const location = encodeURIComponent(this.event.location ?? '');
    const details = encodeURIComponent(`More info: ${this.event.url ?? ''}`);

    const start =
      new Date(this.event.startDateTime)
        .toISOString()
        .replace(/[-:]/g, '')
        .split('.')[0] + 'Z';

    const end =
      new Date(this.event.endDateTime)
        .toISOString()
        .replace(/[-:]/g, '')
        .split('.')[0] + 'Z';

    const url = `https://www.google.com/calendar/render?action=TEMPLATE&text=${title}&dates=${start}/${end}&location=${location}&details=${details}`;
    window.open(url, '_blank');
  }

  addToAppleCalendar() {
    if (!this.event) return;

    const start =
      new Date(this.event.startDateTime)
        .toISOString()
        .replace(/[-:]/g, '')
        .split('.')[0] + 'Z';

    const end =
      new Date(this.event.endDateTime)
        .toISOString()
        .replace(/[-:]/g, '')
        .split('.')[0] + 'Z';

    const summary = this.event.title.replace(/,/g, '\\,');
    const location = (this.event.location ?? '').replace(/,/g, '\\,');
    const url = this.event.url ?? '';

    const ics = `BEGIN:VCALENDAR
VERSION:2.0
BEGIN:VEVENT
URL:${url}
DTSTART:${start}
DTEND:${end}
SUMMARY:${summary}
LOCATION:${location}
END:VEVENT
END:VCALENDAR`;

    const blob = new Blob([ics], { type: 'text/calendar;charset=utf-8' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `${summary}.ics`;
    link.click();
  }
}
