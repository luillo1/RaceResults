import React from "react";
import { useParams } from "react-router";
import { Divider, Header, Tab } from "semantic-ui-react";
import {
  useFetchOrganizationQuery
} from "../../slices/runners/raceresults-api-slice";
import BasePage from "../../utils/basePage";
import { LoadingOrError } from "../../utils/loadingOrError";
import NotFound from "../notFound";
import MembersPane from "./membersPane";
import SubmissionsPane from "./submissionsPane";

const OrganizationPage = () => {
  const { id } = useParams();
  if (id == null) {
    return <NotFound />;
  } else {
    const orgResponse = useFetchOrganizationQuery(id);

    const panes = [
      { menuItem: "Submissions", render: () => <Tab.Pane><SubmissionsPane orgId={id} /></Tab.Pane> },
      { menuItem: "Members", render: () => <Tab.Pane><MembersPane orgId={id} /></Tab.Pane> }
    ];

    return (
      <LoadingOrError
        isLoading={orgResponse.isLoading}
        hasError={orgResponse.isError}
      >
        <BasePage fluid>
          <Header as="h2" content={orgResponse.data?.name} />
          <Divider />
          <Tab panes={panes} />
        </BasePage>
      </LoadingOrError>
    );
  }
};

export default OrganizationPage;
