import { Formik } from "formik";
import React, { useState } from "react";
import { useNavigate } from "react-router";
import { Button, Divider, Form, Header, Message } from "semantic-ui-react";
import {
  AuthType,
  useCreateOrganizationMutation,
  useCreateRaceResultsAuthMutation,
} from "../../../slices/runners/raceresults-api-slice";
import BasePage from "../../../utils/basePage";
import routes from "../../../utils/routes";
import * as Yup from "yup";
import { SemanticTextInputField } from "../../../components/SemanticFields/SemanticTextInputField";

const CreateRaceResultsOrganizationPage = () => {
  const [createOrganization] = useCreateOrganizationMutation();
  const [createRaceResultsAuth] = useCreateRaceResultsAuthMutation();

  const navigate = useNavigate();

  const [error, setError] = useState(false);

  return (
    <BasePage>
      <Header as="h2" content="Create Organization - RaceResults" />
      <Divider />
      <Formik
        initialValues={{ name: "", domain: "", clientId: "", clientSecret: "" }}
        validationSchema={Yup.object({
          name: Yup.string()
            .max(255, "The entered value is too long.")
            .required("This field is required."),
        })}
        onSubmit={async (values, helpers) => {
          setError(false);
          helpers.setSubmitting(true);

          let createdOrgId: string | undefined;
          await createOrganization({
            name: values.name,
            authType: AuthType.RaceResults,
          })
            .unwrap()
            .then((createdOrg) => {
              createdOrgId = createdOrg.id;
            })
            .catch(() => {
              setError(true);
              helpers.setSubmitting(false);
            });

          if (createdOrgId === undefined) {
            return;
          }

          await createRaceResultsAuth({
            orgId: createdOrgId,
            auth: {
              organizationId: createdOrgId,
            },
          })
            .then(() =>
              navigate(routes.organization.createPath(createdOrgId as string))
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
                <Button type="submit">Submit</Button>
              </Form>
            </>
          );
        }}
      </Formik>
    </BasePage>
  );
};

export default CreateRaceResultsOrganizationPage;
