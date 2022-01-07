import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { RootState } from "../../redux/store";
import { Member, RaceResult } from "./raceresults-standard-api-slice";

/*
 * This API slice is for endpoints of the RaceResults
 * backend that require the user to be authenticated via
 * Wild-Apricot
 */
export const raceresultsWaApiSlice = createApi({
  reducerPath: "raceresultsWaApiSlice",
  baseQuery: fetchBaseQuery({
    baseUrl: import.meta.env.VITE_API_URL,
    prepareHeaders: async (headers, api) => {
      const state = api.getState() as RootState;
      if (state.wildApricotAuth.accessToken !== "") {
        headers.set("WA-AccountId", state.wildApricotAuth.accountId.toString());
        headers.set(
          "WA-Authorization",
          "Bearer " + state.wildApricotAuth.accessToken
        );
      }

      return headers;
    },
  }),
  endpoints(builder) {
    return {
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
      fetchMember: builder.query<
        Member,
        { orgId: string; orgAssignedMemberId: string }
      >({
        query: ({ orgId, orgAssignedMemberId }) => ({
          url: `/organizations/${orgId}/members/?orgAssignedMemberId=${orgAssignedMemberId}`,
        }),
      }),
      createMember: builder.mutation<
        Member,
        { orgId: string; member: Partial<Member> }
      >({
        query: ({ orgId, member }) => ({
          url: `/organizations/${orgId}/members`,
          method: "POST",
          body: member,
        }),
      }),
    };
  },
});

export const {
  useCreateRaceResultMutation,
  useFetchMemberQuery,
  useCreateMemberMutation,
} = raceresultsWaApiSlice;
