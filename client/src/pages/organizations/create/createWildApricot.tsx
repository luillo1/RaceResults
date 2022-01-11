import { Formik } from "formik";
import React, { useState } from "react";
import { useNavigate } from "react-router";
import { Button, Divider, Form, Header, Message } from "semantic-ui-react";
import {
  AuthType,
  useCreateOrganizationMutation,
  useCreateWildApricotAuthMutation,
} from "../../../slices/runners/raceresults-api-slice";
import BasePage from "../../../utils/basePage";
import routes from "../../../utils/routes";
import * as Yup from "yup";
import { SemanticTextInputField } from "../../../components/SemanticFields/SemanticTextInputField";

const CreateWildApricotOrganizationPage = () => {
  const [createOrganization] = useCreateOrganizationMutation();
  const [createWildApricotAuth] = useCreateWildApricotAuthMutation();

  const navigate = useNavigate();

  const [error, setError] = useState(false);

  return (
    <BasePage>
      <Header as="h2" content="Create Organization - Wild Apricot" />
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

          let createdOrgId: string | undefined;
          await createOrganization({
            name: values.name,
            authType: AuthType.WildApricot,
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

          await createWildApricotAuth({
            orgId: createdOrgId,
            body: {
              clientSecret: values.clientSecret,
              auth: {
                organizationId: createdOrgId,
                clientId: values.clientId,
                domain: values.domain,
              },
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

export default CreateWildApricotOrganizationPage;
