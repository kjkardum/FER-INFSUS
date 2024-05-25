import { useQuery } from "@tanstack/react-query";
import taskItemEndpoint from "@/api/endpoints/TaskItemEndpoint";
import { taskItemGetTaskItemKey } from "@/api/reactQueryKeys/TaskItemEndpointKeys";

const useTaskItemGetItem = (taskId: string) => {
  return useQuery({
    queryKey: taskItemGetTaskItemKey(taskId),
    queryFn: () => taskItemEndpoint.apiTaskItemIdGet(taskId),
    select: (data) => data.data,
  });
};

export default useTaskItemGetItem;
