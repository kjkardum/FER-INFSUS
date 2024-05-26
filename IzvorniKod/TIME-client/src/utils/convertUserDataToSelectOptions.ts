import { UserDto } from "@/api/generated";

const convertUserDataToSelectOptions = (users: Array<UserDto>) =>
  users.map((user) => ({
    label: `${user.firstName} ${user.lastName} (${user.email?.toLowerCase()})`,
    value: user.id ?? "",
  })) ?? [];

export default convertUserDataToSelectOptions;
