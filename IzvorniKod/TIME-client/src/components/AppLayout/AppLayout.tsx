"use client";
import React, { PropsWithChildren } from "react";
import Header from "@/components/Header/Header";
import { Box, Stack } from "@mui/material";
import SideMenu from "@/components/AppLayout/components/SideMenu";

const AppLayout = ({ children }: PropsWithChildren) => {
  return (
    <Box height={"100vh"}>
      <Header />
      <Stack
        spacing={0}
        direction={"row"}
        width={"100%"}
        height={"calc(100vh - 65px)"}
        overflow={"hidden"}
      >
        <SideMenu />
        <Box
          p={"2rem"}
          sx={{
            overflowX: "hidden",
            overflowY: "auto",
            backgroundColor: "#F9F9F9",
          }}
          width={"100%"}
        >
          {children}
        </Box>
      </Stack>
    </Box>
  );
};

export default AppLayout;
