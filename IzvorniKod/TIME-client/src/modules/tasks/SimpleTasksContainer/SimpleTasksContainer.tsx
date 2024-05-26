"use client";
import React, { useMemo, useState } from "react";
import { Box, Divider, Stack, Typography } from "@mui/material";
import useTaskItemGetAssigned from "@/api/hooks/TaskItemEndpoint/useTaskItemGetAssigned";
import TaskComponent from "@/modules/board/DetailedBoardContainer/components/TaskComponent";
import useAuthentication from "@/hooks/useAuthentication";
import { useRouter } from "next/navigation";
import DeletePrompt from "@/components/DeletePrompt/DeletePrompt";
import taskItemEndpoint from "@/api/endpoints/TaskItemEndpoint";
import useSnackbar from "@/hooks/useSnackbar";
import SnackbarMessages from "@/contexts/snackbar/SnackbarMessages";
import { AxiosError } from "axios";
import { ErrorResponseType } from "@/api/generated/@types/ErrorResponseType";
import WholeSectionLoading from "@/components/WholeSectionLoading/WholeSectionLoading";

const SimpleTasksContainer = () => {
  const [deleteTaskId, setDeleteTaskId] = useState<string | null>(null);

  const { data: tasks, refetch, isLoading } = useTaskItemGetAssigned();
  const { isAdmin } = useAuthentication();
  const router = useRouter();
  const { showSnackbar } = useSnackbar();

  const sortedTasks = useMemo(
    () =>
      tasks?.slice(0, 5)?.sort((a, b) => {
        if (!a.createdAt || !b.createdAt) return 0;
        return (
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
      }),
    [tasks],
  );

  const handleTaskClick = (taskId?: string) => {
    router.push(`/task/${taskId}`);
  };

  const handleOpenDeleteTaskModal = (
    e: React.MouseEvent<HTMLButtonElement, MouseEvent>,
    taskId?: string,
  ) => {
    e.preventDefault();
    e.stopPropagation();
    if (!taskId) return;

    setDeleteTaskId(taskId);
  };

  const handleDeleteTask = () => {
    if (!deleteTaskId) return;
    taskItemEndpoint
      .apiTaskItemIdDelete(deleteTaskId)
      .then(() => {
        showSnackbar(SnackbarMessages.tasks.deleteSuccess, "success");
        setDeleteTaskId(null);
        refetch();
      })
      .catch((error: AxiosError<ErrorResponseType>) => {
        showSnackbar(
          error.response?.data.detail || SnackbarMessages.tasks.deleteError,
          "error",
        );
      });
  };

  return (
    <Box overflow={"auto"}>
      <Typography variant={"h5"} marginBottom={"1rem"} align={"center"}>
        Zadnjih {sortedTasks?.length} zadataka
      </Typography>

      <Divider sx={{ my: 2 }} />

      {tasks?.length === 0 && (
        <Typography variant={"body1"} align={"center"}>
          Trenutno nemate nijedan zadatak.
        </Typography>
      )}

      {isLoading && <WholeSectionLoading />}

      {tasks && tasks?.length > 0 && (
        <Stack spacing={2} py={"1rem"}>
          {sortedTasks?.map((task) => (
            <TaskComponent
              key={task.id}
              showTaskStatus
              hideTaskAssigned
              task={task}
              handleTaskClick={handleTaskClick}
              isAdmin={isAdmin}
              handleOpenDeleteTaskModal={handleOpenDeleteTaskModal}
            />
          ))}
        </Stack>
      )}

      {isAdmin && (
        <DeletePrompt
          open={!!deleteTaskId}
          handleClose={() => setDeleteTaskId(null)}
          handleConfirm={handleDeleteTask}
        />
      )}
    </Box>
  );
};

export default SimpleTasksContainer;
