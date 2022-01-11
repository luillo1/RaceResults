import Home from "../pages/home";
import LoginSuccess from "../pages/loginSuccess";
import { Logout } from "../pages/logout";
import NotFound from "../pages/notFound";
import CreateOrganizationPage from "../pages/organizations/create/createOrganization";
import OrganizationPage from "../pages/organization";
import OrganizationsPage from "../pages/organizations/organizations";
import CreateRaceResultPage from "../pages/organizations/raceResults/createRaceResult";
import WildApricotOAuthLogin from "../pages/wildApricotOAuthLogin";
import CreateWildApricotOrganizationPage from "../pages/organizations/create/createWildApricot";
import CreateRaceResultsOrganizationPage from "../pages/organizations/create/createRaceResults";

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
    createPath: createPath,
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
    "/organizations/create",
    <CreateOrganizationPage />,
    true
  ),

  createRaceResultsOrganization: createRouteWrapper(
    "/organizations/create/raceresults",
    <CreateRaceResultsOrganizationPage />,
    true
  ),

  createWildApricotOrganization: createRouteWrapper(
    "/organizations/create/wildapricot",
    <CreateWildApricotOrganizationPage />,
    true
  ),

  submitRaceResult: createParameterizedRouteWrapper(
    "/organizations/:id/raceresults/create",
    <CreateRaceResultPage />,
    false,
    (orgId: string) => `/organizations/${orgId}/raceresults/create`
  ),

  loginSuccess: createRouteWrapper(
    "/auth/loginSuccess",
    <LoginSuccess />,
    false
  ),

  logout: createRouteWrapper("logout", <Logout />, false),

  wildApricotOAuthLogin: createRouteWrapper(
    "/auth/wildapricot",
    <WildApricotOAuthLogin />,
    false
  ),
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
  { header: "Organizations", route: routes.organizations, requiresLogin: true },
];

export { navbarRoutes };
export default routes;
