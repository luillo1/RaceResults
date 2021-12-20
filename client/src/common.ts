import { RaceResponse } from "./slices/runners/raceresults-api-slice";

export interface IRace extends Omit<RaceResponse, "date"> {
  date: Date | null;
}
