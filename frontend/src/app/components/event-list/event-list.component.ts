import { Component, OnInit } from '@angular/core';
import { EventDetails } from '../../models/EventDetails';
import { EventService } from '../../services/event.service';

@Component({
  selector: 'app-event-list',
  templateUrl: './event-list.component.html',
  styleUrls: ['./event-list.component.css'],
})
export class EventListComponent implements OnInit {
  events: EventDetails[] = [];
  isLoading = true;
  errorMessages: string[] = [];

  filters = {
    minPrice: null as number | null,
    maxPrice: null as number | null,
    startFrom: null as string | null,
    endTo: null as string | null,
    age: null as number | null,
    cities: [] as string[],
    disabilityTag: '',
  };

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

  constructor(private eventService: EventService) {}

  ngOnInit() {
    this.loadAllEvents();
  }

  /** 加载所有活动 */
  loadAllEvents() {
    this.isLoading = true;
    this.eventService.getEvents().subscribe({
      next: (data) => {
        this.events = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('❌ Failed to load events', err);
        this.isLoading = false;
      },
    });
  }

  /** 点击筛选按钮 */
  applyFilters() {
    if (!this.validateFilters()) return;

    const filters = {
      minPrice: this.filters.minPrice,
      maxPrice: this.filters.maxPrice,
      from: this.filters.startFrom,
      to: this.filters.endTo,
      age: this.filters.age,
      disabilityTags: this.filters.disabilityTag
        ? [this.filters.disabilityTag.trim()]
        : [],
      cities: this.filters.cities,
    };

    this.isLoading = true;
    this.eventService.getFilteredEvents(filters).subscribe({
      next: (data) => {
        this.events = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('❌ Failed to filter events', err);
        this.isLoading = false;
      },
    });
  }

  /** 重置过滤 */
  resetFilters() {
    this.filters = {
      minPrice: null,
      maxPrice: null,
      startFrom: null,
      endTo: null,
      age: null,
      cities: [],
      disabilityTag: '',
    };
    this.errorMessages = [];
    this.loadAllEvents();
  }

  /** 多选城市切换 */
  onCityToggle(city: string, event: any) {
    if (event.target.checked) {
      this.filters.cities.push(city);
    } else {
      this.filters.cities = this.filters.cities.filter((c) => c !== city);
    }
  }

  /** 校验过滤项 */
  private validateFilters(): boolean {
    const errors: string[] = [];

    // ✅ price validation
    if (this.filters.minPrice !== null && this.filters.minPrice < 0)
      errors.push('Min Price cannot be negative.');
    if (this.filters.maxPrice !== null && this.filters.maxPrice < 0)
      errors.push('Max Price cannot be negative.');
    if (
      this.filters.minPrice !== null &&
      this.filters.maxPrice !== null &&
      this.filters.minPrice > this.filters.maxPrice
    )
      errors.push('Min Price must be ≤ Max Price.');

    // ✅ age validation
    if (
      this.filters.age !== null &&
      (this.filters.age < 0 || this.filters.age > 150)
    )
      errors.push('Age must be between 0 and 150.');

    // ✅ date validation
    if (this.filters.startFrom && this.filters.endTo) {
      const start = new Date(this.filters.startFrom);
      const end = new Date(this.filters.endTo);
      if (start > end) errors.push('Start date must be before End date.');
    }

    this.errorMessages = errors;
    return errors.length === 0;
  }
}
