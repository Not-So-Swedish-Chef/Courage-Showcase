export interface EventDetails {
  id: number;
  title: string;
  location: string;
  city: string;
  imageUrl: string;
  startDateTime: string;
  endDateTime: string;
  price: number;
  url: string;
  hostId: number;
  minAge: number;
  maxAge: number;
  disabilityTags: string[];
  status: number;
  description?: string;
}
