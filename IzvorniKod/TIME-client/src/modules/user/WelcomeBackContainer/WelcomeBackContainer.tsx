"use client";
import React from "react";
import { Stack, Typography } from "@mui/material";
import { WavingHand } from "@mui/icons-material";

const WelcomeBackContainer = () => {
  return (
    <Stack spacing={2}>
      <Typography variant={"h5"} align={"center"}>
        <WavingHand sx={{ color: "#ffcc33" }} /> Welcome back!
      </Typography>
      <Typography variant={"body1"} align={"center"}>
        We are happy to see you again.
      </Typography>
      <Typography variant={"body1"} align={"center"}>
        Here's where you'll get a summary of the organization, tasks and boards
      </Typography>
    </Stack>
  );
};

export default WelcomeBackContainer;
