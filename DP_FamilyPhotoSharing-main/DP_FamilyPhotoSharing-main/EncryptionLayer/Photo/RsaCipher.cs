using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionLayer.Photo
{
    public interface IRsaCipher
    {
        RSAParameters GetRSAPrivateParameters();
        RSAParameters GetRSAPublicParameters();
        (string privateKey, string publicKey) GetRSAPemKeys();
    }
    public class RsaCipher : IRsaCipher
    {
        private const int RSAKeySize = 3072;
        private RSA _RSA;
        private string PrivateKeyPem = string.Empty;
        private string PublicKeyPem = string.Empty;
        private RSAParameters PrivateRSAParameters;
        private RSAParameters PublicRSAParameters;

        public RsaCipher()
        {
            Reload();
        }
        public RSAParameters GetRSAPrivateParameters() => PrivateRSAParameters;

        public RSAParameters GetRSAPublicParameters() => PublicRSAParameters;
        public (string privateKey, string publicKey) GetRSAPemKeys()
        {
            var privateKey = PrivateKeyPem;
            var publicKey = PublicKeyPem;

            Reload();

            return (privateKey, publicKey);
        }

        public static byte[] EncryptKey(byte[] key, RSA pRSA)
        {
            using RSA publicRsa = pRSA ?? throw new Exception("PublicRSA není inicializováno.");
            return publicRsa.Encrypt(key, RSAEncryptionPadding.OaepSHA256);
        }

        public static byte[] DecryptKey(byte[] encryptedKey, RSA rsa) => rsa.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA256);
        public static RSA CreateFromPem(string pem)
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(pem);

            return rsa;
        }

        private void Reload()
        {
            _RSA = RSA.Create(RSAKeySize);
            LoadParameters();
        }
        private void LoadParameters()
        {
            using (var rsa = _RSA)
            {
                PrivateRSAParameters = rsa.ExportParameters(true);
                PublicRSAParameters = rsa.ExportParameters(false);
                PrivateKeyPem = rsa.ExportPkcs8PrivateKeyPem();
                PublicKeyPem = rsa.ExportSubjectPublicKeyInfoPem();
            }
        }
    }
}
