import React from "react";
import { DeveloperBoard } from "@mui/icons-material";
import SearchInput from "@/components/SearchInput/SearchInput";

interface Props {
  onChange?: (value: string) => void;
}

const BoardsListSearch = ({ onChange }: Props) => {
  return (
    <SearchInput
      onChange={onChange}
      LeftIcon={<DeveloperBoard />}
      inputPlaceholder="Pretraži radne ploče..."
    />
  );
};

export default BoardsListSearch;
