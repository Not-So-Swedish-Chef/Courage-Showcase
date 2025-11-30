export interface CreateEventDto {
  title: string;
  location: string;
  city: string;
  startDateTime: string;
  endDateTime: string;
  price: number;
  url?: string;
  image?: File;
  minAge: number;
  maxAge: number;
  disabilityTags: string[];
  status: number; //Active = 0, Expired = 1, Canceled = 2
  hostId: number;
  imageUrl?: string;
  description?: string;
}
