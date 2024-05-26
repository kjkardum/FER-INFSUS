"use client";
import React, { useMemo, useState } from "react";
import {
  DataGrid,
  GridActionsCellItem,
  GridColDef,
  GridPaginationModel,
  GridSortModel,
} from "@mui/x-data-grid";
import { UserDto } from "@/api/generated";
import { Delete, Edit } from "@mui/icons-material";
import DeletePrompt from "@/components/DeletePrompt/DeletePrompt";
import UserManagementModal from "@/modules/organization/OrganizationContainer/components/UserManagementModal";
import tenantEndpoint from "@/api/endpoints/TenantEndpoint";
import useSnackbar from "@/hooks/useSnackbar";
import { useQueryClient } from "@tanstack/react-query";
import { tenantGetUsersKey } from "@/api/reactQueryKeys/TenantEndpointKeys";
import SnackbarMessages from "@/contexts/snackbar/SnackbarMessages";
import { Box, LabelDisplayedRowsArgs, useTheme } from "@mui/material";
import dayjs from "dayjs";
import { AxiosError } from "axios";
import { ErrorResponseType } from "@/api/generated/@types/ErrorResponseType";

const columns: GridColDef<UserDto>[] = [
  { field: "firstName", headerName: "Ime", width: 150, filterable: false },
  { field: "lastName", headerName: "Prezime", width: 150, filterable: false },
  {
    field: "email",
    headerName: "E-mail",
    width: 200,
    filterable: false,
    valueGetter: (_, row) => row.email?.toLowerCase(),
  },
  {
    field: "userType",
    headerName: "Uloga korisnika",
    width: 150,
    sortable: false,
    filterable: false,
    valueGetter: (_, row) =>
      row.userType === "ADMIN" ? "Administrator" : "Korisnik",
  },
  {
    field: "dateOfBirth",
    headerName: "Datum rođenja",
    width: 150,
    sortable: false,
    filterable: false,
    valueGetter: (_, row) =>
      row.dateOfBirth ? dayjs(row.dateOfBirth).format("DD-MM-YYYY") : "",
  },
];

const labelDisplayedRows = ({
  from,
  to,
  count,
  estimated,
}: LabelDisplayedRowsArgs & { estimated?: number | undefined }) => {
  if (!estimated) {
    return `${from}–${to} od ${count !== -1 ? count : `više nego ${to}`}`;
  }
  return `${from}–${to} od ${count !== -1 ? count : `više nego ${estimated > to ? estimated : to}`}`;
};

interface Props {
  rows: UserDto[];
  paginationModel: GridPaginationModel;
  onPaginationModelChange: (paginationModel: GridPaginationModel) => void;
  sortModel: GridSortModel;
  onSortModelChange: (sortModel: GridSortModel) => void;
  totalUsers: number;
  isLoading?: boolean;
  handleUpdate?: () => void;
}

const EmployeesTable = ({
  rows,
  paginationModel,
  onPaginationModelChange,
  sortModel,
  onSortModelChange,
  totalUsers,
  isLoading,
  handleUpdate,
}: Props) => {
  const [selectedUser, setSelectedUser] = useState<UserDto | undefined>(
    undefined,
  );
  const [deleteUser, setDeleteUser] = useState<UserDto | undefined>(undefined);

  const { showSnackbar } = useSnackbar();
  const queryClient = useQueryClient();
  const theme = useTheme();

  const handleDelete = () => {
    if (deleteUser && deleteUser.id)
      tenantEndpoint
        .apiTenantManagementDeleteUserIdDelete(deleteUser.id)
        .then(() => {
          showSnackbar(
            SnackbarMessages.organization.employees.deleteSuccess,
            "success",
          );
          setDeleteUser(undefined);
          if (handleUpdate) {
            handleUpdate();
            return;
          }
          queryClient.invalidateQueries({ queryKey: tenantGetUsersKey });
        })
        .catch((error: AxiosError<ErrorResponseType>) => {
          showSnackbar(
            error.response?.data.detail ||
              SnackbarMessages.organization.employees.deleteError,
            "error",
          );
        });
  };

  const handleCloseDelete = () => {
    setDeleteUser(undefined);
  };

  const handleCloseEdit = () => {
    setSelectedUser(undefined);
  };

  const handleUserUpdate = () => {
    handleCloseEdit();
    if (handleUpdate) {
      handleUpdate();
    } else {
      queryClient.invalidateQueries({ queryKey: tenantGetUsersKey });
    }
  };

  const columnsWithActions: GridColDef<UserDto>[] = useMemo(
    () => [
      {
        field: "Mogućnosti",
        type: "actions",
        getActions: ({ id, row }) => {
          return [
            <GridActionsCellItem
              key={`edit-${id}`}
              icon={<Edit color={"primary"} />}
              label="Edit"
              onClick={() => setSelectedUser(row)}
            />,
            <GridActionsCellItem
              key={`delete-${id}`}
              icon={<Delete color={"error"} />}
              label="Delete"
              onClick={() => setDeleteUser(row)}
            />,
          ];
        },
        sortable: false,
        filterable: false,
        editable: false,
        resizable: false,
      },
      ...columns,
    ],
    [],
  );

  return (
    <Box
      sx={{ backgroundColor: theme.palette.background.paper }}
      height={"100%"}
    >
      <DataGrid
        columns={columnsWithActions}
        rows={rows}
        localeText={{
          MuiTablePagination: {
            labelRowsPerPage: "Redova po stranici",
            labelDisplayedRows,
          },
          columnMenuSortAsc: "Sortiraj uzlazno",
          columnMenuSortDesc: "Sortiraj silazno",
          columnMenuHideColumn: "Sakrij stupac",
          columnMenuShowColumns: "Prikaži stupce",
          columnMenuManageColumns: "Upravljaj stupcima",
          columnsManagementShowHideAllText: "Prikaži sve",
          columnsManagementReset: "Resetiraj",
          columnsManagementSearchTitle: "Pretraži stupce",
          columnsManagementNoColumns: "Nema stupaca",
          noRowsLabel: "Nema podataka",
        }}
        pageSizeOptions={[5, 10, 20, 50, 100]}
        paginationMode={"server"}
        rowCount={totalUsers}
        paginationModel={paginationModel}
        onPaginationModelChange={onPaginationModelChange}
        loading={!!isLoading}
        sortModel={sortModel}
        onSortModelChange={onSortModelChange}
      />
      <DeletePrompt
        open={!!deleteUser}
        handleClose={handleCloseDelete}
        handleConfirm={handleDelete}
      />
      <UserManagementModal
        open={!!selectedUser}
        user={selectedUser}
        handleClose={handleCloseEdit}
        handleUpdate={handleUserUpdate}
      />
    </Box>
  );
};

export default EmployeesTable;
