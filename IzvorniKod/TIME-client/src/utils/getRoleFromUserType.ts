const getRoleFromUserType = (role: number) => {
  if (role === 1) {
    return "Admin";
  }
  if (role === 2) {
    return "User";
  }

  return "Unknown";
};

export default getRoleFromUserType;
