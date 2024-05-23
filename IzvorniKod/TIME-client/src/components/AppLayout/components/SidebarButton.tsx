import { Button, Stack, Typography } from "@mui/material";
import React from "react";
import Link from "next/link";

type Props = {
  icon?: React.ReactNode;
  text: string;
  color?:
    | "inherit"
    | "primary"
    | "secondary"
    | "error"
    | "info"
    | "success"
    | "warning";
  href: string;
  active?: boolean;
};

const SidebarButton = (props: Props) => {
  return (
    <Link
      href={props.href}
      style={{ textDecoration: "none", color: "inherit" }}
    >
      <Button
        variant={"outlined"}
        color={props.color || "inherit"}
        sx={{
          textTransform: "none",
          borderColor: props.active ? "inherit" : "transparent",
          justifyContent: "start",
          py: "0.5rem",
        }}
        fullWidth={true}
      >
        <Stack direction={"row"} spacing={1}>
          {props.icon}
          <Typography variant={"body1"}>{props.text}</Typography>
        </Stack>
      </Button>
    </Link>
  );
};

export default SidebarButton;
