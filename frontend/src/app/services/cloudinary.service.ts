import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, switchMap, from } from 'rxjs';
import { environment } from '../../environments/environment';

export interface CloudinaryUploadParams {
  apiKey: string;
  cloudName: string;
  signature: string;
  timestamp: number;
  folder: string;
  uploadUrl: string;
}

export interface CloudinaryUploadResponse {
  secure_url: string;
  public_id: string;
  width: number;
  height: number;
  format: string;
  resource_type: string;
  created_at: string;
}

@Injectable({
  providedIn: 'root',
})
export class CloudinaryService {
  constructor(private http: HttpClient) {}

  /**
   * Step 1: Get signed upload parameters from backend
   */
  getUploadSignature(folder: string = 'events'): Observable<CloudinaryUploadParams> {
    return this.http.post<CloudinaryUploadParams>(
      `${environment.apiBaseUrl}/Cloudinary/generate-signature?folder=${folder}`,
      {}
    );
  }

  /**
   * Step 2: Upload image directly to Cloudinary using the signature
   */
  uploadImageToCloudinary(
    file: File,
    uploadParams: CloudinaryUploadParams
  ): Observable<CloudinaryUploadResponse> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('api_key', uploadParams.apiKey);
    formData.append('timestamp', uploadParams.timestamp.toString());
    formData.append('signature', uploadParams.signature);
    formData.append('folder', uploadParams.folder);

    // Upload directly to Cloudinary (not our backend)

    return new Observable<CloudinaryUploadResponse>((observer) => {
      const xhr = new XMLHttpRequest();
      
      xhr.onload = () => {
        if (xhr.status >= 200 && xhr.status < 300) {
          try {
            const response = JSON.parse(xhr.responseText);
            observer.next(response);
            observer.complete();
          } catch (e) {
            observer.error(new Error('Failed to parse Cloudinary response'));
          }
        } else {
          observer.error(new Error(`Upload failed with status ${xhr.status}: ${xhr.statusText}`));
        }
      };

      xhr.onerror = () => {
        observer.error(new Error('Network error during upload'));
      };

      xhr.open('POST', uploadParams.uploadUrl);
      xhr.send(formData);
    });
  }

  /**
   * Complete flow: Get signature, then upload image
   * Returns the secure URL of the uploaded image
   */
  uploadImage(file: File, folder: string = 'events'): Observable<string> {
    return this.getUploadSignature(folder).pipe(
      switchMap((params: CloudinaryUploadParams) =>
        this.uploadImageToCloudinary(file, params).pipe(
          switchMap((response: CloudinaryUploadResponse) => from([response.secure_url]))
        )
      )
    );
  }
}
