"use client";
import { PropsWithChildren, useEffect, useMemo, useState } from "react";
import { User } from "@/contexts/authentication/@types/User";
import AuthenticationContext from "@/contexts/authentication/AuthenticationContext";

const AuthenticationContextProvider = ({ children }: PropsWithChildren) => {
  const [user, setUser] = useState<User | undefined>(undefined);
  const isAuthenticated = useMemo(() => !!user, [user]);

  const redirectToLogin = () => {
    if (location.pathname !== "/login") location.href = "/login";
  };

  useEffect(() => {
    const user = localStorage.getItem("user");
    if (user) {
      setUser(JSON.parse(user));
    } else {
      // redirectToLogin(); TODO: temporarily disabled for development
    }
  }, []);

  const login = (user: User) => {
    localStorage.setItem("user", JSON.stringify(user));
    setUser(user);
  };

  const logout = () => {
    localStorage.removeItem("user");
    setUser(undefined);
    redirectToLogin();
  };

  return (
    <AuthenticationContext.Provider
      value={{ user, login, logout, isAuthenticated }}
    >
      {children}
    </AuthenticationContext.Provider>
  );
};

export default AuthenticationContextProvider;
