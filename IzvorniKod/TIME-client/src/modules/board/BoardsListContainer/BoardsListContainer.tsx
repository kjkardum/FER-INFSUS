import React from "react";
import { Box, Divider, Typography } from "@mui/material";
import BoardsListSearch from "@/modules/board/BoardsListContainer/components/BoardsListSearch";
import BoardsList from "@/modules/board/BoardsListContainer/components/BoardsList";

const BoardsListContainer = () => {
  return (
    <Box>
      <Typography variant="h5" gutterBottom>
        Boards
      </Typography>
      <BoardsListSearch />
      <Divider sx={{ my: 2 }} />
      <BoardsList />
    </Box>
  );
};

export default BoardsListContainer;
