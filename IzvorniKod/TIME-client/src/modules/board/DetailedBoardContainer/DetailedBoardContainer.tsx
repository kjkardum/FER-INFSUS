"use client";
import {
  Box,
  Breadcrumbs,
  Divider,
  Grid,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import React, { useEffect, useState } from "react";
import useTaskboardGetDetails from "@/api/hooks/TaskboardEndpoint/useTaskboardGetDetails";
import { useRouter } from "next/navigation";
import WholeSectionLoading from "@/components/WholeSectionLoading/WholeSectionLoading";
import TasksList from "@/modules/board/DetailedBoardContainer/components/TasksList";
import useSnackbar from "@/hooks/useSnackbar";
import Link from "next/link";
import UsersList from "@/modules/board/DetailedBoardContainer/components/UsersList";
import taskboardEndpoint from "@/api/endpoints/TaskboardEndpoint";

interface Props {
  boardId: string;
}

const DetailedBoardContainer = ({ boardId }: Props) => {
  const [boardName, setBoardName] = useState<string>("");
  const [boardDescription, setBoardDescription] = useState<string>("");
  const [editingBoardName, setEditingBoardName] = useState<boolean>(false);
  const [editingBoardDescription, setEditingBoardDescription] =
    useState<boolean>(false);

  const router = useRouter();
  const { data, isError, isLoading, isSuccess, refetch } =
    useTaskboardGetDetails(boardId);
  const { showSnackbar } = useSnackbar();

  const handleSaveEdited = () => {
    taskboardEndpoint
      .apiTaskboardIdPut(boardId, {
        name: boardName,
        description: boardDescription,
      })
      .then(() => {
        setEditingBoardName(false);
        setEditingBoardDescription(false);
        refetch();
        showSnackbar("Changes saved.", "success");
      })
      .catch(() => {
        showSnackbar("Failed to save changes.", "error");
      });
  };

  useEffect(() => {
    if (isError) {
      showSnackbar("Board not found.", "error");
      router.push("/boards");
    }
  }, [isError, router, showSnackbar]);

  useEffect(() => {
    if (data) {
      setBoardName(data?.name ?? "");
      setBoardDescription(data?.description ?? "");
    }
  }, [data]);

  return (
    <Stack>
      <Breadcrumbs aria-label="breadcrumb" sx={{ mb: "2rem" }}>
        <Link
          href={"/boards"}
          style={{ textDecoration: "none", color: "inherit" }}
        >
          <Typography color="inherit">Boards</Typography>
        </Link>
        <Typography color="text.primary">{boardName}</Typography>
      </Breadcrumbs>
      {!editingBoardName && (
        <Box onClick={() => setEditingBoardName(true)}>
          <Typography variant="h5" gutterBottom>
            {data?.name}
          </Typography>
        </Box>
      )}

      {editingBoardName && (
        <TextField
          label="Board name"
          value={boardName}
          onChange={(e) => setBoardName(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter" || e.key === "Escape") handleSaveEdited();
          }}
          autoFocus
          onBlur={handleSaveEdited}
          fullWidth
        />
      )}

      {!editingBoardDescription && (
        <Box onClick={() => setEditingBoardDescription(true)} zIndex={9999}>
          <Typography variant="body2" gutterBottom>
            {data?.description}
          </Typography>
        </Box>
      )}

      {editingBoardDescription && (
        <TextField
          label="Board description"
          value={boardDescription}
          onChange={(e) => setBoardDescription(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter" || e.key === "Escape") handleSaveEdited();
          }}
          autoFocus
          onBlur={handleSaveEdited}
          fullWidth
        />
      )}

      <Divider sx={{ my: 2 }} />
      {isLoading && <WholeSectionLoading />}
      {data && isSuccess && (
        <Box>
          <Grid container spacing={8}>
            <Grid item xs={12} lg={7}>
              <TasksList board={data} />
            </Grid>
            <Grid item xs={12} lg={5}>
              <UsersList users={data.taskboardUsers ?? []} boardId={boardId} />
            </Grid>
          </Grid>
        </Box>
      )}
    </Stack>
  );
};

export default DetailedBoardContainer;
