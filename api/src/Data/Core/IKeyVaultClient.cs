using System.Threading.Tasks;

namespace RaceResults.Data.Core
{
    public interface IKeyVaultClient
    {
        string GetSecret(string secretName);

        Task<string> GetSecretAsync(string secretName);
    }
}
