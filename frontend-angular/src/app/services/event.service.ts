import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Event } from '../models/event';
import { Observable } from 'rxjs';
import { EventDetails } from '../models/EventDetails';
@Injectable({ providedIn: 'root' })
export class EventService {
  constructor(private http: HttpClient) {}

  /** create */
  createEvent(event: Event): Observable<Event> {
    return this.http.post<Event>('http://localhost:5000/api/event', event);
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
}
