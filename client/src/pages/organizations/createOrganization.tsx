import React, { ChangeEvent, useState } from "react";
import { useNavigate } from "react-router";
import { Button, Divider, Form, Header, Message } from "semantic-ui-react";
import { useCreateOrganizationMutation } from "../../slices/runners/raceresults-standard-api-slice";
import BasePage from "../../utils/basePage";
import routes from "../../utils/routes";

const CreateOrganizationPage = () => {
  const [
    createOrganization, // This is the mutation trigger
    { isLoading: isUpdating }, // This is the destructured mutation result
  ] = useCreateOrganizationMutation();

  const navigate = useNavigate();

  const [name, setName] = useState("");
  const [domain, setDomain] = useState("");
  const [clientId, setClientId] = useState("");
  const [clientSecret, setClientSecret] = useState("");

  const [error, setError] = useState(false);

  const updateName = (event: ChangeEvent<HTMLInputElement>) => {
    setError(false);
    setName(event.target.value);
  };

  const updateClientId = (event: ChangeEvent<HTMLInputElement>) => {
    setError(false);
    setClientId(event.target.value);
  };

  const updateClientSecret = (event: ChangeEvent<HTMLInputElement>) => {
    setError(false);
    setClientSecret(event.target.value);
  };

  const updateDomain = (event: ChangeEvent<HTMLInputElement>) => {
    setError(false);
    setDomain(event.target.value);
  };

  const submitForm = () => {
    setError(false);
    createOrganization({
      name: name,
      domain: domain,
      clientId: clientId,
      clientSecret: clientSecret,
    })
      .unwrap()
      .then((createdOrg) =>
        navigate(routes.organization.createPath(createdOrg.id))
      )
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
        <Form.Input
          placeholder="raceresults.run"
          name="domain"
          value={domain}
          onChange={updateDomain}
        />
        <Form.Input
          placeholder="CientId"
          name="clientId"
          value={clientId}
          onChange={updateClientId}
        />
        <Form.Input
          placeholder="ClientSecret"
          name="clientSecret"
          value={clientSecret}
          onChange={updateClientSecret}
        />
        <Button type="submit">Submit</Button>
      </Form>
    </BasePage>
  );
};

export default CreateOrganizationPage;
