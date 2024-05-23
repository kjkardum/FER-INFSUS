import React from "react";
import {
  CorporateFare,
  Dashboard,
  DeveloperBoard,
  Task,
  ViewTimeline,
} from "@mui/icons-material";
import { SideMenuTabType } from "@/components/AppLayout/@types/SideMenuTabType";

export const SIDE_MENU_TABS: Array<SideMenuTabType> = [
  {
    title: "Dashboard",
    icon: <Dashboard />,
    path: "/",
    color: "primary",
  },
  {
    title: "Boards",
    icon: <DeveloperBoard />,
    path: "/boards",
    color: "secondary",
  },
  {
    title: "Tasks",
    icon: <Task />,
    path: "/tasks",
    color: "error",
  },
  {
    title: "TimeSheet",
    icon: <ViewTimeline />,
    path: "/timesheet",
    color: "success",
  },
  {
    title: "Organization",
    icon: <CorporateFare />,
    path: "/organization",
    color: "warning",
  },
];
