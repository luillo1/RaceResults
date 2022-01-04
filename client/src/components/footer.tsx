import { Grid, Header, List, Container} from "semantic-ui-react";
import "../App.css";

function Footer() {

  return (
      <Grid rows={2}>
        <Grid.Row color="black" inverted>
          <Grid.Column>
            <Container flex className="footer-row">
              <Header color="black" inverted>Useful Links</Header>
              <List>
                <List.Item>
                  </List.Item><a href="/">Meet the Team</a>
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
            </Container>
          </Grid.Column>
        </Grid.Row>
        <Grid.Row color="black" inverted>
          <Grid.Column>
            <Container text flex className="footer-row">
              &copy; RaceResults 2022. All Rights Reserved.
            </Container>
          </Grid.Column>
        </Grid.Row>
      </Grid>
  );
}

export default Footer;