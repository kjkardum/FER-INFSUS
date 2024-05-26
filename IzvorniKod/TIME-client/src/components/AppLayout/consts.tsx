import React from "react";
import {
  CorporateFare,
  Dashboard,
  DeveloperBoard,
  Task,
} from "@mui/icons-material";
import { SideMenuTabType } from "@/components/AppLayout/@types/SideMenuTabType";

export const SIDE_MENU_TABS: Array<SideMenuTabType> = [
  {
    title: "Nadzorna Ploča",
    icon: <Dashboard />,
    path: "/",
    color: "primary",
  },
  {
    title: "Radne Ploče",
    icon: <DeveloperBoard />,
    path: "/boards",
    color: "secondary",
  },
  {
    title: "Zadaci",
    icon: <Task />,
    path: "/tasks",
    color: "error",
  },
  /*
  {
    title: "TimeSheet",
    icon: <ViewTimeline />,
    path: "/timesheet",
    color: "success",
  },
   */
  {
    title: "Organizacija",
    icon: <CorporateFare />,
    path: "/organization",
    color: "warning",
    role: "ADMIN",
  },
];
