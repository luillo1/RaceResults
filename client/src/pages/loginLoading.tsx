import React from "react";
import { Container, Loader, Segment } from "semantic-ui-react";

const LoginLoading = () => (
  <Segment vertical>
    <Container>
      <Loader active inline="centered" content="Logging in..." />
    </Container>
  </Segment>
);

export default LoginLoading;
