using System.Collections.Generic;
using System.Threading.Tasks;
using RaceResults.Common.Models;

namespace RaceResults.Data.Core
{
    public interface IRaceContainerClient
    {
        Task<Race> GetRaceAsync(string raceId);

        Task<IEnumerable<Race>> GetAllRacesAsync();

        Task AddRaceAsync(Race race);
    }
}
