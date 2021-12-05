import React, { useRef } from "react";
import "./App.css";
import Navbar from "./components/navbar";
import { Routes, Route } from "react-router-dom";
import { PublicClientApplication } from "@azure/msal-browser";
import { MsalProvider } from "@azure/msal-react";
import { RequireLogin } from "./utils/RequireLogin";
import routes from "./utils/route";

interface AppProps {
  // Used to make the navbar sticky while scrolling the entire document
  pca: PublicClientApplication;
}

function App({ pca }: AppProps) {
  const appRef = useRef(null);

  return (
    <div ref={appRef}>
      <MsalProvider instance={pca}>
        <Navbar appRef={appRef} />
        <div>
          <Routes>
            {Object.values(routes).map((route, index) => {
              console.log(index);
              if (route.requiresLogin) {
                return (
                  <Route
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
        </div>
      </MsalProvider>
    </div>
  );
}

export default App;
