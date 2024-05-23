export type User = {
  uid: string;
  tenant: string;
  email: string;
  role: "ADMIN" | "USER";
};
