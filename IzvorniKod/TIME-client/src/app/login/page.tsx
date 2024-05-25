"use client";
import React, { FormEventHandler, useState } from "react";
import { Box, Paper, TextField, Typography, useTheme } from "@mui/material";
import useAuthentication from "@/hooks/useAuthentication";
import AuthEndpoint from "@/api/endpoints/AuthEndpoint";
import { LoadingButton } from "@mui/lab";
import { WavingHand } from "@mui/icons-material";
import useSnackbar from "@/hooks/useSnackbar";

export default function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const theme = useTheme();
  const { login } = useAuthentication();
  const { showSnackbar } = useSnackbar();

  const handleLogin: FormEventHandler<HTMLFormElement> = (event) => {
    event.preventDefault();
    setIsLoading(true);
    AuthEndpoint.apiAuthenticationLoginPost({ email, password })
      .then((user) => {
        if (user.data.token) login(user.data.token);
      })
      .catch((error) => {
        showSnackbar("Invalid email or password", "error");
      })
      .finally(() => setIsLoading(false));
  };

  return (
    <Box
      height={"100vh"}
      display={"flex"}
      flexDirection={"column"}
      justifyContent={"center"}
      alignItems={"center"}
      sx={{
        backgroundColor: theme.palette.primary.main,
        opacity: 1,
        backgroundImage: `radial-gradient(${theme.palette.primary.dark} 2px, transparent 2px), radial-gradient(${theme.palette.primary.dark} 2px, ${theme.palette.primary.main} 2px)`,
        backgroundSize: "80px 80px",
        backgroundPosition: "0 0,40px 40px",
      }}
    >
      <Paper elevation={2}>
        <Box
          component={"form"}
          onSubmit={handleLogin}
          sx={{ backgroundColor: theme.palette.background.default }}
          borderRadius={"1rem"}
          padding={"5rem"}
          display={"flex"}
          flexDirection={"column"}
          gap={"2rem"}
          width={500}
        >
          <Typography variant={"h4"} align={"center"} gutterBottom>
            Dobrodošao! <WavingHand sx={{ color: "#ffcc33" }} />
          </Typography>
          <TextField
            label={"E-mail:"}
            name={"email"}
            type={"email"}
            color={"secondary"}
            autoFocus
            value={email}
            onChange={(event) => setEmail(event.target.value)}
          />
          <TextField
            label={"Lozinka:"}
            name={"password"}
            type={"password"}
            value={password}
            color={"secondary"}
            onChange={(event) => setPassword(event.target.value)}
          />
          <Box textAlign={"center"}>
            <LoadingButton
              loading={isLoading}
              type={"submit"}
              variant={"contained"}
              color={"secondary"}
              sx={{ width: 200, height: 40, marginTop: "1rem" }}
            >
              Prijavi se
            </LoadingButton>
          </Box>
        </Box>
      </Paper>
    </Box>
  );
}
