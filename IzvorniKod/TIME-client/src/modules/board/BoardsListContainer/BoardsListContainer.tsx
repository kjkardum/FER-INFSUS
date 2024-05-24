"use client";
import React, { useMemo, useState } from "react";
import { Box, Divider, IconButton, Typography } from "@mui/material";
import BoardsListSearch from "@/modules/board/BoardsListContainer/components/BoardsListSearch";
import BoardsList from "@/modules/board/BoardsListContainer/components/BoardsList";
import useTaskboardGetAllBoards from "@/api/hooks/TaskboardEndpoint/useTaskboardGetBoards";
import WholeSectionLoading from "@/components/WholeSectionLoading/WholeSectionLoading";
import { AddCircle } from "@mui/icons-material";
import BoardManagementModal from "@/modules/board/BoardsListContainer/components/BoardManagementModal";
import useAuthentication from "@/hooks/useAuthentication";

const BoardsListContainer = () => {
  const { isAdmin } = useAuthentication();

  const [createBoardModalOpen, setCreateBoardModalOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState<string>("");

  const { data, isLoading, isSuccess } = useTaskboardGetAllBoards();

  const filteredData = useMemo(
    () =>
      data?.filter((board) =>
        board.name.toLowerCase().includes(searchQuery.toLowerCase()),
      ),
    [data, searchQuery],
  );

  return (
    <Box>
      <Typography variant="h5" gutterBottom>
        Boards
        {isAdmin && (
          <IconButton
            sx={{ ml: "0.25rem" }}
            onClick={() => setCreateBoardModalOpen(true)}
          >
            <AddCircle />
          </IconButton>
        )}
      </Typography>
      <BoardsListSearch
        value={searchQuery}
        onChange={(value) => setSearchQuery(value)}
      />
      <Divider sx={{ my: 2 }} />
      {isLoading && <WholeSectionLoading />}
      {isSuccess && !searchQuery && data && data.length === 0 && (
        <Typography>
          No boards found. Click on the + icon to create a new board.
        </Typography>
      )}
      {isSuccess &&
        searchQuery &&
        filteredData &&
        filteredData.length === 0 && (
          <Typography>No boards found for query</Typography>
        )}
      {isSuccess && filteredData && filteredData.length > 0 && (
        <BoardsList boards={filteredData} />
      )}

      <BoardManagementModal
        open={createBoardModalOpen}
        handleClose={() => setCreateBoardModalOpen(false)}
      />
    </Box>
  );
};

export default BoardsListContainer;
