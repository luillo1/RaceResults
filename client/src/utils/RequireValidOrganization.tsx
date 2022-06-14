import React from "react";
import { useParams } from "react-router";
import {
  Organization,
  useFetchOrganizationQuery,
} from "../slices/runners/raceresults-api-slice";
import { LoadingOrError2 } from "./loadingOrError";

type RenderProps = {
  organization: Organization;
};

/// Ensures there is an organization ID in the current URL
/// and that it can be succesfully fetched.
///
/// If no ID is specified, renders a NotFound page.
///
/// If the organization does not exist, renders an Error page.
///
/// Consumers provide a callback to render the succesfully found
/// organization page.
const RequireValidOrganization: React.FC<{
  children: (props: RenderProps) => React.ReactNode;
}> = ({ children }) => {
  const { id: orgId } = useParams();

  const organization = useFetchOrganizationQuery(orgId || "");

  return (
    <LoadingOrError2 query={organization}>
      {({ data }) => children({ organization: data })}
    </LoadingOrError2>
  );
};

export default RequireValidOrganization;
