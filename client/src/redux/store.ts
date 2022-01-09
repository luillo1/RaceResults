import { configureStore } from "@reduxjs/toolkit";
import organizationAuthSlice from "../slices/auth/organization-auth-slice";
import { raceResultsApiSlice } from "../slices/runners/raceresults-api-slice";

export const store = configureStore({
  reducer: {
    organizationAuth: organizationAuthSlice,
    [raceResultsApiSlice.reducerPath]: raceResultsApiSlice.reducer,
  },
  middleware: (getDefaultMiddleware) => {
    return getDefaultMiddleware().concat(raceResultsApiSlice.middleware);
  },
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
