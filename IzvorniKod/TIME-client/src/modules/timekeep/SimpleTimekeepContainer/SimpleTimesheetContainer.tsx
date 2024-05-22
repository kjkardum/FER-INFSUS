"use client";

import React, { useState } from "react";
import dayjs, { Dayjs } from "dayjs";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import {
  DateCalendar,
  DayCalendarSkeleton,
  LocalizationProvider,
  PickersDay,
  PickersDayProps,
} from "@mui/x-date-pickers";
import { Badge, Grid, Typography } from "@mui/material";

function ServerDay(
  props: PickersDayProps<Dayjs> & { highlightedDays?: number[] },
) {
  const { highlightedDays = [], day, outsideCurrentMonth, ...other } = props;

  const isSelected =
    !props.outsideCurrentMonth &&
    highlightedDays.indexOf(props.day.date()) >= 0;

  return (
    <Badge
      key={props.day.toString()}
      overlap="circular"
      badgeContent={isSelected ? "🌚" : undefined}
    >
      <PickersDay
        {...other}
        outsideCurrentMonth={outsideCurrentMonth}
        day={day}
      />
    </Badge>
  );
}

const SimpleTimesheetContainer = () => {
  const [selectedDate, setSelectedDate] = useState<Dayjs>(dayjs());

  const [isLoading, setIsLoading] = useState(false);
  const [highlightedDays, setHighlightedDays] = React.useState([1, 2, 15]);

  const handleMonthChange = (date: Dayjs) => {
    setHighlightedDays([]);
  };

  const changeSelectedDate = (date: Dayjs) => {
    setSelectedDate(date);
  };

  return (
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <Typography variant={"h4"} marginBottom={"1rem"} align={"center"}>
        My Timesheet
      </Typography>
      <Grid container>
        <Grid item xs={7}>
          <DateCalendar
            value={selectedDate}
            loading={isLoading}
            onMonthChange={handleMonthChange}
            onChange={changeSelectedDate}
            renderLoading={() => <DayCalendarSkeleton />}
            slots={{
              day: ServerDay,
            }}
            slotProps={{
              day: {
                highlightedDays,
              } as any,
            }}
          />
        </Grid>
        <Grid item xs={5}>
          <Typography variant={"h6"}>
            {selectedDate.toDate().toDateString()}
          </Typography>
          <Typography variant={"body2"}>🌚 - Worked</Typography>
        </Grid>
      </Grid>
    </LocalizationProvider>
  );
};

export default SimpleTimesheetContainer;
