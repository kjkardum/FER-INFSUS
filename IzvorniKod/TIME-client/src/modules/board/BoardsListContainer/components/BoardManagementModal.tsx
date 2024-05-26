"use client";
import React, { useEffect, useMemo, useState } from "react";
import {
  Button,
  debounce,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Stack,
  TextField,
} from "@mui/material";
import { LoadingButton } from "@mui/lab";
import { useQueryClient } from "@tanstack/react-query";
import taskboardEndpoint from "@/api/endpoints/TaskboardEndpoint";
import { taskboardGetAllBoardsKey } from "@/api/reactQueryKeys/TaskboardEndpointKeys";
import { TaskboardSimpleDto } from "@/api/generated";
import useSnackbar from "@/hooks/useSnackbar";
import convertUserDataToSelectOptions from "@/utils/convertUserDataToSelectOptions";
import SnackbarMessages from "@/contexts/snackbar/SnackbarMessages";
import { ErrorResponseType } from "@/api/generated/@types/ErrorResponseType";
import { AxiosError } from "axios";
import tenantEndpoint from "@/api/endpoints/TenantEndpoint";
import AsyncSelect from "react-select/async";

interface Props {
  open?: boolean;
  board?: TaskboardSimpleDto;
  handleClose: () => void;
}

const BoardManagementModal = ({ open, board, handleClose }: Props) => {
  const [boardName, setBoardName] = useState<string>(board?.name ?? "");
  const [boardDescription, setBoardDescription] = useState<string>(
    board?.description ?? "",
  );
  const [boardUsers, setBoardUsers] = useState<
    { value: string; label: string }[]
  >([]);
  const [isLoading, setIsLoading] = useState<boolean>(false);

  const queryClient = useQueryClient();
  const { showSnackbar } = useSnackbar();

  useEffect(() => {
    if (board) {
      setBoardName(board.name ?? "");
      setBoardDescription(board.description ?? "");
      setBoardUsers(convertUserDataToSelectOptions(board.taskboardUsers ?? []));
    } else {
      setBoardName("");
      setBoardDescription("");
      setBoardUsers([]);
    }
  }, [board, open]);

  const handleCreateNewBoard = () => {
    if (!boardName || !boardDescription) {
      showSnackbar(SnackbarMessages.common.fillAllFields, "error");
      return;
    }

    setIsLoading(true);
    taskboardEndpoint
      .apiTaskboardPost({
        name: boardName,
        description: boardDescription,
      })
      .then((response) => {
        showSnackbar(SnackbarMessages.boards.createSuccess, "success");
        try {
          if (boardUsers.length) {
            boardUsers
              .map((user) => ({
                userId: user.value,
                taskboardId: response.data.id,
              }))
              .forEach(async (user, index) => {
                await taskboardEndpoint.apiTaskboardAssignPost(user);
                if (index === boardUsers.length - 1)
                  queryClient
                    .invalidateQueries({
                      queryKey: taskboardGetAllBoardsKey,
                    })
                    .then(() => {
                      handleClose();
                    });
              });

            queryClient
              .invalidateQueries({ queryKey: taskboardGetAllBoardsKey })
              .then(() => {
                handleClose();
              });
            return;
          }
        } catch (e) {
          showSnackbar(SnackbarMessages.boards.createError, "error");
        }

        queryClient
          .invalidateQueries({ queryKey: taskboardGetAllBoardsKey })
          .then(() => {
            handleClose();
          });
      })
      .catch((error: AxiosError<ErrorResponseType>) => {
        showSnackbar(
          error.response?.data.detail || SnackbarMessages.boards.createError,
          "error",
        );
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const handleEditBoard = () => {
    if (!boardName || !boardDescription || !board || !board.id) {
      showSnackbar(SnackbarMessages.common.fillAllFields, "error");
      return;
    }

    setIsLoading(true);
    taskboardEndpoint
      .apiTaskboardIdPut(board?.id, {
        name: boardName,
        description: boardDescription,
      })
      .then(() => {
        showSnackbar(SnackbarMessages.boards.updateSuccess, "success");

        const currentBoardUsers =
          board.taskboardUsers?.map((user) => user.id) ?? [];
        const newBoardUsers = boardUsers.map((user) => user.value);

        const usersToAdd = newBoardUsers.filter(
          (userId) => !currentBoardUsers.includes(userId),
        );

        const usersToRemove = currentBoardUsers.filter(
          (userId) => userId && !newBoardUsers.includes(userId),
        );

        try {
          if (usersToAdd.length)
            usersToAdd
              .map((userId) => ({
                userId,
                taskboardId: board.id,
              }))
              .forEach(async (user, index) => {
                await taskboardEndpoint.apiTaskboardAssignPost(user);
                if (index === usersToAdd.length - 1)
                  queryClient
                    .invalidateQueries({
                      queryKey: taskboardGetAllBoardsKey,
                    })
                    .then(() => {
                      handleClose();
                    });
              });

          if (usersToRemove.length)
            usersToRemove.forEach(async (userId, index) => {
              await taskboardEndpoint.apiTaskboardUnassignPost({
                userId,
                taskboardId: board.id,
              });
              if (index === usersToRemove.length - 1)
                queryClient
                  .invalidateQueries({
                    queryKey: taskboardGetAllBoardsKey,
                  })
                  .then(() => {
                    handleClose();
                  });
            });
        } catch (e) {
          showSnackbar(SnackbarMessages.boards.userAssignError, "error");
        }

        queryClient
          .invalidateQueries({ queryKey: taskboardGetAllBoardsKey })
          .then(() => {
            handleClose();
          });
      })
      .catch((error: AxiosError<ErrorResponseType>) => {
        showSnackbar(
          error.response?.data.detail || SnackbarMessages.boards.updateError,
          "error",
        );
      })
      .finally(() => {
        setIsLoading(false);
      });
  };

  const handleSave = () => {
    if (!board) {
      handleCreateNewBoard();
      return;
    }

    handleEditBoard();
  };

  const debouncedLoadOptions = useMemo(() => {
    return debounce(
      (inputValue: string, callback: (options: typeof boardUsers) => void) => {
        tenantEndpoint
          .apiTenantManagementGetUsersGet(1, 100, inputValue)
          .then((response) => {
            callback(convertUserDataToSelectOptions(response.data.data ?? []));
          });
      },
      300,
    );
  }, []);

  return (
    <Dialog
      open={!!open}
      onClose={handleClose}
      aria-labelledby="dialog-userManagment-prompt-title"
      aria-describedby="dialog-userManagment-prompt-description"
      fullWidth={true}
    >
      <DialogTitle id="dialog-userManagment-prompt-title">
        {board ? "Uredi radnu ploču" : "Kreiraj novu radnu ploču"}
      </DialogTitle>
      <DialogContent>
        <Stack direction={"column"} spacing={3} py={"1rem"} component={"form"}>
          <TextField
            label={"Ime radne ploče"}
            fullWidth
            type={"text"}
            value={boardName}
            onChange={(e) => setBoardName(e.target.value)}
          />
          <TextField
            label={"Opis radne ploče"}
            fullWidth
            type={"text"}
            value={boardDescription}
            onChange={(e) => setBoardDescription(e.target.value)}
          />
          <AsyncSelect
            cacheOptions
            defaultOptions
            loadOptions={debouncedLoadOptions}
            value={boardUsers}
            onChange={(selectedOptions) =>
              setBoardUsers(
                selectedOptions as { value: string; label: string }[],
              )
            }
            placeholder={"Dodaj korisnike u radnu ploču..."}
            isSearchable={true}
            isClearable={true}
            isMulti
            menuPosition={"fixed"}
            styles={{
              control: (base) => ({
                ...base,
                minHeight: "56px",
              }),
            }}
          />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button
          variant={"outlined"}
          color={"secondary"}
          onClick={handleClose}
          autoFocus
        >
          Poništi
        </Button>
        <LoadingButton
          variant={"contained"}
          color={"primary"}
          onClick={handleSave}
          loading={isLoading}
        >
          Spremi
        </LoadingButton>
      </DialogActions>
    </Dialog>
  );
};

export default BoardManagementModal;
