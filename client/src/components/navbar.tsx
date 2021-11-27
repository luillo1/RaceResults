import React from "react";
import { NavLink } from "react-router-dom";
import { Sticky, Menu, Container, Button } from "semantic-ui-react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faRunning } from "@fortawesome/free-solid-svg-icons";

function Navbar(props: NavbarProps) {
  return (
    <Sticky context={props.appRef}>
      <Menu as="nav" inverted stackable attached>
        <Container>
          <Menu.Item>
            <FontAwesomeIcon size="2x" icon={faRunning} color="white" />
            <Menu.Header style={{ paddingLeft: 15 }}>RaceResults</Menu.Header>
          </Menu.Item>
          <Menu.Item as={NavLink} to="/" name="home" />
          <Menu.Item className="borderless" position="right">
            <Button inverted as={NavLink} to="/login" content="Log in" />
          </Menu.Item>
        </Container>
      </Menu>
    </Sticky>
  );
}

interface NavbarProps {
  // Used to make the navbar sticky while scrolling the entire document
  appRef: React.MutableRefObject<null>;
}

export default Navbar;
