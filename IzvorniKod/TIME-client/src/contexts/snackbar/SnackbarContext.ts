"use client";
import { createContext } from "react";
import { SnackbarContextType } from "@/contexts/snackbar/@types/SnackbarContextType";

const SnackbarContext = createContext<SnackbarContextType>({
  showSnackbar: () => {},
});

export default SnackbarContext;
