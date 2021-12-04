import React, { useEffect } from "react";
import { useMsal } from "@azure/msal-react";
import { BrowserUtils } from "@azure/msal-browser";
import { Container, Loader, Segment } from "semantic-ui-react";
import BasePage from "../utils/basePage";

export function Logout() {
  const { instance } = useMsal();

  useEffect(() => {
    instance.handleRedirectPromise().then(() => {
      instance.logoutRedirect({
        account: instance.getActiveAccount(),
        onRedirectNavigate: () => !BrowserUtils.isInIframe()
      });
    });
  }, [instance]);

  return (
    <BasePage>
      <Loader active inline="centered" content="Logging out..." />
    </BasePage>
  );
}
