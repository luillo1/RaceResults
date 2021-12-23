import React, { FC } from "react";
import { Container, Segment, SemanticTEXTALIGNMENTS } from "semantic-ui-react";

export const BasePage: FC<{
  textAlign?: SemanticTEXTALIGNMENTS;
  fluid?: boolean
}> = ({ textAlign, fluid, children }) => (
  <Segment vertical>
    <Container fluid={fluid} textAlign={textAlign}>{children}</Container>
  </Segment>
);

export default BasePage;
