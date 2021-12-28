import React from "react";
import { faRunning } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  Segment,
  Container,
  Header,
  Icon,
  Grid,
  GridRow,
  GridColumn,
  Image,
} from "semantic-ui-react";

function Home() {
  return (
    <div id="home-page">
      <Segment
        inverted
        textAlign="center"
        vertical
        padded="very"
        id="landing-header"
      >
        <Container fluid>
          <div>
            <Header as="h1" inverted icon textAlign="center">
              {"Welcome to RaceResults"}
            </Header>
            <Header as="h2" inverted textAlign="center">
              {"Automate your club's race time collection."}
            </Header>
          </div>
        </Container>
      </Segment>
      <Segment vertical className="landing-segment">
        <Grid verticalAlign="middle" stackable container>
          <GridRow style={{ display: "flex", alignItems: "stretch" }}>
            <GridColumn width={7}>
              <Header as="h3">Collect race times from members</Header>
              <p>
                Members of your running organization can manually enter times
                for the races you care about.
              </p>
            </GridColumn>
            <GridColumn width={7} floated="right">
              <Image size="large" bordered rounded src="/images/submit.png" />
            </GridColumn>
          </GridRow>
        </Grid>
      </Segment>
      <Segment vertical className="landing-segment">
        <Grid verticalAlign="middle" stackable container>
          <GridRow>
            <GridColumn width={16}>
              <Header as="h3">Manage the races your members ran</Header>
              <p>
                {
                  "When your members submit a race time, they can choose a race from the ones you create. If their race isn't listed, they can create one instead. After you approve their created race, other members can see it too."
                }
              </p>
            </GridColumn>
          </GridRow>
          <hr />
          <GridRow>
            <GridColumn width={7} floated="left">
              <Image
                size="large"
                bordered
                rounded
                src="/images/create_race.png"
              />
            </GridColumn>
            <GridColumn
              width={2}
              only="computer tablet"
              style={{ display: "flex", alignItems: "center" }}
            >
              <Icon name="arrow circle right" size="huge" />
            </GridColumn>
            <GridColumn
              width={2}
              only="mobile"
              style={{ display: "flex", alignItems: "center" }}
            >
              <Icon name="arrow circle down" size="huge" />
            </GridColumn>
            <GridColumn width={7} floated="right">
              <Image size="large" bordered rounded src="/images/races.png" />
            </GridColumn>
          </GridRow>
        </Grid>
      </Segment>
      <Segment vertical className="landing-segment">
        <Grid verticalAlign="middle" stackable container>
          <GridRow>
            <GridColumn width={16}>
              <Header as="h3">
                {
                  "Coming soon: find your members' race times straight from the web"
                }
              </Header>
              <p>
                After a big race, RaceResults will automatically download the
                times that your club members ran. Then, your members can be sent
                a link to verify their time.
              </p>
            </GridColumn>
          </GridRow>
        </Grid>
      </Segment>
    </div>
  );
}

export default Home;
