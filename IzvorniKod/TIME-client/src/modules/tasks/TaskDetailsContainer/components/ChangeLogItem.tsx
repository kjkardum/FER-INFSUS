"use client";
import { Button, Paper, Typography } from "@mui/material";
import { TaskItemHistoryLogDto } from "@/api/generated";
import React, { useState } from "react";
import dayjs from "dayjs";

interface Props {
  changeLog: TaskItemHistoryLogDto;
}

const ChangeLogItem = ({ changeLog }: Props) => {
  const [isExpanded, setIsExpanded] = useState<boolean>(false);

  return (
    <Paper elevation={1} sx={{ p: "1rem" }}>
      <Typography
        variant="body1"
        gutterBottom
        maxHeight={"400px"}
        overflow={"auto"}
      >
        {!isExpanded && (
          <>
            {changeLog.changelog?.split("\n").at(0)}...
            <Button
              onClick={() => setIsExpanded(true)}
              sx={{ textTransform: "none" }}
              variant={"text"}
            >
              Prikaži više...
            </Button>
          </>
        )}

        {isExpanded && (
          <>
            {changeLog.changelog?.split("\n").map((line, index) => (
              <React.Fragment key={index}>
                {line}
                <br />
              </React.Fragment>
            ))}
            <Button
              onClick={() => setIsExpanded(false)}
              sx={{ textTransform: "none" }}
              variant={"text"}
            >
              Prikaži manje...
            </Button>
          </>
        )}
      </Typography>
      <Typography variant="caption" color={"text.secondary"}>
        {`Promijena napraljena na: ${(changeLog.modifiedAt ? dayjs(changeLog.modifiedAt) : dayjs()).format("DD-MM-YYYY HH:mm:ss")}`}
      </Typography>
    </Paper>
  );
};

export default ChangeLogItem;
