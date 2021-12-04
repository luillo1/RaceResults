import React, { useRef } from "react";
import "./App.css";
import Navbar from "./components/navbar";
import Runners from "./pages/runners";
import { Routes, Route } from "react-router-dom";
import { PublicClientApplication } from "@azure/msal-browser";
import { MsalProvider } from "@azure/msal-react";
import { Logout } from "./pages/logout";
import { RequireLogin } from "./utils/RequireLogin";
import Home from "./pages/home";
import LoginSuccess from "./pages/loginSuccess";

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
            <Route path="/" element={<Home />} />
            <Route
              path="/runners"
              element={
                <RequireLogin>
                  <Runners />
                </RequireLogin>
              }
            />
            <Route path="/logout" element={<Logout />} />
            <Route path="/auth/loginSuccess" element={<LoginSuccess />} />
          </Routes>
        </div>
      </MsalProvider>
    </div>
  );
}

export default App;
