import React, { useState } from "react";
import {
  Header,
  Icon,
  Loader,
  Dimmer,
  Table,
  Button,
  Form,
} from "semantic-ui-react";
import DatePickerInput from "../../components/DatePickerInput";
import {
  useDeleteRaceResultMutation,
  useFetchRaceResultsQuery,
} from "../../slices/runners/raceresults-standard-api-slice";

interface SubmissionsPaneProps {
  orgId: string;
}

//
// Get the current date and 1 week before the current date for
// the initial values of the date filter. We add 1 to the current
// date since endDate is exclusive, and we want to default to including
// submissions that occurred today.
//
const initialEndDate = new Date(new Date().toDateString());
initialEndDate.setDate(initialEndDate.getDate() + 1);

const initialStartDate = new Date(initialEndDate);
initialStartDate.setDate(initialEndDate.getDate() - 8);

const SubmissionsPane = (props: SubmissionsPaneProps) => {
  const [dateRange, setDateRange] = useState<{
    startDate: Date;
    endDate: Date;
  }>({ startDate: initialStartDate, endDate: initialEndDate });

  const updateDateRange = (date: Date, startDate: boolean) => {
    let newStartDate = dateRange.startDate;
    let newEndDate = dateRange.endDate;

    if (startDate) {
      newStartDate = date;
      if (dateRange.endDate < date) {
        newEndDate = date;
      }
    } else {
      newEndDate = date;
      if (dateRange.startDate > date) {
        newStartDate = date;
      }
    }

    setDateRange({ startDate: newStartDate, endDate: newEndDate });
  };

  const raceResultsResponse = useFetchRaceResultsQuery({
    orgId: props.orgId,
    startDate: dateRange.startDate.toISOString(),
    endDate: dateRange.endDate.toISOString(),
  });

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
      <Form>
        <Form.Group>
          <Form.Field
            control={DatePickerInput}
            onChange={(date: Date) => {
              updateDateRange(date, true);
            }}
            selected={dateRange.startDate}
            label={"Submitted on or after"}
          />
          <Form.Field
            control={DatePickerInput}
            onChange={(date: Date) => {
              updateDateRange(date, false);
            }}
            selected={dateRange.endDate}
            label={"Submitted before"}
          />
        </Form.Group>
      </Form>
      <Table celled striped selectable>
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
