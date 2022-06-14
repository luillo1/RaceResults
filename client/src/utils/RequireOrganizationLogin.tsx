import { useMsal } from "@azure/msal-react";
import React, { FC, useEffect } from "react";
import { useLocation, useNavigate } from "react-router";
import { loginRequest } from "../authConfig";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { login } from "../slices/auth/organization-auth-slice";
import {
  Auth,
  AuthType,
  Organization,
  useFetchAuthQuery,
  useLoginRaceResultsMutation,
  WildApricotAuth,
} from "../slices/runners/raceresults-api-slice";
import routes from "./routes";
import UnexpectedError from "../pages/UnexpectedError";
import LoginLoading from "../pages/loginLoading";
import RequireValidOrganization from "./RequireValidOrganization";

const LoggingIntoAuth: FC<{ organization: Organization }> = ({
  organization,
}) => {
  const { instance } = useMsal();

  const dispatch = useAppDispatch();

  const [postLogin] = useLoginRaceResultsMutation();

  const location = useLocation();
  const navigate = useNavigate();

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
    const query = new URLSearchParams(location.search);
    const hostname = query.get("hostname");

    if (hostname !== null && !auth.domains.includes(hostname)) {
      navigate(routes.error.createPath());
      return;
    }

    const loginDomain = hostname !== null ? hostname : auth.domains[0];

    const stateEncoded = encodeURIComponent(
      organization.id + " " + location.pathname
    );

    const redirectUriEncoded = encodeURIComponent(
      window.location.origin + routes.wildApricotOAuthLogin.createPath()
    );

    const loginUri = `${loginDomain}/sys/login/OAuthLogin?client_Id=${auth.clientId}&scope=auto&redirect_uri=${redirectUriEncoded}&state=${stateEncoded}`;
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

const RequireOrganizationLogin2: FC<{
  organization: Organization;
  children: (props: RenderProps) => React.ReactNode;
}> = ({ organization, children }) => {
  const currentAuth = useAppSelector(
    (state) => state.organizationAuth.orgAuths
  );

  if (currentAuth[organization.id] === undefined) {
    return <LoggingIntoAuth organization={organization} />;
  } else {
    return (
      <React.Fragment>
        {children({ organization: organization })}
      </React.Fragment>
    );
  }
};

type RenderProps = {
  organization: Organization;
};

const RequireOrganizationLogin: React.FC<{
  children: (props: RenderProps) => React.ReactNode;
}> = ({ children }) => {
  return (
    <RequireValidOrganization>
      {({ organization }) => (
        <RequireOrganizationLogin2 organization={organization}>
          {children}
        </RequireOrganizationLogin2>
      )}
    </RequireValidOrganization>
  );
};

export default RequireOrganizationLogin;
