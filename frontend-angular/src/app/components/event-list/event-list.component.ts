import { Component, OnInit } from '@angular/core';
import { Event } from '../../models/event';
import { EventService } from '../../services/event.service';

@Component({
  selector: 'app-event-list',
  templateUrl: './event-list.component.html',
  styleUrls: ['./event-list.component.css'],
})
export class EventListComponent implements OnInit {
  events: Event[] = [];
  isLoading = true;

  constructor(private eventService: EventService) {}

  ngOnInit() {
    this.loadEvents();
  }

  loadEvents() {
    this.eventService.getEvents().subscribe({
      next: (data) => {
        this.events = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load events', err);
        this.isLoading = false;
      },
    });
  }
}
