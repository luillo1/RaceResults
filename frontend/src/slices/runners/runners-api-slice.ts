import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

const API_KEY = "TODO";

interface Runner {
  organizationId: string;
  firstName: string;
  lastName: string;
  nicknames: string[];
}

export const runnersApiSlice = createApi({
  reducerPath: "runnersApi",
  baseQuery: fetchBaseQuery({
    prepareHeaders(headers) {
      headers.set("x-api-key", API_KEY);

      return headers;
    }
  }),
  endpoints(builder) {
    return {
      fetchRunner: builder.query<Runner[], void>({
        query() {
          return "/api/runners";
        }
      })
    };
  }
});

export const { useFetchRunnerQuery } = runnersApiSlice;
