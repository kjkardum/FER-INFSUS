"use client";
import { Box, Toolbar, Typography } from "@mui/material";
import React from "react";
import { Timer } from "@mui/icons-material";

interface Props {
  height?: string;
}

const Header = ({ height = "65px" }: Props) => {
  return (
    <Box sx={{ borderBottom: "1px solid #E4E6EA" }} height={height}>
      <Toolbar>
        <Timer color={"primary"} sx={{ marginLeft: "0.25rem" }} />
        <Typography variant="h6" sx={{ flexGrow: 1, marginLeft: "0.5rem" }}>
          TIME
        </Typography>
      </Toolbar>
    </Box>
  );
};

export default Header;
