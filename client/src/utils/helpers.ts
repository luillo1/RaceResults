import { Race } from "../common";

/**
 * Groups the given race models by their eventId.
 * @param races the race models to group
 * @returns an 2-dimensional array where each entry is a list of race models who all
 * share the same eventId.
 */
export function groupByEventId(races: Race[]): Race[][] {
  if (races.length === 0) {
    return [];
  }

  const racesByEventId: Map<string, Race[]> = new Map<string, Race[]>();

  races.forEach((item) => {
    if (
      item.eventId === null ||
      item.eventId === undefined ||
      item.eventId === "" ||
      item.eventId === "00000000-0000-0000-0000-000000000000"
    ) {
      return;
    }

    if (!racesByEventId.has(item.eventId)) {
      racesByEventId.set(item.eventId, []);
    }

    racesByEventId.get(item.eventId)?.push(item);
  });

  return [...racesByEventId.values()];
}
