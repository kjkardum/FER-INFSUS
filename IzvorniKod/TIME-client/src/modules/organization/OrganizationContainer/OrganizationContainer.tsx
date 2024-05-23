import { Box, Grid, Paper, Stack, Typography } from "@mui/material";
import React from "react";

const OrganizationContainer = () => {
  return (
    <Box>
      <Grid container spacing={2}>
        <Grid item xs={12} md={6}>
          <Stack direction={"column"} spacing={2}>
            <Paper elevation={1} sx={{ height: 400, padding: "1rem" }}>
              <Typography variant="h5" gutterBottom>
                Organization Information
              </Typography>
            </Paper>
            <Paper elevation={1} sx={{ height: 400, padding: "1rem" }}>
              <Typography variant="h5" gutterBottom>
                Task logs
              </Typography>
            </Paper>
          </Stack>
        </Grid>
        <Grid item xs={12} md={6}>
          <Paper elevation={1} sx={{ height: 815, padding: "1rem" }}>
            <Typography variant="h5" gutterBottom>
              Employees
            </Typography>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
};

export default OrganizationContainer;
