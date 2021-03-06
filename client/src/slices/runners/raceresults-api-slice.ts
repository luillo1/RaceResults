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

interface SubmissionCheckpoint
{
  id: string;
  organizationId: string;
  checkpointed: string;
}

export enum AuthType {
  RaceResults,
  WildApricot,
}

interface Auth {
  id: string;
  organizationId: string;
}

interface WildApricotAuth extends Auth {
  domains: string[];
  clientId: string;
}

// eslint-disable-next-line @typescript-eslint/no-empty-interface
interface RaceResultsAuth extends Auth {
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
  tagTypes: ["Organization", "Race", "RaceResult", "Auth", "SubmissionCheckpoint"],
  endpoints(builder) {
    return {
      // Organizations
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
      createOrganization: builder.mutation<Organization, Partial<Organization>>(
        {
          query: (organization) => ({
            url: "/organizations",
            method: "POST",
            body: organization
          }),
          invalidatesTags: ["Organization"]
        }
      ),

      // Members
      fetchMembers: builder.query<Member[], string>({
        query(orgId) {
          return `/organizations/${orgId}/members`;
        }
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

      // Races
      fetchRaces: builder.query<RaceResponse[], void>({
        query() {
          // TODO (#52): this needs to be org-specific
          return "/races";
        },
        providesTags: ["Race"]
      }),
      fetchPublicRaces: builder.query<RaceResponse[], void>({
        query() {
          // TODO (#52): this needs to be org-specific
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

      // RaceResults
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

      // SubmissionCheckpoints
      fetchSubmissionCheckpoints: builder.query<SubmissionCheckpoint[], {orgId: string}>({
        query({orgId}) {
          return `/organizations/${orgId}/submissionCheckpoints`;
        },
        providesTags: ["SubmissionCheckpoint"]
      }),
      createSubmissionCheckpoint: builder.mutation<SubmissionCheckpoint, {orgId: string, checkpoint: Partial<SubmissionCheckpoint>}>({
        query: ({orgId, checkpoint}) => ({
          url: `/organizations/${orgId}/submissionCheckpoints`,
          method: "POST",
          body: checkpoint
        }),
        invalidatesTags: ["SubmissionCheckpoint"]
      }),

      // Auth
      fetchAuth: builder.query<Auth, string>({
        query(orgId) {
          return `/organizations/${orgId}/auth`;
        },
        providesTags: ["Auth"]
      }),
      createRaceResultsAuth: builder.mutation<RaceResponse, {orgId: string, auth: Partial<RaceResultsAuth>}>({
        query: ({orgId, auth}) => ({
          url: `/organizations/${orgId}/auth/raceresults`,
          method: "POST",
          body: auth
        }),
        invalidatesTags: ["Auth"]
      }),
      createWildApricotAuth: builder.mutation<RaceResponse, {orgId: string, body: {clientSecret: string, auth: Partial<WildApricotAuth>}}>({
        query: ({orgId, body}) => ({
          url: `/organizations/${orgId}/auth/wildapricot`,
          method: "POST",
          body: body
        }),
        invalidatesTags: ["Auth"]
      }),
      loginRaceResults: builder.mutation<OrganizationLoginResponse, {orgId: string}>({
        query: ({ orgId }) => ({
          url: `/organizations/${orgId}/auth/raceresults/login`,
          method: "POST"
        }),
      }),
      loginWildApricot: builder.mutation<OrganizationLoginResponse, {orgId: string, loginRequest: WildApricotLoginRequest}>({
        query: ({ orgId, loginRequest }) => ({
          url: `/organizations/${orgId}/auth/wildapricot/login`,
          method: "POST",
          body: loginRequest
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
  useLazyFetchRaceResultsQuery,
  useDeleteRaceResultMutation,
  useFetchAuthQuery,
  useCreateRaceResultsAuthMutation,
  useCreateWildApricotAuthMutation,
  useLoginRaceResultsMutation,
  useLoginWildApricotMutation,
  useFetchMemberQuery,
  useCreateMemberMutation,
  useCreateRaceResultMutation,
  useFetchSubmissionCheckpointsQuery,
  useCreateSubmissionCheckpointMutation,
} = raceResultsApiSlice;

export type {
  Member,
  Organization,
  RaceResult,
  RaceResponse,
  RaceResultResponse,
  OrganizationLoginResponse,
  Auth,
  WildApricotAuth,
  RaceResultsAuth,
  WildApricotLoginRequest,
  SubmissionCheckpoint,
};
