import React, { useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router";
import { useSearchParams } from "react-router-dom";
import {
  Divider,
  DropdownProps,
  Form,
  Header,
  Message,
} from "semantic-ui-react";
import * as Yup from "yup";
import {
  useCreateMemberMutation,
  useCreatePublicRaceMutation,
  useCreateRaceResultMutation,
  useFetchMemberIdQuery,
  useFetchPublicRacesQuery,
} from "../../../slices/runners/raceresults-api-slice";
import BasePage from "../../../utils/basePage";
import routes from "../../../utils/routes";
import CreateRaceModal from "../../../components/createRaceModal";
import { Formik } from "formik";
import { SemanticTextInputField } from "../../../components/SemanticFields/SemanticTextInputField";
import { LoadingOrError } from "../../../utils/loadingOrError";
import { Race } from "../../../common";
import { SemanticSelectField } from "../../../components/SemanticFields/SemanticSelectField";
import { SemanticTextAreaField } from "../../../components/SemanticFields/SemanticTextAreaField";

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
  selectedEventIndex: number | undefined;
  selectedRaceIndex: number | undefined;
  comments: string;
  time: string;
}

const CreateRaceResultPage = () => {
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

  // Get the organizaiton ID from the URL.
  const { id } = useParams();

  // Get search params for the given org member ID, firstName, and lastName.
  // We require being given an org member ID, but first & last names are optional.
  const [searchParams] = useSearchParams();

  const orgAssignedMemberId = searchParams.get("memberId");
  const firstName = searchParams.get("firstName");
  const lastName = searchParams.get("lastName");

  if (id === undefined || orgAssignedMemberId === null) {
    const navigate = useNavigate();
    navigate(routes.notFound.createPath());
    return <></>;
  }

  //
  // Fetch all current races to display on the form
  // and update the state with them. This use method will
  // result in fetching races from the backend
  //    1. when we initially load the page
  //    2. whenever the "Race" tag is invalidated. This happens
  //       when we POST a new race in the form submission
  //       handler, meaning we automatically refresh the
  //       races after a successful submission.
  //
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

  const memberIdResponse = useFetchMemberIdQuery({
    orgId: id,
    orgAssignedMemberId: orgAssignedMemberId,
  });

  const [createMember] = useCreateMemberMutation();

  const [createRaceResult] = useCreateRaceResultMutation();

  if (success && !error) {
    return (
      <BasePage>
        <Header as="h2" content="Submit Race Result" />
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
    firstName: firstName ?? "",
    lastName: lastName ?? "",
    selectedEventIndex: undefined,
    selectedRaceIndex: undefined,
    comments: "",
    time: "",
  };

  return (
    <LoadingOrError
      isLoading={racesResponse.isLoading || memberIdResponse.isLoading}
      hasError={racesResponse.isError}
    >
      <BasePage>
        <Header as="h2" content="Submit Race Result" />
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
              values.selectedEventIndex === undefined ||
              values.selectedRaceIndex === undefined
            ) {
              setError(true);
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
              return;
            }

            let memberIdToSubmit = "";
            if (memberIdResponse.isError) {
              // We need to make new member
              await createMember({
                orgId: id,
                member: {
                  firstName: values.firstName,
                  lastName: values.lastName,
                  organizationId: id,
                  orgAssignedMemberId: orgAssignedMemberId,
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
              memberIdToSubmit = memberIdResponse.data as string;
            }

            if (error) {
              setError(true);
              return;
            }

            // Fix the time so we have hours if none were specified
            let timeToSubmit = values.time;
            if (timeToSubmit.split(":").length === 2) {
              timeToSubmit = "00:" + timeToSubmit;
            }

            await createRaceResult({
              orgId: id,
              memberId: memberIdToSubmit,
              raceResult: {
                memberId: memberIdToSubmit,
                raceId: raceIdToSubmit,
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
                setError(true);
              });

            if (error) {
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
                {values.selectedEventIndex !== undefined && (
                  <CreateRaceModal
                    header="Add Distance"
                    open={addDistanceModalOpen}
                    handleClose={() => setAddDistanceModalOpen(false)}
                    distanceOnly={true}
                    onSubmit={(race: Partial<Race>) => {
                      if (values.selectedEventIndex === undefined) {
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
                        _: React.SyntheticEvent<HTMLElement>,
                        data: DropdownProps
                      ) => {
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
                  {values.selectedEventIndex !== undefined && (
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
                            if (values.selectedEventIndex === undefined) {
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

export default CreateRaceResultPage;
