import { useQuery } from "@tanstack/react-query";
import taskboardEndpoint from "@/api/endpoints/TaskboardEndpoint";
import { taskboardGetAllBoardsKey } from "@/api/reactQueryKeys/TaskboardEndpointKeys";
import useAuthentication from "@/hooks/useAuthentication";

const useTaskboardGetAllBoards = () => {
  const { isAdmin } = useAuthentication();

  return useQuery({
    queryKey: taskboardGetAllBoardsKey,
    enabled: isAdmin,
    queryFn: async () => await taskboardEndpoint.apiTaskboardAllGet(),
    select: (data) => data.data,
  });
};

export default useTaskboardGetAllBoards;
