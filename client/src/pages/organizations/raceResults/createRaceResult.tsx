import React, { ChangeEvent, useEffect, useState } from "react";
import { useNavigate } from "react-router";
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
  useCreateRaceMutation,
  useFetchRacesQuery,
} from "../../../slices/runners/raceresults-api-slice";
import BasePage from "../../../utils/basePage";
import routes from "../../../utils/routes";
import SemanticDatepicker from "react-semantic-ui-datepickers";
import "react-semantic-ui-datepickers/dist/react-semantic-ui-datepickers.css";
import CreateRaceModal from "../../../components/createRaceModal";
import { Formik } from "formik";
import { SemanticTextInputField } from "../../../components/SemanticFields/SemanticTextInputField";
import { LoadingOrError } from "../../../utils/loadingOrError";
import { IRace } from "../../../common";
import { SemanticSelectField } from "../../../components/SemanticFields/SemanticSelectField";
import { SemanticTextAreaField } from "../../../components/SemanticFields/SemanticTextAreaField";

/**
 * Groups the given race models by their eventId.
 * @param races the race models to group
 * @returns an 2-dimensional array where each entry is a list of race models who all
 * share the same eventId.
 */
function groupByEventId(races: IRace[]): IRace[][] {
  if (races.length === 0) {
    return [];
  }

  //
  // TODO: for now, group by name since we don't have eventId available
  //

  const racesByEventId: Map<string, IRace[]> = new Map<string, IRace[]>();

  races.forEach((item) => {
    if (!item.name) {
      return;
    }

    if (!racesByEventId.has(item.name)) {
      racesByEventId.set(item.name, []);
    }

    racesByEventId.get(item.name)?.push(item);
  });

  return [...racesByEventId.values()];
}

interface stateTypes {
  error: boolean;
  addRaceModalOpen: boolean;
  addDistanceModalOpen: boolean;
  raceEvents: Partial<IRace>[][];
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
  const [state, setState]: [stateTypes, any] = useState({
    error: false,
    addRaceModalOpen: false,
    addDistanceModalOpen: false,
    raceEvents: [],
  });

  //
  // Fetch all current races to display on the form
  // and update the state with them. We need to convert
  // the ISO strings to Date objects.
  //
  const racesResponse = useFetchRacesQuery();

  useEffect(() => {
    if (racesResponse.data !== undefined) {
      const races = racesResponse.data.map((resp) => {
        return { ...resp, date: new Date(Date.parse(resp.date)) };
      });
      setState({ ...state, raceEvents: groupByEventId(races) });
    }
  }, [racesResponse.data]);

  const [createRace] = useCreateRaceMutation();

  //
  //  Get search params for the given memberId, firstName, and lastName
  //
  const [searchParams] = useSearchParams();

  const orgAssignedMemberId = searchParams.get("memberId");
  const firstName = searchParams.get("firstName");
  const lastName = searchParams.get("lastName");

  const setAddRaceModalOpen = (isOpen: boolean) => {
    setState({ ...state, addRaceModalOpen: isOpen });
  };

