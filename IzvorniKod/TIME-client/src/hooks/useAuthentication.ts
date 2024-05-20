"use client";
import { useContext } from "react";
import AuthenticationContext from "@/contexts/authentication/AuthenticationContext";

const useAuthentication = () => {
  const userContext = useContext(AuthenticationContext);

  if (!userContext) {
    throw new Error(
      "useAuthentication must be used within an AuthenticationContextProvider",
    );
  }

  return userContext;
};

export default useAuthentication;
