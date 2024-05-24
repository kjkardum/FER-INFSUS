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
    >
      <DialogTitle id="dialog-delte-prompt-title">
        Are you sure you want to delete?
      </DialogTitle>
      <DialogContent>
        <DialogContentText id="dialog-delete-prompt-description">
          Once you delete this, it cannot be undone.
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={props.handleClose} variant={"outlined"} autoFocus>
          Cancel
        </Button>
        <Button
          color={"error"}
          variant={"contained"}
          onClick={props.handleConfirm}
        >
          Delete
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default DeletePrompt;
