import { User } from "@/contexts/authentication/@types/User";

export interface AuthenticationContextType {
  user: User | undefined;
  isAuthenticated: boolean;
  login: (user: User) => void;
  logout: () => void;
}
