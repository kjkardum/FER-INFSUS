"use client";
import React from "react";
import { Box, Paper } from "@mui/material";
import EmployeesContainer from "@/modules/organization/OrganizationContainer/components/EmployeesContainer";

const OrganizationContainer = () => {
  return (
    <Box>
      <Paper
        elevation={1}
        sx={{
          height: 815,
          padding: "1rem",
          overflow: "hidden",
          display: "flex",
          flexDirection: "column",
        }}
      >
        <EmployeesContainer />
      </Paper>
    </Box>
  );
};

export default OrganizationContainer;
