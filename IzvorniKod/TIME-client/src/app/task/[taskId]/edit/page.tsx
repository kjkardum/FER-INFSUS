import AppLayout from "@/components/AppLayout/AppLayout";
import EditTaskContainer from "@/modules/tasks/EditTaskContainer/EditTaskContainer";

const EditTaskPage = ({ params }: { params: { taskId: string } }) => {
  return (
    <AppLayout>
      <EditTaskContainer taskId={params.taskId} />
    </AppLayout>
  );
};

export default EditTaskPage;
