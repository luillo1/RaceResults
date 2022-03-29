import React, { useCallback, useEffect, useState } from "react";
import {
  Button,
  Dimmer,
  Form,
  Loader,
  Modal,
  TextArea,
} from "semantic-ui-react";
import {
  Organization,
  RaceResponse,
  RaceResultResponse,
  useCreateSubmissionCheckpointMutation,
  useFetchSubmissionCheckpointsQuery,
  useLazyFetchRaceResultsQuery,
} from "../../slices/runners/raceresults-api-slice";
import { LoadingOrError } from "../../utils/loadingOrError";
import RequireOrganizationLogin from "../organizations/raceResults/RequireOrganizationLogin";

interface NewsletterPaneProps {
  organization: Organization;
}

const NewsletterPane = (props: NewsletterPaneProps) => {
  const [createCheckpoint] = useCreateSubmissionCheckpointMutation();

  const checkpointsResponse = useFetchSubmissionCheckpointsQuery({
    orgId: props.organization.id,
  });
  const [
    fetchRaceResults,
    raceResultsResponse,
  ] = useLazyFetchRaceResultsQuery();

  useEffect(() => {
    if (
      checkpointsResponse.isFetching ||
      checkpointsResponse.isLoading ||
      checkpointsResponse.data === undefined
    ) {
      return;
    }

    const checkpoints = checkpointsResponse.data.map(
      (resp) => new Date(Date.parse(resp.checkpointed))
    );
    const mostRecentCheckpoint =
      checkpoints.length > 0
        ? checkpoints.sort()[checkpoints.length - 1].toISOString()
        : null;

    fetchRaceResults({
      orgId: props.organization.id,
      startDate: mostRecentCheckpoint,
      endDate: null,
    });
  }, [checkpointsResponse.data]);

  const [generating, setGenerating] = useState(false);
  const [generated, setGenerated] = useState(false);
  const [newsletterText, setNewsletterText] = useState("");

  const createNewsletter = useCallback(() => {
    if (raceResultsResponse.data === undefined) {
      return;
    }

    setGenerating(true);

    const racesByEvent = new Map<string, RaceResponse[]>();
    const submissionsByRace = new Map<string, RaceResultResponse[]>();
    raceResultsResponse.data.forEach((result) => {
      const race = result.race;
      if (race === null) {
        return;
      }

      if (racesByEvent.has(race.eventId)) {
        const currentRacesForEvent = racesByEvent.get(
          race.eventId
        ) as RaceResponse[];
        if (
          !currentRacesForEvent.some((knownRace) => knownRace.id === race.id)
        ) {
          currentRacesForEvent.push(race);
        }
      } else {
        racesByEvent.set(race.eventId, [race]);
      }

      if (submissionsByRace.has(race.id)) {
        submissionsByRace.get(race.id)?.push(result);
      } else {
        submissionsByRace.set(race.id, [result]);
      }
    });

    const eventStrings: string[] = [];
    racesByEvent.forEach((races) => {
      const raceForInfo = races[0];

      const multipleDistances = races.length > 1;
      const raceStrings: string[] = [];
      races.forEach((race) => {
        const submissionStrings: string[] = [];
        submissionsByRace.get(race.id)?.forEach((submission) => {
          let text = `• ${submission.member?.firstName} ${submission.member?.lastName} ran a time of ${submission.raceResult.time}.`;
          if (submission.raceResult.comments !== "") {
            text = text + ` They said "${submission.raceResult.comments}".`;
          }
          submissionStrings.push(text);
        });

        const submissionsList = submissionStrings
          .map((t) => `\t${t}`)
          .join("\n");

        const text = multipleDistances
          ? `For the ${race.distance} race:
${submissionsList}`
          : submissionsList;
        raceStrings.push(text);
      });

      const distanceText = multipleDistances
        ? ""
        : `a ${raceForInfo.distance} race`;

      const eventText = `We had members run ${distanceText} at ${
        raceForInfo.name
      } on ${new Date(Date.parse(raceForInfo.date)).toDateString()}.
${raceStrings.join("\n")}`;

      eventStrings.push(eventText);
    });

    const newsletterText = eventStrings.join("\n\n");
    setNewsletterText(newsletterText);

    setGenerating(false);
    setGenerated(true);
  }, [raceResultsResponse.data]);

  return (
    <RequireOrganizationLogin organization={props.organization}>
      <LoadingOrError
        isLoading={
          checkpointsResponse.isLoading || raceResultsResponse.isLoading
        }
        hasError={checkpointsResponse.isError || raceResultsResponse.isError}
      >
        <Dimmer active={generating} inverted>
          <Loader inverted>Generating...</Loader>
        </Dimmer>

        <Form>
          <TextArea
            value={newsletterText}
            placeholder="Click generate below to create a new newsletter."
          />
        </Form>

        <br />

        <Button primary onClick={createNewsletter} disabled={generating}>
          Generate newsletter from submissions since last checkpoint
        </Button>

        <br />
        <br />

        <Modal
          trigger={
            <Button
              content="Create checkpoint"
              disabled={generating || !generated}
            />
          }
          header="Confirm Create Checkpoint"
          content="Are you sure you want to create a submissions checkpoint? This will reset the submissions members see after they submit a new race result, and prevent you from generating a newsletter for the submissions above. MAKE SURE YOU COPY THE NEWSLETTER ABOVE BEFORE DOING THIS."
          actions={[
            "Cancel",
            {
              key: "confirm",
              content: "Create",
              primary: true,
              onClick: () =>
                createCheckpoint({
                  orgId: props.organization.id,
                  checkpoint: {
                    checkpointed: new Date(Date.now()).toISOString(),
                    organizationId: props.organization.id,
                  },
                }),
            },
          ]}
        />
      </LoadingOrError>
    </RequireOrganizationLogin>
  );
};

export default NewsletterPane;
