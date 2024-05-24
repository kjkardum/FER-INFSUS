"use client";
import React, { PropsWithChildren } from "react";
import Header from "@/components/Header/Header";
import { Box, Stack } from "@mui/material";
import SideMenu from "@/components/AppLayout/components/SideMenu";
import { reactQueryInstance } from "@/api/instances/reactQueryInstance";
import { QueryClientProvider } from "@tanstack/react-query";

const AppLayout = ({ children }: PropsWithChildren) => {
  return (
    <QueryClientProvider client={reactQueryInstance}>
      <Box height={"100vh"}>
        <Header />
        <Stack
          spacing={0}
          direction={"row"}
          width={"100%"}
          height={"calc(100vh - 65px)"}
          overflow={"hidden"}
        >
          <Box sx={{ display: { xs: "none", md: "block" } }}>
            <SideMenu />
          </Box>
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
    </QueryClientProvider>
  );
};

export default AppLayout;
