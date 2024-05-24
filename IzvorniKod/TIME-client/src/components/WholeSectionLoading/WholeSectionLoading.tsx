import { Box, CircularProgress } from "@mui/material";
import React from "react";

const WholeSectionLoading = () => {
  return (
    <Box
      width={"100%"}
      height={"100%"}
      display={"flex"}
      justifyContent={"center"}
      alignItems={"center"}
    >
      <CircularProgress color={"inherit"} />
    </Box>
  );
};

export default WholeSectionLoading;
