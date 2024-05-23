import { StatusApi } from "@/api/generated";
import config from "@/api/instances/configurationInstance";
import { BACKEND_URL } from "@/api/consts";
import axiosInstance from "@/api/instances/axiosInstance";

const StatusEndpoint = new StatusApi(config, BACKEND_URL, axiosInstance);

export default StatusEndpoint;
