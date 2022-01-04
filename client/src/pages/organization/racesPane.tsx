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

interface ModalState {
  race: Race | undefined;
  isOpen: boolean;
}

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
            checked={race.isPublic}
            onChange={(
              _: React.FormEvent<HTMLInputElement>,
              data: CheckboxProps
            ) => {
              setisMutating(true);
              updateRace({
                ...race,
                isPublic: data.checked || false,
              }).then(() => setisMutating(false));
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

  const [addDistanceModalState, setAddDistanceModalState] = useState<
    ModalState
  >({ isOpen: false, race: undefined });

  const addDistanceForRace = (race: Race): void => {
    setAddDistanceModalState({ race, isOpen: true });
  };

  // The index into raceEvents that corresponds to the event being edited.
  // We store this so we iterate through each race that needs to be PATCHed
  // after the modal closes
  const [editEventIndex, setEditEventIndex] = useState<number | undefined>(
    undefined
  );

  const [editRaceModalState, setEditRaceModalState] = useState<ModalState>({
    isOpen: false,
    race: undefined,
  });

  useEffect(() => {
    if (editEventIndex !== undefined) {
      setEditRaceModalState({
        isOpen: true,
        race: raceEvents[editEventIndex][0],
      });
      setEditEventIndex(undefined);
    }
  }, [editEventIndex]);

  const editRace = (index: number): void => {
    setEditEventIndex(index);
  };

  if (racesResponse.isError) {
    return (
      <Header as="h4" icon textAlign="center">
        <Icon name="exclamation triangle" />
        There was an issue fetching races.
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
        showDistanceField
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
            isPublic: true,
          }).then(() => setisMutating(false));
        }}
        handleClose={function(): void {
          setAddRaceModalOpen(false);
        }}
        header={"Add Public Race"}
      />
      {addDistanceModalState.race !== undefined && (
        <CreateRaceModal
          showDistanceField
          open={addDistanceModalState.isOpen}
          initialRace={{
            distance: "",
            name: addDistanceModalState.race.name,
            location: addDistanceModalState.race.location,
            date: addDistanceModalState.race.date,
            eventId: addDistanceModalState.race.eventId,
          }}
          distanceOnly={true}
          onSubmit={async (race) => {
            setisMutating(true);
            setAddDistanceModalState({ race: undefined, isOpen: false });
            await createRace({
              name: race.name,
              date: race.date?.toISOString(),
              distance: race.distance,
              location: race.location,
              eventId: race.eventId,
              isPublic: true,
            }).then(() => setisMutating(false));
          }}
          handleClose={function(): void {
            setAddDistanceModalState({ race: undefined, isOpen: false });
          }}
          header={"Add Public Distance - " + addDistanceModalState.race.name}
        />
      )}
      {editRaceModalState.race !== undefined && (
        <CreateRaceModal
          showDistanceField={false}
          distanceOnly={false}
          open={editRaceModalState.isOpen}
          initialRace={{
            distance: "",
            name: editRaceModalState.race.name,
            location: editRaceModalState.race.location,
            date: editRaceModalState.race.date,
            eventId: editRaceModalState.race.eventId,
          }}
          onSubmit={async (race) => {
            if (editEventIndex === undefined) {
              return;
            }

            setisMutating(true);
            setEditRaceModalState({ race: undefined, isOpen: false });

            raceEvents[editEventIndex].forEach(async (raceToUpdate) => {
              await updateRace({
                ...raceToUpdate,
                name: race.name || raceToUpdate.name,
                location: race.location || raceToUpdate.name,
                date: race.date || raceToUpdate.date,
              });
            });

            setisMutating(false);
          }}
          handleClose={function(): void {
            setEditRaceModalState({ race: undefined, isOpen: false });
          }}
          header={"Edit Race - " + editRaceModalState.race.name}
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
          {raceEvents.map((races, outerIndex) => {
            return races.map((race, index) => (
              <Table.Row key={race.id}>
                {index === 0 && (
                  <Table.Cell rowSpan={races.length}>
                    <b>{race.name}</b> <br />
                    {race.location} - {race.date?.toDateString()} <br />
                    <a href="#" onClick={() => addDistanceForRace(race)}>
                      Add New Distance
                    </a>
                    {" | "}
                    <a href="#" onClick={() => editRace(outerIndex)}>
                      Edit
                    </a>
                  </Table.Cell>
                )}
                {raceCells(race)}
              </Table.Row>
            ));
          })}
        </Table.Body>
      </Table>
      <Button
        primary
        onClick={() => setAddRaceModalOpen(true)}
        content="Create Race"
      />
    </>
  );
};

export default RacesPane;
