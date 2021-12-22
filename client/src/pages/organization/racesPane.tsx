import React, { useEffect, useMemo, useState } from "react";
import {
  Header,
  Icon,
  Loader,
  Dimmer,
  Table,
  Checkbox,
  CheckboxProps,
  Button,
} from "semantic-ui-react";
import { Race } from "../../common";
import CreateRaceModal from "../../components/createRaceModal";
import {
  useCreateRaceMutation,
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
  const [isMutating, setisMutating] = useState(false);
  const [addRaceModalOpen, setAddRaceModalOpen] = useState(false);
  const racesResponse = useFetchRacesQuery();
  const [updateRace] = useUpdateRaceMutation();
  const [createRace] = useCreateRaceMutation();

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
              setisMutating(true);
              updateRace({ ...race, public: data.checked || false }).then(() =>
                setisMutating(false)
              );
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

  const [addDistanceModalOpen, setAddDistanceModalOpen] = useState(false);
  const [addDistanceRace, setAddDistanceRace] = useState<Race | undefined>(
    undefined
  );

  useEffect(() => {
    setAddDistanceModalOpen(true);
  }, [addDistanceRace]);

  const addDistanceForRace = (race: Race): void => {
    setAddDistanceRace(race);
  };

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
        active={
          racesResponse.isFetching || racesResponse.isLoading || isMutating
        }
        inverted
      >
        <Loader active indeterminate>
          Loading races...
        </Loader>
      </Dimmer>
      <CreateRaceModal
        open={addRaceModalOpen}
        initialRace={{ name: "", distance: "", location: "" }}
        distanceOnly={false}
        onSubmit={async (race) => {
          setisMutating(true);
          setAddRaceModalOpen(false);
          await createRace({
            name: race.name,
            date: race.date?.toISOString(),
            distance: race.distance,
            location: race.location,
            eventId: race.eventId,
            public: true,
          }).then(() => setisMutating(false));
        }}
        handleClose={function(): void {
          setAddRaceModalOpen(false);
        }}
        header={"Add Public Race"}
      />
      {addDistanceRace !== undefined && (
        <CreateRaceModal
          open={addDistanceModalOpen}
          initialRace={{
            distance: "",
            name: addDistanceRace.name,
            location: addDistanceRace.location,
            date: addDistanceRace.date,
            eventId: addDistanceRace.eventId,
          }}
          distanceOnly={true}
          onSubmit={async (race) => {
            setisMutating(true);
            setAddDistanceModalOpen(false);
            await createRace({
              name: race.name,
              date: race.date?.toISOString(),
              distance: race.distance,
              location: race.location,
              eventId: race.eventId,
              public: true,
            }).then(() => setisMutating(false));
          }}
          handleClose={function(): void {
            setAddDistanceModalOpen(false);
          }}
          header={"Add Public Distance - " + addDistanceRace.name}
        />
      )}
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
                    {race.location} - {race.date?.toDateString()} <br />
                    <a href="#" onClick={() => addDistanceForRace(race)}>
                      Add New Distance
                    </a>
                  </Table.Cell>
                )}
                {raceCells(race)}
              </Table.Row>
            ));
          })}
        </Table.Body>
      </Table>
      <Button onClick={() => setAddRaceModalOpen(true)} content="Create Race" />
    </>
  );
};

export default RacesPane;
