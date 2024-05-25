"use client";
import React, { useMemo, useState } from "react";
import { DataGrid, GridActionsCellItem, GridColDef } from "@mui/x-data-grid";
import { UserDto } from "@/api/generated";
import { Delete, Edit } from "@mui/icons-material";
import DeletePrompt from "@/components/DeletePrompt/DeletePrompt";
import UserManagementModal from "@/modules/organization/OrganizationContainer/components/UserManagementModal";
import tenantEndpoint from "@/api/endpoints/TenantEndpoint";
import useSnackbar from "@/hooks/useSnackbar";
import { useQueryClient } from "@tanstack/react-query";
import { tenantGetUsersKey } from "@/api/reactQueryKeys/TenantEndpointKeys";

const columns: GridColDef<UserDto>[] = [
  { field: "firstName", headerName: "First Name", width: 150 },
  { field: "lastName", headerName: "Last Name", width: 150 },
  {
    field: "email",
    headerName: "Email",
    width: 150,
    valueGetter: (_, row) => row.email?.toLowerCase(),
  },
  {
    field: "userType",
    headerName: "Role",
    width: 150,
  },
  {
    field: "dateOfBirth",
    headerName: "Date Of Birth",
    width: 150,
    valueGetter: (_, row) =>
      row.dateOfBirth ? new Date(row.dateOfBirth).toLocaleDateString() : "",
  },
];

interface Props {
  rows: UserDto[];
}

const EmployeesTable = ({ rows }: Props) => {
  const [selectedUser, setSelectedUser] = useState<UserDto | undefined>(
    undefined,
  );
  const [deleteUser, setDeleteUser] = useState<UserDto | undefined>(undefined);

  const { showSnackbar } = useSnackbar();
  const queryClient = useQueryClient();

  const handleDelete = () => {
    if (deleteUser && deleteUser.id)
      tenantEndpoint
        .apiTenantManagementDeleteUserIdDelete(deleteUser.id)
        .then(() => {
          showSnackbar("User deleted.", "success");
          setDeleteUser(undefined);
          queryClient.invalidateQueries({ queryKey: tenantGetUsersKey });
        });
  };

  const handleCloseDelete = () => {
    setDeleteUser(undefined);
  };

  const handleCloseEdit = () => {
    setSelectedUser(undefined);
  };

  const columnsWithActions: GridColDef<UserDto>[] = useMemo(
    () => [
      {
        field: "actions",
        type: "actions",
        getActions: ({ id, row }) => {
          return [
            <GridActionsCellItem
              key={`edit-${id}`}
              icon={<Edit />}
              label="Edit"
              onClick={() => setSelectedUser(row)}
            />,
            <GridActionsCellItem
              key={`delete-${id}`}
              icon={<Delete />}
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
    <>
      <DataGrid columns={columnsWithActions} rows={rows} />
      <DeletePrompt
        open={!!deleteUser}
        handleClose={handleCloseDelete}
        handleConfirm={handleDelete}
      />
      <UserManagementModal
        open={!!selectedUser}
        user={selectedUser}
        handleClose={handleCloseEdit}
      />
    </>
  );
};

export default EmployeesTable;
