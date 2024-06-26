"use client";
import React, { useState } from "react";
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Box,
  Button,
  Chip,
  Stack,
  Typography,
} from "@mui/material";
import { ExpandMore } from "@mui/icons-material";
import {
  TaskboardDetailedDto,
  TaskItemSimpleDto,
  TaskItemState,
} from "@/api/generated";
import useAuthentication from "@/hooks/useAuthentication";
import CreateNewTask from "@/modules/board/DetailedBoardContainer/components/CreateNewTask";
import { useRouter } from "next/navigation";
import DeletePrompt from "@/components/DeletePrompt/DeletePrompt";
import taskItemEndpoint from "@/api/endpoints/TaskItemEndpoint";
import { useQueryClient } from "@tanstack/react-query";
import { taskboardGetBoardDetailsKey } from "@/api/reactQueryKeys/TaskboardEndpointKeys";
import useSnackbar from "@/hooks/useSnackbar";
import getColorFromTaskStatus from "@/utils/getColorFromTaskStatus";
import SnackbarMessages from "@/contexts/snackbar/SnackbarMessages";
import { AxiosError } from "axios";
import { ErrorResponseType } from "@/api/generated/@types/ErrorResponseType";
import TaskComponent from "@/modules/board/DetailedBoardContainer/components/TaskComponent";

const groupTasksByStatus = (tasks: TaskItemSimpleDto[]) => {
  const states = Object.keys(TaskItemState);
  const groups: { [key: string]: TaskItemSimpleDto[] } = {};

  states.forEach((state) => {
    const tasksInState = tasks.filter((task) => task.state === state);
    if (tasksInState.length) groups[state] = tasksInState;
  });

  return groups;
};

interface Props {
  board: TaskboardDetailedDto;
}

const TasksList = ({ board }: Props) => {
  const tasks = board.taskItems ?? [];
  const boardId = board.id ?? "";

  const [openCreateTaskModal, setOpenCreateTaskModal] =
    useState<boolean>(false);
  const [deleteTaskId, setDeleteTaskId] = useState<string | undefined>(
    undefined,
  );

  const { showSnackbar } = useSnackbar();
  const { isAdmin } = useAuthentication();
  const queryClient = useQueryClient();
  const router = useRouter();
  const groupedTasks = groupTasksByStatus(tasks ?? []);

  const handleCreateTask = () => {
    setOpenCreateTaskModal(true);
  };

  const handleCloseCreateTaskModal = () => {
    setOpenCreateTaskModal(false);
  };

  const handleTaskClick = (taskId?: string) => {
    if (!taskId) return;

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
        queryClient.invalidateQueries({
          queryKey: taskboardGetBoardDetailsKey(boardId),
        });
      })
      .catch((error: AxiosError<ErrorResponseType>) => {
        showSnackbar(
          error.response?.data.detail || SnackbarMessages.tasks.deleteError,
          "error",
        );
      })
      .finally(() => setDeleteTaskId(undefined));
  };

  return (
    <Box>
      <Stack
        direction="row"
        spacing={2}
        justifyContent={"space-between"}
        mb={"1rem"}
      >
        <Typography variant="h6" gutterBottom>
          Zadaci
        </Typography>

        <Button
          size={"small"}
          variant="contained"
          color="primary"
          sx={{ ml: "0.5rem" }}
          onClick={handleCreateTask}
        >
          Dodaj zadatak
        </Button>
      </Stack>
      {(!tasks || tasks.length === 0) && (
        <Typography variant="body1">Nema zadataka.</Typography>
      )}
      {Object.keys(groupedTasks).map((key) => (
        <Accordion key={`${key}_${groupedTasks[key].length}`} defaultExpanded>
          <AccordionSummary
            expandIcon={<ExpandMore />}
            aria-controls={`panel1bh-content-${key}`}
            id={`panel1bh-header-${key}`}
          >
            <Stack direction={"row"} alignItems={"center"}>
              <Chip
                sx={{ mr: "0.5rem" }}
                size={"small"}
                label={`${groupedTasks[key]?.length || 0} ${(groupedTasks[key]?.length || 0) === 1 ? "zadatak" : "zadataka"}`}
                color={getColorFromTaskStatus(key as TaskItemState)}
              />
              <Typography variant="subtitle1">{key}</Typography>
            </Stack>
          </AccordionSummary>
          <AccordionDetails>
            <Stack spacing={2}>
              {groupedTasks[key].map((task) => (
                <TaskComponent
                  key={task.id}
                  task={task}
                  handleTaskClick={handleTaskClick}
                  isAdmin={isAdmin}
                  handleOpenDeleteTaskModal={handleOpenDeleteTaskModal}
                />
              ))}
            </Stack>
          </AccordionDetails>
        </Accordion>
      ))}

      <CreateNewTask
        open={openCreateTaskModal}
        board={board}
        onClose={handleCloseCreateTaskModal}
      />

      {isAdmin && (
        <DeletePrompt
          open={!!deleteTaskId}
          handleClose={() => setDeleteTaskId(undefined)}
          handleConfirm={handleDeleteTask}
        />
      )}
    </Box>
  );
};

export default TasksList;
