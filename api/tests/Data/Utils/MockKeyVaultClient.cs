using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using RaceResults.Common.Models;
using RaceResults.Data.KeyVault;

namespace Internal.Data.Utils
{
    public class MockKeyVaultClient : IKeyVaultClient
    {
        private readonly Dictionary<string, string> database;

        public MockKeyVaultClient()
        {
            this.database = new Dictionary<string, string>();
        }

        public Task<string> GetSecretAsync(string secretName)
        {
            return Task.FromResult(database[secretName]);
        }

        public Task<string> PutSecretAsync(string secretName, string secret)
        {
            database[secretName] = secret;
            return Task.FromResult(secret);
        }

        public IEnumerable<string> GetSecrets()
        {
            return database.Values;
        }
    }
}
