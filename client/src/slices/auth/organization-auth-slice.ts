import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { OrganizationLoginResponse } from "../runners/raceresults-api-slice";

interface AuthState {
  orgAuths: { [id: string]: OrganizationLoginResponse };
}

const initialState: AuthState = {
  orgAuths: {},
};

const organizationAuthSlice = createSlice({
  name: "login",
  initialState,
  reducers: {
    login(
      state,
      action: PayloadAction<{
        orgId: string;
        response: OrganizationLoginResponse;
      }>
    ) {
      const newState = { ...state.orgAuths };
      newState[action.payload.orgId] = action.payload.response;
      state.orgAuths = newState;
    },
  },
});

export const { login } = organizationAuthSlice.actions;
export default organizationAuthSlice.reducer;
