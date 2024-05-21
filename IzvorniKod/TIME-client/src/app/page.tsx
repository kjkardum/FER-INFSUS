import React from "react";
import AppLayout from "@/components/AppLayout/AppLayout";
import SimpleBoardsContainer from "@/modules/board/SimpleBoardsContainer/SimpleBoardsContainer";
import { Box, Grid } from "@mui/material";
import SimpleTimesheetContainer from "@/modules/timekeep/SimpleTimekeepContainer/SimpleTimesheetContainer";
import SimpleTasksContainer from "@/modules/tasks/SimpleTasksContainer/SimpleTasksContainer";

export default function Home() {
  return (
    <AppLayout>
      <Grid container spacing={2} marginBottom={"1rem"}>
        <Grid item xs={12} md={6}>
          <Box
            height={"400px"}
            padding={"1rem"}
            boxShadow={
              "0px 3px 1px -2px rgba(0,0,0,0.2),0px 2px 2px 0px rgba(0,0,0,0.14),0px 1px 5px 0px rgba(0,0,0,0.12)"
            }
          >
            <SimpleBoardsContainer />
          </Box>
        </Grid>
        <Grid item xs={12} md={6}>
          <Box
            height={"400px"}
            padding={"1rem"}
            boxShadow={
              "0px 3px 1px -2px rgba(0,0,0,0.2),0px 2px 2px 0px rgba(0,0,0,0.14),0px 1px 5px 0px rgba(0,0,0,0.12)"
            }
          >
            <SimpleTimesheetContainer />
          </Box>
        </Grid>
      </Grid>
      <Box
        padding={"1rem"}
        boxShadow={
          "0px 3px 1px -2px rgba(0,0,0,0.2),0px 2px 2px 0px rgba(0,0,0,0.14),0px 1px 5px 0px rgba(0,0,0,0.12)"
        }
      >
        <SimpleTasksContainer />
      </Box>
    </AppLayout>
  );
}
