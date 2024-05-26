"use client";
import {
  Box,
  Chip,
  debounce,
  IconButton,
  Paper,
  Stack,
  Typography,
} from "@mui/material";
import React, { useMemo, useState } from "react";
import useAuthentication from "@/hooks/useAuthentication";
import { UserDto } from "@/api/generated";
import convertUserDataToSelectOptions from "@/utils/convertUserDataToSelectOptions";
import { Close } from "@mui/icons-material";
import taskboardEndpoint from "@/api/endpoints/TaskboardEndpoint";
import { useQueryClient } from "@tanstack/react-query";
import { taskboardGetBoardDetailsKey } from "@/api/reactQueryKeys/TaskboardEndpointKeys";
import DeletePrompt from "@/components/DeletePrompt/DeletePrompt";
import tenantEndpoint from "@/api/endpoints/TenantEndpoint";
import AsyncSelect from "react-select/async";

interface Props {
  users: Array<UserDto>;
  boardId: string;
}

const UsersList = ({ users, boardId }: Props) => {
  const [userToUnassign, setUserToUnassign] = useState<UserDto | null>(null);

  const { isAdmin } = useAuthentication();
  const queryClient = useQueryClient();

  const handleAddUserToBoard = (userId: string) => {
    taskboardEndpoint
      .apiTaskboardAssignPost({
        userId,
        taskboardId: boardId,
      })
      .then(() => {
        queryClient.invalidateQueries({
          queryKey: taskboardGetBoardDetailsKey(boardId),
        });
      });
  };

  const handleRemoveUserFromBoard = () => {
    if (!userToUnassign) return;

    taskboardEndpoint
      .apiTaskboardUnassignPost({
        userId: userToUnassign.id,
        taskboardId: boardId,
      })
      .then(() => {
        setUserToUnassign(null);
        queryClient.invalidateQueries({
          queryKey: taskboardGetBoardDetailsKey(boardId),
        });
      });
  };

  const debouncedLoadOptions = useMemo(() => {
    return debounce(
      (
        inputValue: string,
        callback: (options: { value: string; label: string }[]) => void,
      ) => {
        tenantEndpoint
          .apiTenantManagementGetUsersGet(1, 100, inputValue)
          .then((response) => {
            callback(
              convertUserDataToSelectOptions(
                response.data.data?.filter(
                  (user) => !users.some((u) => u.id === user.id),
                ) ?? [],
              ),
            );
          });
      },
      300,
    );
  }, [users]);

  return (
    <Box width={"100%"}>
      <Stack
        direction="row"
        spacing={2}
        justifyContent={"space-between"}
        mb={"1rem"}
      >
        <Typography variant="h6" gutterBottom>
          Članovi radne ploče
        </Typography>
      </Stack>
      {isAdmin && (
        <AsyncSelect
          cacheOptions
          defaultOptions
          loadOptions={debouncedLoadOptions}
          value={null}
          onChange={(option) => {
            if (!option) return;
            handleAddUserToBoard(option.value);
          }}
          placeholder={"Dodaj korisnika u radnu ploču..."}
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
      )}
      <Stack direction={"column"} spacing={2} mt={"1rem"}>
        {users.map((user) => (
          <Paper key={user.id} sx={{ p: "1rem" }}>
            <Stack direction={"row"} alignItems={"center"}>
              {isAdmin && (
                <Box pr={"1rem"}>
                  <IconButton onClick={() => setUserToUnassign(user)}>
                    <Close />
                  </IconButton>
                </Box>
              )}

              {`${user.firstName} ${user.lastName} (${user.email?.toLowerCase()})`}
              <Box>
                <Chip
                  variant={"filled"}
                  color={user.userType === "ADMIN" ? "error" : "default"}
                  label={user.userType}
                  sx={{ ml: "1rem" }}
                />
              </Box>
            </Stack>
          </Paper>
        ))}
      </Stack>

      {isAdmin && (
        <DeletePrompt
          open={!!userToUnassign}
          handleClose={() => setUserToUnassign(null)}
          handleConfirm={handleRemoveUserFromBoard}
        />
      )}
    </Box>
  );
};

export default UsersList;
