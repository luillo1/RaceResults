import React from "react";
import { Header, Icon, Loader, Dimmer, Table } from "semantic-ui-react";
import { useFetchMembersQuery } from "../../slices/runners/raceresults-standard-api-slice";

interface MembersPaneProps {
  orgId: string;
}

const MembersPane = (props: MembersPaneProps) => {
  const membersResponse = useFetchMembersQuery(props.orgId);

  if (membersResponse.isError) {
    return (
      <Header as="h4" icon textAlign="center">
        <Icon name="exclamation triangle" />
        There was an issue fetching members.
      </Header>
    );
  }

  return (
    <>
      <Dimmer active={membersResponse.isLoading} inverted>
        <Loader active indeterminate>
          Loading members...
        </Loader>
      </Dimmer>
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
              <Table.Cell>{member.nicknames?.join(",")}</Table.Cell>
            </Table.Row>
          ))}
        </Table.Body>
      </Table>
    </>
  );
};

export default MembersPane;
