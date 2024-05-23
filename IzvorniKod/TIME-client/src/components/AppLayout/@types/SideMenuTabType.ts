import { ReactNode } from "react";

export interface SideMenuTabType {
  title: string;
  icon: ReactNode;
  path: string;
  color:
    | "inherit"
    | "primary"
    | "secondary"
    | "error"
    | "info"
    | "success"
    | "warning";
}
