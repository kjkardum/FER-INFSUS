"use client";
import { AppBar, IconButton, Toolbar, Typography } from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import React, { useState } from "react";
import useAuthentication from "@/hooks/useAuthentication";
import UserProfileMenu from "@/components/Header/components/UserProfileMenu";
import UserDrawer from "@/components/Header/components/UserDrawer";

const Header = () => {
  const { isAuthenticated } = useAuthentication();
  const [drawerOpen, setDrawerOpen] = useState(false);

  return (
    <AppBar position="static">
      <Toolbar>
        <IconButton
          size="large"
          edge="start"
          color="inherit"
          aria-label="menu"
          sx={{ mr: 2 }}
          onClick={() => setDrawerOpen(true)}
        >
          <MenuIcon />
        </IconButton>
        <Typography variant="h6" sx={{ flexGrow: 1 }}>
          TIME
        </Typography>
        {isAuthenticated && <UserProfileMenu />}
      </Toolbar>
      <UserDrawer open={drawerOpen} onClose={() => setDrawerOpen(false)} />
    </AppBar>
  );
};

export default Header;
