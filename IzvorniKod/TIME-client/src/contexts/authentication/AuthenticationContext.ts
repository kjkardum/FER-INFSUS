import { createContext } from "react";
import { User } from "@/contexts/authentication/@types/User";

interface AuthenticationContextType {
  user: User | null;
  isAuthenticated: boolean;
  login: (user: User) => void;
  logout: () => void;
}

const AuthenticationContext = createContext<AuthenticationContextType>({
  user: null,
  isAuthenticated: false,
  login: () => {},
  logout: () => {},
});

export default AuthenticationContext;
