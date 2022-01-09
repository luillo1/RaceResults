import { Formik } from "formik";
import React, { useState } from "react";
import { useNavigate } from "react-router";
import { Button, Divider, Form, Header, Message } from "semantic-ui-react";
import { useCreateOrganizationMutation } from "../../slices/runners/raceresults-api-slice";
import BasePage from "../../utils/basePage";
import routes from "../../utils/routes";
import * as Yup from "yup";
import { SemanticTextInputField } from "../../components/SemanticFields/SemanticTextInputField";

const CreateOrganizationPage = () => {
  const [createOrganization] = useCreateOrganizationMutation();

  const navigate = useNavigate();

  const [error, setError] = useState(false);

  return (
    <BasePage>
      <Header as="h2" content="Create Organization" />
      <Divider />
      <Formik
        initialValues={{ name: "", domain: "", clientId: "", clientSecret: "" }}
        validationSchema={Yup.object({
          name: Yup.string()
            .max(255, "The entered value is too long.")
            .required("This field is required."),
          domain: Yup.string()
            .url("Not a valid url")
            .matches(/^https:.*$/, "Must start with https")
            .matches(/^.*[^/\\]$/, "Cannot end with slash")
            .max(255, "The entered value is too long.")
            .required("This field is required."),
          clientId: Yup.string()
            .max(255, "The entered value is too long.")
            .required("This field is required."),
          clientSecret: Yup.string()
            .max(255, "The entered value is too long.")
            .required("This field is required."),
        })}
        onSubmit={async (values, helpers) => {
          setError(false);
          helpers.setSubmitting(true);
          await createOrganization({
            organization: {
              name: values.name,
              wildApricotDomain: values.domain,
              wildApricotClientId: values.clientId,
            },
            clientSecret: values.clientSecret,
          })
            .unwrap()
            .then((createdOrg) =>
              navigate(routes.organization.createPath(createdOrg.id))
            )
            .catch(() => {
              setError(true);
              helpers.setSubmitting(false);
            });
        }}
      >
        {({ isSubmitting, handleSubmit }) => {
          return (
            <>
              {error && (
                <Message negative>
                  <Message.Header>
                    There was a problem creating your organization.
                  </Message.Header>
                </Message>
              )}
              <Form onSubmit={handleSubmit} loading={isSubmitting}>
                <SemanticTextInputField
                  name="name"
                  label="Organization Name"
                  placeholder="My Running Club"
                />
                <SemanticTextInputField
                  name="domain"
                  label="Wild Apricot Domain"
                  placeholder="raceresults.run"
                />
                <SemanticTextInputField
                  name="clientId"
                  label="Wild Apricot Client ID"
                />
                <SemanticTextInputField
                  name="clientSecret"
                  label="Wild Apricot Client Secret"
                />
                <Button type="submit">Submit</Button>
              </Form>
            </>
          );
        }}
      </Formik>
    </BasePage>
  );
};

export default CreateOrganizationPage;
