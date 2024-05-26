"use client";
import React from "react";
import { Box } from "@mui/material";
import EmployeesContainer from "@/modules/organization/OrganizationContainer/components/EmployeesContainer";

const OrganizationContainer = () => {
  return (
    <Box
      sx={{
        height: 815,
        padding: "1rem",
        overflow: "hidden",
        display: "flex",
        flexDirection: "column",
      }}
    >
      <EmployeesContainer />
    </Box>
  );
};

export default OrganizationContainer;
