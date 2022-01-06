import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { WildApricotAuthResponse } from "../wild-apricot/wild-apricot-api-slice";

interface WildApricotAuthState {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  loginTime: number;
  accountId: number;
}

const initialState: WildApricotAuthState = {
  accessToken: "",
  refreshToken: "",
  expiresIn: -1,
  accountId: -1,
  loginTime: -1,
};

const wildApricotAuthSlice = createSlice({
  name: "counter",
  initialState,
  reducers: {
    login(state, action: PayloadAction<WildApricotAuthResponse>) {
      state.accessToken = action.payload.access_token;
      state.refreshToken = action.payload.refresh_token;
      state.expiresIn = action.payload.expires_in;
      state.accountId = action.payload.Permissions[0].AccountId;
      state.loginTime = Date.now();
    },
  },
});

export const { login } = wildApricotAuthSlice.actions;
export default wildApricotAuthSlice.reducer;
