import React, { useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { useAppDispatch } from "../redux/hooks";
import { login } from "../slices/auth/organization-auth-slice";
import LoginLoading from "./loginLoading";
import routes from "../utils/routes";
import {
  useFetchOrganizationQuery,
  useLoginWildApricotMutation,
  WildApricotLoginRequest,
} from "../slices/runners/raceresults-api-slice";
import NotFound from "./notFound";

const WildApricotOAuthLogin = () => {
  const [params] = useSearchParams();
  const authCode = params.get("code") as string;
  const state = decodeURIComponent(params.get("state") as string);
  const orgId = state.split(" ")[0];
  const redirectUri = state.split(" ")[1];

  const [postLogin] = useLoginWildApricotMutation();

  const navigate = useNavigate();

  const dispatch = useAppDispatch();

  const organization = useFetchOrganizationQuery(orgId || "");

  useEffect(() => {
    if (organization.isSuccess) {
      const request: WildApricotLoginRequest = {
        authorizationCode: authCode,
        redirectUri:
          window.location.origin + routes.wildApricotOAuthLogin.createPath(),
        scope: "auto",
      };

      postLogin({
        orgId: organization.data.id,
        loginRequest: request,
      })
        .unwrap()
        .then((resp) => {
          dispatch(login({ orgId: organization.data.id, response: resp }));
          navigate(redirectUri);
        })
        .catch((reason) => {
          console.log(reason);
          navigate(routes.home.createPath());
        });
    }
  }, [organization.data]);

  if (organization.isError) {
    return <NotFound />;
  } else {
    return <LoginLoading />;
  }
};

export default WildApricotOAuthLogin;
