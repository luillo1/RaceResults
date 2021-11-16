using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Threading.Tasks;

namespace RaceResults.Data.KeyVault
{
    public class KeyVaultClient : IKeyVaultClient
    {
        private const string KeyVaultEndpoint = "https://raceresults-kv.vault.azure.net/";

        private readonly SecretClient secretClient;

        public KeyVaultClient()
        {
            this.secretClient = new SecretClient(
                new Uri(KeyVaultClient.KeyVaultEndpoint),
                new DefaultAzureCredential());
        }

        public string GetSecret(string secretName)
        {
            KeyVaultSecret secret = this.secretClient.GetSecret(secretName);
            return secret.Value;
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            KeyVaultSecret secret = await this.secretClient.GetSecretAsync(secretName);
            return secret.Value;
        }
    }
}
