import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Event } from '../../../models/event';
import { EventService } from '../../../services/event.service';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-event-form',
  templateUrl: './event-form.component.html',
  styleUrls: ['./event-form.component.css'],
})
export class EventFormComponent {
  event: Event = {
    id: 0,
    title: '',
    location: '',
    imageUrl: '',
    startDateTime: '',
    endDateTime: '',
    price: 0,
    url: '',
  };

  isSubmitting = false;

  constructor(
    private eventService: EventService,
    private auth: AuthService,
    private router: Router
  ) {}

  onSubmit(form: NgForm) {
    if (form.invalid) return;

    this.isSubmitting = true;

    this.eventService.createEvent(this.event).subscribe({
      next: (created) => {
        if (created) {
          this.router.navigate(['/events']);
        }
      },
      error: (err) => {
        console.error('Create event failed:', err);
      },
      complete: () => {
        this.isSubmitting = false;
      },
    });
  }
}
