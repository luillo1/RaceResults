import React from "react";
import { useMsal } from "@azure/msal-react";
import { Button } from "semantic-ui-react";

function LogoutButton() {
  const { instance } = useMsal();

  const handleLogout = () => {
    instance.logoutRedirect();
  };

  return <Button inverted onClick={() => handleLogout()} content="Log out" />;
}

export default LogoutButton;
