"use client";
import { Box, Drawer, IconButton, Toolbar, Typography } from "@mui/material";
import React, { useState } from "react";
import { Menu, Timer } from "@mui/icons-material";
import SideMenu from "@/components/AppLayout/components/SideMenu";

interface Props {
  height?: string;
}

const Header = ({ height = "65px" }: Props) => {
  const [open, setOpen] = useState(false);

  return (
    <Box sx={{ borderBottom: "1px solid #E4E6EA" }} height={height}>
      <Toolbar>
        <IconButton
          edge="start"
          color="inherit"
          aria-label="menu"
          sx={{ display: { xs: "inline-flex", md: "none" } }}
          onClick={() => setOpen(true)}
        >
          <Menu />
        </IconButton>
        <Timer color={"primary"} sx={{ marginLeft: "0.25rem" }} />
        <Typography variant="h6" sx={{ flexGrow: 1, marginLeft: "0.5rem" }}>
          TIME
        </Typography>
      </Toolbar>
      <Drawer open={open} onClose={() => setOpen(false)}>
        <Box height={"100vh"}>
          <SideMenu />
        </Box>
      </Drawer>
    </Box>
  );
};

export default Header;
