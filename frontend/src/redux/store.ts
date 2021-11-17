import { configureStore } from "@reduxjs/toolkit";
import { apiSlice } from "../features/runners/runners-api-slice";

export const store = configureStore({
  reducer: {
    [apiSlice.reducerPath]: apiSlice.reducer
  },

  middleware: (getDefaultMiddleware) => {
    /*
    * This is where we can add a bunch of API-specific middleware
    */
    return getDefaultMiddleware()
      .concat(apiSlice.middleware);
  }
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
