"use client";
import React from "react";
import { Button, Stack, Typography } from "@mui/material";
import SidebarButton from "@/components/AppLayout/components/SidebarButton";
import { AccountCircle } from "@mui/icons-material";
import useAuthentication from "@/hooks/useAuthentication";
import { SIDE_MENU_TABS } from "@/components/AppLayout/consts";
import { usePathname } from "next/navigation";

const SideMenu = () => {
  const { user, logout } = useAuthentication();
  const pathname = usePathname();

  return (
    <Stack
      spacing={0}
      direction={"column"}
      borderRight={"1px solid #E4E6EA"}
      borderRadius={"2px"}
      paddingY={2}
      px={"0.75rem"}
      justifyContent={"space-between"}
      sx={{ overflow: "hidden" }}
      width={275}
    >
      <Stack spacing={1} sx={{ overflowX: "hidden", overflowY: "auto" }}>
        {SIDE_MENU_TABS.map((tab) => (
          <SidebarButton
            key={tab.path}
            text={tab.title}
            icon={tab.icon}
            color={tab.color}
            href={tab.path}
            active={pathname === tab.path}
          />
        ))}
      </Stack>
      <Stack
        spacing={2}
        direction={"column"}
        justifyContent={"center"}
        alignItems={"center"}
        width={"100%"}
      >
        <Button
          variant={"outlined"}
          color={"inherit"}
          disableRipple={true}
          sx={{
            textTransform: "none",
            borderColor: "transparent",
            justifyContent: "start",
          }}
        >
          <Stack direction={"row"} spacing={1}>
            <AccountCircle />
            <Typography variant={"body1"}>{user?.email}</Typography>
          </Stack>
        </Button>
        <Button
          variant={"outlined"}
          color={"error"}
          onClick={logout}
          sx={{ width: 150 }}
        >
          Logout
        </Button>
      </Stack>
    </Stack>
  );
};

export default SideMenu;
