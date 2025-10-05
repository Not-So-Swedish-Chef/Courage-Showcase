export interface UpdateEventDto {
  id: number;
  title?: string;
  location?: string;
  city?: string;
  startDateTime?: string;
  endDateTime?: string;
  price?: number;
  url?: string;
  image?: File | null;
  minAge?: number;
  maxAge?: number;
  disabilityTags?: string[];
  status?: number;
}
