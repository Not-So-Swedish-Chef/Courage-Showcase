import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

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
        .get<any>(`http://localhost:5000/api/user/host/${id}`)
        .subscribe({
          next: (res) => {
            this.host = res.data;
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
