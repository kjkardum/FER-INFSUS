import { TaskboardApi } from "@/api/generated";
import config from "@/api/instances/configurationInstance";
import { BACKEND_URL } from "@/api/consts";
import axiosInstance from "@/api/instances/axiosInstance";

const TaskboardEndpoint = new TaskboardApi(config, BACKEND_URL, axiosInstance);

export default TaskboardEndpoint;
