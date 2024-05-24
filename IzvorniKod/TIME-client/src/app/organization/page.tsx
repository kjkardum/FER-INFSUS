"use client";
import React, { useEffect } from "react";
import AppLayout from "@/components/AppLayout/AppLayout";
import OrganizationContainer from "@/modules/organization/OrganizationContainer/OrganizationContainer";
import useAuthentication from "@/hooks/useAuthentication";
import { useRouter } from "next/navigation";

const Organization = () => {
  const router = useRouter();
  const { isAdmin, isInitialized } = useAuthentication();

  useEffect(() => {
    if (isInitialized && !isAdmin) {
      router.push("/");
    }
  }, [isAdmin, isInitialized, router]);

  return (
    <AppLayout>
      <OrganizationContainer />
    </AppLayout>
  );
};

export default Organization;
