import React, { ChangeEvent, useState } from "react";
import { useNavigate } from "react-router";
import { useSearchParams } from "react-router-dom";
import { Button, Divider, Form, Header, Message } from "semantic-ui-react";
import { useCreateOrganizationMutation } from "../../../slices/runners/runners-api-slice";
import BasePage from "../../../utils/basePage";
import routes from "../../../utils/routes";

const tempMemberIds = {
  1: {
    firstName: "Luke",
    lastName: "B"
  },
  2: {
    firstName: "Luis",
    lastName: "T"
  }
};

const tempRaces = {
  boston: {
    name: "Boston Marathon",
    distances: ["Marathon"]
  },
  turkey: {
    name: "Redmond Turkey Trot",
    distances: ["5K", "10K"]
  },
  other: {
    name: "Other",
    distances: []
  }
} as const;

const CreateRaceResultPage = () => {
  const [searchParams] = useSearchParams();

  const id = searchParams.get("memberId");
  const firstName = searchParams.get("firstName");
  const lastName = searchParams.get("lastName");

  const [state, setState] = useState({
    firstName: firstName,
    lastName: lastName,
    race: "",
    distance: ""
  });

  const [error, setError] = useState(false);

  const handleChange = (e) => {
    setState({ ...state, [e.target.name]: e.target.value });
  };

  const handleDropdownChange = (e, data) => {
    setState({ ...state, [data.name]: data.value });
  };

  const submitForm = () => {
    setError(false);
  };

  const options = [
    { key: "m", text: "Male", value: "male" },
    { key: "f", text: "Female", value: "female" },
    { key: "o", text: "Other", value: "other" }
  ];

  return (
    <BasePage>
      <Header as="h2" content="Submit Race Result" />
      <Divider />
      {error && (
        <Message negative>
          <Message.Header>
            There was a problem creating your organization.
          </Message.Header>
        </Message>
      )}
      <Form>
        <Form.Group widths="equal">
          <Form.Input
            fluid
            name="firstName"
            label="First name"
            placeholder="First name"
            value={state.firstName}
            onChange={handleChange}
          />
          <Form.Input
            fluid
            name="lastName"
            label="Last name"
            placeholder="Last name"
            value={state.lastName}
            onChange={handleChange}
          />
        </Form.Group>
        <Form.Group widths="equal">
          <Form.Select
            fluid
            label="Race"
            name="race"
            value={state.race}
            onChange={handleDropdownChange}
            options={Object.keys(tempRaces).map((race) => ({
              key: race,
              value: race,
              text: tempRaces[race].name
            }))}
          />
          {state.race === "other" && (
            <Form.Input fluid name="raceName" placeholder="5K" />
          )}
        </Form.Group>
        {state.race !== "" && (
          <Form.Group widths="equal">
            <Form.Select
              fluid
              label="Distance"
              name="distance"
              value={state.distance}
              onChange={handleDropdownChange}
              options={tempRaces[state.race].distances.map((distance) => ({
                key: distance,
                value: distance,
                text: distance
              }))}
            />
          </Form.Group>
        )}
        <Form.Button>Submit</Form.Button>
      </Form>
    </BasePage>
  );
};

export default CreateRaceResultPage;
