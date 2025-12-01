import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { EventDetails } from '../../models/EventDetails';
import { AuthService } from '../../services/auth.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-saved-events',
  templateUrl: './saved-events.component.html',
  styleUrls: ['./saved-events.component.css'],
})
export class SavedEventsComponent implements OnInit {
  savedEvents: EventDetails[] = [];
  loading = true;
  apiUrl = `${environment.apiBaseUrl}/User/saved`;

  constructor(private http: HttpClient, private auth: AuthService) {}

  ngOnInit() {
    this.loadSavedEvents();
  }

  loadSavedEvents() {
    if (!this.auth.user?.token) {
      this.loading = false;
      return;
    }

    this.http.get<EventDetails[]>(this.apiUrl).subscribe({
      next: (data) => {
        this.savedEvents = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to load saved events:', err);
        this.loading = false;
      },
    });
  }
}
