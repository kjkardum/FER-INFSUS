"use client";
import React, { useMemo, useState } from "react";
import {
  Backdrop,
  Box,
  CircularProgress,
  Divider,
  Stack,
  Typography,
} from "@mui/material";
import useAuthentication from "@/hooks/useAuthentication";
import { useRouter } from "next/navigation";
import useTaskItemGetAssigned from "@/api/hooks/TaskItemEndpoint/useTaskItemGetAssigned";
import SearchInput from "@/components/SearchInput/SearchInput";
import { Task } from "@mui/icons-material";
import TaskComponent from "@/modules/board/DetailedBoardContainer/components/TaskComponent";
import taskItemEndpoint from "@/api/endpoints/TaskItemEndpoint";
import useSnackbar from "@/hooks/useSnackbar";
import SnackbarMessages from "@/contexts/snackbar/SnackbarMessages";
import { AxiosError } from "axios";
import { ErrorResponseType } from "@/api/generated/@types/ErrorResponseType";
import DeletePrompt from "@/components/DeletePrompt/DeletePrompt";

const AssignedTasksContainer = () => {
  const [searchValue, setSearchValue] = useState<string>("");
  const [deleteTaskId, setDeleteTaskId] = useState<string | null>(null);

  const { isAdmin } = useAuthentication();
  const { showSnackbar } = useSnackbar();
  const router = useRouter();
  const { data: tasks, isLoading, refetch } = useTaskItemGetAssigned();

  const filteredTasks = useMemo(
    () =>
      tasks?.filter(
        (task) =>
          task.name?.toLowerCase().includes(searchValue.toLowerCase()) ||
          task.description?.toLowerCase().includes(searchValue.toLowerCase()),
      ),
    [tasks, searchValue],
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
    <>
      <Box>
        <Typography variant="h4" gutterBottom>
          Vaši zadaci
        </Typography>
        <SearchInput
          onChange={(value) => setSearchValue(value)}
          inputPlaceholder="Pretraži zadatke..."
          LeftIcon={<Task />}
        />
        <Divider sx={{ my: 2 }} />
        {filteredTasks && filteredTasks.length === 0 && (
          <Typography>Nemate niti jedan zadatak.</Typography>
        )}
        <Stack spacing={2}>
          {filteredTasks &&
            filteredTasks.map((task) => (
              <TaskComponent
                key={task.id}
                showTaskStatus
                hideTaskAssigned
                task={task}
                isAdmin={isAdmin}
                handleTaskClick={handleTaskClick}
                handleOpenDeleteTaskModal={handleOpenDeleteTaskModal}
              />
            ))}
        </Stack>
      </Box>

      <Backdrop open={isLoading}>
        <CircularProgress color={"inherit"} />
      </Backdrop>

      {isAdmin && (
        <DeletePrompt
          open={!!deleteTaskId}
          handleClose={() => setDeleteTaskId(null)}
          handleConfirm={handleDeleteTask}
        />
      )}
    </>
  );
};

export default AssignedTasksContainer;
