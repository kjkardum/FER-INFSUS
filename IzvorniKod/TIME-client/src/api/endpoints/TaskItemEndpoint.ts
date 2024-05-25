import { TaskItemApi } from "@/api/generated";
import config from "@/api/instances/configurationInstance";
import { BACKEND_URL } from "@/api/consts";
import axiosInstance from "@/api/instances/axiosInstance";

const TaskItemEndpoint = new TaskItemApi(config, BACKEND_URL, axiosInstance);

export default TaskItemEndpoint;
