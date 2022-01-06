import React, { useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { useAppDispatch } from "../redux/hooks";
import { login } from "../slices/wild-apricot-auth/wild-apricot-auth-slice";
import LoginLoading from "./loginLoading";
import routes from "../utils/routes";
import { usePostAccessCodeMutation } from "../slices/wild-apricot/wild-apricot-api-slice";
import { useFetchOrganizationQuery } from "../slices/runners/raceresults-standard-api-slice";
import NotFound from "./notFound";

const OAuthLogin = () => {
  const [params] = useSearchParams();
  const token = params.get("code") as string;
  const orgId = params.get("state") as string;

  const [postAccessCode] = usePostAccessCodeMutation();

  const navigate = useNavigate();

  const dispatch = useAppDispatch();

  const organization = useFetchOrganizationQuery(orgId || "");

  useEffect(() => {
    if (organization.data !== undefined) {
      postAccessCode({
        authCode: token,
        scope: "auto",
        organization: organization.data,
      })
        .unwrap()
        .then((resp) => {
          dispatch(login(resp));
          navigate(routes.submitRaceResult.createPath(orgId));
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

export default OAuthLogin;
