const getRoleFromUserType = (role: number) => {
  if (role === 1) {
    return "Admin";
  }
  if (role === 0) {
    return "User";
  }

  return "Unknown";
};

export default getRoleFromUserType;
