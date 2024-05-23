import { AuthenticationApi } from "@/api/generated";
import config from "@/api/instances/configurationInstance";
import { BACKEND_URL } from "@/api/consts";
import axiosInstance from "@/api/instances/axiosInstance";

const AuthEndpoint = new AuthenticationApi(config, BACKEND_URL, axiosInstance);

export default AuthEndpoint;
