import React from "react";
import { NavLink } from "react-router-dom";
import { Sticky, Menu } from "semantic-ui-react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faRunning } from "@fortawesome/free-solid-svg-icons";

function Navbar(props: NavbarProps) {
  return (
    <Sticky context={props.appRef.current}>
      <Menu as="nav" inverted stackable size="huge" attached>
        <Menu.Item>
          <FontAwesomeIcon size="2x" icon={faRunning} color="white" />
          <Menu.Header style={{ paddingLeft: 15 }}>RaceResults</Menu.Header>
        </Menu.Item>
        <Menu.Item as={NavLink} to="/" name="home" />
        <Menu.Item
          className="borderless"
          position="right"
          as={NavLink}
          to="/login"
          name="login"
        />
      </Menu>
    </Sticky>
  );
}

interface NavbarProps {
  // Used to make the navbar sticky while scrolling the entire document
  appRef: React.MutableRefObject<null>;
}

export default Navbar;
