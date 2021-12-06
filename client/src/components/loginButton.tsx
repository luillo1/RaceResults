import React from "react";
import { useMsal } from "@azure/msal-react";
import { Button } from "semantic-ui-react";
import { loginRequest } from "../authConfig";

function LoginButton() {
  const { instance } = useMsal();

  const handleLogin = () => {
    instance.loginRedirect(loginRequest);
  };

  return <Button inverted onClick={() => handleLogin()} content="Log in" />;
}

export default LoginButton;
