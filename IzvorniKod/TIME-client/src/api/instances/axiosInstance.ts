import axios from "axios";
import Cookies from "js-cookie";
import { BACKEND_URL, COOKIE_TOKEN } from "@/api/consts";

export const axiosInstance = axios.create({
  baseURL: BACKEND_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

// @ts-ignore - ignore the error because we are adding an interceptor
axiosInstance.interceptors.request.use((config) => {
  const token = Cookies.get(COOKIE_TOKEN);

  return {
    ...config,
    headers: {
      ...config.headers,
      Authorization: token ? `Bearer ${token}` : "",
    },
  };
});

export default axiosInstance;
