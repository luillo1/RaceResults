import React from "react";
import { NavLink } from "react-router-dom";
import { Button, Divider, Header, List } from "semantic-ui-react";
import { useFetchOrganizationsQuery } from "../../slices/runners/raceresults-standard-api-slice";
import BasePage from "../../utils/basePage";
import { LoadingOrError } from "../../utils/loadingOrError";
import routes from "../../utils/routes";

const OrganizationsPage = () => {
  const queryResponse = useFetchOrganizationsQuery();

  return (
    <LoadingOrError
      isLoading={queryResponse.isLoading}
      hasError={queryResponse.isError}
    >
      <BasePage>
        <Header as="h2" content="Your Organizations" />
        <Divider />
        <List divided relaxed selection verticalAlign="middle">
          {queryResponse.data?.map((organization) => (
            <List.Item
              key={organization.id}
              as={NavLink}
              to={`/organizations/${organization.id}`}
            >
              <List.Content>
                <List.Header>{organization.name}</List.Header>
                <List.Description as="a">
                  Place something interesting here
                </List.Description>
              </List.Content>
            </List.Item>
          ))}
        </List>
        <Button
          primary
          as={NavLink}
          to={routes.createOrganization.path}
          content="Create"
        />
      </BasePage>
    </LoadingOrError>
  );
};

export default OrganizationsPage;
