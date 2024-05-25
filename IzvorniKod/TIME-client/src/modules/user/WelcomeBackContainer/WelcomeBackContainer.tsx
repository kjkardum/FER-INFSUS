"use client";
import React from "react";
import { Stack, Typography } from "@mui/material";
import { WavingHand } from "@mui/icons-material";

const WelcomeBackContainer = () => {
  return (
    <Stack spacing={2}>
      <Typography variant={"h5"} align={"center"}>
        <WavingHand sx={{ color: "#ffcc33" }} /> Pozdrav!
      </Typography>
      <Typography variant={"body1"} align={"center"}>
        Dobrodošli natrag!
      </Typography>
      <Typography variant={"body1"} align={"center"}>
        Ovdje možete pronaći sve svoje radne ploče i zadatke.
      </Typography>
    </Stack>
  );
};

export default WelcomeBackContainer;
