import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private auth: AuthService) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const token = this.auth.user?.token;
    
    // Check if request is to ngrok URL
    const isNgrokRequest = req.url.includes('ngrok-free.app');
    
    let headers: { [key: string]: string } = {};
    
    // Add ngrok header for ngrok requests
    if (isNgrokRequest) {
      headers['ngrok-skip-browser-warning'] = 'true';
    }
    
    // Add auth token if available
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    if (Object.keys(headers).length > 0) {
      const cloned = req.clone({
        setHeaders: headers,
      });
      return next.handle(cloned);
    }

    return next.handle(req);
  }
}
