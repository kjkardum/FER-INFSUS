import { TaskItemState } from "@/api/generated";

const getColorFromTaskStatus = (status: TaskItemState) => {
  switch (status) {
    case "Novo":
      return "default";
    case "Aktivan":
      return "secondary";
    case "Dovršen":
      return "success";
    case "Spreman":
      return "info";
    case "Prekinut":
      return "error";
    default:
      return "default";
  }
};

export default getColorFromTaskStatus;
