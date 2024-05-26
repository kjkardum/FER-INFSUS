import AppLayout from "@/components/AppLayout/AppLayout";
import AssignedTasksContainer from "@/modules/tasks/AssignedTasksContainer/AssignedTasksContainer";

const TasksPage = () => {
  return (
    <AppLayout>
      <AssignedTasksContainer />
    </AppLayout>
  );
};

export default TasksPage;
