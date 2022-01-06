import React from "react";
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

import submitImgUrl from "../../images/submit.png";
import createRaceUrl from "../../images/create_race.png";
import racesImgUrl from "../../images/races.png";
import "../styles/home.less";

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
            <Header as="h1" textAlign="center">
              {"Welcome to RaceResults"}
            </Header>
            <Header as="h2" textAlign="center">
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
              <Image size="large" bordered rounded src={submitImgUrl} />
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
              <Image size="large" bordered rounded src={createRaceUrl} />
            </GridColumn>
            <GridColumn
              width={2}
              only="computer tablet"
              style={{ display: "flex", alignItems: "center" }}
            >
              <Icon
                name="arrow circle right"
                className="color-secondary"
                size="huge"
              />
            </GridColumn>
            <GridColumn
              width={2}
              only="mobile"
              style={{ display: "flex", alignItems: "center" }}
            >
              <Icon
                name="arrow circle down"
                className="color-secondary"
                size="huge"
              />
            </GridColumn>
            <GridColumn width={7} floated="right">
              <Image size="large" bordered rounded src={racesImgUrl} />
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
