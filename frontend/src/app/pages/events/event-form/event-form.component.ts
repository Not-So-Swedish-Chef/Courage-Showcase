import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EventDetails } from '../../../models/EventDetails';
import { CreateEventDto } from '../../../models/CreateEventDto';
import { UpdateEventDto } from '../../../models/UpdateEventDto';
import { EventService } from '../../../services/event.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-event-form',
  templateUrl: './event-form.component.html',
  styleUrls: ['./event-form.component.css'],
})
export class EventFormComponent implements OnInit {
  // --- UI States ---
  isEditMode = false;
  isSubmitting = false;
  selectedFile?: File;

  // --- Default Values ---
  readonly DEFAULT_EVENT_IMAGE =
    'https://images.unsplash.com/photo-1503264116251-35a269479413?auto=format&fit=crop&w=1200&q=60';

  // --- Ontario Cities ---
  ontarioCities = [
    'Ajax',
    'Aurora',
    'Barrie',
    'Belleville',
    'Brampton',
    'Brantford',
    'Burlington',
    'Hamilton',
    'Kingston',
    'Kitchener',
    'London',
    'Mississauga',
    'NiagaraFalls',
    'Ottawa',
    'Toronto',
    'Waterloo',
    'Windsor',
  ];

  // --- Data Model ---
  event: EventDetails = {
    id: 0,
    title: '',
    location: '',
    city: '',
    imageUrl: '',
    startDateTime: '',
    endDateTime: '',
    price: 0,
    url: '',
    minAge: 0,
    maxAge: 99,
    disabilityTags: [],
    status: 0,
    hostId: 0,
  };

  disabilityTagsInput = '';
  urlError = '';
  dateError = '';
  ageError = '';

  constructor(
    private eventService: EventService,
    private route: ActivatedRoute,
    private router: Router,
    private auth: AuthService
  ) {}

  // --- Load existing event if editing ---
  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.eventService.getEventById(Number(id)).subscribe({
        next: (data) => {
          // ✅ copy only matching fields (防止额外host嵌套字段报错)
          this.event = {
            id: data.id,
            title: data.title,
            location: data.location,
            city: data.city,
            imageUrl: data.imageUrl,
            startDateTime: data.startDateTime,
            endDateTime: data.endDateTime,
            price: data.price,
            url: data.url,
            hostId: data.hostId,
            minAge: data.minAge,
            maxAge: data.maxAge,
            disabilityTags: data.disabilityTags ?? [],
            status: data.status,
          };
          this.disabilityTagsInput = (data.disabilityTags || []).join(', ');
        },
        error: (err) => console.error('❌ Failed to load event:', err),
      });
    }
  }

  // --- Handle file selection ---
  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  // --- Validate URL format ---
  validateUrl() {
    const url = this.event.url?.trim();
    if (!url) {
      this.urlError = '';
      return;
    }
    const pattern = /^(https?:\/\/)([\w.-]+)(:[0-9]+)?(\/.*)?$/i;
    this.urlError = pattern.test(url)
      ? ''
      : '⚠️ URL must start with http:// or https:// and be a valid link.';
  }

  // --- Validate Start < End ---
  validateDateRange() {
    if (!this.event.startDateTime || !this.event.endDateTime) {
      this.dateError = '';
      return;
    }
    const start = new Date(this.event.startDateTime);
    const end = new Date(this.event.endDateTime);
    this.dateError =
      end > start ? '' : '⚠️ End date must be later than start date.';
  }

  // --- Validate MinAge ≤ MaxAge ---
  validateAgeRange() {
    if (this.event.minAge == null || this.event.maxAge == null) {
      this.ageError = '';
      return;
    }
    this.ageError =
      this.event.minAge <= this.event.maxAge
        ? ''
        : '⚠️ Min age must be ≤ Max age.';
  }

  // --- Submit Handler ---
  onSubmit(form: NgForm) {
    this.validateUrl();
    this.validateDateRange();
    this.validateAgeRange();

    if (form.invalid || this.urlError || this.dateError || this.ageError) {
      alert('⚠️ Please fix validation errors before submitting.');
      return;
    }

    this.isSubmitting = true;

    if (!this.event.url || this.event.url.trim() === '') {
      this.event.url = 'https://';
    }

    const tags = this.disabilityTagsInput
      .split(',')
      .map((t) => t.trim())
      .filter((t) => t.length > 0);

    const currentUser = this.auth.user;
    if (!currentUser) {
      alert('⚠️ Please log in first.');
      this.isSubmitting = false;
      return;
    }

    if (currentUser.userType !== 1) {
      alert('❌ Only hosts can create or edit events.');
      this.isSubmitting = false;
      return;
    }

    const hostId = currentUser.id!;

    // ✅ Build DTO (Create or Update)
    const dto: CreateEventDto | UpdateEventDto = {
      ...this.event,
      disabilityTags: tags,
      imageUrl: this.event.imageUrl || this.DEFAULT_EVENT_IMAGE,
      hostId,
    };

    const request$ = this.isEditMode
      ? this.eventService.updateEvent(dto as UpdateEventDto)
      : this.eventService.createEvent(dto as CreateEventDto);

    request$.subscribe({
      next: () => {
        alert(this.isEditMode ? '✅ Event updated!' : '🎉 Event created!');
        this.router.navigate(['/events']);
      },
      error: (err) => {
        console.error('❌ Save failed:', err);
        alert('Failed to save event, see console for details.');
      },
      complete: () => (this.isSubmitting = false),
    });
  }
}
