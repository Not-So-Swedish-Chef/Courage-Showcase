export interface UpdateEventDto {
    id: number;
    title?: string;
    location?: string;
    startDateTime?: string;
    endDateTime?: string;
    price?: number;
    url?: string;
    image?: File | null; 
  }