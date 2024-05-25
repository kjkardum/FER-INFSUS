"use client";
import React, { useEffect, useState } from "react";
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Stack,
  TextField,
} from "@mui/material";
import { LoadingButton } from "@mui/lab";
import { useQueryClient } from "@tanstack/react-query";
import taskboardEndpoint from "@/api/endpoints/TaskboardEndpoint";
import { taskboardGetAllBoardsKey } from "@/api/reactQueryKeys/TaskboardEndpointKeys";
import { TaskboardSimpleDto } from "@/api/generated";
import useSnackbar from "@/hooks/useSnackbar";

interface Props {
  open?: boolean;
  board?: TaskboardSimpleDto;
  handleClose: () => void;
}

const BoardManagementModal = ({ open, board, handleClose }: Props) => {
  const [boardName, setBoardName] = useState<string>(board?.name ?? "");
  const [boardDescription, setBoardDescription] = useState<string>(
    board?.description ?? "",
  );
  const [isLoading, setIsLoading] = useState<boolean>(false);

  const queryClient = useQueryClient();
  const { showSnackbar } = useSnackbar();

  useEffect(() => {
    if (board) {
      setBoardName(board.name ?? "");
      setBoardDescription(board.description ?? "");
    }
  }, [board]);

  const handleCreateNewBoard = () => {
    if (!boardName || !boardDescription) {
      showSnackbar("Please fill all fields.", "error");
      return;
    }

    setIsLoading(true);
    taskboardEndpoint
      .apiTaskboardPost({
        name: boardName,
        description: boardDescription,
      })
      .then(() => {
        showSnackbar("Board saved successfully.", "success");
        queryClient
          .invalidateQueries({ queryKey: taskboardGetAllBoardsKey })
          .then(() => {
            handleClose();
          });
      })
      .catch(() => {
        showSnackbar("Failed to save board.", "error");
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const handleEditBoard = () => {
    if (!boardName || !boardDescription || !board || !board.id) {
      showSnackbar("Please fill all fields.", "error");
      return;
    }

    setIsLoading(true);
    taskboardEndpoint
      .apiTaskboardIdPut(board?.id, {
        name: boardName,
        description: boardDescription,
      })
      .then(() => {
        showSnackbar("Board saved successfully.", "success");
        queryClient
          .invalidateQueries({ queryKey: taskboardGetAllBoardsKey })
          .then(() => {
            handleClose();
          });
      })
      .catch(() => {
        showSnackbar("Failed to save board.", "error");
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
        <Stack direction={"column"} spacing={3} py={"1rem"} component={"form"}>
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
  );
};

export default BoardManagementModal;
