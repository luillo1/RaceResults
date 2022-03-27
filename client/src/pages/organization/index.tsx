import React from "react";
import { useLocation, useNavigate, useParams } from "react-router";
import { useSearchParams } from "react-router-dom";
import { Divider, Header, Tab, TabProps } from "semantic-ui-react";
import {
  Organization,
  useFetchOrganizationQuery,
} from "../../slices/runners/raceresults-api-slice";
import BasePage from "../../utils/basePage";
import { LoadingOrError } from "../../utils/loadingOrError";
import NotFound from "../notFound";
import LinksPane from "./linksPane";
import MembersPane from "./membersPane";
import NewsletterPane from "./newsletterPane";
import RacesPane from "./racesPane";
import SubmissionsPane from "./submissionsPane";

const OrganizationPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const location = useLocation();
  if (id == null) {
    return <NotFound />;
  } else {
    const orgResponse = useFetchOrganizationQuery(id);
    const [params] = useSearchParams();
    const activeIndex = parseInt(params.get("tab") || "0");

    const panes = [
      {
        menuItem: "Submissions",
        render: () => (
          <Tab.Pane>
            <SubmissionsPane orgId={id} />
          </Tab.Pane>
        ),
      },
      {
        menuItem: "Races",
        render: () => (
          <Tab.Pane>
            <RacesPane />
          </Tab.Pane>
        ),
      },
      {
        menuItem: "Members",
        render: () => (
          <Tab.Pane>
            <MembersPane orgId={id} />
          </Tab.Pane>
        ),
      },
      {
        menuItem: "Links",
        render: () => (
          <Tab.Pane>
            <LinksPane orgId={id} />
          </Tab.Pane>
        ),
      },
      {
        menuItem: "Newsletters",
        render: () => (
          <Tab.Pane>
            <NewsletterPane organization={orgResponse.data as Organization} />
          </Tab.Pane>
        ),
      },
    ];

    return (
      <LoadingOrError
        isLoading={orgResponse.isLoading}
        hasError={orgResponse.isError}
      >
        <BasePage fluid>
          <Header as="h2" content={orgResponse.data?.name} />
          <Divider />
          <Tab
            panes={panes}
            activeIndex={activeIndex}
            onTabChange={(
              _: React.MouseEvent<HTMLDivElement, MouseEvent>,
              data: TabProps
            ) => navigate(location.pathname + "?tab=" + data.activeIndex)}
          />
        </BasePage>
      </LoadingOrError>
    );
  }
};

export default OrganizationPage;
