import { configureStore } from "@reduxjs/toolkit";
import { runnersApiSlice } from "../slices/runners/runners-api-slice";

export const store = configureStore({
  reducer: {
    [runnersApiSlice.reducerPath]: runnersApiSlice.reducer
  },
  middleware: (getDefaultMiddleware) => {
    /*
    * This is where we can add a bunch of API-specific middleware
    */
    return getDefaultMiddleware()
      .concat(runnersApiSlice.middleware);
  }
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
