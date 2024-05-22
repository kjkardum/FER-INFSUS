import React from "react";
import AppLayout from "@/components/AppLayout/AppLayout";
import SimpleBoardsContainer from "@/modules/board/SimpleBoardsContainer/SimpleBoardsContainer";
import { Grid, Paper } from "@mui/material";
import SimpleTimesheetContainer from "@/modules/timekeep/SimpleTimekeepContainer/SimpleTimesheetContainer";
import SimpleTasksContainer from "@/modules/tasks/SimpleTasksContainer/SimpleTasksContainer";

export default function Home() {
  return (
    <AppLayout>
      <Grid container spacing={2} marginBottom={"1rem"}>
        <Grid item xs={12} md={6}>
          <Paper sx={{ padding: "1rem", height: "400px" }} elevation={2}>
            <SimpleBoardsContainer />
          </Paper>
        </Grid>
        <Grid item xs={12} md={6}>
          <Paper sx={{ padding: "1rem", height: "400px" }} elevation={2}>
            <SimpleTimesheetContainer />
          </Paper>
        </Grid>
      </Grid>
      <Paper sx={{ padding: "1rem", height: "450px" }} elevation={2}>
        <SimpleTasksContainer />
      </Paper>
    </AppLayout>
  );
}
