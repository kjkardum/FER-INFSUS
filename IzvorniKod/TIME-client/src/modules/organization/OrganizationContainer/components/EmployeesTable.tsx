"use client";
import React, { useMemo, useState } from "react";
import { DataGrid, GridActionsCellItem, GridColDef } from "@mui/x-data-grid";
import { UserDto } from "@/api/generated";
import getRoleFromUserType from "@/utils/getRoleFromUserType";
import { Delete, Edit } from "@mui/icons-material";
import DeletePrompt from "@/components/DeletePrompt/DeletePrompt";
import UserManagementModal from "@/modules/organization/OrganizationContainer/components/UserManagementModal";

const columns: GridColDef<UserDto>[] = [
  { field: "firstName", headerName: "First Name", width: 150 },
  { field: "lastName", headerName: "Last Name", width: 150 },
  { field: "email", headerName: "Email", width: 150 },
  {
    field: "userType",
    headerName: "Role",
    width: 150,
    valueGetter: (_, row) => getRoleFromUserType(row.userType ?? 0),
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

  const handleDelete = () => {
    if (deleteUser) {
      // delete user
    }
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
