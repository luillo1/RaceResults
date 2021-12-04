import React from "react";
import {
  useMsal,
  AuthenticatedTemplate,
  UnauthenticatedTemplate
} from "@azure/msal-react";
import { NavLink } from "react-router-dom";
import { Sticky, Menu, Container, Button } from "semantic-ui-react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faRunning } from "@fortawesome/free-solid-svg-icons";
import { loginRequest } from "../authConfig";

interface NavbarProps {
  // Used to make the navbar sticky while scrolling the entire document
  appRef: React.MutableRefObject<null>;
}

function Navbar(props: NavbarProps) {
  const { instance, accounts } = useMsal();

  const handleLogin = () => {
    instance.loginRedirect(loginRequest);
  };

  const handleLogout = () => {
    instance.logoutRedirect();
  };

  return (
    <Sticky context={props.appRef}>
      <Menu as="nav" inverted stackable attached>
        <Container fluid>
          <Menu.Item>
            <FontAwesomeIcon size="2x" icon={faRunning} color="white" />
            <Menu.Header style={{ paddingLeft: 15 }}>RaceResults</Menu.Header>
          </Menu.Item>
          <Menu.Item as={NavLink} to="/" name="home" />
          <Menu.Item as={NavLink} to="/organizations" name="organizations" />
          <Menu.Item as={NavLink} to="/runners" name="runners" />
          <AuthenticatedTemplate>
            <Menu.Item className="borderless" position="right">
              {accounts[0] != null && (
                <span>Logged in as {accounts[0].username}&nbsp;&nbsp;</span>
              )}
              <Button
                inverted
                onClick={() => handleLogout()}
                content="Log out"
              />
            </Menu.Item>
          </AuthenticatedTemplate>
          <UnauthenticatedTemplate>
            <Menu.Item className="borderless" position="right">
              <Button inverted onClick={() => handleLogin()} content="Log in" />
            </Menu.Item>
          </UnauthenticatedTemplate>
        </Container>
      </Menu>
    </Sticky>
  );
}

export default Navbar;
