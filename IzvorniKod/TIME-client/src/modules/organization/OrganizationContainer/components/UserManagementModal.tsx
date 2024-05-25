"use client";
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  FormControlLabel,
  FormLabel,
  Radio,
  RadioGroup,
  Stack,
  TextField,
} from "@mui/material";
import React, {
  ChangeEvent,
  Dispatch,
  SetStateAction,
  useEffect,
  useState,
} from "react";
import { UserDto } from "@/api/generated";
import { DatePicker, LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import dayjs from "dayjs";
import { LoadingButton } from "@mui/lab";
import tenantEndpoint from "@/api/endpoints/TenantEndpoint";
import { useQueryClient } from "@tanstack/react-query";
import { tenantGetUsersKey } from "@/api/reactQueryKeys/TenantEndpointKeys";
import useSnackbar from "@/hooks/useSnackbar";

interface Props {
  user?: UserDto;
  open?: boolean;
  handleClose: () => void;
}

const UserManagementModal = ({ open, user, handleClose }: Props) => {
  const queryClient = useQueryClient();
  const { showSnackbar } = useSnackbar();

  const [firstName, setFirstName] = useState<string>(user?.firstName ?? "");
  const [lastName, setLastName] = useState<string>(user?.lastName ?? "");
  const [email, setEmail] = useState<string>(user?.email ?? "");
  const [password, setPassword] = useState<string>("");
  const [dateOfBirth, setDateOfBirth] = useState<dayjs.Dayjs>(
    user?.dateOfBirth ? dayjs(user.dateOfBirth) : dayjs(),
  );
  const [userRole, setUserRole] = useState<string | undefined>(
    user ? (user.userType ?? "USER").toLowerCase() : "user",
  );
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    if (user) {
      setFirstName(user.firstName ?? "");
      setLastName(user.lastName ?? "");
      setEmail(user.email?.toLowerCase() ?? "");
      setDateOfBirth(user.dateOfBirth ? dayjs(user.dateOfBirth) : dayjs());
      setUserRole((user.userType ?? "USER").toLowerCase());
    }
  }, [user]);

  const handleRoleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setUserRole((event.target as HTMLInputElement).value);
  };

  const handleStringChange = (
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
    setter: Dispatch<SetStateAction<string>>,
  ) => {
    setter(event.target.value);
  };

  const handleDateChange = (date: dayjs.Dayjs | null) => {
    setDateOfBirth(date ?? dayjs());
  };

  const handleEdit = () => {
    if (!user || !user.id) return;

    if (!firstName || !lastName || !email || !userRole || !dateOfBirth) {
      showSnackbar("Please fill all fields", "error");
      return;
    }

    setIsLoading(true);
    tenantEndpoint
      .apiTenantManagementUpdateUserIdPut(user?.id, {
        firstName: firstName ?? "",
        lastName: lastName ?? "",
        dateOfBirth: dateOfBirth.toISOString(),
        newPassword: password || null,
      })
      .then(() => {
        showSnackbar("User updated successfully", "success");
        queryClient
          .invalidateQueries({ queryKey: tenantGetUsersKey })
          .then(() => {
            handleClose();
          });
      })
      .catch(() => {
        showSnackbar("Failed to update user", "error");
      })
      .finally(() => setIsLoading(false));
  };

  const handleSave = () => {
    if (!user) {
      if (
        !firstName ||
        !lastName ||
        !email ||
        !password ||
        !userRole ||
        !dateOfBirth
      ) {
        showSnackbar("Please fill all fields", "error");
        return;
      }

      setIsLoading(true);
      tenantEndpoint
        .apiTenantManagementCreateUserPost({
          firstName: firstName ?? "",
          lastName: lastName ?? "",
          email: email ?? "",
          password: password ?? "",
          userType: userRole === "admin" ? "ADMIN" : "USER",
          dateOfBirth: dateOfBirth.toISOString(),
        })
        .then(() => {
          showSnackbar("User created successfully", "success");
          queryClient
            .invalidateQueries({ queryKey: tenantGetUsersKey })
            .then(() => {
              handleClose();
            });
        })
        .catch(() => {
          showSnackbar("Failed to create user", "error");
        })
        .finally(() => setIsLoading(false));
    }

    handleEdit();
  };

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <Dialog
        open={!!open}
        onClose={handleClose}
        aria-labelledby="dialog-userManagment-prompt-title"
        aria-describedby="dialog-userManagment-prompt-description"
        fullWidth={true}
      >
        <DialogTitle id="dialog-userManagment-prompt-title">
          {user ? "Edit" : "Create new"} user
        </DialogTitle>
        <DialogContent>
          <Stack
            direction={"column"}
            spacing={3}
            py={"1rem"}
            component={"form"}
          >
            <TextField
              label={"First Name"}
              fullWidth
              type={"text"}
              value={firstName}
              onChange={(e) => handleStringChange(e, setFirstName)}
            />
            <TextField
              label={"Last Name"}
              fullWidth
              type={"text"}
              value={lastName}
              onChange={(e) => handleStringChange(e, setLastName)}
            />
            {!user && (
              <TextField
                label={"Email"}
                fullWidth
                type={"email"}
                value={email}
                onChange={(e) => handleStringChange(e, setEmail)}
              />
            )}
            <TextField
              label={"Password"}
              fullWidth
              type={"password"}
              value={password}
              onChange={(e) => handleStringChange(e, setPassword)}
            />
            <DatePicker
              label={"Date of birth"}
              value={dateOfBirth}
              onChange={handleDateChange}
            />
            <FormControl>
              <FormLabel id="demo-controlled-radio-buttons-group">
                Role
              </FormLabel>
              <RadioGroup
                aria-labelledby="demo-controlled-radio-buttons-group"
                name="controlled-radio-buttons-group"
                value={userRole}
                onChange={handleRoleChange}
                row
              >
                <FormControlLabel
                  value="admin"
                  control={<Radio />}
                  label="Admin"
                />
                <FormControlLabel
                  value="user"
                  control={<Radio />}
                  label="User"
                />
              </RadioGroup>
            </FormControl>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button
            variant={"outlined"}
            color={"secondary"}
            onClick={handleClose}
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
    </LocalizationProvider>
  );
};

export default UserManagementModal;
