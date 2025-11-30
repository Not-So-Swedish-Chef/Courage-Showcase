import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EventDetails } from '../../../models/EventDetails';
import { CreateEventDto } from '../../../models/CreateEventDto';
import { EventService } from '../../../services/event.service';
import { AuthService } from '../../../services/auth.service';
import { CloudinaryService } from '../../../services/cloudinary.service';
import { forkJoin, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { UpdateEventDto } from '../../../models/UpdateEventDto';

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
    'Brant',
    'Brantford',
    'Brockville',
    'Burlington',
    'Caledon',
    'Cambridge',
    'Clarington',
    'Cornwall',
    'EastGwillimbury',
    'FortErie',
    'Georgina',
    'Grimsby',
    'Guelph',
    'HaltonHills',
    'Hamilton',
    'KawarthaLakes',
    'Kingston',
    'Kitchener',
    'LaSalle',
    'London',
    'Markham',
    'Milton',
    'Mississauga',
    'Newmarket',
    'NiagaraFalls',
    'NorfolkCounty',
    'NorthBay',
    'Oakville',
    'Oshawa',
    'Ottawa',
    'Peterborough',
    'Pickering',
    'RichmondHill',
    'Sarnia',
    'SaultSteMarie',
    'StCatharines',
    'StThomas',
    'Stratford',
    'Sudbury',
    'ThunderBay',
    'Timmins',
    'Toronto',
    'Vaughan',
    'Waterloo',
    'Welland',
    'Whitby',
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
    description: '',
  };

  disabilityTagsInput = '';
  urlError = '';
  dateError = '';
  ageError = '';
  imageError = '';
  isUploadingImage = false;
  minStartDate = '';

  constructor(
    private eventService: EventService,
    private route: ActivatedRoute,
    private router: Router,
    private auth: AuthService,
    private cloudinaryService: CloudinaryService
  ) {
    // Set minimum start date to current date/time
    const now = new Date();
    this.minStartDate = now.toISOString().slice(0, 16);
  }

  // --- Load existing event if editing ---
  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.eventService.getEventById(Number(id)).subscribe({
        next: (data) => {
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
            description: data.description ?? '',
          };
          this.disabilityTagsInput = (data.disabilityTags || []).join(', ');
        },
        error: (err) => console.error('❌ Failed to load event:', err),
      });
    }
  }

  // --- Handle file selection ---
  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (!file) {
      this.selectedFile = undefined;
      this.imageError = '';
      return;
    }

    // Validate file type
    const allowedTypes = [
      'image/jpeg',
      'image/jpg',
      'image/png',
      'image/gif',
      'image/webp',
    ];
    if (!allowedTypes.includes(file.type)) {
      this.imageError =
        '⚠️ Invalid file type. Please upload JPG, PNG, GIF, or WebP.';
      this.selectedFile = undefined;
      return;
    }

    // Validate file size (max 10MB)
    const maxSize = 10 * 1024 * 1024; // 10MB
    if (file.size > maxSize) {
      this.imageError =
        '⚠️ File size exceeds 10MB. Please choose a smaller image.';
      this.selectedFile = undefined;
      return;
    }

    this.selectedFile = file;
    this.imageError = '';
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

  // --- Validate Start < End and Start > Now ---
  validateDateRange() {
    if (!this.event.startDateTime || !this.event.endDateTime) {
      this.dateError = '';
      return;
    }
    const start = new Date(this.event.startDateTime);
    const end = new Date(this.event.endDateTime);
    const now = new Date();
    
    // Check if start date is in the past (only for new events)
    if (!this.isEditMode && start < now) {
      this.dateError = '⚠️ Start date must be in the future.';
      return;
    }
    
    // Check if end date is after start date
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

    if (
      form.invalid ||
      this.urlError ||
      this.dateError ||
      this.ageError ||
      this.imageError
    ) {
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

    const uploadImage$ = this.selectedFile
      ? this.cloudinaryService.uploadImage(this.selectedFile, 'events')
      : of(this.event.imageUrl || this.DEFAULT_EVENT_IMAGE);

    this.isUploadingImage = !!this.selectedFile;

    uploadImage$
      .pipe(
        switchMap((imageUrl: string) => {
          // Step 2: Build DTO with the image URL
          const dto: CreateEventDto | UpdateEventDto = {
            ...this.event,
            disabilityTags: tags,
            imageUrl: imageUrl,
            hostId,
          };

          // Step 3: Create or update event
          return this.isEditMode
            ? this.eventService.updateEvent(dto as UpdateEventDto)
            : this.eventService.createEvent(dto as CreateEventDto);
        })
      )
      .subscribe({
        next: () => {
          alert(this.isEditMode ? '✅ Event updated!' : '🎉 Event created!');
          this.router.navigate(['/events']);
        },
        error: (err) => {
          console.error('❌ Save failed:', err);
          if (err.error?.error) {
            alert(`Failed: ${err.error.error}`);
          } else {
            alert('Failed to save event. See console for details.');
          }
        },
        complete: () => {
          this.isSubmitting = false;
          this.isUploadingImage = false;
        },
      });
  }
}
