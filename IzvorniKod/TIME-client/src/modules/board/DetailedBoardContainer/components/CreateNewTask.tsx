import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Stack,
  TextField,
} from "@mui/material";
import { LoadingButton } from "@mui/lab";
import React, { ChangeEvent, useEffect, useMemo, useState } from "react";
import taskItemEndpoint from "@/api/endpoints/TaskItemEndpoint";
import { useQueryClient } from "@tanstack/react-query";
import { taskboardGetBoardDetailsKey } from "@/api/reactQueryKeys/TaskboardEndpointKeys";
import useTenantGetUsers from "@/api/hooks/TenantEndpoint/useTenantGetUsers";
import Select from "react-select";
import { TaskItemState } from "@/api/generated";
import useSnackbar from "@/hooks/useSnackbar";
import TaskStateSelector from "@/components/TaskStateSelector/TaskStateSelector";
import convertUserDataToSelectOptions from "@/utils/convertUserDataToSelectOptions";

interface Props {
  open?: boolean;
  boardId: string;
  onClose: () => void;
}

const CreateNewTaskModal = ({ open, boardId, onClose }: Props) => {
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [taskName, setTaskName] = useState<string>("");
  const [taskDescription, setTaskDescription] = useState<string>("");
  const [taskState, setTaskState] = useState<number>(0);
  const [assignedTo, setAssignedTo] = useState<string>("");

  const queryClient = useQueryClient();
  const { data: usersData } = useTenantGetUsers();
  const { showSnackbar } = useSnackbar();

  const users = useMemo(
    () => convertUserDataToSelectOptions(usersData?.data ?? []),
    [usersData],
  );

  useEffect(() => {
    setTaskName("");
    setTaskDescription("");
    setTaskState(0);
    setAssignedTo("");
  }, [open]);

  const handleSave = () => {
    if (!taskName || !taskDescription) {
      showSnackbar("Please fill all fields.", "error");
      return;
    }

    setIsLoading(true);
    taskItemEndpoint
      .apiTaskItemPost({
        name: taskName,
        description: taskDescription,
        taskboardId: boardId,
      })
      .then((response) => {
        showSnackbar("Task saved successfully.", "success");
        queryClient
          .invalidateQueries({
            queryKey: taskboardGetBoardDetailsKey(boardId),
          })
          .then(() => onClose());

        if (taskState !== 0) {
          taskItemEndpoint
            .apiTaskItemIdChangeStatePost(response.data.id ?? "", {
              newState: taskState as TaskItemState,
            })
            .then(() => {
              queryClient
                .invalidateQueries({
                  queryKey: taskboardGetBoardDetailsKey(boardId),
                })
                .then(() => onClose());
            })
            .catch(() => {
              showSnackbar("Failed to change task state.", "error");
            });
        }

        if (assignedTo) {
          taskItemEndpoint
            .apiTaskItemIdAssignPost(response.data.id ?? "", {
              assignedUserId: assignedTo,
            })
            .then(() => {
              queryClient
                .invalidateQueries({
                  queryKey: taskboardGetBoardDetailsKey(boardId),
                })
                .then(() => onClose());
            })
            .catch(() => {
              showSnackbar("Failed to assign task.", "error");
            });
        }
      })
      .catch(() => {})
      .finally(() => setIsLoading(false));
  };

  const handleStateChange = (event: ChangeEvent<HTMLInputElement>) => {
    setTaskState(Number(event.target.value));
  };

  return (
    <Dialog
      onClose={onClose}
      open={!!open}
      aria-labelledby="dialog-createNewTask-prompt-title"
      aria-describedby="dialog-createNewTask-prompt-description"
      fullWidth={true}
    >
      <DialogTitle>Create new task for board</DialogTitle>
      <DialogContent>
        <Stack spacing={2} py={"0.5rem"}>
          <TextField
            autoFocus
            name={"taskName"}
            value={taskName}
            onChange={(e) => setTaskName(e.target.value)}
            label="Task name"
            type="text"
            fullWidth
          />
          <TextField
            value={taskDescription}
            onChange={(e) => setTaskDescription(e.target.value)}
            name={"taskDescription"}
            label="Task description"
            type="text"
            rows={4}
            fullWidth
            multiline
          />
          <TaskStateSelector
            taskState={taskState}
            handleStateChange={handleStateChange}
          />
          <Select
            options={users}
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
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button
          variant={"outlined"}
          color={"secondary"}
          onClick={onClose}
          autoFocus
        >
          Cancel
        </Button>
        <LoadingButton
          variant={"contained"}
          color={"primary"}
          onClick={handleSave}
          loading={isLoading}
        >
          Save
        </LoadingButton>
      </DialogActions>
    </Dialog>
  );
};

export default CreateNewTaskModal;
