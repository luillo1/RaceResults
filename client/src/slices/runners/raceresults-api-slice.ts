import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { loginRequest } from "../../authConfig";
import { Race } from "../../common";
import { RootState } from "../../redux/store";
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
  authType: AuthType;
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

interface OrganizationLoginResponse {
  orgAssignedMemberId: string;
  requiredHeaders: { key: string, value: string }[];
}

interface QueryParams {
  [key: string]: string | null | undefined;
}

// eslint-disable-next-line @typescript-eslint/no-empty-interface
interface LoginRequest {
}

// interface RaceResultsLoginRequest extends LoginRequest {}

interface WildApricotLoginRequest extends LoginRequest {
  authorizationCode: string;
  redirectUri: string;
  scope: string;
}

export enum AuthType {
  RaceResults,
  WildApricot,
}

interface Auth {
  id: string;
  organizationId: string;
  authType: AuthType;
}

interface WildApricotAuth extends Auth {
  domain: string;
  clientId: string;
  authType: AuthType.WildApricot;
}

// eslint-disable-next-line @typescript-eslint/no-empty-interface
interface RaceResultsAuth extends Auth {
  authType: AuthType.RaceResults;
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
    prepareHeaders: async (headers, api) => {
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

      // Attach any headers we need for organization auths.
      // TODO: these MAY conflict (e.g. if there's two WildApricot auths).
      const state = api.getState() as RootState;
      const orgAuths = state.organizationAuth.orgAuths;
      Object.values(orgAuths).forEach((loginResponse) => {
        loginResponse.requiredHeaders.forEach((requiredHeader) => {
          headers.set(requiredHeader.key, requiredHeader.value);
        });
      });

      return headers;
    }
  }),
  tagTypes: ["Organization", "Race", "RaceResult", "Auth"],
  endpoints(builder) {
    return {
      fetchOrganization: builder.query<Organization, string>({
        query(id) {
          return `/organizations/${id}`;
        },
        transformResponse: (response: Organization) => {
          return { ...response, authType: AuthType[response.authType] as unknown as AuthType};
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
      }),
      fetchAuth: builder.query<Auth, string>({
        query(orgId) {
          return `/organizations/${orgId}/auth`;
        },
        providesTags: ["Auth"]
      }),
      createAuth: builder.mutation<RaceResponse, {orgId: string, auth: Partial<Auth>}>({
        query: ({orgId, auth}) => ({
          url: `/organizations/${orgId}/auth`,
          method: "POST",
          body: auth
        }),
        invalidatesTags: ["Auth"]
      }),
      loginRaceResults: builder.mutation<OrganizationLoginResponse, {orgId: string}>({
        query: ({ orgId }) => ({
          url: `/organizations/${orgId}/auth/login/raceresults`,
          method: "POST"
        }),
      }),
      loginWildApricot: builder.mutation<OrganizationLoginResponse, {orgId: string, loginRequest: WildApricotLoginRequest}>({
        query: ({ orgId, loginRequest }) => ({
          url: `/organizations/${orgId}/auth/login/wildapricot`,
          method: "POST",
          body: loginRequest
        }),
      }),
      fetchMember: builder.query<
        Member,
        { orgId: string; orgAssignedMemberId: string }
      >({
        query: ({ orgId, orgAssignedMemberId }) => ({
          url: `/organizations/${orgId}/members/orgAssignedMemberId/${orgAssignedMemberId}`,
        }),
      }),
      createMember: builder.mutation<
        Member,
        { orgId: string; orgAssignedMemberId: string }
      >({
        query: ({ orgId, orgAssignedMemberId }) => ({
          url: `/organizations/${orgId}/members/orgAssignedMemberId/${orgAssignedMemberId}`,
          method: "POST",
        }),
      }),
      createRaceResult: builder.mutation<
        RaceResult,
        {
          orgId: string;
          memberId: string;
          raceResult: Partial<RaceResult>;
        }
      >({
        query: ({ orgId, memberId, raceResult }) => ({
          url: `/organizations/${orgId}/members/${memberId}/raceresults`,
          method: "POST",
          body: raceResult,
        }),
      }),
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
  useDeleteRaceResultMutation,
  useFetchAuthQuery,
  useCreateAuthMutation,
  useLoginRaceResultsMutation,
  useLoginWildApricotMutation,
  useFetchMemberQuery,
  useCreateMemberMutation,
  useCreateRaceResultMutation,
} = raceResultsApiSlice;

export type {
  Member,
  Organization,
  RaceResult,
  RaceResponse,
  OrganizationLoginResponse,
  Auth,
  WildApricotAuth,
  RaceResultsAuth,
  WildApricotLoginRequest,
};
