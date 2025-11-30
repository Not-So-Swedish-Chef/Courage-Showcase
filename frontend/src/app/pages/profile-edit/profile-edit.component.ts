import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

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

  constructor(private http: HttpClient, private authService: AuthService) {}

  ngOnInit(): void {
    const userId = this.authService.user?.id;
    if (!userId) {
      console.error('User not logged in');
      this.isLoading = false;
      return;
    }

    this.http.get<any>(`${environment.apiBaseUrl}/Host/${userId}`).subscribe({
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


    this.http.put(`${environment.apiBaseUrl}/Host`, update).subscribe({
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
