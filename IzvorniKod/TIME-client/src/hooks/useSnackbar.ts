import { useContext } from "react";
import SnackbarContext from "@/contexts/snackbar/SnackbarContext";

const useSnackbar = () => {
  const snackBarContext = useContext(SnackbarContext);

  if (!snackBarContext) {
    throw new Error(
      "useSnackbar must be used within a SnackbarContextProvider",
    );
  }

  return snackBarContext;
};

export default useSnackbar;
