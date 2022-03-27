import React, { FC } from "react";
import BasePage from "./basePage";
import { Loader } from "semantic-ui-react";
import NotFound from "../pages/notFound";
import UnexpectedError from "../pages/UnexpectedError";

type RenderProps<T> = {
  data: T;
};

interface LoadingOrErrorProps<T> {
  isLoading: boolean;
  isError: boolean;
  data?: T;
}

export function LoadingOrError2<T>(props: {
  query: LoadingOrErrorProps<T>;
  children: (props: RenderProps<T>) => React.ReactNode;
}) {
  if (props.query.isError) {
    return <NotFound />;
  }

  if (props.query.isLoading) {
    return (
      <BasePage>
        <Loader active inline="centered" content="Loading..." />
      </BasePage>
    );
  }

  if (props.query.data === undefined) {
    return <UnexpectedError />;
  }

  return <>{props.children({ data: props.query.data })}</>;
}

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
