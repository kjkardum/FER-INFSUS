"use client";
import { Box, Chip, IconButton, Paper, Stack, Typography } from "@mui/material";
import React, { useMemo, useState } from "react";
import useAuthentication from "@/hooks/useAuthentication";
import { UserDto } from "@/api/generated";
import Select from "react-select";
import useTenantGetUsers from "@/api/hooks/TenantEndpoint/useTenantGetUsers";
import convertUserDataToSelectOptions from "@/utils/convertUserDataToSelectOptions";
import { Close } from "@mui/icons-material";
import taskboardEndpoint from "@/api/endpoints/TaskboardEndpoint";
import { useQueryClient } from "@tanstack/react-query";
import { taskboardGetBoardDetailsKey } from "@/api/reactQueryKeys/TaskboardEndpointKeys";
import DeletePrompt from "@/components/DeletePrompt/DeletePrompt";

interface Props {
  users: Array<UserDto>;
  boardId: string;
}

const UsersList = ({ users, boardId }: Props) => {
  const [userToUnassign, setUserToUnassign] = useState<UserDto | null>(null);

  const { isAdmin } = useAuthentication();
  const queryClient = useQueryClient();

  const { data: usersData } = useTenantGetUsers();

  const usersNotInBoard = useMemo(() => {
    return usersData?.data?.filter(
      (user) => !users.some((u) => u.id === user.id),
    );
  }, [usersData, users]);

  const usersOptions = useMemo(() => {
    return convertUserDataToSelectOptions(usersNotInBoard ?? []);
  }, [usersNotInBoard]);

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

  return (
    <Box width={"100%"}>
      <Stack
        direction="row"
        spacing={2}
        justifyContent={"space-between"}
        mb={"1rem"}
      >
        <Typography variant="h6" gutterBottom>
          Users in board
        </Typography>
      </Stack>
      <Select
        options={usersOptions}
        value={null}
        onChange={(option) => {
          if (!option) return;
          handleAddUserToBoard(option.value);
        }}
        placeholder={"Add user to board..."}
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
      <Stack direction={"column"} spacing={2} mt={"1rem"}>
        {users.map((user) => (
          <Paper key={user.id} sx={{ p: "1rem" }}>
            <Stack direction={"row"} alignItems={"center"} spacing={2}>
              <Box pr={"1rem"}>
                <IconButton onClick={() => setUserToUnassign(user)}>
                  <Close />
                </IconButton>
              </Box>
              {`${user.firstName} ${user.lastName} (${user.email?.toLowerCase()})`}
              <Chip
                variant={"filled"}
                color={user.userType === "ADMIN" ? "error" : "default"}
                label={user.userType}
              />
            </Stack>
          </Paper>
        ))}
      </Stack>

      <DeletePrompt
        open={!!userToUnassign}
        handleClose={() => setUserToUnassign(null)}
        handleConfirm={handleRemoveUserFromBoard}
      />
    </Box>
  );
};

export default UsersList;
