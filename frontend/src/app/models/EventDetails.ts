// models/event-details.ts
export interface EventDetails {
  id: number;
  title: string;
  location: string;
  imageUrl: string;
  startDateTime: string;
  endDateTime: string;
  price: number;
  url: string;
  hostId: number;
  host: {
    id: number;
    agencyName: string;
    bio: string;
    user: {
      id: number;
      userName: string;
      email: string;
      firstName: string;
      lastName: string;
      userType: number;
    };
    events: string[];
  };
  usersWhoSaved: {
    id: number;
    userName: string;
    email: string;
    firstName: string;
    lastName: string;
    userType: number;
  }[];
}
