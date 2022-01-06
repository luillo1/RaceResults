using System.Threading.Tasks;

namespace RaceResults.Data.KeyVault
{
    public interface IKeyVaultClient
    {
        Task<string> GetSecretAsync(string secretName);

        Task<string> PutSecretAsync(string secretName, string secret);
    }
}
