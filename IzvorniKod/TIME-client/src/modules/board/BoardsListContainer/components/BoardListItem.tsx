import {
  Card,
  CardActions,
  CardContent,
  IconButton,
  styled,
  Typography,
} from "@mui/material";
import { Menu as MenuIcon } from "@mui/icons-material";
import Link from "next/link";
import React, { MouseEventHandler } from "react";
import { TaskboardSimpleDto } from "@/api/generated";

const CardWithHover = styled(Card)(({ theme }) => ({
  "&:hover": {
    boxShadow: theme.shadows[2],
    cursor: "pointer",
  },
}));

interface Props {
  board: TaskboardSimpleDto;
  isAdmin?: boolean;
  menuOpen: MouseEventHandler<HTMLButtonElement>;
}

const BoardListItem = (props: Props) => {
  return (
    <Link
      href={`/board/${props.board.id}`}
      style={{ textDecoration: "none", color: "inherit" }}
    >
      <CardWithHover variant="outlined">
        <CardContent>
          <Typography variant="h5" component="div">
            {props.board.name}
          </Typography>
          <Typography sx={{ mb: 1.5 }} color="text.secondary">
            3 Users - 10 Tasks
          </Typography>
          <Typography variant="body2">{props.board.description}</Typography>
        </CardContent>
        {props.isAdmin && (
          <CardActions>
            <IconButton onClick={props.menuOpen}>
              <MenuIcon />
            </IconButton>
          </CardActions>
        )}
      </CardWithHover>
    </Link>
  );
};

export default BoardListItem;
