"use client";
import useTaskItemGetItem from "@/api/hooks/TaskItemEndpoint/useTaskItemGetItem";
import React, { ChangeEvent, useEffect, useMemo, useState } from "react";
import {
  Backdrop,
  Box,
  Button,
  CircularProgress,
  Divider,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { useRouter } from "next/navigation";
import useSnackbar from "@/hooks/useSnackbar";
import TaskStateSelector from "@/components/TaskStateSelector/TaskStateSelector";
import Select from "react-select";
import useTenantGetUsers from "@/api/hooks/TenantEndpoint/useTenantGetUsers";
import convertUserDataToSelectOptions from "@/utils/convertUserDataToSelectOptions";
import DeletePrompt from "@/components/DeletePrompt/DeletePrompt";
import taskItemEndpoint from "@/api/endpoints/TaskItemEndpoint";
import { TaskItemState } from "@/api/generated";
import { LoadingButton } from "@mui/lab";

interface Props {
  taskId: string;
}

const EditTaskContainer = ({ taskId }: Props) => {
  const [taskName, setTaskName] = useState<string>("");
  const [taskDescription, setTaskDescription] = useState<string>("");
  const [taskState, setTaskState] = useState<number>(0);
  const [assignedTo, setAssignedTo] = useState<string>("");
  const [isDeletePromptOpen, setIsDeletePromptOpen] = useState<boolean>(false);
  const [editIsLoading, setEditIsLoading] = useState<boolean>(false);

  const {
    data: task,
    isLoading,
    isError,
    isSuccess,
  } = useTaskItemGetItem(taskId);
  const { data: usersData } = useTenantGetUsers();
  const router = useRouter();
  const { showSnackbar } = useSnackbar();

  const users = useMemo(
    () => convertUserDataToSelectOptions(usersData?.data ?? []),
    [usersData],
  );

  const handleStateChange = (state: ChangeEvent<HTMLInputElement>) => {
    setTaskState(Number(state.target.value));
  };

  const handleNameChange = (event: ChangeEvent<HTMLInputElement>) => {
    setTaskName(event.target.value);
  };

  const handleDescriptionChange = (event: ChangeEvent<HTMLInputElement>) => {
    setTaskDescription(event.target.value);
  };

  const showSuccessMessageAndRedirect = () => {
    showSnackbar("Task edited successfully.", "success");
    router.push(`/board/${task?.taskboardId}`);
  };

  const showErrorMessage = () => {
    showSnackbar("Oops! Something went wrong.", "error");
  };

  const handleSave = async () => {
    if (!taskName || !taskDescription) {
      showSnackbar("Please fill all fields.", "error");
      return;
    }

    const taskNameIsDifferent = taskName !== task?.name;
    const taskDescriptionIsDifferent = taskDescription !== task?.description;
    const taskStateIsDifferent = taskState !== task?.state;
    const assignedToIsDifferent = assignedTo !== task?.assignedUserId;

    if (
      !taskNameIsDifferent &&
      !taskDescriptionIsDifferent &&
      !taskStateIsDifferent &&
      !assignedToIsDifferent
    ) {
      showSnackbar("No changes detected.", "info");
      return;
    }

    try {
      setEditIsLoading(true);
      if (taskNameIsDifferent)
        await taskItemEndpoint.apiTaskItemIdRenamePost(taskId, {
          newName: taskName,
        });

      if (taskDescriptionIsDifferent)
        await taskItemEndpoint.apiTaskItemIdChangeDescriptionPost(taskId, {
          newDescription: taskDescription,
        });

      if (taskStateIsDifferent)
        await taskItemEndpoint.apiTaskItemIdChangeStatePost(taskId, {
          newState: taskState as TaskItemState,
        });

      if (assignedToIsDifferent)
        await taskItemEndpoint.apiTaskItemIdAssignPost(taskId, {
          assignedUserId: assignedTo ?? "",
        });

      showSuccessMessageAndRedirect();
    } catch (error) {
      showErrorMessage();
    } finally {
      setEditIsLoading(false);
    }
  };

  const handleDelete = () => {
    taskItemEndpoint.apiTaskItemIdDelete(taskId).then(() => {
      showSnackbar("Task deleted successfully.", "success");
      router.push(`/board/${task?.taskboardId}`);
    });
  };

  const handleOpenDeletePrompt = () => {
    setIsDeletePromptOpen(true);
  };

  useEffect(() => {
    if (isError) {
      showSnackbar("Task not found.", "error");
      router.push("/boards");
    }
  }, [isError, router, showSnackbar]);

  useEffect(() => {
    if (isSuccess && task) {
      setTaskName(task.name ?? "");
      setTaskDescription(task.description ?? "");
      setTaskState(task.state ?? 0);
      setAssignedTo(task.assignedUserId ?? "");
    }
  }, [isSuccess, task]);

  return (
    <>
      <Backdrop
        sx={{ color: "#fff", zIndex: (theme) => theme.zIndex.drawer + 1 }}
        open={isLoading}
      >
        <CircularProgress color="inherit" />
      </Backdrop>
      {isSuccess && task && (
        <Box>
          <Typography variant={"h5"}>Edit task - {taskName}</Typography>
          <Divider sx={{ my: 2 }} />
          <Stack spacing={2}>
            <TextField
              label="Task name"
              value={taskName}
              onChange={handleNameChange}
              fullWidth
            />
            <TextField
              label="Task description"
              value={taskDescription}
              onChange={handleDescriptionChange}
              fullWidth
              multiline
              rows={4}
            />
            <TaskStateSelector
              taskState={taskState}
              handleStateChange={handleStateChange}
            />
            <Select
              options={users}
              value={users.find((user) => user.value === assignedTo)}
              onChange={(selectedOption) =>
                setAssignedTo(selectedOption?.value ?? "")
              }
              placeholder={"Assign to user"}
              isSearchable={true}
              isClearable={true}
              menuPosition={"fixed"}
              styles={{
                input: (base) => ({
                  ...base,
                  height: "46px",
                }),
              }}
            />
            <Stack direction={"row"} spacing={3} pt={"1rem"}>
              <LoadingButton
                variant={"contained"}
                color={"primary"}
                onClick={handleSave}
                loading={editIsLoading}
              >
                Save
              </LoadingButton>
              <Button
                variant={"contained"}
                color={"error"}
                onClick={handleOpenDeletePrompt}
              >
                Delete
              </Button>
            </Stack>
          </Stack>

          <DeletePrompt
            open={isDeletePromptOpen}
            handleClose={() => setIsDeletePromptOpen(false)}
            handleConfirm={handleDelete}
          />
        </Box>
      )}
    </>
  );
};

export default EditTaskContainer;
