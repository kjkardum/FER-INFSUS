"use client";
import { Grid, Menu, MenuItem } from "@mui/material";
import React, { useState } from "react";
import DeletePrompt from "@/components/DeletePrompt/DeletePrompt";
import BoardListItem from "@/modules/board/BoardsListContainer/components/BoardListItem";

const BoardsList = () => {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [deleteBoardId, setDeleteBoardId] = useState<string | undefined>(
    undefined,
  );

  const menuOpen = (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();
    event.stopPropagation();
    setAnchorEl(event.currentTarget);
  };

  const menuClose = () => {
    setAnchorEl(null);
  };

  const onOpenDeleteBoardPrompt = (id: string) => {
    setDeleteBoardId(id);
  };

  const onDeleteBoard = () => {
    // TODO: implement delete logic
  };

  return (
    <>
      <Grid container>
        <Grid item xs={12} sm={6} md={4} lg={3} xl={2}>
          <BoardListItem menuOpen={menuOpen} />
        </Grid>
      </Grid>
      <Menu
        id="basic-menu"
        anchorEl={anchorEl}
        open={!!anchorEl}
        onClose={menuClose}
      >
        <MenuItem onClick={menuClose}>Edit</MenuItem>
        <MenuItem onClick={() => onOpenDeleteBoardPrompt("balsdkfgjas")}>
          Delete
        </MenuItem>
      </Menu>
      {deleteBoardId && (
        <DeletePrompt
          handleClose={() => setDeleteBoardId(undefined)}
          handleConfirm={onDeleteBoard}
        />
      )}
    </>
  );
};

export default BoardsList;
