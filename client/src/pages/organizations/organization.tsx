import React from "react";
import { useParams } from "react-router";
import { Divider, Header } from "semantic-ui-react";
import { useFetchOrganizationQuery } from "../../slices/runners/runners-api-slice";
import BasePage from "../../utils/basePage";
import { LoadingOrError } from "../../utils/loadingOrError";

const OrganizationPage = () => {
  const { id } = useParams();
  const queryResponse = useFetchOrganizationQuery(id);

  return (
    <LoadingOrError
      isLoading={queryResponse.isLoading}
      hasError={queryResponse.isError}
    >
      <BasePage>
        <Header as="h2" content={queryResponse.data?.name} />
        <Divider />
      </BasePage>
    </LoadingOrError>
  );
};

export default OrganizationPage;
