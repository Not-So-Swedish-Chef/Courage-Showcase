import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-profile-edit',
  templateUrl: './profile-edit.component.html',
  styleUrl: './profile-edit.component.css',
})
export class ProfileEditComponent implements OnInit {
  agencyName = '';
  bio = '';
  isSubmitting = false;
  isLoading = true;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.http.get<any>('http://localhost:5000/api/host/profile').subscribe({
      next: (data) => {
        this.agencyName = data.agencyName || '';
        this.bio = data.bio || '';
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load host profile', err);
        this.isLoading = false;
      },
    });
  }

  onSubmit(form: NgForm) {
    if (form.invalid) return;

    this.isSubmitting = true;
    const update = {
      agencyName: this.agencyName,
      bio: this.bio,
    };


    this.http.put('http://localhost:5000/api/host', update).subscribe({
      next: () => {
        alert('Profile updated successfully!');
      },
      error: (err) => {
        console.error('Profile update failed:', err);
        alert('Update failed.');
      },
      complete: () => {
        this.isSubmitting = false;
      },
    });
  }
}
