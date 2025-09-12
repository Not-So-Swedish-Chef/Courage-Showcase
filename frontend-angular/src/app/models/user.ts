export type UserType = 0 | 1 | 2;
export interface User {
  id?: number;
  firstName?: string;
  lastName?: string;
  email?: string;
  userType: UserType;
  token?: string; 
}
