import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EventDetails } from '../models/EventDetails';
import { CreateEventDto } from '../models/CreateEventDto';
import { UpdateEventDto } from '../models/UpdateEventDto';
import { environment } from '../../environments/environment';
@Injectable({ providedIn: 'root' })
export class EventService {
  constructor(private http: HttpClient) {}
  private readonly BASE_URL = `${environment.apiBaseUrl}/Event`;

  /** create */
  createEvent(dto: CreateEventDto): Observable<EventDetails> {
    const payload = {
      title: dto.title,
      location: dto.location,
      city: dto.city,
      imageUrl: dto.imageUrl,
      startDateTime: dto.startDateTime,
      endDateTime: dto.endDateTime,
      price: dto.price,
      url: dto.url,
      hostId: dto.hostId,
      minAge: dto.minAge,
      maxAge: dto.maxAge,
      disabilityTags: dto.disabilityTags,
      status: dto.status,
      description: dto.description,
    };

    return this.http.post<EventDetails>(this.BASE_URL, payload, {
      headers: { 'Content-Type': 'application/json' },
    });
  }

  /** update */
  updateEvent(dto: UpdateEventDto): Observable<EventDetails> {
    return this.http.put<EventDetails>(
      `${this.BASE_URL}/${dto.id}`,
      {
        id: dto.id,
        title: dto.title,
        location: dto.location,
        city: dto.city,
        startDateTime: dto.startDateTime,
        endDateTime: dto.endDateTime,
        price: dto.price,
        url: dto.url,
        imageUrl: dto.imageUrl,
        hostId: dto.hostId,
        minAge: dto.minAge,
        maxAge: dto.maxAge,
        disabilityTags: dto.disabilityTags,
        status: dto.status,
        description: dto.description,
      },
      {
        headers: { 'Content-Type': 'application/json' },
      }
    );
  }
  /** get all events */
  getEvents(): Observable<EventDetails[]> {
    return this.http.get<EventDetails[]>(`${this.BASE_URL}`);
  }

  /** filtered events */
  getFilteredEvents(filters?: {
    minPrice?: number | null;
    maxPrice?: number | null;
    from?: string | null;
    to?: string | null;
    age?: number | null;
    disabilityTags?: string[]; // backend expects a list
    cities?: string[]; // backend expects a list
  }): Observable<EventDetails[]> {
    let params = new HttpParams();

    if (filters) {
      Object.entries(filters).forEach(([key, value]) => {
        if (Array.isArray(value)) {
          value.forEach((v) => (params = params.append(key, v)));
        } else if (value !== null && value !== undefined && value !== '') {
          params = params.set(key, value.toString());
        }
      });
    }

    return this.http.get<EventDetails[]>(`${this.BASE_URL}/search`, { params });
  }

  /** get detail */
  getEventById(id: number): Observable<EventDetails> {
    return this.http.get<EventDetails>(`${this.BASE_URL}/${id}`);
  }

  /** delete */
  deleteEvent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.BASE_URL}/${id}`);
  }

  getMyEvents(): Observable<EventDetails[]> {
    return this.http.get<EventDetails[]>(
      `${environment.apiBaseUrl}/host/events`
    );
  }
}
