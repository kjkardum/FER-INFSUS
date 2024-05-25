const getTaskStateFromStateNumber = (stateNumber: number): string => {
  switch (stateNumber) {
    case 0:
      return "New";
    case 1:
      return "Ready";
    case 2:
      return "In Progress";
    case 3:
      return "Done";
    case 4:
      return "Canceled";
    default:
      return "Unknown state";
  }
};

export default getTaskStateFromStateNumber;
