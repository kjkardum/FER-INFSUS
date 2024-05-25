"use client";
import {
  Backdrop,
  Box,
  Breadcrumbs,
  Button,
  Chip,
  CircularProgress,
  Divider,
  Paper,
  Stack,
  TextField,
  Typography,
  useTheme,
} from "@mui/material";
import useTaskItemGetItem from "@/api/hooks/TaskItemEndpoint/useTaskItemGetItem";
import dayjs from "dayjs";
import Select from "react-select";
import React, { useEffect, useMemo, useState } from "react";
import { TaskItemDetailedDto, TaskItemState } from "@/api/generated/api";
import useTaskboardGetDetails from "@/api/hooks/TaskboardEndpoint/useTaskboardGetDetails";
import useTaskboardGetAllBoards from "@/api/hooks/TaskboardEndpoint/useTaskboardGetBoards";
import convertUserDataToSelectOptions from "@/utils/convertUserDataToSelectOptions";
import Link from "next/link";
import getColorFromTaskStatus from "@/utils/getColorFromTaskStatus";
import ChangeLogItem from "@/modules/tasks/TaskDetailsContainer/components/ChangeLogItem";
import TaskItemEndpoint from "@/api/endpoints/TaskItemEndpoint";
import useSnackbar from "@/hooks/useSnackbar";
import { useQueryClient } from "@tanstack/react-query";
import { taskItemGetTaskItemKey } from "@/api/reactQueryKeys/TaskItemEndpointKeys";
import DeletePrompt from "@/components/DeletePrompt/DeletePrompt";
import { useRouter } from "next/navigation";
import useAuthentication from "@/hooks/useAuthentication";

interface Props {
  task: TaskItemDetailedDto;
}

const TaskContainer = ({ taskId }: { taskId: string }) => {
  const { data, isError } = useTaskItemGetItem(taskId);
  const { showSnackbar } = useSnackbar();
  const router = useRouter();

  useEffect(() => {
    if (isError) {
      showSnackbar("Task not found.", "error");
      router.push("/boards");
    }
  }, [isError, router, showSnackbar]);

  return data && <TaskDetailsContainer task={data} />;
};

