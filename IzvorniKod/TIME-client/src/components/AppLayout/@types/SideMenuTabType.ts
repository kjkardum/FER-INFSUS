import { ReactNode } from "react";
import { UserRole } from "@/contexts/authentication/@types/User";

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
  role?: UserRole;
}
