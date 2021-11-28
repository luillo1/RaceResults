namespace RaceResults.Data.Core
{
    using System;
    using System.Threading.Tasks;
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;

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
