"use client";
import React, { useEffect, useMemo, useState } from "react";
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
import { useQueryClient } from "@tanstack/react-query";
import taskboardEndpoint from "@/api/endpoints/TaskboardEndpoint";
import { taskboardGetAllBoardsKey } from "@/api/reactQueryKeys/TaskboardEndpointKeys";
import { TaskboardSimpleDto } from "@/api/generated";
import useSnackbar from "@/hooks/useSnackbar";
import Select from "react-select";
import useTenantGetUsers from "@/api/hooks/TenantEndpoint/useTenantGetUsers";
import convertUserDataToSelectOptions from "@/utils/convertUserDataToSelectOptions";
import SnackbarMessages from "@/contexts/snackbar/SnackbarMessages";
import { ErrorResponseType } from "@/api/generated/@types/ErrorResponseType";
import { AxiosError } from "axios";

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
  const [boardUsers, setBoardUsers] = useState<string[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(false);

  const queryClient = useQueryClient();
  const { showSnackbar } = useSnackbar();
  const { data: usersData } = useTenantGetUsers();

  const users = useMemo(
    () => convertUserDataToSelectOptions(usersData?.data ?? []),
    [usersData],
  );

  useEffect(() => {
    if (board) {
      setBoardName(board.name ?? "");
      setBoardDescription(board.description ?? "");
      setBoardUsers(board.taskboardUsers?.map((user) => user.id ?? "") ?? []);
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
              .map((userId) => ({
                userId,
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

        const usersToAdd = boardUsers.filter(
          (userId) => !currentBoardUsers.includes(userId),
        );

        const usersToRemove = currentBoardUsers.filter(
          (userId) => userId && !boardUsers.includes(userId),
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
          <Select
            options={users}
            value={users.filter((user) => boardUsers.includes(user.value))}
            onChange={(selectedOptions) =>
              setBoardUsers(selectedOptions.map((option) => option.value))
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
