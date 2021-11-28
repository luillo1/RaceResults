namespace RaceResults.Data.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ICosmosDbContainerClient<T>
    {
        Task<T> GetItemAsync(string id);

        Task<IEnumerable<T>> GetItemsAsync();

        Task AddItemAsync(T item);

        Task DeleteItemAsync(string id);
    }
}
