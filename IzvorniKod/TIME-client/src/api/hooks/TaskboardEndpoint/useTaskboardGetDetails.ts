import { useQuery } from "@tanstack/react-query";
import { taskboardGetBoardDetailsKey } from "@/api/reactQueryKeys/TaskboardEndpointKeys";
import taskboardEndpoint from "@/api/endpoints/TaskboardEndpoint";

const useTaskboardGetDetails = (boardId: string, disabled?: boolean) => {
  return useQuery({
    queryKey: taskboardGetBoardDetailsKey(boardId),
    queryFn: async () => await taskboardEndpoint.apiTaskboardIdGet(boardId),
    select: (data) => data.data,
    retry: 1,
    enabled: !disabled,
  });
};

export default useTaskboardGetDetails;
