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
import useTaskboardGetAssignedBoards from "@/api/hooks/TaskboardEndpoint/useTaskboardGetAssignedBoards";

const BoardsListContainer = () => {
  const { isAdmin } = useAuthentication();

  const [createBoardModalOpen, setCreateBoardModalOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState<string>("");

  const { data: allBoards, isLoading, isSuccess } = useTaskboardGetAllBoards();
  const {
    data: myBoards,
    isLoading: isLoadingMyBoards,
    isSuccess: isSuccessMyBoards,
  } = useTaskboardGetAssignedBoards(isAdmin);

  // TODO: check potential caching issues

  const filteredData = useMemo(
    () =>
      (allBoards || myBoards)?.filter(
        (board) =>
          board?.name &&
          board?.name.toLowerCase().includes(searchQuery.toLowerCase()),
      ),
    [allBoards, searchQuery, myBoards],
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
      {isLoadingMyBoards && isLoading && <WholeSectionLoading />}
      {(isSuccess || isSuccessMyBoards) &&
        !searchQuery &&
        filteredData &&
        filteredData.length === 0 && (
          <Typography>
            No boards found.{" "}
            {isAdmin && "Click on the + icon to create a new board."}
          </Typography>
        )}
      {(isSuccess || isSuccessMyBoards) &&
        searchQuery &&
        filteredData &&
        filteredData.length === 0 && (
          <Typography>No boards found for query</Typography>
        )}
      {(isSuccess || isSuccessMyBoards) &&
        filteredData &&
        filteredData.length > 0 && <BoardsList boards={filteredData} />}
      {isAdmin && (
        <BoardManagementModal
          open={createBoardModalOpen}
          handleClose={() => setCreateBoardModalOpen(false)}
        />
      )}
    </Box>
  );
};

export default BoardsListContainer;
