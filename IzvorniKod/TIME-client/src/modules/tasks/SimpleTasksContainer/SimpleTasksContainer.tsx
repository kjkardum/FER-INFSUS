"use client";
import React from "react";
import { Box, Typography } from "@mui/material";
import { DataGrid, GridColDef, GridRowsProp } from "@mui/x-data-grid";

const rows: GridRowsProp = [
  { id: 1, col1: "Hello", col2: "World" },
  { id: 2, col1: "DataGridPro", col2: "is Awesome" },
  { id: 3, col1: "MUI", col2: "is Amazing" },
];

const columns: GridColDef[] = [
  { field: "col1", headerName: "Column 1", width: 150 },
  { field: "col2", headerName: "Column 2", width: 150 },
];

const SimpleTasksContainer = () => {
  return (
    <Box>
      <Typography variant={"h5"} marginBottom={"1rem"} align={"center"}>
        My Tasks
      </Typography>
      <Box height={"350px"} width={"100%"}>
        <DataGrid rows={rows} columns={columns} />
      </Box>
    </Box>
  );
};

export default SimpleTasksContainer;
