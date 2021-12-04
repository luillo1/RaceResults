import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { loginRequest } from "../../authConfig";
import { msalInstance } from "../../utils/mcalInstance";

interface Runner {
  organizationId: string;
  firstName: string;
  lastName: string;
  nicknames: string[];
}

interface Organization {
  id: string;
  name: string;
}

export const runnersApiSlice = createApi({
  reducerPath: "runnersApi",
  baseQuery: fetchBaseQuery({
    baseUrl: import.meta.env.VITE_API_URL,
    prepareHeaders: async(headers) => {
      //
      // See if we're logged in. If we are, attach the bearer
      // token to this request.
      //
      const activeAccount = msalInstance.getActiveAccount();
      const accounts = msalInstance.getAllAccounts();
      if (activeAccount || accounts.length > 0) {
        const request = {
          ...loginRequest,
          account: activeAccount || accounts[0]
        };

        const response = await msalInstance.acquireTokenSilent(request);
        headers.set("Authorization", "Bearer " + response.accessToken);
      }

      return headers;
    }
  }),
  endpoints(builder) {
    return {
      fetchRunner: builder.query<Runner[], void>({
        query() {
          return "/runners";
        }
      }),
      fetchOrganization: builder.query<Organization, string>({
        query(id) {
          return "/organizations/" + id;
        }
      }),
      fetchOrganizations: builder.query<Organization[], void>({
        query() {
          return "/organizations";
        }
      }),
      createOrganization: builder.mutation<Organization, Partial<Organization>>({
        query: (post) => ({
          url: "/organizations",
          method: "POST",
          body: post
        })
      })
    };
  }
});

export const { useFetchRunnerQuery, useFetchOrganizationQuery, useFetchOrganizationsQuery, useCreateOrganizationMutation } = runnersApiSlice;
