import React, { FC } from "react";
import { Container, Segment, SemanticTEXTALIGNMENTS } from "semantic-ui-react";

export const BasePage: FC<{
  textAlign?: SemanticTEXTALIGNMENTS;
  fluid?: boolean;
}> = ({ textAlign, fluid, children }) => (
  <Segment
    vertical
    style={{
      marginLeft: fluid ? "1rem" : "0px",
      marginRight: fluid ? "1rem" : "0px",
    }}
  >
    <Container fluid={fluid} textAlign={textAlign}>
      {children}
    </Container>
  </Segment>
);

export default BasePage;
