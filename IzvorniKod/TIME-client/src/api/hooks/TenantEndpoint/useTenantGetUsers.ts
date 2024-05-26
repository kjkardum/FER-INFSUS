"use client";
import { useQuery } from "@tanstack/react-query";
import tenantEndpoint from "@/api/endpoints/TenantEndpoint";
import { tenantGetUsersKey } from "@/api/reactQueryKeys/TenantEndpointKeys";
import useAuthentication from "@/hooks/useAuthentication";

const useTenantGetUsers = (disabled?: boolean) => {
  const { isAdmin } = useAuthentication();

  return useQuery({
    queryKey: tenantGetUsersKey,
    queryFn: async () =>
      await tenantEndpoint.apiTenantManagementGetUsersGet(1, 100),
    select: (data) => data.data,
    enabled: !disabled && isAdmin,
  });
};

export default useTenantGetUsers;
