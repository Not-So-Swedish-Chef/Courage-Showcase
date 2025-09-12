import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Event } from '../models/event';
import { Observable } from 'rxjs';
import { EventDetails } from '../models/EventDetails';
@Injectable({ providedIn: 'root' })
export class EventService {
  private apiUrl = 'http://localhost:5000/api/event';

  constructor(private http: HttpClient) {}

  createEvent(event: Event): Observable<Event> {
    return this.http.post<Event>(`${this.apiUrl}`, event);
  }

  getEvents(): Observable<Event[]> {
    return this.http.get<Event[]>(this.apiUrl);
  }

  getEventById(id: number): Observable<EventDetails> {
    return this.http.get<EventDetails>(`${this.apiUrl}/${id}`);
  }

  deleteEvent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
