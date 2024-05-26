"use client";
import AppLayout from "@/components/AppLayout/AppLayout";
import TaskContainer from "@/modules/tasks/TaskDetailsContainer/TaskDetailsContainer";

const TaskPage = ({ params }: { params: { taskId: string } }) => {
  return (
    <AppLayout>
      <TaskContainer taskId={params.taskId} />
    </AppLayout>
  );
};

export default TaskPage;
