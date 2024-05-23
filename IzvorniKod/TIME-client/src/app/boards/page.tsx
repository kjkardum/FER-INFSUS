"use client";
import React from "react";
import AppLayout from "@/components/AppLayout/AppLayout";
import BoardsListContainer from "@/modules/board/BoardsListContainer/BoardsListContainer";

const Boards = () => {
  return (
    <AppLayout>
      <BoardsListContainer />
    </AppLayout>
  );
};

export default Boards;
