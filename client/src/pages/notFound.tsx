import React from "react";
import { Container, Header, Icon, Segment } from "semantic-ui-react";

function NotFound() {
  return (
    <Segment vertical>
      <Container textAlign="center">
        <Header icon>
          <Icon name="exclamation triangle" />
          Page Not Found
        </Header>
        <Segment.Inline>
          The requested page could not be found 🥺
        </Segment.Inline>
      </Container>
    </Segment>
  );
}

export default NotFound;
