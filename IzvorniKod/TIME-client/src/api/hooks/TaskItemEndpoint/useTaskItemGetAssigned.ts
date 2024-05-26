import { useQuery } from "@tanstack/react-query";
import { taskItemGetAssignedKey } from "@/api/reactQueryKeys/TaskItemEndpointKeys";
import taskItemEndpoint from "@/api/endpoints/TaskItemEndpoint";

const useTaskItemGetAssigned = () => {
  return useQuery({
    queryKey: taskItemGetAssignedKey,
    queryFn: async () => await taskItemEndpoint.apiTaskItemAssignedGet(),
    select: (data) => data.data,
  });
};

export default useTaskItemGetAssigned;
