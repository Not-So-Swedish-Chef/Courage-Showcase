import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { EventDetails } from '../models/EventDetails';
import { CALENDAR_CONFIG } from '../config/calendar.config';

declare var gapi: any;

export interface CalendarEvent {
  title: string;
  description: string;
  location: string;
  startDateTime: string;
  endDateTime: string;
  url?: string;
}

@Injectable({
  providedIn: 'root'
})
export class CalendarService {
  private isGapiLoaded = false;
  private isGapiInitialized = false;
  private isBrowser: boolean;

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    this.isBrowser = isPlatformBrowser(this.platformId);
    // Temporarily disabled Google API loading to prevent console errors
    // Uncomment the line below when Google Cloud Console is properly set up
    // if (this.isBrowser) {
    //   this.loadGoogleAPI();
    // }
  }

  /**
   * Load Google API script dynamically
   */
  private loadGoogleAPI(): Promise<void> {
    return new Promise((resolve, reject) => {
      if (!this.isBrowser) {
        reject(new Error('Google API can only be loaded in browser environment'));
        return;
      }

      if (this.isGapiLoaded) {
        resolve();
        return;
      }

      const script = document.createElement('script');
      script.src = 'https://apis.google.com/js/api.js';
      script.onload = () => {
        this.isGapiLoaded = true;
        resolve();
      };
      script.onerror = () => reject(new Error('Failed to load Google API'));
      document.head.appendChild(script);
    });
  }

  /**
   * Initialize Google API with OAuth
   */
  private async initializeGoogleAPI(): Promise<void> {
    if (this.isGapiInitialized) return;

    await this.loadGoogleAPI();
    
    return new Promise((resolve, reject) => {
      gapi.load('client:auth2', async () => {
        try {
          // Check if credentials are properly configured
          if (!CALENDAR_CONFIG.googleApiKey || CALENDAR_CONFIG.googleApiKey === 'YOUR_GOOGLE_API_KEY_HERE' ||
              !CALENDAR_CONFIG.googleClientId || CALENDAR_CONFIG.googleClientId === 'YOUR_GOOGLE_CLIENT_ID_HERE.apps.googleusercontent.com') {
            throw new Error('Google Calendar credentials not configured. Please update calendar.config.ts with your actual API credentials.');
          }

          await gapi.client.init({
            apiKey: CALENDAR_CONFIG.googleApiKey,
            clientId: CALENDAR_CONFIG.googleClientId,
            discoveryDocs: ['https://www.googleapis.com/discovery/v1/apis/calendar/v3/rest'],
            scope: 'https://www.googleapis.com/auth/calendar.events'
          });
          this.isGapiInitialized = true;
          resolve();
        } catch (error) {
          console.error('Google Calendar API initialization failed:', error);
          reject(error);
        }
      });
    });
  }

  /**
   * Add event to Google Calendar
   */
  async addToGoogleCalendar(eventDetails: EventDetails): Promise<boolean> {
    try {
      await this.initializeGoogleAPI();
      
      const authInstance = gapi.auth2.getAuthInstance();
      if (!authInstance.isSignedIn.get()) {
        await authInstance.signIn();
      }

      const calendarEvent = this.convertToCalendarEvent(eventDetails);
      
      const request = {
        calendarId: 'primary',
        resource: {
          summary: calendarEvent.title,
          description: calendarEvent.description,
          location: calendarEvent.location,
          start: {
            dateTime: calendarEvent.startDateTime,
            timeZone: CALENDAR_CONFIG.defaultTimeZone
          },
          end: {
            dateTime: calendarEvent.endDateTime,
            timeZone: CALENDAR_CONFIG.defaultTimeZone
          },
          source: {
            title: 'Courage Showcase Event',
            url: calendarEvent.url
          }
        }
      };

      const response = await gapi.client.calendar.events.insert(request);
      return response.status === 200;
    } catch (error) {
      console.error('Failed to add event to Google Calendar:', error);
      throw error;
    }
  }

  /**
   * Generate ICS file for universal calendar support
   */
  generateICSFile(eventDetails: EventDetails): string {
    const calendarEvent = this.convertToCalendarEvent(eventDetails);
    
    const startDate = new Date(calendarEvent.startDateTime).toISOString().replace(/[-:]/g, '').split('.')[0] + 'Z';
    const endDate = new Date(calendarEvent.endDateTime).toISOString().replace(/[-:]/g, '').split('.')[0] + 'Z';
    const now = new Date().toISOString().replace(/[-:]/g, '').split('.')[0] + 'Z';
    
    const icsContent = [
      'BEGIN:VCALENDAR',
      'VERSION:2.0',
      'PRODID:-//Courage Showcase//Event Calendar//EN',
      'BEGIN:VEVENT',
      `UID:${eventDetails.id}-${now}@courage-showcase.com`,
      `DTSTAMP:${now}`,
      `DTSTART:${startDate}`,
      `DTEND:${endDate}`,
      `SUMMARY:${this.escapeICSText(calendarEvent.title)}`,
      `DESCRIPTION:${this.escapeICSText(calendarEvent.description)}`,
      `LOCATION:${this.escapeICSText(calendarEvent.location)}`,
      calendarEvent.url ? `URL:${calendarEvent.url}` : '',
      'STATUS:CONFIRMED',
      'END:VEVENT',
      'END:VCALENDAR'
    ].filter(line => line !== '').join('\r\n');

    return icsContent;
  }

  /**
   * Download ICS file
   */
  downloadICSFile(eventDetails: EventDetails): void {
    const icsContent = this.generateICSFile(eventDetails);
    const blob = new Blob([icsContent], { type: 'text/calendar;charset=utf-8' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    
    link.setAttribute('href', url);
    link.setAttribute('download', `${eventDetails.title.replace(/[^a-z0-9]/gi, '_').toLowerCase()}.ics`);
    link.style.visibility = 'hidden';
    
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  }

  /**
   * Generate calendar URLs for different providers
   */
  generateCalendarUrls(eventDetails: EventDetails): { [key: string]: string } {
    const calendarEvent = this.convertToCalendarEvent(eventDetails);
    const startDate = new Date(calendarEvent.startDateTime);
    const endDate = new Date(calendarEvent.endDateTime);
    
    // Format dates for URL parameters
    const formatGoogleDate = (date: Date): string => {
      return date.toISOString().replace(/[-:]/g, '').split('.')[0] + 'Z';
    };

    const formatOutlookDate = (date: Date): string => {
      return date.toISOString();
    };

    const googleUrl = `https://calendar.google.com/calendar/render?action=TEMPLATE&text=${encodeURIComponent(calendarEvent.title)}&dates=${formatGoogleDate(startDate)}/${formatGoogleDate(endDate)}&details=${encodeURIComponent(calendarEvent.description)}&location=${encodeURIComponent(calendarEvent.location)}`;
    
    const outlookUrl = `https://outlook.live.com/calendar/0/deeplink/compose?subject=${encodeURIComponent(calendarEvent.title)}&startdt=${formatOutlookDate(startDate)}&enddt=${formatOutlookDate(endDate)}&body=${encodeURIComponent(calendarEvent.description)}&location=${encodeURIComponent(calendarEvent.location)}`;

    return {
      google: googleUrl,
      outlook: outlookUrl
    };
  }

  /**
   * Convert EventDetails to CalendarEvent format
   */
  private convertToCalendarEvent(eventDetails: EventDetails): CalendarEvent {
    const description = [
      `Event: ${eventDetails.title}`,
      `Price: ${eventDetails.price === 0 ? 'FREE' : '$' + eventDetails.price}`,
      `Age Range: ${eventDetails.minAge || 0} - ${eventDetails.maxAge || 'Any'}`,
      eventDetails.url ? `Website: ${eventDetails.url}` : '',
      `Event ID: ${eventDetails.id}`
    ].filter(line => line !== '').join('\n');

    return {
      title: eventDetails.title,
      description: description,
      location: `${eventDetails.location}, ${eventDetails.city}`,
      startDateTime: eventDetails.startDateTime,
      endDateTime: eventDetails.endDateTime,
      url: eventDetails.url
    };
  }

  /**
   * Escape special characters for ICS format
   */
  private escapeICSText(text: string): string {
    return text
      .replace(/\\/g, '\\\\')
      .replace(/;/g, '\\;')
      .replace(/,/g, '\\,')
      .replace(/\n/g, '\\n')
      .replace(/\r/g, '');
  }

  /**
   * Check if Google Calendar API is available
   */
  async isGoogleCalendarAvailable(): Promise<boolean> {
    // Temporarily disable direct Google Calendar integration
    // until proper credentials are set up in Google Cloud Console
    return false;
    
    /* Uncomment this section when you have proper Google Cloud Console setup:
    
    if (!this.isBrowser) {
      return false;
    }
    
    try {
      await this.initializeGoogleAPI();
      return true;
    } catch (error) {
      console.warn('Google Calendar API not available:', error);
      return false;
    }
    */
  }
}
