using System.Threading.Tasks;

namespace RaceResults.Common
{
    public interface IKeyVaultClient
    {
        string GetSecret(string secretName);

        Task<string> GetSecretAsync(string secretName);
    }
}
