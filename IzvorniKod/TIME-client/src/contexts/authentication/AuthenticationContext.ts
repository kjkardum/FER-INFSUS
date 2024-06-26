"use client";
import { createContext } from "react";
import { AuthenticationContextType } from "@/contexts/authentication/@types/AuthenticationContextType";

const AuthenticationContext = createContext<AuthenticationContextType>({
  user: undefined,
  isAuthenticated: false,
  isInitialized: false,
  isAdmin: false,
  login: () => {},
  logout: () => {},
});

export default AuthenticationContext;
