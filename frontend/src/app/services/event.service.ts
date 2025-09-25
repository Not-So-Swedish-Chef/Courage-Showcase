import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Event } from '../models/event';
import { Observable } from 'rxjs';
import { EventDetails } from '../models/EventDetails';
import { CreateEventDto } from '../models/CreateEventDto';
import { UpdateEventDto } from '../models/UpdateEventDto';
@Injectable({ providedIn: 'root' })
export class EventService {
  constructor(private http: HttpClient) {}

  /** create */
  createEvent(dto: CreateEventDto, file?: File): Observable<Event> {
    const formData = new FormData();
    formData.append('title', dto.title);
    formData.append('location', dto.location);
    formData.append('startDateTime', dto.startDateTime);
    formData.append('endDateTime', dto.endDateTime);
    formData.append('price', dto.price.toString());

    if (dto.url) {
      formData.append('url', dto.url);
    }
    if (file) {
      formData.append('image', file);
    }

    return this.http.post<Event>('http://localhost:5000/api/event', formData);
  }

  /** update */
  updateEvent(dto: UpdateEventDto, file?: File | null): Observable<Event> {
    const formData = new FormData();

    //MUST HAVEid
    formData.append('id', dto.id.toString());

    if (dto.title !== undefined) formData.append('title', dto.title);
    if (dto.location !== undefined) formData.append('location', dto.location);
    if (dto.startDateTime !== undefined)
      formData.append('startDateTime', dto.startDateTime);
    if (dto.endDateTime !== undefined)
      formData.append('endDateTime', dto.endDateTime);
    if (dto.price !== undefined) formData.append('price', dto.price.toString());
    if (dto.url !== undefined) formData.append('url', dto.url);

    // image file
    if (file !== undefined) {
      if (file) {
        formData.append('image', file);
      } else {
        formData.append('image', '');
      }
    }

    return this.http.put<Event>(
      `http://localhost:5000/api/event/${dto.id}`,
      formData
    );
  }
  /** get all events */
  getEvents(): Observable<Event[]> {
    return this.http.get<Event[]>('http://localhost:5000/api/event');
  }

  /** filtered events */
  getFilteredEvents(filters?: {
    minPrice?: number | null;
    maxPrice?: number | null;
    startFrom?: string | null;
    endTo?: string | null;
  }): Observable<Event[]> {
    let params = new HttpParams();

    if (filters) {
      if (filters.minPrice !== null && filters.minPrice !== undefined) {
        params = params.set('minPrice', filters.minPrice.toString());
      }
      if (filters.maxPrice !== null && filters.maxPrice !== undefined) {
        params = params.set('maxPrice', filters.maxPrice.toString());
      }
      if (filters.startFrom) {
        params = params.set('from', filters.startFrom); //
      }
      if (filters.endTo) {
        const end = new Date(filters.endTo);
        end.setDate(end.getDate() + 1);
        params = params.set('to', end.toISOString()); // + 1 day
      }
    }

    return this.http.get<Event[]>('http://localhost:5000/api/event/search', {
      params,
    });
  }

  /** get detail */
  getEventById(id: number): Observable<EventDetails> {
    return this.http.get<EventDetails>(`http://localhost:5000/api/event/${id}`);
  }

  /** delete */
  deleteEvent(id: number): Observable<void> {
    return this.http.delete<void>(`http://localhost:5000/api/event/${id}`);
  }

  getMyEvents(): Observable<Event[]> {
    return this.http.get<Event[]>('http://localhost:5000/api/host/events');
  }
}
