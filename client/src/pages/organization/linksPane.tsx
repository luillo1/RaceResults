import React from "react";
import { Link } from "react-router-dom";
import routes from "../../utils/routes";

interface LinksPaneProps {
  orgId: string;
}

const LinksPane = (props: LinksPaneProps) => {
  return (
    <>
      <Link to={routes.submitRaceResult.createPath(props.orgId)}>
        Submit time (send to members)
      </Link>
    </>
  );
};

export default LinksPane;
