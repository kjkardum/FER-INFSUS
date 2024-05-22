import React, { PropsWithChildren } from "react";
import Header from "@/components/Header/Header";
import { Box } from "@mui/material";

const AppLayout = ({ children }: PropsWithChildren) => {
  return (
    <main>
      <Header />
      <Box m={"1rem"}>{children}</Box>
    </main>
  );
};

export default AppLayout;
