import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-host-profile',
  templateUrl: './host-profile.component.html',
  styleUrl: './host-profile.component.css',
})
export class HostProfileComponent implements OnInit {
  host: any;
  isLoading = true;

  constructor(private route: ActivatedRoute, private http: HttpClient) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.http
        .get<any>(`${environment.apiBaseUrl}/Host/${id}`)
        .subscribe({
          next: (res) => {
            this.host = res;
            this.isLoading = false;
          },
          error: (err) => {
            console.error('Failed to load host info', err);
            this.isLoading = false;
          },
        });
    }
  }
}
