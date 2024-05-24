export type UserRole = "ADMIN" | "USER";

export type User = {
  uid: string;
  tenant: string;
  email: string;
  role: UserRole;
};
