import { configureStore } from "@reduxjs/toolkit";
import { raceResultsApiSlice } from "../slices/runners/raceresults-api-slice";

export const store = configureStore({
  reducer: {
    [raceResultsApiSlice.reducerPath]: raceResultsApiSlice.reducer
  },
  middleware: (getDefaultMiddleware) => {
    /*
    * This is where we can add a bunch of API-specific middleware
    */
    return getDefaultMiddleware()
      .concat(raceResultsApiSlice.middleware);
  }
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
