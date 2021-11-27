import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";

const API_KEY = "TODO";

interface Runner {
  organizationId: string;
  firstName: string;
  lastName: string;
  nicknames: string[];
}

console.log(import.meta.env);

export const runnersApiSlice = createApi({
  reducerPath: "runnersApi",
  baseQuery: fetchBaseQuery({
    baseUrl: import.meta.env.VITE_API_URL,
    prepareHeaders(headers) {
      headers.set("Access-Control-Allow-Origin", "*");
      return headers;
    }
  }),
  endpoints(builder) {
    return {
      fetchRunner: builder.query<Runner[], void>({
        query() {
          return "/runners";
        }
      })
    };
  }
});

export const { useFetchRunnerQuery } = runnersApiSlice;
