import { PublicClientApplication } from "@azure/msal-browser";
import { msalConfig } from "../authConfig";

/*
  This file allows both main.tsx and the redux store to access the same instance
  of the PublicClientApplication. If the instance was created/exported from
  main.tsx, there would be a circular dependency with the redux store.

  The redux store needs access to this instance so it can determine if a user
  is logged in/fetch the user's access token to include in request headers.
*/
export const msalInstance = new PublicClientApplication(msalConfig);
