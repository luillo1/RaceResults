using System.Threading.Tasks;

namespace RaceResults.Data.KeyVault
{
    public interface IKeyVaultClient
    {
        string GetSecret(string secretName);

        Task<string> GetSecretAsync(string secretName);
    }
}
