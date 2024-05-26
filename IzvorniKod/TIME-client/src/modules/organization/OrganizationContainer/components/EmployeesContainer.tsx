"use client";
import { Divider, IconButton, Typography } from "@mui/material";
import EmployeesTable from "@/modules/organization/OrganizationContainer/components/EmployeesTable";
import React, { useCallback, useMemo, useRef, useState } from "react";
import { AddCircle, Person } from "@mui/icons-material";
import UserManagementModal from "@/modules/organization/OrganizationContainer/components/UserManagementModal";
import SearchInput from "@/components/SearchInput/SearchInput";
import { useQuery } from "@tanstack/react-query";
import { tenantGetUsersKey } from "@/api/reactQueryKeys/TenantEndpointKeys";
import tenantEndpoint from "@/api/endpoints/TenantEndpoint";
import { GridPaginationModel, GridSortModel } from "@mui/x-data-grid";

const convertSortModelToString = (sortModel: GridSortModel) => {
  return sortModel.map((sort) => `${sort.field} ${sort.sort}`).join(",");
};

const EmployeesContainer = () => {
  const [createUserModalOpen, setCreateUserModalOpen] = useState(false);
  const [searchValue, setSearchValue] = useState<string>("");
  const [paginationModel, setPaginationModel] = useState<GridPaginationModel>({
    page: 0,
    pageSize: 5,
  });
  const [sortModel, setSortModel] = useState<GridSortModel>([]);

  const { data, isLoading, refetch } = useQuery({
    queryKey: [
      tenantGetUsersKey[0],
      paginationModel.page + 1,
      paginationModel.pageSize,
      searchValue,
      convertSortModelToString(sortModel),
    ],
    queryFn: async () =>
      await tenantEndpoint.apiTenantManagementGetUsersGet(
        paginationModel.page + 1,
        paginationModel.pageSize,
        searchValue,
        convertSortModelToString(sortModel),
      ),
    select: (data) => data.data,
  });

  const rowCountRef = useRef(data?.totalRecords || 0);

  const rowCount = useMemo(() => {
    if (data?.totalRecords !== undefined) {
      rowCountRef.current = data?.totalRecords;
    }
    return rowCountRef.current;
  }, [data?.totalRecords]);

  const handleCreateUser = () => {
    setCreateUserModalOpen(true);
  };

  const handleClose = () => {
    setCreateUserModalOpen(false);
  };

  const handleSearch = (value: string) => {
    setSearchValue(value);
  };

  const handlePaginationModelChange = useCallback(
    (paginationModel: GridPaginationModel) => {
      setPaginationModel(paginationModel);
    },
    [],
  );

  const handleSortModelChange = useCallback((sortModel: GridSortModel) => {
    setSortModel(sortModel);
  }, []);

  const handleUpdate = () => {
    setPaginationModel({
      page: 0,
      pageSize: paginationModel.pageSize,
    });
    refetch();
    setCreateUserModalOpen(false);
  };

  return (
    <>
      <Typography variant={"h5"} gutterBottom>
        Zaposlenici
        <IconButton sx={{ ml: "0.5rem" }} onClick={handleCreateUser}>
          <AddCircle />
        </IconButton>
      </Typography>
      <SearchInput
        onChange={handleSearch}
        inputPlaceholder={"Pretraži zaposlenike..."}
        LeftIcon={<Person />}
      />
      <Divider sx={{ my: 2 }} />

      <EmployeesTable
        rows={data?.data || []}
        isLoading={isLoading}
        onPaginationModelChange={handlePaginationModelChange}
        paginationModel={paginationModel}
        totalUsers={rowCount}
        sortModel={sortModel}
        onSortModelChange={handleSortModelChange}
        handleUpdate={handleUpdate}
      />

      <UserManagementModal
        open={createUserModalOpen}
        handleClose={handleClose}
        handleUpdate={handleUpdate}
      />
    </>
  );
};

export default EmployeesContainer;
