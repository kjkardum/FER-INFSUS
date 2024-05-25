export type SnackbarColor = "success" | "error" | "info" | "warning";

export type SnackbarContextType = {
  showSnackbar: (
    message: string,
    color: SnackbarColor,
    duration?: number,
  ) => void;
};
