"use client";
import React, { useState } from "react";
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Box,
  Button,
  Chip,
  IconButton,
  Paper,
  Stack,
  styled,
  Typography,
} from "@mui/material";
import { Delete, ExpandMore } from "@mui/icons-material";
import {
  TaskboardDetailedDto,
  TaskItemSimpleDto,
  TaskItemState,
} from "@/api/generated";
import useAuthentication from "@/hooks/useAuthentication";
import CreateNewTask from "@/modules/board/DetailedBoardContainer/components/CreateNewTask";
import dayjs from "dayjs";
import { useRouter } from "next/navigation";
import DeletePrompt from "@/components/DeletePrompt/DeletePrompt";
import taskItemEndpoint from "@/api/endpoints/TaskItemEndpoint";
import { useQueryClient } from "@tanstack/react-query";
import { taskboardGetBoardDetailsKey } from "@/api/reactQueryKeys/TaskboardEndpointKeys";
import useSnackbar from "@/hooks/useSnackbar";
import getColorFromTaskStatus from "@/utils/getColorFromTaskStatus";

const groupTasksByStatus = (tasks: TaskItemSimpleDto[]) => {
  const states = Object.keys(TaskItemState);
  const groups: { [key: string]: TaskItemSimpleDto[] } = {};

  states.forEach((state) => {
    const tasksInState = tasks.filter((task) => task.state === state);
    if (tasksInState.length) groups[state] = tasksInState;
  });

  return groups;
};

const HoverPaper = styled(Paper)(({ theme }) => ({
  "&:hover": {
    boxShadow: theme.shadows[3],
    cursor: "pointer",
  },
}));

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
        showSnackbar("Task deleted successfully.", "success");
        queryClient.invalidateQueries({
          queryKey: taskboardGetBoardDetailsKey(boardId),
        });
      })
      .catch(() => {
        showSnackbar("Failed to delete task.", "error");
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
          Tasks
        </Typography>

        <Button
          size={"small"}
          variant="contained"
          color="primary"
          sx={{ ml: "0.5rem" }}
          onClick={handleCreateTask}
        >
          Add Task
        </Button>
      </Stack>
      {(!tasks || tasks.length === 0) && (
        <Typography variant="body1">No tasks found.</Typography>
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
                label={`${groupedTasks[key]?.length || 0} tasks`}
                color={getColorFromTaskStatus(key as TaskItemState)}
              />
              <Typography variant="subtitle1">{key}</Typography>
            </Stack>
          </AccordionSummary>
          <AccordionDetails>
            <Stack spacing={2}>
              {groupedTasks[key].map((task) => (
                <HoverPaper
                  sx={{
                    p: "1rem",
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center",
                  }}
                  key={task.id}
                  elevation={2}
                  onClick={() => handleTaskClick(task.id)}
                >
                  <Box>
                    <Typography variant="h6">{task.name}</Typography>
                    <Typography variant="body1">{task.description}</Typography>
                    {/* TODO: clip description if too long */}
                    <Typography variant="body2">
                      {`Created at: ${task.createdAt ? dayjs(task.createdAt).toDate().toDateString() : dayjs().toDate().toDateString()}`}
                    </Typography>
                  </Box>
                  <Stack
                    columnGap={"0.5rem"}
                    alignItems={"center"}
                    direction={"row"}
                  >
                    <Stack
                      columnGap={"0.5rem"}
                      rowGap={"0.25rem"}
                      alignItems={"center"}
                      sx={{ flexDirection: { xs: "column", md: "row" } }}
                    >
                      {task.assignedUser && (
                        <Chip
                          color={"secondary"}
                          label={`Assigned to: ${task.assignedUser.firstName} ${task.assignedUser.lastName}`}
                        />
                      )}
                      {!task.assignedUser && <Chip label={`Not assigned`} />}
                    </Stack>
                    {isAdmin && (
                      <IconButton
                        onClick={(e) => handleOpenDeleteTaskModal(e, task.id)}
                      >
                        <Delete />
                      </IconButton>
                    )}
                  </Stack>
                </HoverPaper>
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
