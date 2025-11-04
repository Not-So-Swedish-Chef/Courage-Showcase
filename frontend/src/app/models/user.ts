export type UserType = 0 | 1 | 2;
export type UserStatus = 0 | 1 | 2; // 0 = Active, 1 = Suspended, 2 = Banned

export interface User {
  id?: number;
  userName?: string;
  firstName?: string;
  lastName?: string;
  email?: string;
  userType: UserType;
  token?: string;

  // Backend DTO fields
  status: UserStatus;
  suspensionEndDate?: Date | null;

  // Legacy fields for backward compatibility
  suspendUntil?: Date | null;
  isBanned?: boolean;
  bannedAt?: Date | null;
}
