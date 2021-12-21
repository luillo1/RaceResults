import React from "react";
import {
  AuthenticatedTemplate,
  UnauthenticatedTemplate
} from "@azure/msal-react";
import { NavLink } from "react-router-dom";
import { Menu, MenuItemProps } from "semantic-ui-react";
import { navbarRoutes } from "../utils/routes";

interface NavLinkProps {
  onClick?: (
    event: React.MouseEvent<HTMLAnchorElement>,
    data: MenuItemProps
  ) => void;
}

function NavLinks(props: NavLinkProps) {
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
              onClick={props.onClick}
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
