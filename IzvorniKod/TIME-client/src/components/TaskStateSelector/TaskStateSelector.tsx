import {
  FormControl,
  FormControlLabel,
  FormLabel,
  Radio,
  RadioGroup,
} from "@mui/material";
import getTaskStateFromStateNumber from "@/utils/getTaskStateFromStateNumber";
import React from "react";

interface Props {
  taskState: number;
  handleStateChange: (event: React.ChangeEvent<HTMLInputElement>) => void;
}

const TaskStateSelector = ({ taskState, handleStateChange }: Props) => {
  return (
    <FormControl>
      <FormLabel id="controlled-radio-buttons-group-taskStateSelector">
        Role
      </FormLabel>
      <RadioGroup
        aria-labelledby="controlled-radio-buttons-group-taskStateSelector"
        name="controlled-radio-buttons-group-taskStateSelector"
        value={taskState}
        onChange={handleStateChange}
        row
      >
        {[0, 1, 2, 3, 4].map((value) => (
          <FormControlLabel
            key={value}
            value={value}
            control={<Radio />}
            label={getTaskStateFromStateNumber(value)}
          />
        ))}
      </RadioGroup>
    </FormControl>
  );
};

export default TaskStateSelector;
