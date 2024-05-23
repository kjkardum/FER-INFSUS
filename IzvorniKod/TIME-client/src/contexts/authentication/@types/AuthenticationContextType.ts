import { User } from "@/contexts/authentication/@types/User";

export interface AuthenticationContextType {
  user: User | undefined;
  isAuthenticated: boolean;
  isAdmin: boolean;
  login: (jwt_token: string) => void;
  logout: () => void;
}
