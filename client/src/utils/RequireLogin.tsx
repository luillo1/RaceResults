import React, { FC } from "react";
import { InteractionType } from "@azure/msal-browser";
import { MsalAuthenticationTemplate } from "@azure/msal-react";
import { loginRequest } from "../authConfig";
import LoginError from "../pages/loginError";
import LoginLoading from "../pages/loginLoading";

/*
  Wrap a page/component with this component to ensure the user is logged
  in before the component is rendered.
*/
export const RequireLogin: FC<unknown> = ({ children }) => {
  const authRequest = {
    ...loginRequest
  };

  return (
    <MsalAuthenticationTemplate
      interactionType={InteractionType.Redirect}
      authenticationRequest={authRequest}
      errorComponent={LoginError}
      loadingComponent={LoginLoading}
    >
      {children}
    </MsalAuthenticationTemplate>
  );
};