  const setAddDistanceModalOpen = (isOpen: boolean) => {
    setState({ ...state, addDistanceModalOpen: isOpen });
  };

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
      isLoading={racesResponse.isLoading}
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
                RegExp("^(\\d?\\d:)?\\d\\d:\\d\\d(\\.\\d*)?$"),
                "Time must be in either hh:mm:ss or mm:ss format."
              ),
            comments: Yup.string().notRequired(),
          })}
          onSubmit={async (values, helpers) => {
            helpers.setSubmitting(true);

            if (
              values.selectedEventIndex === undefined ||
              values.selectedRaceIndex === undefined
            ) {
              setState({ ...state, error: true });
              return;
            }

            const race =
              state.raceEvents[values.selectedEventIndex][
                values.selectedRaceIndex
              ];

            if (race.id === null) {
              // This is a new race we have to first create
              await createRace({
                name: race.name,
                date: race.date?.toISOString(),
                distance: race.distance,
                location: race.location,
              });
            }

            helpers.setSubmitting(false);
          }}
        >
          {({
            values,
            touched,
            errors,
            isSubmitting,
            setSubmitting,
            setFieldValue,
            handleSubmit,
          }) => {
            return (
              <div>
                <CreateRaceModal
                  header="Create New Race"
                  open={state.addRaceModalOpen}
                  handleClose={() => {
                    setAddRaceModalOpen(false);
                  }}
                  distanceOnly={false}
                  onSubmit={(race: Partial<IRace>) => {
                    //
                    // Add the new race as a new group
                    //
                    const newRaces = state.raceEvents.concat([[race]]);

                    //
                    // Set the state so that the newly created
                    // event + distance is selected
                    //

                    setState({
                      ...state,
                      raceEvents: newRaces,
                      addRaceModalOpen: false,
                    });

                    setFieldValue("selectedEventIndex", newRaces.length - 1);
                    setFieldValue("selectedRaceIndex", 0);
                  }}
                  initialRace={{
                    name: "",
                    distance: "",
                    date: null,
                    id: "",
                    //eventId: "",
                    location: "",
                  }}
                />
                {values.selectedEventIndex !== undefined && (
                  <CreateRaceModal
                    header="Add Distance"
                    open={state.addDistanceModalOpen}
                    handleClose={() => setAddDistanceModalOpen(false)}
                    distanceOnly={true}
                    onSubmit={(race: Partial<IRace>) => {
                      if (values.selectedEventIndex === undefined) {
                        // TODO: throw error?
                        return;
                      }

                      const newRaces = [...state.raceEvents];
                      newRaces[values.selectedEventIndex].push(race);

                      setState({
                        ...state,
                        raceEvents: newRaces,
                        addDistanceModalOpen: false,
                      });

                      setFieldValue(
                        "selectedRaceIndex",
                        newRaces[values.selectedEventIndex].length - 1
                      );
                    }}
                    initialRace={{
                      ...state.raceEvents[values.selectedEventIndex][0],
                      distance: "",
                    }}
                  />
                )}
                {state.error && (
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
                      onChange={(_: any, data: DropdownProps) => {
                        if (data.value === state.raceEvents.length) {
                          setAddRaceModalOpen(true);
                        } else {
                          setFieldValue(
                            "selectedEventIndex",
                            data.value as number
                          );
                          setFieldValue("selectedRaceIndex", 0);
                        }
                      }}
                      options={state.raceEvents
                        .map((races: Partial<IRace>[], index: number) => ({
                          key: index,
                          value: index,
                          text: races[0].name,
                        }))
                        .concat([
                          {
                            key: state.raceEvents.length,
                            value: state.raceEvents.length,
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
                            state.raceEvents[values.selectedEventIndex][0]
                              .location
                          }
                        />
                        <Form.Input
                          name="date"
                          label="Race Date"
                          disabled
                          clearable={false}
                          placeholder="YYYY-MM-DD"
                          as={SemanticDatepicker}
                          value={
                            state.raceEvents[values.selectedEventIndex][0].date
                          }
                        />
                      </Form.Group>
                      <Form.Group widths="equal">
                        <SemanticSelectField
                          label="Distance"
                          name="selectedRaceIndex"
                          value={values.selectedRaceIndex}
                          onChange={(_, data: DropdownProps) => {
                            if (values.selectedEventIndex === undefined) {
                              // TODO: throw error?
                              return;
                            }

                            if (
                              data.value ===
                              state.raceEvents[values.selectedEventIndex].length
                            ) {
                              setAddDistanceModalOpen(true);
                            } else {
                              setFieldValue(
                                "selectedRaceIndex",
                                data.value as number
                              );
                            }
                          }}
                          options={state.raceEvents[values.selectedEventIndex]
                            .map((race: Partial<IRace>, index: number) => ({
                              key: index,
                              value: index,
                              text: race.distance,
                            }))
                            .concat([
                              {
                                key:
                                  state.raceEvents[values.selectedEventIndex]
                                    .length,
                                value:
                                  state.raceEvents[values.selectedEventIndex]
                                    .length,
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
