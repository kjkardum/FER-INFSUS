"use client";
import { useQuery } from "@tanstack/react-query";
import tenantEndpoint from "@/api/endpoints/TenantEndpoint";
import { tenantGetUsersKey } from "@/api/reactQueryKeys/TenantEndpointKeys";

const useTenantGetUsers = (disabled?: boolean) => {
  return useQuery({
    queryKey: tenantGetUsersKey,
    queryFn: async () => await tenantEndpoint.apiTenantManagementGetUsersGet(),
    select: (data) => data.data,
    enabled: !disabled,
  });
};

export default useTenantGetUsers;
