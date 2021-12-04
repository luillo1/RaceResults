import React from "react";
import { Header, Icon, Segment } from "semantic-ui-react";
import { MsalAuthenticationResult } from "@azure/msal-react";
import BasePage from "../utils/basePage";

const LoginError = ({ error }: MsalAuthenticationResult) => {
  return (
    <BasePage textAlign="center">
      <Header icon>
        <Icon name="exclamation triangle" />
        There was an issue logging you in
      </Header>
      {error != null && (
        <Segment.Inline>
          Error code {error.errorCode}: {error.message}
        </Segment.Inline>
      )}
    </BasePage>
  );
};

export default LoginError;
