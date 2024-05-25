import {
  FormControl,
  FormControlLabel,
  FormLabel,
  Radio,
  RadioGroup,
} from "@mui/material";
import React from "react";
import { TaskItemState } from "@/api/generated";

interface Props {
  taskState: TaskItemState;
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
        {Object.keys(TaskItemState).map((value) => (
          <FormControlLabel
            key={value}
            value={value}
            control={<Radio />}
            label={value}
          />
        ))}
      </RadioGroup>
    </FormControl>
  );
};

export default TaskStateSelector;
