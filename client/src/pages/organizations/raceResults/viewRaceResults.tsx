import React from "react";
import Header from "semantic-ui-react/dist/commonjs/elements/Header";
import { Organization } from "../../../slices/runners/raceresults-api-slice";
import BasePage from "../../../utils/basePage";
import RequireOrganizationLogin from "../../../utils/RequireOrganizationLogin";
import SubmissionsPane from "../../organization/submissionsPane";

const ViewRaceResultPageBase: React.FC<{ organization: Organization }> = ({
  organization,
}) => {
  return <SubmissionsPane orgId={organization.id} />;
};

const ViewRaceResultPage = () => {
  return (
    <RequireOrganizationLogin>
      {({ organization }) => (
        <BasePage>
          <Header as="h2" content={`Submissions - ${organization.name}`} />
          <ViewRaceResultPageBase organization={organization} />
        </BasePage>
      )}
    </RequireOrganizationLogin>
  );
};

export default ViewRaceResultPage;
