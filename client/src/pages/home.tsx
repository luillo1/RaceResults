import { faRunning } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import React from "react";
import {
  Segment,
  Dimmer,
  Loader,
  Placeholder,
  Container,
  Header,
  Icon,
  Grid,
  GridRow,
  GridColumn
} from "semantic-ui-react";

function Home() {
  return (
    <>
      <Segment inverted textAlign="center" vertical padded="very">
        <Container fluid>
          <div>
            <Header as="h1" inverted icon textAlign="center">
              <Icon>
                <FontAwesomeIcon icon={faRunning} color="white" />
              </Icon>
              <Header.Content>Welcome to RaceResults</Header.Content>
            </Header>
          </div>
        </Container>
      </Segment>
      <Segment vertical>
        <Grid verticalAlign="middle" stackable container>
          <GridRow>
            <GridColumn textAlign="center">
              <Header as="h2">This is our landing page</Header>
              <p>We can put stuff here.</p>
            </GridColumn>
          </GridRow>
        </Grid>
      </Segment>
    </>
  );
}

export default Home;
