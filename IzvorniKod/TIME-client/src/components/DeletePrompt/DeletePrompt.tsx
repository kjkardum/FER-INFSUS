import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Typography,
} from "@mui/material";
import React from "react";

interface Props {
  handleClose: () => void;
  handleConfirm: () => void;
}

const DeletePrompt = (props: Props) => {
  return (
    <Dialog
      open={true}
      onClose={props.handleClose}
      aria-labelledby="dialog-delte-prompt-title"
      aria-describedby="dialog-delete-prompt-description"
    >
      <DialogTitle id="dialog-delte-prompt-title">
        <Typography variant="h6">Are you sure you want to delete?</Typography>
      </DialogTitle>
      <DialogContent>
        <DialogContentText id="dialog-delete-prompt-description">
          <Typography variant="body1">
            Once you delete this, it cannot be undone.
          </Typography>
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={props.handleClose} autoFocus>
          Cancel
        </Button>
        <Button color={"error"} onClick={props.handleClose}>
          Delete
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default DeletePrompt;
