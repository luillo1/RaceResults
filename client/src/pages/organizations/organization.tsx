import React from "react";
import { useParams } from "react-router";
import { Divider, Header, Table } from "semantic-ui-react";
import {
  useFetchMembersQuery,
  useFetchOrganizationQuery
} from "../../slices/runners/raceresults-api-slice";
import BasePage from "../../utils/basePage";
import { LoadingOrError } from "../../utils/loadingOrError";
import NotFound from "../notFound";

const OrganizationPage = () => {
  const { id } = useParams();
  if (id == null) {
    return <NotFound />;
  } else {
    const orgResponse = useFetchOrganizationQuery(id);
    const membersResponse = useFetchMembersQuery(id);

    return (
      <LoadingOrError
        isLoading={orgResponse.isLoading || membersResponse.isLoading}
        hasError={orgResponse.isError || membersResponse.isError}
      >
        <BasePage>
          <Header as="h2" content={orgResponse.data?.name} />
          <Divider />
          <Table celled>
            <Table.Header>
              <Table.Row>
                <Table.HeaderCell>Name</Table.HeaderCell>
                <Table.HeaderCell>Aliases</Table.HeaderCell>
              </Table.Row>
            </Table.Header>
            <Table.Body>
              {membersResponse.data?.map((member) => (
                <Table.Row key={member.id}>
                  <Table.Cell>{`${member.firstName} ${member.lastName}`}</Table.Cell>
                  <Table.Cell>{member.nicknames.join(",")}</Table.Cell>
                </Table.Row>
              ))}
            </Table.Body>
          </Table>
        </BasePage>
      </LoadingOrError>
    );
  }
};

export default OrganizationPage;
