import { Component, Input, OnInit, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CalendarService } from '../../services/calendar.service';
import { EventDetails } from '../../models/EventDetails';

@Component({
  selector: 'app-calendar-integration',
  templateUrl: './calendar-integration.component.html',
  styleUrls: ['./calendar-integration.component.css']
})
export class CalendarIntegrationComponent implements OnInit {
  @Input() event!: EventDetails;
  
  isGoogleCalendarAvailable = false;
  isLoading = false;
  showDropdown = false;
  isBrowser: boolean;

  constructor(
    private calendarService: CalendarService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
  }

  async ngOnInit(): Promise<void> {
    // Always set to false since we disabled Google Calendar direct integration
    this.isGoogleCalendarAvailable = false;
  }

  toggleDropdown(): void {
    this.showDropdown = !this.showDropdown;
  }

  closeDropdown(): void {
    this.showDropdown = false;
  }

  async addToGoogleCalendar(): Promise<void> {
    if (!this.event) return;

    this.isLoading = true;
    try {
      const success = await this.calendarService.addToGoogleCalendar(this.event);
      if (success) {
        alert('Event successfully added to your Google Calendar!');
      } else {
        throw new Error('Failed to add event');
      }
    } catch (error: any) {
      console.error('Error adding to Google Calendar:', error);
      
      let errorMessage = 'Failed to add event to Google Calendar.\n\n';
      
      if (error.message && error.message.includes('credentials not configured')) {
        errorMessage += 'Google Calendar API credentials need to be set up.\n';
        errorMessage += 'Please use one of the other calendar options below.';
      } else if (error.message && error.message.includes('idpIframe_initialization_failed')) {
        errorMessage += 'Google Calendar authentication failed.\n';
        errorMessage += 'Please try the "Google Calendar (Web)" option instead.';
      } else {
        errorMessage += 'Please try downloading the calendar file or using the web options.';
      }
      
      alert(errorMessage);
    } finally {
      this.isLoading = false;
      this.closeDropdown();
    }
  }

  downloadCalendarFile(): void {
    if (!this.event) return;

    try {
      this.calendarService.downloadICSFile(this.event);
      alert('Calendar file downloaded! Open it with your preferred calendar app.');
    } catch (error) {
      console.error('Error downloading calendar file:', error);
      alert('Failed to download calendar file.');
    }
    this.closeDropdown();
  }

  openGoogleCalendarUrl(): void {
    if (!this.event) return;

    const urls = this.calendarService.generateCalendarUrls(this.event);
    window.open(urls['google'], '_blank');
    this.closeDropdown();
  }

  openOutlookCalendarUrl(): void {
    if (!this.event) return;

    const urls = this.calendarService.generateCalendarUrls(this.event);
    window.open(urls['outlook'], '_blank');
    this.closeDropdown();
  }
}
