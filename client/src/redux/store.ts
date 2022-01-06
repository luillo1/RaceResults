import { configureStore } from "@reduxjs/toolkit";
import wildApricotAuthSlice from "../slices/wild-apricot-auth/wild-apricot-auth-slice";
import { wildApricotApiSlice } from "../slices/wild-apricot/wild-apricot-api-slice";
import { raceResultsApiSlice } from "../slices/runners/raceresults-standard-api-slice";
import { raceresultsWaApiSlice } from "../slices/runners/raceresults-wa-api-slice";

export const store = configureStore({
  reducer: {
    wildApricotAuth: wildApricotAuthSlice,
    [wildApricotApiSlice.reducerPath]: wildApricotApiSlice.reducer,
    [raceresultsWaApiSlice.reducerPath]: raceresultsWaApiSlice.reducer,
    [raceResultsApiSlice.reducerPath]: raceResultsApiSlice.reducer,
  },
  middleware: (getDefaultMiddleware) => {
    return getDefaultMiddleware()
      .concat(raceResultsApiSlice.middleware)
      .concat(raceresultsWaApiSlice.middleware)
      .concat(wildApricotApiSlice.middleware);
  },
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
