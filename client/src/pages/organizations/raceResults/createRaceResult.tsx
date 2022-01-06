import React, { useMemo, useState } from "react";
import {
  Divider,
  DropdownProps,
  Form,
  Header,
  Message,
} from "semantic-ui-react";
import * as Yup from "yup";
import {
  Organization,
  useCreatePublicRaceMutation,
  useFetchOrganizationQuery,
  useFetchPublicRacesQuery,
} from "../../../slices/runners/raceresults-standard-api-slice";
import BasePage from "../../../utils/basePage";
import CreateRaceModal from "../../../components/createRaceModal";
import { Formik } from "formik";
import { SemanticTextInputField } from "../../../components/SemanticFields/SemanticTextInputField";
import { LoadingOrError } from "../../../utils/loadingOrError";
import { Race } from "../../../common";
import { SemanticSelectField } from "../../../components/SemanticFields/SemanticSelectField";
import { SemanticTextAreaField } from "../../../components/SemanticFields/SemanticTextAreaField";
import { useAppSelector } from "../../../redux/hooks";
import {
  useFetchUserInfoQuery,
  UserInfo,
} from "../../../slices/wild-apricot/wild-apricot-api-slice";
import { useParams } from "react-router-dom";
import {
  useCreateMemberMutation,
  useCreateRaceResultMutation,
  useFetchMemberQuery,
} from "../../../slices/runners/raceresults-wa-api-slice";
import RequireWildApricotLogin from "./requireWildApricotLogin";

/**
 * Groups the given race models by their eventId.
 * @param races the race models to group
 * @returns an 2-dimensional array where each entry is a list of race models who all
 * share the same eventId.
 */
function groupByEventId(races: Race[]): Race[][] {
  if (races.length === 0) {
    return [];
  }

  const racesByEventId: Map<string, Race[]> = new Map<string, Race[]>();

  races.forEach((item) => {
    if (
      item.eventId === null ||
      item.eventId === undefined ||
      item.eventId === "" ||
      item.eventId === "00000000-0000-0000-0000-000000000000"
    ) {
      return;
    }

    if (!racesByEventId.has(item.eventId)) {
      racesByEventId.set(item.eventId, []);
    }

    racesByEventId.get(item.eventId)?.push(item);
  });

  return [...racesByEventId.values()];
}

interface FormValues {
  firstName: string;
  lastName: string;
  selectedEventIndex: number;
  selectedRaceIndex: number;
  comments: string;
  time: string;
}

