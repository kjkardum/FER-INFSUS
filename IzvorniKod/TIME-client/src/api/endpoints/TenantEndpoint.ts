import { TenantManagementApi } from "@/api/generated";
import config from "@/api/instances/configurationInstance";
import { BACKEND_URL } from "@/api/consts";
import axiosInstance from "@/api/instances/axiosInstance";

const TenantEndpoint = new TenantManagementApi(
  config,
  BACKEND_URL,
  axiosInstance,
);

export default TenantEndpoint;
