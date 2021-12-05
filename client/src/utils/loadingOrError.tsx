import React, { FC } from "react";
import BasePage from "./basePage";
import { Loader } from "semantic-ui-react";
import NotFound from "../pages/notFound";

export const LoadingOrError: FC<{
  isLoading: boolean;
  hasError: boolean;
}> = ({ isLoading, hasError, children }) => {
  if (hasError) {
    return <NotFound />;
  }

  if (isLoading) {
    return (
      <BasePage>
        <Loader active inline="centered" content="Loading..." />
      </BasePage>
    );
  }

  return <>{children}</>;
};
