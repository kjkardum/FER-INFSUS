"use client";
import {
  PropsWithChildren,
  useCallback,
  useEffect,
  useMemo,
  useState,
} from "react";
import { User } from "@/contexts/authentication/@types/User";
import AuthenticationContext from "@/contexts/authentication/AuthenticationContext";
import Cookies from "js-cookie";
import StatusEndpoint from "@/api/endpoints/StatusEndpoint";
import { COOKIE_TOKEN } from "@/api/consts";
import { jwtDecode } from "jwt-decode";
import { usePathname, useRouter } from "next/navigation";

const AuthenticationContextProvider = ({ children }: PropsWithChildren) => {
  const pathname = usePathname();
  const router = useRouter();

  const [isInitialized, setIsInitialized] = useState(false);
  const [user, setUser] = useState<User | undefined>(undefined);
  const isAuthenticated = useMemo(() => !!user, [user]);
  const isAdmin = useMemo(() => user?.role === "ADMIN", [user]);

  const redirectToLogin = useCallback(() => {
    if (pathname !== "/login") router.push("/login");
  }, [pathname, router]);

  const redirectToHome = useCallback(() => {
    if (pathname === "/login") router.push("/");
  }, [pathname, router]);

  useEffect(() => {
    StatusEndpoint.authenticatedGet()
      .then(() => {
        const token = Cookies.get(COOKIE_TOKEN);
        if (token) {
          const decodedToken = jwtDecode(token);
          setUser(decodedToken as User);
        }

        redirectToHome();
      })
      .catch(() => {
        redirectToLogin();
      })
      .finally(() => setIsInitialized(true));
  }, [redirectToHome, redirectToLogin]);

  const login = useCallback(
    (jwt_token: string) => {
      Cookies.set(COOKIE_TOKEN, jwt_token, { expires: 7, secure: true });
      const user = jwtDecode(jwt_token) as User;
      setUser(user);
      redirectToHome();
    },
    [redirectToHome],
  );

  const logout = useCallback(() => {
    Cookies.remove(COOKIE_TOKEN);
    setUser(undefined);
    redirectToLogin();
  }, [redirectToLogin]);

  return (
    <AuthenticationContext.Provider
      value={{ user, login, logout, isAuthenticated, isAdmin, isInitialized }}
    >
      {children}
    </AuthenticationContext.Provider>
  );
};

export default AuthenticationContextProvider;
