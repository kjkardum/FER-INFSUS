"use client";
import { Box, Divider, Grid, Typography } from "@mui/material";
import React, { useEffect } from "react";
import useTaskboardGetDetails from "@/api/hooks/TaskboardEndpoint/useTaskboardGetDetails";
import { useRouter } from "next/navigation";
import WholeSectionLoading from "@/components/WholeSectionLoading/WholeSectionLoading";
import TasksList from "@/modules/board/DetailedBoardContainer/components/TasksList";
import useSnackbar from "@/hooks/useSnackbar";

interface Props {
  boardId: string;
}

const DetailedBoardContainer = ({ boardId }: Props) => {
  const router = useRouter();
  const { data, isError, isLoading, isSuccess } =
    useTaskboardGetDetails(boardId);
  const { showSnackbar } = useSnackbar();

  useEffect(() => {
    if (isError) {
      showSnackbar("Board not found.", "error");
      router.push("/boards");
    }
  }, [isError, router, showSnackbar]);

  return (
    <Box>
      <Typography variant="h5" gutterBottom>
        {data?.name}
      </Typography>
      <Typography variant="body2" gutterBottom>
        {data?.description}
      </Typography>
      <Divider sx={{ my: 2 }} />
      {isLoading && <WholeSectionLoading />}
      {isSuccess && (
        <Grid container spacing={2}>
          <Grid item xs={12} lg={8}>
            <TasksList tasks={data?.taskItems ?? []} boardId={boardId} />
          </Grid>
          <Grid item xs={12} md={4}></Grid>
        </Grid>
      )}
    </Box>
  );
};

export default DetailedBoardContainer;
