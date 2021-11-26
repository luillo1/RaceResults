using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceResults.Data.Core
{
    public interface ICosmosDbContainerClient<T>
    {
        Task<T> GetItemAsync(string id);

        Task<IEnumerable<T>> GetItemsAsync();

        Task AddItemAsync(T item);

        Task DeleteItemAsync(string id);
    }
}
