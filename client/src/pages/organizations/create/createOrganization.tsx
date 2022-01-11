import React from "react";
import {
  Divider,
  Header,
  List,
  ListItem,
  Image,
  ListContent,
  ListIcon,
} from "semantic-ui-react";
import BasePage from "../../../utils/basePage";
import routes from "../../../utils/routes";
import { NavLink } from "react-router-dom";

const CreateOrganizationPage = () => {
  return (
    <BasePage>
      <Header as="h2" content="Create Organization" />
      <Divider />
      <List divided relaxed selection verticalAlign="middle" size="large">
        <ListItem
          as={NavLink}
          to={routes.createRaceResultsOrganization.createPath()}
          header={"RaceResults Authentication"}
        >
          <ListIcon name="user" size="large" verticalAlign="middle" />
          <ListContent>
            <List.Header>RaceResults Authentication</List.Header>
            <List.Description as="a">
              Require members to be authenticated against a @raceresults.run
              account.
            </List.Description>
          </ListContent>
        </ListItem>
        <ListItem
          as={NavLink}
          to={routes.createWildApricotOrganization.createPath()}
          header={"WildApricot Authentication"}
        >
          <ListIcon name="user" size="large" verticalAlign="middle" />
          <ListContent>
            <List.Header>WildApricot Authentication</List.Header>
            <List.Description as="a">
              Require members to be authenticated against an organization backed
              by WildApricot. Requires a WildApricot organization with SSO
              enabled along with a clientId and clientSecret.
            </List.Description>
          </ListContent>
        </ListItem>
      </List>
    </BasePage>
  );
};

export default CreateOrganizationPage;
