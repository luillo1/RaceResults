import React, { ChangeEvent, useState } from "react";
import { useNavigate } from "react-router";
import { Button, Form, Header, Message } from "semantic-ui-react";
import MainDivider from "../../components/mainDivider";
import { useCreateOrganizationMutation } from "../../slices/runners/raceresults-api-slice";
import BasePage from "../../utils/basePage";
import routes from "../../utils/routes";

const CreateOrganizationPage = () => {
  const [
    createOrganization, // This is the mutation trigger
    { isLoading: isUpdating }, // This is the destructured mutation result
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
      .then((createdOrg) =>
        navigate(routes.organization.createPath(createdOrg.id))
      )
      .catch(() => setError(true));
  };

  return (
    <BasePage>
      <Header as="h2" content="Create Organization" />
      <MainDivider />
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
