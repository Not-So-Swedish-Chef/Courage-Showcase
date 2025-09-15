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
  errorMessages: string[] = [];

  // filter
  filters = {
    minPrice: null as number | null,
    maxPrice: null as number | null,
    startFrom: null as string | null,
    endTo: null as string | null,
  };

  constructor(private eventService: EventService) {}

  ngOnInit() {
    this.loadAllEvents();
  }

  loadAllEvents() {
    this.isLoading = true;
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

  applyFilters() {
    if (!this.validateFilters()) {
      return; // if not validation do not send request
    }

    this.isLoading = true;
    this.eventService.getFilteredEvents(this.filters).subscribe({
      next: (data) => {
        this.events = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to filter events', err);
        this.isLoading = false;
      },
    });
  }

  resetFilters() {
    this.filters = {
      minPrice: null,
      maxPrice: null,
      startFrom: null,
      endTo: null,
    };
    this.loadAllEvents();
  }

  private validateFilters(): boolean {
    const errors: string[] = [];

    // price is number
    if (
      (this.filters.minPrice !== null &&
        isNaN(Number(this.filters.minPrice))) ||
      (this.filters.maxPrice !== null && isNaN(Number(this.filters.maxPrice)))
    ) {
      errors.push('Price must be a valid number.');
    }

    // price is not negtive
    if (
      (this.filters.minPrice !== null && this.filters.minPrice < 0) ||
      (this.filters.maxPrice !== null && this.filters.maxPrice < 0)
    ) {
      errors.push('Price must not be negative.');
    }

    //  minPrice ≤ maxPrice
    if (
      this.filters.minPrice !== null &&
      this.filters.maxPrice !== null &&
      this.filters.minPrice > this.filters.maxPrice
    ) {
      errors.push('Min Price must be less than or equal to Max Price.');
    }

    // startFrom < endTo
    if (this.filters.startFrom && this.filters.endTo) {
      const start = new Date(this.filters.startFrom);
      const end = new Date(this.filters.endTo);
      if (start > end) {
        errors.push('Start date must be earlier than End date.');
      }
    }

    if (errors.length > 0) {
      this.errorMessages = errors;
      return false;
    }

    this.errorMessages = [];
    return true;
  }
}
