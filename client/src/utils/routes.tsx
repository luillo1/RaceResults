import Home from "../pages/home";
import LoginSuccess from "../pages/loginSuccess";
import { Logout } from "../pages/logout";
import NotFound from "../pages/notFound";
import CreateOrganizationPage from "../pages/organizations/createOrganization";
import OrganizationPage from "../pages/organization";
import OrganizationsPage from "../pages/organizations/organizations";
import CreateRaceResultPage from "../pages/organizations/raceResults/createRaceResult";

export interface RouteWrapper {
  path: string;
  requiresLogin: boolean;
  element: JSX.Element;
  createPath: (...args: string[]) => string;
}

const createParameterizedRouteWrapper = (
  path: string,
  element: JSX.Element,
  requiresLogin: boolean,
  createPath: (...args: string[]) => string
): RouteWrapper => {
  return {
    path: path,
    element: element,
    requiresLogin: requiresLogin,
    createPath: createPath
  };
};

const createRouteWrapper = (
  path: string,
  element: JSX.Element,
  requiresLogin: boolean
): RouteWrapper => {
  return createParameterizedRouteWrapper(
    path,
    element,
    requiresLogin,
    () => path
  );
};

/**
 * All of the routes in our application.
 */
const routes = {
  notFound: createRouteWrapper("*", <NotFound />, false),

  home: createRouteWrapper("/", <Home />, false),

  organizations: createRouteWrapper(
    "/organizations",
    <OrganizationsPage />,
    true
  ),

  organization: createParameterizedRouteWrapper(
    "/organizations/:id",
    <OrganizationPage />,
    true,
    (orgId: string) => `/organizations/${orgId}`
  ),

  createOrganization: createRouteWrapper(
    "/organizations/new",
    <CreateOrganizationPage />,
    true
  ),

  submitRaceResult: createRouteWrapper(
    "/organizations/:id/raceresults/create",
    <CreateRaceResultPage />,
    false
  ),

  loginSuccess: createRouteWrapper(
    "/auth/loginSuccess",
    <LoginSuccess />,
    false
  ),

  logout: createRouteWrapper("logout", <Logout />, false)
} as const;

/**
 * All of the routes we want navbar elements for.
 */
const navbarRoutes: {
  header: string;
  route: RouteWrapper;
  requiresLogin: boolean;
}[] = [
  { header: "Home", route: routes.home, requiresLogin: false },
  { header: "Organizations", route: routes.organizations, requiresLogin: true }
];

export { navbarRoutes };
export default routes;
