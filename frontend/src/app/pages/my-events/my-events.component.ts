import { Component, OnInit } from '@angular/core';
import { EventService } from '../../services/event.service';
import { EventDetails } from '../../models/EventDetails';

@Component({
  selector: 'app-my-events',
  templateUrl: './my-events.component.html',
  styleUrls: ['./my-events.component.css'],
})
export class MyEventsComponent implements OnInit {
  events: EventDetails[] = [];
  isLoading = true;

  constructor(private eventService: EventService) {}

  ngOnInit(): void {
    this.loadMyEvents();
  }

  loadMyEvents() {
    this.isLoading = true;
    this.eventService.getMyEvents().subscribe({
      next: (data) => {
        this.events = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load my events', err);
        this.isLoading = false;
      },
    });
  }
}
