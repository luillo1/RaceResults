import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { loginRequest } from "../../authConfig";
import { Race } from "../../common";
import { msalInstance } from "../../utils/mcalInstance";

interface Member {
  id: string;
  organizationId: string;
  orgAssignedMemberId: string
  firstName: string;
  lastName: string;
  email: string;
  nicknames: string[];
}

interface Organization {
  id: string;
  name: string;
  wildApricotClientId: string;
  wildApricotDomain: string;
}

interface RaceResponse {
  id: string;
  eventId: string;
  name: string;
  date: string;
  distance: string;
  location: string;
  isPublic: boolean;
  submitted: string;
}

interface RaceResult {
  id: string;
  memberId: string;
  raceId: string;
  time: string;
  dataSource: string;
  comments: string;
  submitted: string;
}

interface RaceResultResponse {
  raceResult: RaceResult;
  race: RaceResponse | null;
  member: Member | null;
}

interface QueryParams {
  [key: string]: string | null | undefined;
}

function constructQueryParams(params: QueryParams){
  const validKeys = Object.keys(params).filter((key) => params[key] !== null && params[key] !== undefined);
  if (validKeys.length === 0) {
    return "";
  }

  const pairs = validKeys.map((key) => key + "=" + params[key]);
  return "?" + pairs.join("&");
}

export const raceResultsApiSlice = createApi({
  reducerPath: "raceResultsApi",
  baseQuery: fetchBaseQuery({
    baseUrl: import.meta.env.VITE_API_URL,
    prepareHeaders: async (headers) => {
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
  tagTypes: ["Organization", "Race", "RaceResult"],
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
      createOrganization: builder.mutation<Organization, {clientSecret: string, organization: Partial<Organization>}>(
        {
          query: (params) => ({
            url: "/organizations",
            method: "POST",
            body: {
              organization: params.organization,
              clientSecret: params.clientSecret
            }
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
      fetchPublicRaces: builder.query<RaceResponse[], void>({
        query() {
          // TODO: this needs to be org-specific
          return "/races/public";
        },
        providesTags: ["Race"]
      }),
      createPublicRace: builder.mutation<RaceResponse, Partial<RaceResponse>>({
        query: (race) => ({
          url: "/races/public",
          method: "POST",
          body: race
        }),
        invalidatesTags: ["Race"]
      }),
      createRace: builder.mutation<RaceResponse, Partial<RaceResponse>>({
        query: (race) => ({
          url: "/races",
          method: "POST",
          body: race
        }),
        invalidatesTags: ["Race"]
      }),
      updateRace: builder.mutation<RaceResponse, Race>({
        query: (race) => ({
          url: "/races",
          method: "PUT",
          body: race
        }),
        invalidatesTags: ["Race"]
      }),
      fetchRaceResults: builder.query<RaceResultResponse[], {orgId: string, startDate: string | null, endDate: string | null}>({
        query({orgId, startDate, endDate}) {
          const url = `/organizations/${orgId}/raceresults`;
          return url + constructQueryParams({startDate, endDate});
        },
        providesTags: ["RaceResult"]
      }),
      deleteRaceResult: builder.mutation<RaceResult, {orgId: string, memberId: string, raceResultId: string}>({
        query: ({ orgId, memberId, raceResultId }) => ({
          url: `/organizations/${orgId}/members/${memberId}/raceresults/${raceResultId}`,
          method: "DELETE"
        }),
        invalidatesTags: ["RaceResult"]
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
  useFetchPublicRacesQuery,
  useCreatePublicRaceMutation,
  useCreateRaceMutation,
  useUpdateRaceMutation,
  useFetchRaceResultsQuery,
  useDeleteRaceResultMutation
} = raceResultsApiSlice;

export type {
  Member,
  Organization,
  RaceResult,
  RaceResponse
};
