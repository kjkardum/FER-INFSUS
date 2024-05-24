"use client";
import { useQuery } from "@tanstack/react-query";
import tenantEndpoint from "@/api/endpoints/TenantEndpoint";
import { tenantGetUsersKey } from "@/api/reactQueryKeys/TenantEndpointKeys";

const useTenantGetUsers = () => {
  return useQuery({
    queryKey: tenantGetUsersKey,
    queryFn: async () => await tenantEndpoint.apiTenantManagementGetUsersGet(),
    select: (data) => data.data,
  });
};

export default useTenantGetUsers;