const CreateRaceResultPageForm = (props: {
  organization: Organization;
  user: UserInfo;
}) => {
  // State to track if there is an error creating submission
  const [error, setError] = useState(false);

  // State to track if the submission was created successfully
  const [success, setSuccess] = useState(false);

  // State to track if the "add race" modal is currently open
  const [addRaceModalOpen, setAddRaceModalOpen] = useState(false);

  // State to track if the "add distance" modal is currently open
  const [addDistanceModalOpen, setAddDistanceModalOpen] = useState(false);

  // State to track all the races being displayed on the form.
  // This initially comes from the backend server, but gets updated as
  // the user adds new races/distances.
  const [raceEvents, setRaceEvents] = useState<Partial<Race>[][]>([]);

  const racesResponse = useFetchPublicRacesQuery();

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

  const [createRace] = useCreatePublicRaceMutation();

  const [createRaceResult] = useCreateRaceResultMutation();

  const [createMember] = useCreateMemberMutation();

  const memberResponse = useFetchMemberQuery({
    orgId: props.organization.id,
    orgAssignedMemberId: props.user.id.toString(),
  });

  const header = `Submit Race Result - ${props.organization.name}`;

  if (success && !error) {
    return (
      <BasePage>
        <Header as="h2" content={header} />
        <Divider />
        <Message positive>
          <Message.Header>Time submitted</Message.Header>
          <p>
            Your result will be published in the next newsletter. Thank you!
          </p>
          <p>
            <a
              style={{ cursor: "pointer" }}
              role="button"
              onClick={() => {
                // Remove all added races
                const newRaces: Partial<Race>[][] = [];
                raceEvents.forEach((events) => {
                  const newEvents = events.filter(
                    (race) => race.id !== undefined
                  );
                  if (newEvents.length > 0) {
                    newRaces.push(newEvents);
                  }
                });
                setRaceEvents(newRaces);

                setError(false);
                setSuccess(false);
              }}
            >
              Click here
            </a>{" "}
            to submit another time.
          </p>
        </Message>
      </BasePage>
    );
  }

  const initialFormValues: FormValues = {
    firstName: props.user.firstName,
    lastName: props.user.lastName,
    selectedEventIndex: -1,
    selectedRaceIndex: -1,
    comments: "",
    time: "",
  };

  return (
    <LoadingOrError
      isLoading={racesResponse.isLoading || memberResponse.isLoading}
      hasError={racesResponse.isError}
    >
      <BasePage>
        <Header as="h2" content={header} />
        <Divider />
        <Formik
          initialValues={initialFormValues}
          validationSchema={Yup.object({
            firstName: Yup.string()
              .max(255, "The entered value is too long.")
              .required("This field is required."),
            lastName: Yup.string()
              .max(255, "The entered value is too long.")
              .required("This field is required."),
            selectedEventIndex: Yup.number()
              .nullable()
              .required("This field is required."),
            selectedRaceIndex: Yup.number()
              .nullable()
              .required("This field is required."),
            time: Yup.string()
              .required("This field is required.")
              .matches(
                /^(\d?\d:)?\d\d:\d\d(\.\d*)?$/,
                "Time must be in either hh:mm:ss or mm:ss format."
              )
              .matches(
                /^(\d?\d:)?\d\d:\d\d(\.\d{0,4})?$/,
                "Seconds cannot contain more than 4 decimal places."
              ),
            comments: Yup.string().notRequired(),
          })}
          onSubmit={async (values, helpers) => {
            setError(false);
            setSuccess(false);

            if (
              values.selectedEventIndex === -1 ||
              values.selectedRaceIndex === -1
            ) {
              return;
            }

            let error = false;

            const race =
              raceEvents[values.selectedEventIndex][values.selectedRaceIndex];

            let raceIdToSubmit = "";
            if (race.id === undefined) {
              // This is a new race we have to first create
              await createRace({
                name: race.name,
                date: race.date?.toISOString(),
                distance: race.distance,
                location: race.location,
                eventId: race.eventId,
              })
                .unwrap()
                .then((createdRace) => {
                  raceIdToSubmit = createdRace.id;
                })
                .catch(() => {
                  error = true;
                });
            } else {
              raceIdToSubmit = race.id;
            }

            if (error) {
              setError(true);
              return;
            }

            let memberIdToSubmit = "";

            if (memberResponse.isError || memberResponse.data === undefined) {
              // We need to make a new member first
              await createMember({
                orgId: props.organization.id,
                member: {
                  organizationId: props.organization.id,
                  orgAssignedMemberId: props.user.id.toString(),
                  firstName: props.user.firstName,
                  lastName: props.user.lastName,
                  email: props.user.email,
                },
              })
                .unwrap()
                .then((createdMember) => {
                  memberIdToSubmit = createdMember.id;
                })
                .catch(() => {
                  error = true;
                });
            } else {
              memberIdToSubmit = memberResponse.data.id;
            }

            if (error) {
              setError(true);
              return;
            }

            // Fix the time so we have hours if none were specified
            let timeToSubmit = values.time;
            const timeParts = timeToSubmit.split(":");
            if (timeParts.length === 2) {
              timeToSubmit = "00:" + timeToSubmit;
            } else if (timeParts.length === 3) {
              if (timeParts[0].length === 1) {
                // Server expects 2 digits for hours place :)
                timeToSubmit = "0" + timeToSubmit;
              }
            }

            await createRaceResult({
              orgId: props.organization.id,
              memberId: memberIdToSubmit,
              raceResult: {
                raceId: raceIdToSubmit,
                memberId: memberIdToSubmit,
                time: timeToSubmit,
                comments: values.comments || "",
                dataSource: "user-submission",
              },
            })
              .unwrap()
              .then((createdRaceResult) => {
                console.log(createdRaceResult);
              })
              .catch(() => {
                error = true;
              });

            if (error) {
              setError(true);
              return;
            }

            helpers.resetForm();
            setSuccess(true);
          }}
        >
          {({ values, isSubmitting, setValues, handleSubmit }) => {
            return (
              <div>
                <CreateRaceModal
                  showDistanceField
                  header="Create New Race"
                  open={addRaceModalOpen}
                  handleClose={() => {
                    setAddRaceModalOpen(false);
                  }}
                  distanceOnly={false}
                  onSubmit={(race: Partial<Race>) => {
                    //
                    // Add the new race as a new group
                    //
                    const newRaces = raceEvents.concat([[race]]);

                    //
                    // Set the state so that the newly created
                    // event + distance is selected
                    //

                    setRaceEvents(newRaces);
                    setAddRaceModalOpen(false);

                    setValues({
                      ...values,
                      selectedEventIndex: newRaces.length - 1,
                      selectedRaceIndex: 0,
                    });
                  }}
                  initialRace={{
                    name: "",
                    distance: "",
                    date: null,
                    id: "",
                    location: "",
                  }}
                />
                {values.selectedEventIndex !== -1 && (
                  <CreateRaceModal
                    showDistanceField
                    header="Add Distance"
                    open={addDistanceModalOpen}
                    handleClose={() => setAddDistanceModalOpen(false)}
                    distanceOnly={true}
                    onSubmit={(race: Partial<Race>) => {
                      if (values.selectedEventIndex === -1) {
                        // TODO: throw error?
                        return;
                      }

                      const newRaces = [...raceEvents];
                      newRaces[values.selectedEventIndex].push(race);

                      setRaceEvents(newRaces);
                      setAddDistanceModalOpen(false);

                      setValues({
                        ...values,
                        selectedRaceIndex:
                          newRaces[values.selectedEventIndex].length - 1,
                      });
                    }}
                    initialRace={{
                      ...raceEvents[values.selectedEventIndex][0],
                      distance: "",
                    }}
                  />
                )}
                {error && (
                  <Message negative>
                    <Message.Header>
                      There was a problem submitting your time.
                    </Message.Header>
                  </Message>
                )}
                <Form onSubmit={handleSubmit} loading={isSubmitting}>
                  <Form.Group widths="equal">
                    <SemanticTextInputField
                      name="firstName"
                      label="First Name"
                      placeholder="First name"
                    />
                    <SemanticTextInputField
                      name="lastName"
                      label="Last name"
                      placeholder="Last name"
                    />
                  </Form.Group>
                  <Form.Group widths="equal">
                    <SemanticSelectField
                      label="Race Name"
                      name="selectedEventIndex"
                      onChange={(
                        e: React.SyntheticEvent<HTMLElement>,
                        data: DropdownProps
                      ) => {
                        e.preventDefault();
                        e.stopPropagation();
                        if (data.value === raceEvents.length) {
                          setAddRaceModalOpen(true);
                        } else {
                          setValues({
                            ...values,
                            selectedEventIndex: data.value as number,
                            selectedRaceIndex: 0,
                          });
                        }
                      }}
                      options={raceEvents
                        .map((races: Partial<Race>[], index: number) => ({
                          key: index,
                          value: index,
                          text: races[0].name,
                        }))
                        .concat([
                          {
                            key: raceEvents.length,
                            value: raceEvents.length,
                            text: "Add New",
                          },
                        ])}
                    />
                  </Form.Group>
                  {values.selectedEventIndex !== -1 && (
                    <>
                      <Form.Group widths="equal">
                        <Form.Input
                          name="location"
                          disabled
                          label="Race Location"
                          value={
                            raceEvents[values.selectedEventIndex][0].location
                          }
                        />

                        <Form.Input
                          name="date"
                          disabled
                          label="Race Date"
                          value={raceEvents[
                            values.selectedEventIndex
                          ][0].date?.toLocaleDateString()}
                        />
                      </Form.Group>
                      <Form.Group widths="equal">
                        <SemanticSelectField
                          label="Distance"
                          name="selectedRaceIndex"
                          value={values.selectedRaceIndex}
                          onChange={(
                            _: React.SyntheticEvent<HTMLElement>,
                            data: DropdownProps
                          ) => {
                            if (values.selectedEventIndex === -1) {
                              // TODO: throw error?
                              return;
                            }

                            if (
                              data.value ===
                              raceEvents[values.selectedEventIndex].length
                            ) {
                              setAddDistanceModalOpen(true);
                            } else {
                              setValues({
                                ...values,
                                selectedRaceIndex: data.value as number,
                              });
                            }
                          }}
                          options={raceEvents[values.selectedEventIndex]
                            .map((race: Partial<Race>, index: number) => ({
                              key: index,
                              value: index,
                              text: race.distance,
                            }))
                            .concat([
                              {
                                key:
                                  raceEvents[values.selectedEventIndex].length,
                                value:
                                  raceEvents[values.selectedEventIndex].length,
                                text: "Add New",
                              },
                            ])}
                        />
                      </Form.Group>
                      <Form.Group widths="equal">
                        <SemanticTextInputField
                          name="time"
                          label="Finish Time"
                          placeholder="2:45:13.24"
                        />
                      </Form.Group>
                      <Form.Group widths="equal">
                        <SemanticTextAreaField
                          name="comments"
                          label="Comments (optional)"
                          rows="5"
                          placeholder="This is a personal record!"
                        />
                      </Form.Group>
                    </>
                  )}
                  <Form.Button>Submit</Form.Button>
                </Form>
              </div>
            );
          }}
        </Formik>
      </BasePage>
    </LoadingOrError>
  );
};

const CreateRaceResultPage = () => {
  const { id: orgId } = useParams();
  const accountId = useAppSelector((state) => state.wildApricotAuth.accountId);
  const userResponse = useFetchUserInfoQuery(accountId);

  const organization = useFetchOrganizationQuery(orgId || "");

  return (
    <LoadingOrError
      isLoading={userResponse.isLoading || organization.isLoading}
      hasError={userResponse.isError || organization.isError}
    >
      <RequireWildApricotLogin organization={organization.data as Organization}>
        <CreateRaceResultPageForm
          user={userResponse.data as UserInfo}
          organization={organization.data as Organization}
        />
      </RequireWildApricotLogin>
    </LoadingOrError>
  );
};

export default CreateRaceResultPage;
