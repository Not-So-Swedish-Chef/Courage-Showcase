import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Event } from '../../../models/event';
import { EventService } from '../../../services/event.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { CreateEventDto } from '../../../models/CreateEventDto';
import { UpdateEventDto } from '../../../models/UpdateEventDto';

@Component({
  selector: 'app-event-form',
  templateUrl: './event-form.component.html',
  styleUrls: ['./event-form.component.css'],
})
export class EventFormComponent implements OnInit {
  isEditMode = false;
  isSubmitting = false;
  selectedFile?: File;

  event: Partial<Event> = {
    title: '',
    location: '',
    startDateTime: '',
    endDateTime: '',
    price: 0,
    url: '',
  };

  constructor(
    private eventService: EventService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.eventService.getEventById(Number(id)).subscribe({
        next: (data) => {
          this.event = data;
        },
        error: (err) => console.error('Failed to load event', err),
      });
    }
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  onSubmit(form: NgForm) {
    if (form.invalid) return;
    this.isSubmitting = true;

    if (this.isEditMode && this.event.id) {
      const dto: UpdateEventDto = {
        id: this.event.id,
        title: this.event.title,
        location: this.event.location,
        startDateTime: this.event.startDateTime,
        endDateTime: this.event.endDateTime,
        price: this.event.price,
        url: this.event.url,
      };

      this.eventService.updateEvent(dto, this.selectedFile).subscribe({
        next: () => {
          alert('Event updated');
          this.router.navigate(['/events']);
        },
        error: (err) => {
          console.error('Update failed:', err);
        },
        complete: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      const dto: CreateEventDto = {
        title: this.event.title!,
        location: this.event.location!,
        startDateTime: this.event.startDateTime!,
        endDateTime: this.event.endDateTime!,
        price: this.event.price!,
        url: this.event.url,
      };

      this.eventService.createEvent(dto, this.selectedFile).subscribe({
        next: () => {
          alert('Event created');
          this.router.navigate(['/events']);
        },
        error: (err) => {
          console.error('Create failed:', err);
        },
        complete: () => {
          this.isSubmitting = false;
        },
      });
    }
  }
}
