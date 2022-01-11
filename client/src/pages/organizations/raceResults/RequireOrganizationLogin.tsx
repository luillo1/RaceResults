import { useMsal } from "@azure/msal-react";
import React, { FC, useEffect } from "react";
import { useLocation } from "react-router";
import { loginRequest } from "../../../authConfig";
import { useAppDispatch, useAppSelector } from "../../../redux/hooks";
import { login } from "../../../slices/auth/organization-auth-slice";
import {
  Auth,
  AuthType,
  Organization,
  useFetchAuthQuery,
  useLoginRaceResultsMutation,
  WildApricotAuth,
} from "../../../slices/runners/raceresults-api-slice";
import routes from "../../../utils/routes";
import UnexpectedError from "../../error";
import LoginLoading from "../../loginLoading";

const LoggingIntoAuth: FC<{ organization: Organization }> = ({
  organization,
}) => {
  const { instance } = useMsal();

  const dispatch = useAppDispatch();

  const [postLogin] = useLoginRaceResultsMutation();

  const location = useLocation();

  const startRaceResultsLogin = async () => {
    await instance.loginPopup(loginRequest);

    // User is logged in. No need to redirect
    await postLogin({ orgId: organization.id })
      .unwrap()
      .then((loginResponse) => {
        dispatch(login({ orgId: organization.id, response: loginResponse }));
      });
  };

  const startWildApricotLogin = (auth: WildApricotAuth) => {
    const stateEncoded = encodeURIComponent(
      organization.id + " " + location.pathname
    );

    const redirectUriEncoded = encodeURIComponent(
      window.location.origin + routes.wildApricotOAuthLogin.createPath()
    );

    const loginUri = `${auth.domain}/sys/login/OAuthLogin?client_Id=${auth.clientId}&scope=auto&redirect_uri=${redirectUriEncoded}&state=${stateEncoded}`;
    window.location.replace(loginUri);
  };

  const startLoginFlow = (organization: Organization, auth: Auth) => {
    switch (organization.authType) {
      case AuthType.RaceResults:
        startRaceResultsLogin();
        break;
      case AuthType.WildApricot:
        startWildApricotLogin(auth as WildApricotAuth);
        break;
    }
  };

  const authResponse = useFetchAuthQuery(organization.id);

  useEffect(() => {
    if (authResponse.isSuccess) {
      const auth = authResponse.data;
      startLoginFlow(organization, auth);
    }
  }, [authResponse.isSuccess]);

  if (authResponse.error) {
    return <UnexpectedError />;
  }

  return <LoginLoading />;
};

const RequireOrganizationLogin: FC<{ organization: Organization }> = ({
  organization,
  children,
}) => {
  const currentAuth = useAppSelector(
    (state) => state.organizationAuth.orgAuths
  );

  if (currentAuth[organization.id] === undefined) {
    return <LoggingIntoAuth organization={organization} />;
  } else {
    return <React.Fragment>{children}</React.Fragment>;
  }
};

export default RequireOrganizationLogin;
