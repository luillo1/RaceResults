import React from "react";
import { Loader } from "semantic-ui-react";
import BasePage from "../utils/basePage";

const LoginLoading = () => (
  <BasePage>
    <Loader active inline="centered" content="Logging in..." />
  </BasePage>
);

export default LoginLoading;
