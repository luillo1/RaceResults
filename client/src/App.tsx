import React, { useRef } from "react";
import "./App.css";
import Navbar from "./components/navbar";
import { Routes, Route, useLocation } from "react-router-dom";
import {
  AuthenticatedTemplate,
  UnauthenticatedTemplate,
  useMsal,
} from "@azure/msal-react";
import { RequireLogin } from "./utils/RequireLogin";
import routes from "./utils/routes";
import { Menu, Sidebar } from "semantic-ui-react";
import LogoutButton from "./components/logoutButton";
import LoginButton from "./components/loginButton";
import NavLinks from "./components/navbarlinks";

function App() {
  const appRef = useRef(null);

  /**
   * To add a new page/route, do the following:
   *     1. Create a new page component under ./pages (typically)
   *        this component is wrapped in a BasePage
   *     2. Add a new entry to routes in ./utils/routes
   *     3. If you want this page to appear in the navbar, add a new
   *        entry to navbarRoutes in ./utils/routes
   */

  const [sidebarIsVisible, setSidebarIsVisible] = React.useState(false);

  const { accounts } = useMsal();

  const location = useLocation();

  return (
    <div ref={appRef} className="full-height">
      <Sidebar.Pushable style={{ transform: "none" }}>
        <Sidebar
          className="flex-container"
          as={Menu}
          animation="overlay"
          inverted
          vertical
          onHide={() => setSidebarIsVisible(false)}
          visible={sidebarIsVisible}
        >
          <NavLinks onClick={() => setSidebarIsVisible(false)} />
          <div className="bottom-aligned">
            <AuthenticatedTemplate>
              {accounts[0] != null && (
                <Menu.Item>
                  <span>Logged in as {accounts[0].username}&nbsp;&nbsp;</span>
                </Menu.Item>
              )}

              <Menu.Item className="borderless">
                <LogoutButton />
              </Menu.Item>
            </AuthenticatedTemplate>
            <UnauthenticatedTemplate>
              {location.pathname === routes.home.path && (
                <Menu.Item className="borderless">
                  <LoginButton />
                </Menu.Item>
              )}
            </UnauthenticatedTemplate>
          </div>
        </Sidebar>
        <Sidebar.Pusher>
          <Navbar
            appRef={appRef}
            sidebarIsVisible={sidebarIsVisible}
            setSidebarIsVisible={setSidebarIsVisible}
          />
          <Routes>
            {Object.values(routes).map((route, index) => {
              if (route.requiresLogin) {
                return (
                  <Route
                    key={index}
                    path={route.path}
                    element={<RequireLogin>{route.element}</RequireLogin>}
                  />
                );
              } else {
                return (
                  <Route
                    key={index}
                    path={route.path}
                    element={route.element}
                  />
                );
              }
            })}
          </Routes>
        </Sidebar.Pusher>
      </Sidebar.Pushable>
    </div>
  );
}

export default App;
