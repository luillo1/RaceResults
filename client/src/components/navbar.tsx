import React from "react";
import {
  useMsal,
  AuthenticatedTemplate,
  UnauthenticatedTemplate,
} from "@azure/msal-react";
import { Sticky, Menu, Container, Grid, Icon } from "semantic-ui-react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faRunning } from "@fortawesome/free-solid-svg-icons";
import LogoutButton from "./logoutButton";
import LoginButton from "./loginButton";
import NavLinks from "./navbarlinks";

interface NavbarProps {
  // Used to make the navbar sticky while scrolling the entire document
  appRef: React.MutableRefObject<null>;
  sidebarIsVisible: boolean;
  setSidebarIsVisible: React.Dispatch<React.SetStateAction<boolean>>;
}

function Navbar(props: NavbarProps) {
  const { accounts } = useMsal();

  // A menu item containing the app name + icon
  const brandMenuItem = (
    <Menu.Item>
      <FontAwesomeIcon size="2x" icon={faRunning} color="white" />
      <Menu.Header style={{ paddingLeft: 15 }}>RaceResults</Menu.Header>
    </Menu.Item>
  );

  const normalNavbarMenu = (
    <Menu as="nav" inverted stackable attached>
      <Container fluid>
        {brandMenuItem}
        <NavLinks />
        <AuthenticatedTemplate>
          <Menu.Item className="borderless" position="right">
            {accounts[0] != null && (
              <span>Logged in as {accounts[0].username}&nbsp;&nbsp;</span>
            )}
            <LogoutButton />
          </Menu.Item>
        </AuthenticatedTemplate>
        <UnauthenticatedTemplate>
          <Menu.Item className="borderless" position="right">
            <LoginButton />
          </Menu.Item>
        </UnauthenticatedTemplate>
      </Container>
    </Menu>
  );

  const mobileNavbarMenu = (
    <Menu as="nav" inverted attached borderless>
      <Container fluid>
        {brandMenuItem}
        <Menu.Item position="right">
          <Menu.Header>
            <Icon
              name="bars"
              link
              size="big"
              inverted
              onClick={() => props.setSidebarIsVisible(!props.sidebarIsVisible)}
            />
          </Menu.Header>
        </Menu.Item>
      </Container>
    </Menu>
  );

  return (
    <Grid>
      <Grid.Row columns={1} only="computer tablet">
        <Grid.Column>
          <Sticky context={props.appRef}>{normalNavbarMenu}</Sticky>
        </Grid.Column>
      </Grid.Row>
      <Grid.Row columns={1} only="mobile">
        <Grid.Column>
          <Sticky context={props.appRef}>{mobileNavbarMenu}</Sticky>
        </Grid.Column>
      </Grid.Row>
    </Grid>
  );
}

export default Navbar;
