"use client";
import { Box, IconButton, Typography } from "@mui/material";
import EmployeesTable from "@/modules/organization/OrganizationContainer/components/EmployeesTable";
import React, { useState } from "react";
import useTenantGetUsers from "@/api/hooks/TenantEndpoint/useTenantGetUsers";
import { AddCircle } from "@mui/icons-material";
import UserManagementModal from "@/modules/organization/OrganizationContainer/components/UserManagementModal";
import WholeSectionLoading from "@/components/WholeSectionLoading/WholeSectionLoading";

const EmployeesContainer = () => {
  const [createUserModalOpen, setCreateUserModalOpen] = useState(false);

  const { data, isLoading, isSuccess } = useTenantGetUsers();

  const handleCreateUser = () => {
    setCreateUserModalOpen(true);
  };

  const handleClose = () => {
    setCreateUserModalOpen(false);
  };

  return (
    <Box height={"100%"}>
      <Typography variant={"h5"} marginBottom={"1rem"}>
        Employees
        <IconButton sx={{ ml: "0.5rem" }} onClick={handleCreateUser}>
          <AddCircle />
        </IconButton>
      </Typography>
      {isLoading && <WholeSectionLoading />}
      {isSuccess && <EmployeesTable rows={data?.data || []} />}
      <UserManagementModal
        open={createUserModalOpen}
        handleClose={handleClose}
      />
    </Box>
  );
};

export default EmployeesContainer;
