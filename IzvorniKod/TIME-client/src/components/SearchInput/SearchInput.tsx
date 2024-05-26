import { IconButton, InputBase, Paper } from "@mui/material";
import { Search } from "@mui/icons-material";
import React, { ChangeEvent, useCallback } from "react";

interface Props {
  onChange?: (value: string) => void;
  LeftIcon?: React.ReactNode;
  inputPlaceholder?: string;
}

const SearchInput = ({ onChange, LeftIcon, inputPlaceholder }: Props) => {
  const debouncedOnChange = useCallback(() => {
    let timeout: NodeJS.Timeout;
    return (event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
      clearTimeout(timeout);
      timeout = setTimeout(() => {
        onChange?.(event.target.value);
      }, 500);
    };
  }, [onChange]);

  return (
    <Paper
      sx={{
        p: "2px 4px",
        display: "flex",
        alignItems: "center",
        maxWidth: 400,
      }}
    >
      {LeftIcon && (
        <IconButton sx={{ p: "10px" }} aria-label="menu" disableRipple>
          {LeftIcon}
        </IconButton>
      )}

      <InputBase
        sx={{ ml: 1, flex: 1 }}
        placeholder={inputPlaceholder}
        inputProps={{ "aria-label": inputPlaceholder }}
        onChange={debouncedOnChange()}
      />
      <IconButton
        type="button"
        sx={{ p: "10px" }}
        aria-label="search"
        disableRipple
      >
        <Search />
      </IconButton>
    </Paper>
  );
};

export default SearchInput;
