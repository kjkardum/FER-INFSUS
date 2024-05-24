import { Typography } from "@mui/material";
import AppLayout from "@/components/AppLayout/AppLayout";

const BoardDetailPage = ({ params }: { params: { boardId: string } }) => {
  return (
    <AppLayout>
      BoardDetailPage{" "}
      <Typography variant={"body2"}>{params.boardId}</Typography>{" "}
    </AppLayout>
  );
};

export default BoardDetailPage;
