import React from "react";
import { Container, Header, Icon, Segment } from "semantic-ui-react";
import { MsalAuthenticationResult } from "@azure/msal-react";

const LoginError = ({ error }: MsalAuthenticationResult) => {
  return (
    <Segment vertical>
      <Container textAlign="center">
        <Header icon>
          <Icon name="exclamation triangle" />
          There was an issue logging you in
        </Header>
        {error != null && (
          <Segment.Inline>
            Error code {error.errorCode}: {error.message}
          </Segment.Inline>
        )}
      </Container>
    </Segment>
  );
};

export default LoginError;
