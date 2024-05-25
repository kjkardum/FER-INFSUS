"use client";
import React, { PropsWithChildren, useCallback, useState } from "react";
import { SnackbarColor } from "@/contexts/snackbar/@types/SnackbarContextType";
import SnackbarContext from "./SnackbarContext";
import { Alert, Snackbar } from "@mui/material";

type SnackbarType = {
  message: string;
  color: SnackbarColor;
  duration: number;
};

const SnackbarContextProvider = ({ children }: PropsWithChildren) => {
  const [snackbarOpen, setSnackbarOpen] = useState(false);
  const [snackbar, setSnackbar] = useState<SnackbarType | null>(null);

  const showSnackbar = useCallback(
    (message: string, color: SnackbarColor, duration = 6000) => {
      setSnackbar({ message, color, duration });
      setSnackbarOpen(true);
    },
    [],
  );

  const handleClose = useCallback(() => {
    setSnackbarOpen(false);
    setTimeout(() => {
      setSnackbar(null);
    }, 200);
  }, []);

  return (
    <SnackbarContext.Provider value={{ showSnackbar }}>
      {children}
      <Snackbar
        open={snackbarOpen}
        autoHideDuration={6000}
        onClose={handleClose}
      >
        <Alert
          onClose={handleClose}
          severity={snackbar?.color}
          variant="filled"
          sx={{ width: "100%" }}
        >
          {snackbar?.message}
        </Alert>
      </Snackbar>
    </SnackbarContext.Provider>
  );
};

export default SnackbarContextProvider;
