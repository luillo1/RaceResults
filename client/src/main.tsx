import React from "react";
import ReactDOM from "react-dom";
import { Provider } from "react-redux";
import { store } from "./redux/store";
import "./index.css";
import App from "./App";
import { BrowserRouter } from "react-router-dom";
import { AuthenticationResult, EventType } from "@azure/msal-browser";
import { msalInstance } from "./utils/mcalInstance";

// Default to using the first account if no account is active on page load
if (
  !msalInstance.getActiveAccount() &&
  msalInstance.getAllAccounts().length > 0
) {
  // Account selection logic is app dependent. Adjust as needed for different use cases.
  msalInstance.setActiveAccount(msalInstance.getAllAccounts()[0]);
}

// Optional - This will update account state if a user signs in from another tab or window
msalInstance.enableAccountStorageEvents();

msalInstance.addEventCallback((event) => {
  if (
    event != null &&
    event.payload != null &&
    event.eventType === EventType.LOGIN_SUCCESS &&
    (event.payload as AuthenticationResult).account
  ) {
    const account = (event.payload as AuthenticationResult).account;
    msalInstance.setActiveAccount(account);
  }
});

ReactDOM.render(
  <React.StrictMode>
    <Provider store={store}>
      <BrowserRouter>
        <App pca={msalInstance} />
      </BrowserRouter>
    </Provider>
  </React.StrictMode>,
  document.getElementById("root")
);
