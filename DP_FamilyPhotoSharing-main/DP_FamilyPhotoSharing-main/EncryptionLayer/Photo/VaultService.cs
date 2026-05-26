using Microsoft.Extensions.Configuration;
using ModelLayer.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace EncryptionLayer.Photo
{
    public class VaultService
    {
        private readonly IVaultClient _client;
        private readonly VaultOptions _options;

        public VaultService(VaultOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            var authMethod = new TokenAuthMethodInfo(_options.Token);
            var settings = new VaultClientSettings(_options.Address, authMethod);

            _client = new VaultClient(settings);
        }

        public async Task<string> GetApiKeyAsync()
        {
            var secret = await _client.V1.Secrets.KeyValue.V2.ReadSecretAsync(
                path: _options.SecretPath,
                mountPoint: "secret"
            );

            return secret.Data.Data["ApiKey"].ToString();
        }
    }


}
