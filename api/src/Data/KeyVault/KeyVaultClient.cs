using System;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace RaceResults.Data.KeyVault
{
    public class KeyVaultClient : IKeyVaultClient
    {
        private const string KeyVaultName = "raceresults-kv";

        private readonly SecretClient secretClient;

        public KeyVaultClient()
        {
            Uri keyVaultUri = new Uri($"https://{KeyVaultName}.vault.azure.net");
            TokenCredential tokenCredential = new DefaultAzureCredential();
            secretClient = new SecretClient(keyVaultUri, tokenCredential);
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            Response<KeyVaultSecret> response = await secretClient.GetSecretAsync(secretName);
            return response.Value.Value;
        }
    }
}
