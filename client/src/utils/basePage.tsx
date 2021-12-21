import React, { FC } from "react";
import { Container, Segment, SemanticTEXTALIGNMENTS } from "semantic-ui-react";

export const BasePage: FC<{
  textAlign?: SemanticTEXTALIGNMENTS;
}> = ({ textAlign, children }) => (
  <Segment vertical>
    <Container textAlign={textAlign}>{children}</Container>
  </Segment>
);

export default BasePage;
