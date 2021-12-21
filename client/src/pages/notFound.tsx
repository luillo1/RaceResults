import React from "react";
import { Header, Icon, Segment } from "semantic-ui-react";
import BasePage from "../utils/basePage";

function NotFound() {
  return (
    <BasePage textAlign="center">
      <Header icon>
        <Icon name="exclamation triangle" />
        Page Not Found
      </Header>
      <Segment.Inline textAlign="center">
        The requested page could not be found 🥺
      </Segment.Inline>
    </BasePage>
  );
}

export default NotFound;
