namespace RaceResults.Data.Core
{
    using System.Threading.Tasks;

    public interface IKeyVaultClient
    {
        string GetSecret(string secretName);

        Task<string> GetSecretAsync(string secretName);
    }
}
