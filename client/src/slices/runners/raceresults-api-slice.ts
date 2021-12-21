import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { loginRequest } from "../../authConfig";
import { msalInstance } from "../../utils/mcalInstance";

interface Member {
  id: string;
  organizationId: string;
  orgAssignedMemberId: string
  firstName: string;
  lastName: string;
  nicknames: string[];
}

interface Organization {
  id: string;
  name: string;
}

interface RaceResponse {
  id: string;
  eventId: string;
  name: string;
  date: string;
  distance: string;
  location: string;
}

interface RaceResult {
  id: string;
  memberId: string;
  raceId: string;
  time: string;
  dataSource: string;
  comments: string;
}

export const raceResultsApiSlice = createApi({
  reducerPath: "raceResultsApi",
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
  tagTypes: ["Organization", "Race"],
  endpoints(builder) {
    return {
      fetchOrganization: builder.query<Organization, string>({
        query(id) {
          return `/organizations/${id}`;
        }
      }),
      fetchOrganizations: builder.query<Organization[], void>({
        query() {
          return "/organizations";
        },
        providesTags: ["Organization"]
      }),
      createOrganization: builder.mutation<Organization, Partial<Organization>>(
        {
          query: (post) => ({
            url: "/organizations",
            method: "POST",
            body: post
          }),
          invalidatesTags: ["Organization"]
        }
      ),
      fetchMembers: builder.query<Member[], string>({
        query(orgId) {
          return `/organizations/${orgId}/members`;
        }
      }),
      fetchRaces: builder.query<RaceResponse[], void>({
        query() {
          // TODO: this needs to be org-specific
          return "/races";
        },
        providesTags: ["Race"]
      }),
      createRace: builder.mutation<RaceResponse, Partial<RaceResponse>>({
        query: (race) => ({
          url: "/races",
          method: "POST",
          body: race
        }),
        invalidatesTags: ["Race"]
      }),
      fetchMemberId: builder.query<string, {orgId: string, orgAssignedMemberId: string}>({
        query: ({ orgId, orgAssignedMemberId }) => ({
          url: `/organizations/${orgId}/members/ids/${orgAssignedMemberId}`
        })
      }),
      createMember: builder.mutation<Member, {orgId: string, member: Partial<Member>}>({
        query: ({ orgId, member }) => ({
          url: `/organizations/${orgId}/members`,
          method: "POST",
          body: member
        })
      }),
      createRaceResult: builder.mutation<RaceResult, {orgId: string, memberId: string, raceResult: Partial<RaceResult>}>({
        query: ({ orgId, memberId, raceResult }) => ({
          url: `/organizations/${orgId}/members/${memberId}/raceresults`,
          method: "POST",
          body: raceResult
        })
      })
    };
  }
});

export const {
  useFetchMembersQuery,
  useFetchOrganizationQuery,
  useFetchOrganizationsQuery,
  useCreateOrganizationMutation,
  useFetchRacesQuery,
  useCreateRaceMutation,
  useFetchMemberIdQuery,
  useCreateMemberMutation,
  useCreateRaceResultMutation
} = raceResultsApiSlice;

export type {
  RaceResponse
};
