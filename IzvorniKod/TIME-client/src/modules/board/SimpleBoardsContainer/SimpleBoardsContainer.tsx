"use client";
import { Box, Button, Divider, Typography } from "@mui/material";
import React from "react";
import WholeSectionLoading from "@/components/WholeSectionLoading/WholeSectionLoading";
import Link from "next/link";
import useTaskboardGetAssignedBoards from "@/api/hooks/TaskboardEndpoint/useTaskboardGetAssignedBoards";

const SimpleBoardsContainer = () => {
  const { data, isLoading, isSuccess } = useTaskboardGetAssignedBoards();

  return (
    <Box height={"100%"}>
      <Typography variant={"h5"} marginBottom={"1rem"} align={"center"}>
        My Boards
      </Typography>
      {isLoading && <WholeSectionLoading />}
      {isSuccess && data && data.length === 0 && (
        <Typography align={"center"}>No boards found.</Typography>
      )}
      {isSuccess && data && data.length > 0 && (
        <>
          <Divider sx={{ my: 2 }} />
          <Box
            display={"grid"}
            gridTemplateColumns={"1fr 1fr"}
            rowGap={"1rem"}
            columnGap={"1rem"}
          >
            {data?.slice(0, 10).map((board) => (
              <Link
                href={`/board/${board.id}`}
                key={board.id}
                style={{
                  textDecoration: "none",
                  color: "inherit",
                  width: "100%",
                }}
              >
                <Button
                  variant={"outlined"}
                  color={"inherit"}
                  key={board.id}
                  fullWidth
                >
                  {board.name}
                </Button>
              </Link>
            ))}
          </Box>
        </>
      )}
    </Box>
  );
};

export default SimpleBoardsContainer;
