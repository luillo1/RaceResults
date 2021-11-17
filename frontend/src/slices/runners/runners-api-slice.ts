import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

const API_KEY = "TODO";
const BASE_URL = "TODO";

interface Runner {
  organizationId: string,
  firstName: string,
  lastName: string,
  aliases: string[]
}

export const runnersApiSlice = createApi({
  reducerPath: "runnersApi",
  baseQuery: fetchBaseQuery({
    baseUrl: BASE_URL,
    prepareHeaders(headers) {
      headers.set("x-api-key", API_KEY);

      return headers;
    }
  }),
  endpoints(builder) {
    return {
      fetchRunner: builder.query<Runner[], string>({
        query(id) {
          return `/runners?id=${id}`;
        }
      })
    };
  }
});

export const { useFetchRunnerQuery } = runnersApiSlice;
