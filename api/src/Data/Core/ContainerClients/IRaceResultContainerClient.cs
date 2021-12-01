using System.Collections.Generic;
using System.Threading.Tasks;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public interface IRaceResultContainerClient
    {
        Task<RaceResult> GetRaceResultAsync(string memberId, string raceResultId);

        Task<IEnumerable<RaceResult>> GetAllRaceResultsAsync(string memberId);

        Task<IEnumerable<RaceResult>> GetAllRaceResultsAsync(IEnumerable<string> memberIds);

        Task AddRaceResultAsync(RaceResult raceResult);
    }
}
