import React from "react";
import { Header, Icon, Loader, Dimmer, Table, Button } from "semantic-ui-react";
import {
  useDeleteRaceResultMutation,
  useFetchRaceResultsQuery,
} from "../../slices/runners/raceresults-api-slice";

interface SubmissionsPaneProps {
  orgId: string;
}

const SubmissionsPane = (props: SubmissionsPaneProps) => {
  const raceResultsResponse = useFetchRaceResultsQuery(props.orgId);
  const [deleteRaceResult] = useDeleteRaceResultMutation();

  if (raceResultsResponse.isError) {
    return (
      <Header as="h4" icon textAlign="center">
        <Icon name="exclamation triangle" />
        There was an issue fetching submissions.
      </Header>
    );
  }

  // TODO: make this table sortable
  // TODO: add ability to delete submissions
  // TODO: add ability to query range of dates

  return (
    <>
      <Dimmer
        active={raceResultsResponse.isLoading || raceResultsResponse.isFetching}
        inverted
      >
        <Loader active indeterminate>
          Loading submissions...
        </Loader>
      </Dimmer>
      <Table celled striped>
        <Table.Header fullWidth>
          <Table.Row>
            <Table.HeaderCell>Race</Table.HeaderCell>
            <Table.HeaderCell>Member</Table.HeaderCell>
            <Table.HeaderCell>Time</Table.HeaderCell>
            <Table.HeaderCell>Submitted</Table.HeaderCell>
            <Table.HeaderCell>Comments</Table.HeaderCell>
            <Table.HeaderCell width={"1"}>Actions</Table.HeaderCell>
          </Table.Row>
        </Table.Header>
        <Table.Body>
          {raceResultsResponse.data?.map((raceResult) => (
            <Table.Row key={raceResult.raceResult.id}>
              <Table.Cell>
                {raceResult.race !== null ? raceResult.race.name : "ERROR"}
              </Table.Cell>
              <Table.Cell>
                {raceResult.member !== null
                  ? `${raceResult.member.firstName} ${raceResult.member.lastName} (${raceResult.member.orgAssignedMemberId})`
                  : "ERROR"}
              </Table.Cell>
              <Table.Cell>{raceResult.raceResult.time}</Table.Cell>
              <Table.Cell>
                {new Date(
                  Date.parse(raceResult.raceResult.submitted)
                ).toDateString()}
              </Table.Cell>
              <Table.Cell>{raceResult.raceResult.comments}</Table.Cell>
              <Table.Cell>
                <Button
                  title="Delete"
                  color="red"
                  negative
                  compact
                  basic
                  icon="delete"
                  onClick={() =>
                    deleteRaceResult({
                      orgId: props.orgId,
                      memberId: raceResult.raceResult.memberId,
                      raceResultId: raceResult.raceResult.id,
                    })
                  }
                />
              </Table.Cell>
            </Table.Row>
          ))}
        </Table.Body>
      </Table>
    </>
  );
};

export default SubmissionsPane;
