export interface CreateEventDto {
    title: string;
    location: string;
    startDateTime: string;
    endDateTime: string;
    price: number;
    url?: string;
    image?: File;
  }