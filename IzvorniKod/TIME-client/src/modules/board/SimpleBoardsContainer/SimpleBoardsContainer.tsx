import { Box, Button, Typography } from "@mui/material";

const SimpleBoardsContainer = () => {
  return (
    <Box>
      <Typography variant={"h5"} marginBottom={"1rem"} align={"center"}>
        My Boards
      </Typography>
      <Box
        display={"grid"}
        gridTemplateColumns={"1fr 1fr"}
        rowGap={"1rem"}
        columnGap={"1rem"}
      >
        <Button variant={"outlined"} color={"inherit"}>
          Board 1
        </Button>
        <Button variant={"outlined"} color={"inherit"}>
          Board 2
        </Button>
        <Button variant={"outlined"} color={"inherit"}>
          Board 3
        </Button>
        <Button variant={"outlined"} color={"inherit"}>
          Board 4
        </Button>
      </Box>
    </Box>
  );
};

export default SimpleBoardsContainer;
