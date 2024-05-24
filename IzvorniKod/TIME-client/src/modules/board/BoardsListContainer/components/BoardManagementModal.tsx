"use client";
import React, { useEffect, useState } from "react";
import {
  Alert,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Snackbar,
  Stack,
  TextField,
} from "@mui/material";
import { LoadingButton } from "@mui/lab";
import { Board } from "@/modules/board/BoardsListContainer/@types/Board";
import { useQueryClient } from "@tanstack/react-query";
import taskboardEndpoint from "@/api/endpoints/TaskboardEndpoint";
import { taskboardGetAllBoardsKey } from "@/api/reactQueryKeys/TaskboardEndpointKeys";

interface Props {
  open?: boolean;
  board?: Board;
  handleClose: () => void;
}

const BoardManagementModal = ({ open, board, handleClose }: Props) => {
  const [boardName, setBoardName] = useState<string>(board?.name ?? "");
  const [boardDescription, setBoardDescription] = useState<string>(
    board?.description ?? "",
  );
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [snackBarMessage, setSnackBarMessage] = useState<
    | { message: string; color: "success" | "error" | "info" | "warning" }
    | undefined
  >(undefined);

  const queryClient = useQueryClient();

  useEffect(() => {
    if (board) {
      setBoardName(board.name);
      setBoardDescription(board.description);
    }
  }, [board]);

  const handleCreateNewBoard = () => {
    if (!boardName || !boardDescription) {
      setSnackBarMessage({
        message: "Please fill all fields.",
        color: "error",
      });
      return;
    }

    setIsLoading(true);
    taskboardEndpoint
      .apiTaskboardPost({
        name: boardName,
        description: boardDescription,
      })
      .then(() => {
        setSnackBarMessage({
          message: "Board saved successfully.",
          color: "success",
        });
        queryClient
          .invalidateQueries({ queryKey: taskboardGetAllBoardsKey })
          .then(() => {
            handleClose();
          });
      })
      .catch(() => {
        setSnackBarMessage({
          message: "Failed to save board.",
          color: "error",
        });
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const handleEditBoard = () => {
    if (!boardName || !boardDescription || !board) {
      setSnackBarMessage({
        message: "Please fill all fields.",
        color: "error",
      });
      return;
    }

    setIsLoading(true);
    taskboardEndpoint
      .apiTaskboardIdPut(board?.id, {
        name: boardName,
        description: boardDescription,
      })
      .then(() => {
        setSnackBarMessage({
          message: "Board saved successfully.",
          color: "success",
        });
        queryClient
          .invalidateQueries({ queryKey: taskboardGetAllBoardsKey })
          .then(() => {
            handleClose();
          });
      })
      .catch(() => {
        setSnackBarMessage({
          message: "Failed to save board.",
          color: "error",
        });
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const handleSave = () => {
    if (!board) {
      handleCreateNewBoard();
      return;
    }

    handleEditBoard();
  };

  return (
    <>
      <Dialog
        open={!!open}
        onClose={handleClose}
        aria-labelledby="dialog-userManagment-prompt-title"
        aria-describedby="dialog-userManagment-prompt-description"
        fullWidth={true}
      >
        <DialogTitle id="dialog-userManagment-prompt-title">
          {board ? "Edit" : "Create new"} board
        </DialogTitle>
        <DialogContent>
          <Stack
            direction={"column"}
            spacing={3}
            py={"1rem"}
            component={"form"}
          >
            <TextField
              label={"Board name"}
              fullWidth
              type={"text"}
              value={boardName}
              onChange={(e) => setBoardName(e.target.value)}
            />
            <TextField
              label={"Board description"}
              fullWidth
              type={"text"}
              value={boardDescription}
              onChange={(e) => setBoardDescription(e.target.value)}
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button
            variant={"outlined"}
            color={"secondary"}
            onClick={handleClose}
            autoFocus
          >
            Cancel
          </Button>
          <LoadingButton
            variant={"contained"}
            color={"primary"}
            onClick={handleSave}
            loading={isLoading}
          >
            Save
          </LoadingButton>
        </DialogActions>
      </Dialog>
      <Snackbar
        open={!!snackBarMessage}
        autoHideDuration={6000}
        onClose={() => setSnackBarMessage(undefined)}
      >
        <Alert
          onClose={() => setSnackBarMessage(undefined)}
          severity={snackBarMessage?.color}
          variant="filled"
          sx={{ width: "100%" }}
        >
          {snackBarMessage?.message}
        </Alert>
      </Snackbar>
    </>
  );
};

export default BoardManagementModal;
