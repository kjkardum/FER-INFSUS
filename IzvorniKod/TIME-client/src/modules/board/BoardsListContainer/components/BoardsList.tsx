"use client";
import { Grid, Menu, MenuItem } from "@mui/material";
import React, { useState } from "react";
import DeletePrompt from "@/components/DeletePrompt/DeletePrompt";
import BoardListItem from "@/modules/board/BoardsListContainer/components/BoardListItem";
import TaskboardEndpoint from "@/api/endpoints/TaskboardEndpoint";
import { useQueryClient } from "@tanstack/react-query";
import { taskboardGetAllBoardsKey } from "@/api/reactQueryKeys/TaskboardEndpointKeys";
import BoardManagementModal from "@/modules/board/BoardsListContainer/components/BoardManagementModal";
import useAuthentication from "@/hooks/useAuthentication";
import { TaskboardSimpleDto } from "@/api/generated";
import useSnackbar from "@/hooks/useSnackbar";

interface BoardsListProps {
  boards: Array<TaskboardSimpleDto>;
}

const BoardsList = ({ boards }: BoardsListProps) => {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [openDeleteBoardPrompt, setOpenDeleteBoardPrompt] =
    useState<boolean>(false);
  const [openEditBoardPrompt, setOpenEditBoardPrompt] =
    useState<boolean>(false);
  const [selectedBoard, setSelectedBoard] = useState<
    TaskboardSimpleDto | undefined
  >(undefined);

  const { isAdmin } = useAuthentication();
  const queryClient = useQueryClient();
  const { showSnackbar } = useSnackbar();

  const menuOpen = (
    event: React.MouseEvent<HTMLButtonElement>,
    board: TaskboardSimpleDto,
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
          showSnackbar("Board deleted successfully.", "success");
          queryClient.invalidateQueries({ queryKey: taskboardGetAllBoardsKey });
        })
        .catch(() => {
          showSnackbar("Failed to delete board.", "error");
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
        <MenuItem onClick={onOpenEditBoardPrompt}>Uredi</MenuItem>
        <MenuItem onClick={onOpenDeleteBoardPrompt}>Izbriši</MenuItem>
      </Menu>
      {isAdmin && (
        <>
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
      )}
    </>
  );
};

export default BoardsList;