const TaskDetailsContainer = ({ task }: Props) => {
  const { data: board } = useTaskboardGetDetails(
    task?.taskboardId ?? "",
    task === undefined,
  );
  const { data: allBoards } = useTaskboardGetAllBoards();

  const [taskName, setTaskName] = useState<string>(task?.name ?? "");
  const [taskDescription, setTaskDescription] = useState<string>(
    task?.description ?? "",
  );
  const [taskState, setTaskState] = useState<TaskItemState>(
    task?.state ?? "Novo",
  );
  const [assignedTo, setAssignedTo] = useState<string>(
    task?.assignedUserId ?? "",
  );
  const [onBoard, setOnBoard] = useState<string>(task?.taskboardId ?? "");
  const [isHistoryExpanded, setIsHistoryExpanded] = useState<boolean>(false);
  const [isDeletePromptOpen, setIsDeletePromptOpen] = useState<boolean>(false);
  const [isEditingTaskName, setIsEditingTaskName] = useState<boolean>(false);
  const [isEditingDescription, setIsEditingDescription] =
    useState<boolean>(false);
  const [isEditingState, setIsEditingState] = useState<boolean>(false);

  const theme = useTheme();
  const { showSnackbar } = useSnackbar();
  const queryClient = useQueryClient();
  const router = useRouter();
  const { isAdmin } = useAuthentication();

  const users = useMemo(
    () => convertUserDataToSelectOptions(board?.taskboardUsers ?? []),
    [board?.taskboardUsers],
  );

  const boardsOptions = useMemo(
    () =>
      allBoards?.map((board) => ({
        value: board.id ?? "",
        label: board.name ?? "",
      })) ?? [
        {
          value: board?.id ?? "",
          label: board?.name ?? "",
        },
      ],
    [allBoards, board],
  );

  const dateSortedHistoryLogs = useMemo(
    () =>
      task?.historyLogs
        ?.sort(
          (a, b) =>
            new Date(b.modifiedAt).getTime() - new Date(a.modifiedAt).getTime(),
        )
        .slice(0, isHistoryExpanded ? undefined : 5),
    [isHistoryExpanded, task?.historyLogs],
  );

  const handleResetData = () => {
    queryClient.invalidateQueries({
      queryKey: taskItemGetTaskItemKey(task.id ?? ""),
    });
  };

  const handleSaveTaskName = () => {
    setIsEditingTaskName(false);
    if (taskName === task?.name) return;

    TaskItemEndpoint.apiTaskItemIdRenamePost(task.id ?? "", {
      newName: taskName,
    })
      .then(() => {
        showSnackbar("Task name updated successfully.", "success");
        handleResetData();
      })
      .catch(() => {
        showSnackbar("Failed to update task name.", "error");
      });
  };

  const handleSaveDescription = () => {
    setIsEditingDescription(false);
    if (taskDescription === task?.description) return;

    TaskItemEndpoint.apiTaskItemIdChangeDescriptionPost(task.id ?? "", {
      newDescription: taskDescription,
    })
      .then(() => {
        showSnackbar("Task description updated successfully.", "success");
        handleResetData();
      })
      .catch(() => {
        showSnackbar("Failed to update task description.", "error");
      });
  };

  const handleSaveState = (newTaskState: TaskItemState) => {
    setIsEditingState(false);
    if (newTaskState === task?.state) return;

    setTaskState(newTaskState);

    TaskItemEndpoint.apiTaskItemIdChangeStatePost(task.id ?? "", {
      newState: newTaskState,
    })
      .then(() => {
        showSnackbar("Task state updated successfully.", "success");
        handleResetData();
      })
      .catch(() => {
        showSnackbar("Failed to update task state.", "error");
      });
  };

  const handleSaveAssignedTo = (assignedUserId: string) => {
    if (assignedUserId === task?.assignedUser?.id) return;

    setAssignedTo(assignedUserId);

    TaskItemEndpoint.apiTaskItemIdAssignPost(task.id ?? "", {
      assignedUserId: assignedUserId || null,
    })
      .then(() => {
        showSnackbar("Task assigned successfully.", "success");
        handleResetData();
      })
      .catch(() => {
        showSnackbar("Failed to assign task.", "error");
      });
  };

  const handleSaveOnBoard = (newTaskboardId: string) => {
    if (newTaskboardId === task?.taskboardId || newTaskboardId.trim() === "")
      return;

    setOnBoard(newTaskboardId);

    TaskItemEndpoint.apiTaskItemIdMovePost(task.id ?? "", {
      newTaskboardId: newTaskboardId,
    })
      .then(() => {
        showSnackbar("Task moved successfully.", "success");
        handleResetData();
      })
      .catch(() => {
        showSnackbar("Failed to move task.", "error");
      });
  };

  return (
    <>
      <Backdrop open={false}>
        <CircularProgress color="inherit" />
      </Backdrop>
      {task && (
        <Box>
          <Breadcrumbs aria-label="breadcrumb" sx={{ mb: "2rem" }}>
            <Link
              href={"/boards"}
              style={{ textDecoration: "none", color: "inherit" }}
            >
              <Typography color="inherit">Radne Ploče</Typography>
            </Link>
            <Link
              href={`/board/${task.taskboardId}`}
              style={{ textDecoration: "none", color: "inherit" }}
            >
              <Typography color="inherit">{board?.name}</Typography>
            </Link>
            <Typography color="text.primary">{taskName}</Typography>
          </Breadcrumbs>

          {!isEditingTaskName && (
            <Box mb={"1rem"} onClick={() => setIsEditingTaskName(true)}>
              <Typography variant="h4">{taskName}</Typography>
            </Box>
          )}
          {isEditingTaskName && (
            <Stack direction="row" spacing={2} sx={{ mb: "1rem" }}>
              <TextField
                label={"Ime zadatka"}
                value={taskName}
                onChange={(e) => setTaskName(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === "Enter" || e.key === "Escape")
                    handleSaveTaskName();
                }}
                autoFocus
                onBlur={handleSaveTaskName}
                fullWidth
              />
            </Stack>
          )}

          <Stack direction="row" spacing={2} sx={{ mb: "1rem" }}>
            {!isEditingState && (
              <Chip
                label={taskState}
                color={getColorFromTaskStatus(taskState)}
                onClick={() => setIsEditingState(true)}
              />
            )}
            {isEditingState && (
              <Select
                options={Object.values(TaskItemState).map((state) => ({
                  value: state,
                  label: state,
                }))}
                value={{
                  value: taskState,
                  label: taskState,
                }}
                onChange={(selectedOption) => {
                  handleSaveState(selectedOption?.value ?? "Novo");
                }}
                autoFocus
                defaultMenuIsOpen={true}
                onBlur={() => setIsEditingState(false)}
                placeholder={"Select task state"}
                isSearchable={true}
                isClearable={false}
                styles={{
                  control: (base) => ({
                    ...base,
                    height: "46px",
                    width: "200px",
                  }),
                }}
              />
            )}
            <Chip
              label={`Kreirano: ${task.createdAt ? dayjs(task.createdAt).format("DD-MM-YYYY HH:mm") : ""}`}
            />
          </Stack>

          <Box
            display={"grid"}
            gridTemplateColumns={"0.5fr 1fr"}
            gridTemplateRows={"1fr 1fr"}
            alignItems={"center"}
            rowGap={"1rem"}
            width={{ xs: "100%", md: "500px" }}
          >
            <Typography variant="subtitle1">Dodijeljeno:</Typography>
            <Select
              options={users}
              value={users.find((user) => user.value === assignedTo)}
              onChange={(selectedOption) =>
                handleSaveAssignedTo(selectedOption?.value ?? "")
              }
              placeholder={"Nije dodijeljeno."}
              isSearchable={true}
              isClearable={true}
              styles={{
                control: (base) => ({
                  ...base,
                  height: "46px",
                  width: "100%",
                }),
              }}
            />

            <Typography variant="subtitle1">Na radnoj ploči:</Typography>
            <Select
              options={boardsOptions}
              value={boardsOptions.find((board) => board.value === onBoard)}
              onChange={(selectedOption) =>
                handleSaveOnBoard(selectedOption?.value ?? "")
              }
              placeholder={"Nije na radnoj ploči."}
              isSearchable={true}
              isClearable={false}
              isDisabled={!isAdmin}
              styles={{
                control: (base) => ({
                  ...base,
                  height: "46px",
                  width: "100%",
                }),
              }}
            />
          </Box>

          <Divider sx={{ my: "2rem" }} />
          <Typography variant="h6" gutterBottom>
            Opis
          </Typography>
          {!isEditingDescription && (
            <Paper
              elevation={1}
              onClick={() => setIsEditingDescription(true)}
              sx={{
                backgroundColor: theme.palette.background.paper,
                p: "1rem",
              }}
            >
              <Typography
                variant="body1"
                gutterBottom
                minHeight={"100px"}
                maxHeight={"400px"}
                overflow={"auto"}
              >
                {taskDescription.split("\n").map((line, index) => (
                  <React.Fragment key={index}>
                    {line}
                    <br />
                  </React.Fragment>
                ))}
              </Typography>
            </Paper>
          )}
          {isEditingDescription && (
            <TextField
              label={"Opis zadatka"}
              value={taskDescription}
              aria-multiline={true}
              onChange={(e) => setTaskDescription(e.target.value)}
              onBlur={handleSaveDescription}
              onKeyDown={(e) => {
                if (e.key === "Escape") handleSaveDescription();
              }}
              autoFocus
              fullWidth
              multiline
              minRows={4}
              maxRows={17}
              sx={{ backgroundColor: theme.palette.background.paper }}
            />
          )}

          <Typography variant="h6" gutterBottom mt={"2rem"}>
            Povijest promjena
          </Typography>

          <Stack spacing={2}>
            {dateSortedHistoryLogs?.map((changeLogItem) => (
              <ChangeLogItem
                key={changeLogItem.modifiedAt}
                changeLog={changeLogItem}
              />
            ))}
            {(task?.historyLogs?.length ?? 0) > 5 && (
              <Button
                variant={"text"}
                onClick={() => setIsHistoryExpanded(!isHistoryExpanded)}
              >
                {isHistoryExpanded ? "Prikaži manje" : "Prikaži više"}
              </Button>
            )}
          </Stack>

          {isAdmin && (
            <>
              <Button
                variant={"contained"}
                color={"error"}
                onClick={() => setIsDeletePromptOpen(true)}
                sx={{ mt: "2rem" }}
              >
                Izbriši zadatak
              </Button>

              <DeletePrompt
                open={isDeletePromptOpen}
                handleClose={() => setIsDeletePromptOpen(false)}
                handleConfirm={() => {
                  TaskItemEndpoint.apiTaskItemIdDelete(task.id ?? "")
                    .then(() => {
                      showSnackbar("Task deleted successfully.", "success");
                      router.push(`/board/${task?.taskboardId}`);
                      handleResetData();
                    })
                    .catch(() => {
                      showSnackbar("Failed to delete task.", "error");
                    });
                }}
              />
            </>
          )}
        </Box>
      )}
    </>
  );
};

export default TaskContainer;
