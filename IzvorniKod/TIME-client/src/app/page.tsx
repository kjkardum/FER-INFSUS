import React from "react";
import AppLayout from "@/components/AppLayout/AppLayout";
import SimpleBoardsContainer from "@/modules/board/SimpleBoardsContainer/SimpleBoardsContainer";
import { Grid, Paper } from "@mui/material";
import SimpleTasksContainer from "@/modules/tasks/SimpleTasksContainer/SimpleTasksContainer";
import WelcomeBackContainer from "@/modules/user/WelcomeBackContainer/WelcomeBackContainer";

export default function Home() {
  return (
    <AppLayout>
      <Grid container spacing={2} marginBottom={"1rem"}>
        <Grid item xs={12} md={6}>
          <Paper
            sx={{
              padding: "1rem",
              height: "400px",
              display: "flex",
              flexDirection: "column",
              justifyContent: "center",
            }}
            elevation={1}
          >
            <WelcomeBackContainer />
          </Paper>
        </Grid>
        <Grid item xs={12} md={6}>
          <Paper
            sx={{
              padding: "1rem",
              height: "400px",
              display: "flex",
              flexDirection: "column",
              justifyContent: "center",
            }}
            elevation={1}
          >
            <SimpleBoardsContainer />
          </Paper>
        </Grid>
      </Grid>
      <Paper sx={{ padding: "1rem", height: "450px" }} elevation={1}>
        <SimpleTasksContainer />
      </Paper>
    </AppLayout>
  );
}
