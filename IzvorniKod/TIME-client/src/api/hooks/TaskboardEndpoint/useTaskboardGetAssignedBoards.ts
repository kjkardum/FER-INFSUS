import { useQuery } from "@tanstack/react-query";
import { taskbaordGetAssignedBoardsKey } from "@/api/reactQueryKeys/TaskboardEndpointKeys";
import taskboardEndpoint from "@/api/endpoints/TaskboardEndpoint";

const useTaskboardGetAssignedBoards = (disabled?: boolean) => {
  return useQuery({
    queryKey: taskbaordGetAssignedBoardsKey,
    enabled: !disabled,
    queryFn: async () => await taskboardEndpoint.apiTaskboardAssignedGet(),
    select: (data) => data.data,
  });
};

export default useTaskboardGetAssignedBoards;
