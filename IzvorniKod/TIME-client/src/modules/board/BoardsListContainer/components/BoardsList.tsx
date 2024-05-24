"use client";
import { Alert, Grid, Menu, MenuItem, Snackbar } from "@mui/material";
import React, { useState } from "react";
import DeletePrompt from "@/components/DeletePrompt/DeletePrompt";
import BoardListItem from "@/modules/board/BoardsListContainer/components/BoardListItem";
import TaskboardEndpoint from "@/api/endpoints/TaskboardEndpoint";
import { Board } from "@/modules/board/BoardsListContainer/@types/Board";
import { useQueryClient } from "@tanstack/react-query";
import { taskboardGetAllBoardsKey } from "@/api/reactQueryKeys/TaskboardEndpointKeys";
import BoardManagementModal from "@/modules/board/BoardsListContainer/components/BoardManagementModal";
import useAuthentication from "@/hooks/useAuthentication";

interface BoardsListProps {
  boards: Array<Board>;
}

const BoardsList = ({ boards }: BoardsListProps) => {
  const { isAdmin } = useAuthentication();

  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [openDeleteBoardPrompt, setOpenDeleteBoardPrompt] =
    useState<boolean>(false);
  const [openEditBoardPrompt, setOpenEditBoardPrompt] =
    useState<boolean>(false);
  const [selectedBoard, setSelectedBoard] = useState<Board | undefined>(
    undefined,
  );
  const [snackBarMessage, setSnackBarMessage] = useState<
    | {
        message: string;
        color: "success" | "error" | "info" | "warning";
      }
    | undefined
  >(undefined);

  const queryClient = useQueryClient();

  const menuOpen = (
    event: React.MouseEvent<HTMLButtonElement>,
    board: Board,
  ) => {
    event.preventDefault();
    event.stopPropagation();
    setAnchorEl(event.currentTarget);
    setSelectedBoard(board);
  };

  const menuClose = () => {
    setAnchorEl(null);
    setSelectedBoard(undefined);
  };

  const onOpenDeleteBoardPrompt = () => {
    setOpenDeleteBoardPrompt(true);
    setAnchorEl(null);
  };

  const onOpenEditBoardPrompt = () => {
    setOpenEditBoardPrompt(true);
    setAnchorEl(null);
  };

  const onCloseEditBoardPrompt = () => {
    setOpenEditBoardPrompt(false);
    setSelectedBoard(undefined);
  };

  const onDeleteBoard = () => {
    if (selectedBoard?.id)
      TaskboardEndpoint.apiTaskboardIdDelete(selectedBoard?.id)
        .then(() => {
          setSnackBarMessage({
            message: "Board deleted successfully.",
            color: "success",
          });
          queryClient.invalidateQueries({ queryKey: taskboardGetAllBoardsKey });
        })
        .catch(() => {
          setSnackBarMessage({
            message: "Failed to delete board.",
            color: "error",
          });
        })
        .finally(() => {
          setOpenDeleteBoardPrompt(false);
          menuClose();
        });
  };

  return (
    <>
      <Grid container spacing={2}>
        {boards.map((board) => (
          <Grid item xs={12} sm={6} md={4} lg={3} xl={2} key={board.id}>
            <BoardListItem
              isAdmin={isAdmin}
              menuOpen={(e) => menuOpen(e, board)}
              board={board}
            />
          </Grid>
        ))}
      </Grid>
      <Menu
        id="basic-menu"
        anchorEl={anchorEl}
        open={!!anchorEl}
        onClose={menuClose}
      >
        <MenuItem onClick={onOpenEditBoardPrompt}>Edit</MenuItem>
        <MenuItem onClick={onOpenDeleteBoardPrompt}>Delete</MenuItem>
      </Menu>
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

      <DeletePrompt
        open={openDeleteBoardPrompt}
        handleClose={() => setOpenDeleteBoardPrompt(false)}
        handleConfirm={onDeleteBoard}
      />
      <BoardManagementModal
        open={openEditBoardPrompt}
        board={selectedBoard}
        handleClose={onCloseEditBoardPrompt}
      />
    </>
  );
};

export default BoardsList;
