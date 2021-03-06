import { RaceResponse } from "./slices/runners/raceresults-api-slice";

export interface Race extends Omit<RaceResponse, "date"> {
  date: Date | null;
}
