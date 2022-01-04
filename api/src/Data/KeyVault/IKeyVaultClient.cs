using System;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace RaceResults.Data.KeyVault
{
    public interface IKeyVaultClient
    {
        Task<string> GetSecretAsync(string secretName);
    }
}
