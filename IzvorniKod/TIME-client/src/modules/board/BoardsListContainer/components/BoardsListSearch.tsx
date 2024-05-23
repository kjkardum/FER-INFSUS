import React from "react";
import { IconButton, InputBase, Paper } from "@mui/material";
import { DeveloperBoard, Search } from "@mui/icons-material";

const BoardsListSearch = () => {
  return (
    <Paper
      component="form"
      sx={{ p: "2px 4px", display: "flex", alignItems: "center", width: 400 }}
    >
      <IconButton sx={{ p: "10px" }} aria-label="menu">
        <DeveloperBoard />
      </IconButton>
      <InputBase
        sx={{ ml: 1, flex: 1 }}
        placeholder="Search Boards"
        inputProps={{ "aria-label": "search boards" }}
      />
      <IconButton type="button" sx={{ p: "10px" }} aria-label="search">
        <Search />
      </IconButton>
    </Paper>
  );
};

export default BoardsListSearch;
