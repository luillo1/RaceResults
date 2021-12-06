import React from "react";
import {
  AuthenticatedTemplate,
  UnauthenticatedTemplate
} from "@azure/msal-react";
import { NavLink } from "react-router-dom";
import { Menu } from "semantic-ui-react";
import { navbarRoutes } from "../utils/routes";

function NavLinks() {
  return (
    <>
      <AuthenticatedTemplate>
        {navbarRoutes.map((nbroute, index) => {
          return (
            <Menu.Item
              key={index}
              as={NavLink}
              to={nbroute.route.path}
              name={nbroute.header}
            />
          );
        })}
      </AuthenticatedTemplate>
      <UnauthenticatedTemplate>
        {navbarRoutes
          .filter((nbroute) => !nbroute.requiresLogin)
          .map((navbarRoute, index) => {
            return (
              <Menu.Item
                key={index}
                as={NavLink}
                to={navbarRoute.route.path}
                name={navbarRoute.header}
              />
            );
          })}
      </UnauthenticatedTemplate>
    </>
  );
}

export default NavLinks;
