import { useQuery, UseQueryResult } from "@tanstack/react-query";
import taskboardEndpoint from "@/api/endpoints/TaskboardEndpoint";
import { taskboardGetAllBoardsKey } from "@/api/reactQueryKeys/TaskboardEndpointKeys";

const useTaskboardGetAllBoards = () => {
  return useQuery({
    queryKey: taskboardGetAllBoardsKey,
    queryFn: async () => await taskboardEndpoint.apiTaskboardAllGet(),
    select: (data) => data.data,
  }) as UseQueryResult<
    Array<{ id: string; name: string; description: string }>,
    Error
  >;
};

export default useTaskboardGetAllBoards;
