import React, { ChangeEvent, useState } from "react";
import { useNavigate } from "react-router";
import {
  Button,
  Container,
  Divider,
  Form,
  Header,
  Message,
  Segment
} from "semantic-ui-react";
import { useCreateOrganizationMutation } from "../../slices/runners/runners-api-slice";
import BasePage from "../../utils/basePage";

const CreateOrganizationPage = () => {
  const [
    createOrganization, // This is the mutation trigger
    { isLoading: isUpdating } // This is the destructured mutation result
  ] = useCreateOrganizationMutation();

  const navigate = useNavigate();

  const [name, setName] = useState("");

  const [error, setError] = useState(false);

  const updateName = (event: ChangeEvent<HTMLInputElement>) => {
    setError(false);
    setName(event.target.value);
  };

  const submitForm = () => {
    setError(false);
    createOrganization({ name: name })
      .unwrap()
      .then((createdOrg) => navigate("/organizations/" + createdOrg.id))
      .catch(() => setError(true));
  };

  return (
    <BasePage>
      <Header as="h2" content="Create Organization" />
      <Divider />
      {error && (
        <Message negative>
          <Message.Header>
            There was a problem creating your organization.
          </Message.Header>
        </Message>
      )}
      <Form loading={isUpdating} onSubmit={submitForm}>
        <Form.Input
          placeholder="Name"
          name="name"
          value={name}
          onChange={updateName}
        />
        <Button type="submit">Submit</Button>
      </Form>
    </BasePage>
  );
};

export default CreateOrganizationPage;
