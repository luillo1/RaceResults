/* eslint-disable camelcase */
import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { RootState } from "../../redux/store";
import { Organization } from "../runners/raceresults-standard-api-slice";

interface UserInfo {
  id: number;
  url: string;
  firstName: string;
  lastName: string;
  organization: string;
  email: string;
  Phone: string;
  termsOfUseAccepted: boolean;
  HasAvailableUserCard: boolean;
  membershipStateDescription: string;
  isRecurringPaymentsActive: boolean;
}

interface WildAprictotPermission {
  AccountId: number,
  AvailableScopes: string[]
}

interface WildApricotAuthResponse {
  access_token: string,
  expires_in: number,
  refresh_token: string,
  Permissions: WildAprictotPermission[]
}

export const wildApricotApiSlice = createApi({
  reducerPath: "wildApricotApiSlice",
  baseQuery: fetchBaseQuery({
    baseUrl: import.meta.env.VITE_API_URL + "/wa",
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
      postAccessCode: builder.mutation<WildApricotAuthResponse, {organization: Organization, scope: string, authCode: string}>({
        query: (params) => ({
            url: `/oauth/${params.authCode}`,
            method: "POST",
            body: {redirectUri: window.location.origin + "/auth/oauth", scope: params.scope, organizationId: params.organization.id, clientId: params.organization.wildApricotClientId}
          }),
      }),
      fetchUserInfo: builder.query<UserInfo, number>({
        query(accountId) {
          return `currentUser/${accountId}`;
        },
      }),
    };
  },
});

export const { useFetchUserInfoQuery, usePostAccessCodeMutation } = wildApricotApiSlice;

export type {
  UserInfo,
  WildApricotAuthResponse,
};