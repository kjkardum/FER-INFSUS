"use client";
import React from "react";
import { Box, Button, TextField, Typography, useTheme } from "@mui/material";

export default function Login() {
  const theme = useTheme();

  return (
    <Box
      height={"100vh"}
      display={"flex"}
      flexDirection={"column"}
      justifyContent={"center"}
      alignItems={"center"}
      sx={{ backgroundColor: theme.palette.primary.main }}
    >
      <Box
        boxShadow={
          "0px 3px 1px -2px rgba(0,0,0,0.2),0px 2px 2px 0px rgba(0,0,0,0.14),0px 1px 5px 0px rgba(0,0,0,0.12)"
        }
        sx={{ backgroundColor: theme.palette.background.default }}
        borderRadius={"1rem"}
        padding={"5rem"}
        display={"flex"}
        flexDirection={"column"}
        gap={"2rem"}
        width={500}
      >
        <Typography variant={"h3"} align={"center"} marginBottom={"1rem"}>
          Login
        </Typography>
        <TextField label={"Username"} name={"username"} autoFocus />
        <TextField label={"Password"} name={"password"} type={"password"} />
        <Box textAlign={"center"}>
          <Button
            variant={"contained"}
            sx={{ width: 200, height: 40, marginTop: "1rem" }}
          >
            Login
          </Button>
        </Box>
      </Box>
    </Box>
  );
}
