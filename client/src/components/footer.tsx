import { Grid, Header, List, Container} from "semantic-ui-react";
import "../App.css";

function Footer() {

  return (
      <Grid>
        <Grid.Row columns={2} color="black" inverted>
          <Grid.Column textAlign="center">
            <Header color="black" inverted>About Us</Header>
            <List>
              <List.Item>
                <a href="/">Meet the Team</a>
              </List.Item>
            </List>
          </Grid.Column>
          <Grid.Column textAlign="center">
            <Header color="black" inverted>Useful Links</Header>
            <List>
              <List.Item>
                <a href="/">Contact Us</a>
              </List.Item>
              <List.Item>
                <a href="/">Terms and Conditions</a>
              </List.Item>
              <List.Item>
                <a href="/">Privacy Policy</a>
              </List.Item>
            </List>
          </Grid.Column>
        </Grid.Row>
        <Grid.Row columns={1} color="black" inverted>
          <Container text textAlign="center">
            &copy; RaceResults 2022. All Rights Reserved.
          </Container>
        </Grid.Row>
      </Grid>
  );
}

export default Footer;