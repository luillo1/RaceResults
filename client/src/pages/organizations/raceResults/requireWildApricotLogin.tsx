import React, { FC } from "react";
import { useAppSelector } from "../../../redux/hooks";
import { Organization } from "../../../slices/runners/raceresults-standard-api-slice";
import LoginLoading from "../../loginLoading";
import UnexpectedError from "../../error";

const RequireWildApricotLogin: FC<{ organization: Organization }> = ({
  organization,
  children,
}) => {
  const loggedInAt = useAppSelector((state) => state.wildApricotAuth.loginTime);
  const expiresIn = useAppSelector((state) => state.wildApricotAuth.expiresIn);
  const accountId = useAppSelector((state) => state.wildApricotAuth.accountId);

  if (
    organization.wildApricotDomain === "" ||
    organization.wildApricotDomain === undefined
  ) {
    return <UnexpectedError />;
  }

  if (
    accountId === -1 ||
    accountId === undefined ||
    Date.now() > loggedInAt + expiresIn * 1000 - 5 * 60 * 1000
  ) {
    const redirect = `${organization.wildApricotDomain}/sys/login/OAuthLogin?client_Id=${organization.wildApricotClientId}&scope=auto&redirect_uri=https%3A%2F%2Flocalhost%3A3000%2Fauth%2Foauth&state=${organization.id}`;
    window.location.replace(redirect);

    return <LoginLoading />;
  } else {
    return <React.Fragment>{children}</React.Fragment>;
  }
};

export default RequireWildApricotLogin;
