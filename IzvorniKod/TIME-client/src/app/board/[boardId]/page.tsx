import AppLayout from "@/components/AppLayout/AppLayout";
import DetailedBoardContainer from "@/modules/board/DetailedBoardContainer/DetailedBoardContainer";

const BoardDetailPage = ({ params }: { params: { boardId: string } }) => {
  return (
    <AppLayout>
      <DetailedBoardContainer boardId={params.boardId} />
    </AppLayout>
  );
};

export default BoardDetailPage;
