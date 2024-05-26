"use client";
import { TaskItemForTasklistDto, TaskItemSimpleDto } from "@/api/generated";
import {
  Box,
  Chip,
  IconButton,
  Paper,
  Stack,
  styled,
  Typography,
} from "@mui/material";
import dayjs from "dayjs";
import { Delete } from "@mui/icons-material";
import React from "react";
import getColorFromTaskStatus from "@/utils/getColorFromTaskStatus";
import { useRouter } from "next/navigation";

const HoverPaper = styled(Paper)(({ theme }) => ({
  "&:hover": {
    boxShadow: theme.shadows[3],
    cursor: "pointer",
  },
}));

interface Props {
  task: TaskItemSimpleDto | TaskItemForTasklistDto;
  handleTaskClick: (taskId?: string) => void;
  isAdmin: boolean;
  handleOpenDeleteTaskModal: (
    e: React.MouseEvent<HTMLButtonElement>,
    taskId?: string,
  ) => void;
  showTaskStatus?: boolean;
  hideTaskAssigned?: boolean;
}

const TaskComponent = ({
  task,
  handleTaskClick,
  isAdmin,
  handleOpenDeleteTaskModal,
  showTaskStatus,
  hideTaskAssigned,
}: Props) => {
  const router = useRouter();

  const handleTaskboardClick = (e: React.MouseEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.stopPropagation();
    if (!task.taskboardId) return;

    router.push(`/board/${task.taskboardId}`);
  };

  return (
    <HoverPaper
      sx={{
        p: "1rem",
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
      }}
      key={task.id}
      elevation={2}
      onClick={() => handleTaskClick(task.id)}
    >
      <Box>
        <Typography variant="h6">{task.name}</Typography>
        <Typography variant="body1">{task.description}</Typography>
        {/* TODO: clip description if too long */}
        <Typography variant="body2">
          {`Kreirano: ${task.createdAt ? dayjs(task.createdAt).format("DD-MM-YYYY HH:mm") : dayjs().format("DD-MM-YYYY HH:mm")}`}
        </Typography>
      </Box>
      <Stack columnGap={"0.5rem"} alignItems={"center"} direction={"row"}>
        <Stack
          columnGap={"0.5rem"}
          rowGap={"0.25rem"}
          alignItems={"center"}
          sx={{ flexDirection: { xs: "column", md: "row" } }}
        >
          {!hideTaskAssigned && task.assignedUser && (
            <Chip
              color={"secondary"}
              label={`Dodijeljeno: ${task.assignedUser.firstName} ${task.assignedUser.lastName}`}
            />
          )}
          {!hideTaskAssigned && !task.assignedUser && (
            <Chip label={`Nije dodijeljeno`} />
          )}
        </Stack>
        {!!showTaskStatus && (
          <>
            {"taskboardName" in task && (
              <Chip label={task.taskboardName} onClick={handleTaskboardClick} />
            )}
            <Chip
              color={getColorFromTaskStatus(task.state ?? "Novo")}
              label={task.state}
            />
          </>
        )}
        {isAdmin && (
          <IconButton onClick={(e) => handleOpenDeleteTaskModal(e, task.id)}>
            <Delete />
          </IconButton>
        )}
      </Stack>
    </HoverPaper>
  );
};

export default TaskComponent;
