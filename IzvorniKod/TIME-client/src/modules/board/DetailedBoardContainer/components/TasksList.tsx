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
import { Delete, Edit, ExpandMore } from "@mui/icons-material";
import { TaskItemSimpleDto } from "@/api/generated";
import useAuthentication from "@/hooks/useAuthentication";
import CreateNewTask from "@/modules/board/DetailedBoardContainer/components/CreateNewTask";
import getTaskStateFromStateNumber from "@/utils/getTaskStateFromStateNumber";
import dayjs from "dayjs";
import { useRouter } from "next/navigation";
import DeletePrompt from "@/components/DeletePrompt/DeletePrompt";
import taskItemEndpoint from "@/api/endpoints/TaskItemEndpoint";
import { useQueryClient } from "@tanstack/react-query";
import { taskboardGetBoardDetailsKey } from "@/api/reactQueryKeys/TaskboardEndpointKeys";
import useSnackbar from "@/hooks/useSnackbar";

const groupTasksByStatus = (tasks: TaskItemSimpleDto[]) => {
  return tasks.reduce(
    (acc, task) => {
      if (task?.state === undefined) {
        return acc;
      }

      if (!acc[task.state]) {
        acc[task.state] = [];
      }

      acc[task.state].push(task);

      return acc;
    },
    {} as Record<string, TaskItemSimpleDto[]>,
  );
};

const statusToColor = (status: number) => {
  switch (status) {
    case 0:
      return "primary";
    case 1:
      return "secondary";
    case 2:
      return "info";
    case 3:
      return "success";
    case 4:
      return "error";
    default:
      return "default";
  }
};

const HoverPaper = styled(Paper)(({ theme }) => ({
  "&:hover": {
    boxShadow: theme.shadows[3],
    cursor: "pointer",
  },
}));

interface Props {
  tasks?: TaskItemSimpleDto[];
  boardId: string;
}

const TasksList = ({ tasks, boardId }: Props) => {
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
  const groupedTasksKeys = Object.keys(groupedTasks).sort();

  const handleCreateTask = () => {
    setOpenCreateTaskModal(true);
  };

  const handleCloseCreateTaskModal = () => {
    setOpenCreateTaskModal(false);
  };

  const handleEditTask = (
    e: React.MouseEvent<HTMLButtonElement, MouseEvent>,
    taskId?: string,
  ) => {
    e.preventDefault();
    e.stopPropagation();
    if (!taskId) return;

    router.push(`/task/${taskId}/edit`);
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
        {isAdmin && (
          <Button
            size={"small"}
            variant="contained"
            color="primary"
            sx={{ ml: "0.5rem" }}
            onClick={handleCreateTask}
          >
            Add Task
          </Button>
        )}
      </Stack>
      {groupedTasksKeys.map((key) => (
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
                color={statusToColor(Number(key))}
              />
              <Typography variant="subtitle1">
                {getTaskStateFromStateNumber(Number(key))}
              </Typography>
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
                    <IconButton onClick={(e) => handleEditTask(e, task.id)}>
                      <Edit />
                    </IconButton>
                    <IconButton
                      onClick={(e) => handleOpenDeleteTaskModal(e, task.id)}
                    >
                      <Delete />
                    </IconButton>
                  </Stack>
                </HoverPaper>
              ))}
            </Stack>
          </AccordionDetails>
        </Accordion>
      ))}

      {isAdmin && (
        <CreateNewTask
          open={openCreateTaskModal}
          boardId={boardId}
          onClose={handleCloseCreateTaskModal}
        />
      )}

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
