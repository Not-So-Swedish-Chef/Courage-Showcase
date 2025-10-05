export interface Event {
  id: number;
  title: string;
  location: string;
  city: string;
  imageUrl: string;
  startDateTime: string;
  endDateTime: string;
  price: number;
  url: string;
  minAge: number;
  maxAge: number;
  disabilityTags: string[];
  status: number;
}
