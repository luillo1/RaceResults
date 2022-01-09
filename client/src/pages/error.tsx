import React from "react";
import { Header, Icon } from "semantic-ui-react";
import BasePage from "../utils/basePage";

const UnexpectedError = () => {
  return (
    <BasePage textAlign="center">
      <Header icon>
        <Icon name="exclamation triangle" />
        There was an unexpected error processing your request.
      </Header>
    </BasePage>
  );
};

export default UnexpectedError;
