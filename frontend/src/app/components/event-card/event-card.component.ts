import { Component, Input } from '@angular/core';
import { Event } from '../../models/event';
import { Router } from '@angular/router';

@Component({
  selector: 'app-event-card',
  templateUrl: './event-card.component.html',
  styleUrls: ['./event-card.component.css'],
})
export class EventCardComponent {
  @Input() event!: Event;

  constructor(private router: Router) {}

  goToDetail() {
    this.router.navigate(['/events', this.event.id]);
  }
}
