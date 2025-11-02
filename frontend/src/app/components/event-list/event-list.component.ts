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

  citySearchTerm = '';

  readonly ontarioCities = [
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
  ].sort((a, b) => a.localeCompare(b));

  get selectedCities(): string[] {
    return this.sortCities(this.filters.cities);
  }

  get availableCities(): string[] {
    return this.ontarioCities.filter((city) =>
      !this.filters.cities.includes(city)
    );
  }

  get filteredAvailableCities(): string[] {
    const search = this.citySearchTerm.trim().toLowerCase();
    const available = this.availableCities;

    if (!search) {
      return available;
    }

    return available.filter((city) =>
      city.toLowerCase().includes(search)
    );
  }

  get matchingSelectedCities(): string[] {
    const search = this.citySearchTerm.trim().toLowerCase();
    if (!search) {
      return [];
    }

    return this.selectedCities.filter((city) =>
      city.toLowerCase().includes(search)
    );
  }

  get allCitiesSelected(): boolean {
    return this.filters.cities.length === this.ontarioCities.length;
  }

  get noCityMatchesHeadline(): string {
    if (this.filteredAvailableCities.length > 0) {
      return '';
    }

    if (!this.citySearchTerm.trim() && this.allCitiesSelected) {
      return 'All Ontario cities are selected.';
    }

    if (!this.citySearchTerm.trim()) {
      return 'No cities available to add.';
    }

    return 'No city names match.';
  }

  get noCityMatchesDetail(): string | null {
    if (this.filteredAvailableCities.length > 0) {
      return null;
    }

    const search = this.citySearchTerm.trim();
    if (!search) {
      return null;
    }

    if (this.matchingSelectedCities.length > 0) {
      const list = this.matchingSelectedCities.join(', ');
      return `${list} ${this.matchingSelectedCities.length === 1 ? 'is' : 'are'} already selected.`;
    }

    return null;
  }

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
    this.citySearchTerm = '';
    this.errorMessages = [];
    this.loadAllEvents();
  }

  addCity(city: string) {
    if (this.filters.cities.includes(city)) {
      return;
    }
    this.filters.cities = this.sortCities([...this.filters.cities, city]);
  }

  removeCity(city: string) {
    this.filters.cities = this.filters.cities.filter((c) => c !== city);
  }

  clearSelectedCities() {
    this.filters.cities = [];
  }

  clearCitySearch() {
    this.citySearchTerm = '';
  }

  private sortCities(cities: string[]): string[] {
    return [...cities].sort((a, b) => a.localeCompare(b));
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
