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
import { navbarRoutes } from "../utils/route";

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
          <AuthenticatedTemplate>
            {navbarRoutes.map((nbroute, index) => {
              return (
                <Menu.Item
                  key={index}
                  as={NavLink}
                  to={nbroute.route.path}
                  name={nbroute.header}
                />
              );
            })}
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
            {navbarRoutes
              .filter((nbroute) => !nbroute.requiresLogin)
              .map((navbarRoute, index) => {
                return (
                  <Menu.Item
                    key={index}
                    as={NavLink}
                    to={navbarRoute.route.path}
                    name={navbarRoute.header}
                  />
                );
              })}
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
