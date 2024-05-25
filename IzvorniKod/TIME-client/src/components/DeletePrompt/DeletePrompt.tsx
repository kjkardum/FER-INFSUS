import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from "@mui/material";
import React from "react";

interface Props {
  open?: boolean;
  handleClose: () => void;
  handleConfirm: () => void;
}

const DeletePrompt = (props: Props) => {
  return (
    <Dialog
      open={!!props.open}
      onClose={props.handleClose}
      aria-labelledby="dialog-delte-prompt-title"
      aria-describedby="dialog-delete-prompt-description"
      fullWidth
    >
      <DialogTitle id="dialog-delte-prompt-title">
        Jeste li sigurni da želite obrisati?
      </DialogTitle>
      <DialogContent>
        <DialogContentText id="dialog-delete-prompt-description">
          Jednom kada obrišete, ne možete vratiti.
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={props.handleClose} variant={"outlined"} autoFocus>
          Poništi
        </Button>
        <Button
          color={"error"}
          variant={"contained"}
          onClick={props.handleConfirm}
        >
          Obriši
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default DeletePrompt;
