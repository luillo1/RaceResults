import React, { useMemo, useState } from "react";
import {
  Header,
  Icon,
  Loader,
  Dimmer,
  Table,
  Checkbox,
  CheckboxProps,
} from "semantic-ui-react";
import { Race } from "../../common";
import {
  useFetchRacesQuery,
  useUpdateRaceMutation,
} from "../../slices/runners/raceresults-api-slice";
import { groupByEventId } from "../../utils/helpers";

/*  TODO: use
interface RacesPaneProps {
  orgId: string;
}
*/

const RacesPane = () => {
  const [raceEvents, setRaceEvents] = useState<Race[][]>([]);
  const racesResponse = useFetchRacesQuery();
  const [updateRace] = useUpdateRaceMutation();

  const raceCells = (race: Race): JSX.Element => {
    return (
      <>
        <Table.Cell>{race.distance}</Table.Cell>
        <Table.Cell>
          <Checkbox
            checked={race.public}
            onChange={(
              _: React.FormEvent<HTMLInputElement>,
              data: CheckboxProps
            ) => {
              updateRace({ ...race, public: data.checked || false });
            }}
          />
        </Table.Cell>
      </>
    );
  };

  useMemo(() => {
    if (racesResponse.isSuccess && racesResponse.data !== undefined) {
      // We need to first convert the ISO strings to Date objects.
      const races = racesResponse.data.map((resp) => {
        return { ...resp, date: new Date(Date.parse(resp.date)) };
      });

      // Replace all race events with what just came from the backend
      setRaceEvents(groupByEventId(races));
    }
  }, [racesResponse.data]);

  if (racesResponse.isError) {
    return (
      <Header as="h4" icon textAlign="center">
        <Icon name="exclamation triangle" />
        There was an issue fetching members.
      </Header>
    );
  }

  return (
    <>
      <Dimmer
        active={racesResponse.isFetching || racesResponse.isLoading}
        inverted
      >
        <Loader active indeterminate>
          Loading races...
        </Loader>
      </Dimmer>
      <Table celled structured>
        <Table.Header>
          <Table.Row>
            <Table.HeaderCell>Event</Table.HeaderCell>
            <Table.HeaderCell>Distance</Table.HeaderCell>
            <Table.HeaderCell collapsing>Show</Table.HeaderCell>
          </Table.Row>
        </Table.Header>
        <Table.Body>
          {raceEvents.map((races) => {
            return races.map((race, index) => (
              <Table.Row key={race.id}>
                {index === 0 && (
                  <Table.Cell rowSpan={races.length}>
                    <b>{race.name}</b> <br />
                    {race.location} - {race.date?.toDateString()}
                  </Table.Cell>
                )}
                {raceCells(race)}
              </Table.Row>
            ));
          })}
        </Table.Body>
      </Table>
    </>
  );
};

export default RacesPane;
