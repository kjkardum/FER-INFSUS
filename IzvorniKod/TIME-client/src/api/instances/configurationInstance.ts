import { Configuration } from "@/api/generated";
import { BACKEND_URL, COOKIE_TOKEN } from "@/api/consts";
import Cookies from "js-cookie";

const config = new Configuration({
  basePath: BACKEND_URL,
  accessToken: () => {
    const token = Cookies.get(COOKIE_TOKEN);
    return token ?? "";
  },
});

export default config;
