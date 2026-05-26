using EncryptionLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionLayer.Photo
{
    public interface ICryptoService
    {
        bool KeyExists();
        string GetAes();
        (EncryptedDataModel privatePem, EncryptedDataModel publicPem) GetNewRSAPemEncrypted();
        string DecryptRSAPEM(UserKeyModel keyPemEncrypted);
        List<EncryptedDataModel> EncryptData(byte[] data, ref byte[] dataEncrypted, List<UserKeyModel> publicKeyPemEncryptedList);
        public byte[] DecryptData(byte[] data, EncryptedDataModel dataKey, UserKeyModel privateKeyPemEncrypted);
        public EncryptedDataModel AddUserToEncryptedData(EncryptedDataModel dataKey, UserKeyModel privateKeyPemEncrypted, UserKeyModel publicKeyPemEncrypted);
        public EncryptedDataModel AddUserToEncryptedDataFast(EncryptedDataModel dataKey, int userId, string privateKeyPem, string publicKeyPem);
    }
    public class CryptoService : CryptoBase, ICryptoService
    {
        private readonly IAesGcmCipher _aesEncryption;
        private readonly IRsaCipher _rSAEncryption;
        private readonly VaultService _vault;
        private string aes;
        public CryptoService(IAesGcmCipher aesEncryption, IRsaCipher rSAEncryption, VaultService vault)
        {
            _aesEncryption = aesEncryption;
            _rSAEncryption = rSAEncryption;
            _vault = vault;

            aes = Task.Run(() => vault.GetApiKeyAsync()).Result;
        }

        public bool KeyExists() => aes.Length > 1;

        public EncryptedDataModel AddUserToEncryptedData(EncryptedDataModel dataKey, UserKeyModel privateKeyPemEncrypted, UserKeyModel publicKeyPemEncrypted)
        {
            // Dešifrování RSA klíčů uživatele
            var publicKeyPemDecrypted = DecryptRSAPEM(publicKeyPemEncrypted);
            var privateKeyPemDecrypted = DecryptRSAPEM(privateKeyPemEncrypted);

            // Dešifrování AES klíče pro data
            var rsaPrivate = RsaCipher.CreateFromPem(privateKeyPemDecrypted);
            var decryptedAES = RsaCipher.DecryptKey(dataKey.AesKeyEncrypted, rsaPrivate);

            // Zašifrování klíče AES pro dalšího uživatele
            var rsaPublic = RsaCipher.CreateFromPem(publicKeyPemDecrypted);

            // zašifruji AES pro daného uživatele
            var aesEncrypted = RsaCipher.EncryptKey(decryptedAES, rsaPublic);

            return new EncryptedDataModel
            {
                AesKeyEncrypted = aesEncrypted,
                UserId = publicKeyPemEncrypted.UserId,
                EncrptData = null, // v tomto scénáři nejsou obsaženy žádná data
                Nonce = dataKey.Nonce,
                Tag = dataKey.Tag,
            };
        }

        public byte[] DecryptData(byte[] data, EncryptedDataModel dataKey, UserKeyModel privateKeyPemEncrypted)
        {
            // Dešifrování RSA klíče uživatele
            var privateKeyPemDecrypted = DecryptRSAPEM(privateKeyPemEncrypted);

            // Dešifrování AES klíče pro data
            var rsa = RsaCipher.CreateFromPem(privateKeyPemDecrypted);
            var decryptedAES = RsaCipher.DecryptKey(dataKey.AesKeyEncrypted, rsa);

            // Dešifrování dat
            var dataDecrypted = AesGcmCipher.DecryptData(new EncryptedDataModel
            {
                AesKeyPlain = decryptedAES,
                EncrptData = data,
                Nonce = dataKey.Nonce,
                Tag = dataKey.Tag,
            });

            return dataDecrypted;
        }

        public List<EncryptedDataModel> EncryptData(byte[] data, ref byte[] dataEncrypted, List<UserKeyModel> publicKeyPemEncryptedList)
        {
            // zašifruji data
            var encryptedData = _aesEncryption.EncryptData(data) ?? throw new Exception("Data se nepodařilo zašifrovat!");
            dataEncrypted = encryptedData.EncrptData;
            var encryptedDataList = new List<EncryptedDataModel>();

            foreach (var publicKeyPemEncrypted in publicKeyPemEncryptedList)
            {
                var publicKeyPemDecrypted = DecryptRSAPEM(publicKeyPemEncrypted);
                var rsa = RsaCipher.CreateFromPem(publicKeyPemDecrypted);

                // zašifruji AES pro daného uživatele
                var aesEncrypted = RsaCipher.EncryptKey(encryptedData.AesKeyPlain, rsa);

                // z důvodu možné pamětové náročnosti obsahuje pouze první položka listu
                encryptedDataList.Add(new EncryptedDataModel
                {
                    AesKeyEncrypted = aesEncrypted,
                    UserId = publicKeyPemEncrypted.UserId,
                    EncrptData = null, // z důvodu paměťové náročnosti přenášena samostatně 
                    Nonce = encryptedData.Nonce,
                    Tag = encryptedData.Tag,
                });
            }

            return encryptedDataList;
        }

        public string GetAes() => AesGcmCipher.GetAesKeyBase64();

        public (EncryptedDataModel privatePem, EncryptedDataModel publicPem) GetNewRSAPemEncrypted()
        {
            var (privatePem, publicPem) = _rSAEncryption.GetRSAPemKeys();

            var encryptedDataPublicKey = AesGcmCipher.EncryptData(TextToBytes(publicPem), Base64ToBytes(aes));
            var encryptedDataPrivateKey = AesGcmCipher.EncryptData(TextToBytes(privatePem), Base64ToBytes(aes));

            return (encryptedDataPrivateKey, encryptedDataPublicKey);
        }

        public string DecryptRSAPEM(UserKeyModel keyPemEncrypted)
        {
            // Klíč dešifruje
            try
            {
                var keyPemDecryptedData = AesGcmCipher.DecryptData(new EncryptedDataModel
                {
                    AesKeyPlain = Base64ToBytes(aes),
                    EncrptData = keyPemEncrypted.KeyPem,
                    Nonce = keyPemEncrypted.Nonce,
                    Tag = keyPemEncrypted.Tag
                });

                // převede klíč z bytes na string PEM
                return BytesToText(keyPemDecryptedData);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public EncryptedDataModel AddUserToEncryptedDataFast(EncryptedDataModel dataKey, int userId, string privateKeyPem, string publicKeyPem)
        {
            // Dešifrování AES klíče pro data
            var rsaPrivate = RsaCipher.CreateFromPem(privateKeyPem);
            var decryptedAES = RsaCipher.DecryptKey(dataKey.AesKeyEncrypted, rsaPrivate);

            // Zašifrování klíče AES pro dalšího uživatele
            var rsaPublic = RsaCipher.CreateFromPem(publicKeyPem);

            // zašifruji AES pro daného uživatele
            var aesEncrypted = RsaCipher.EncryptKey(decryptedAES, rsaPublic);

            return new EncryptedDataModel
            {
                AesKeyEncrypted = aesEncrypted,
                UserId = userId,
                EncrptData = null, // v tomto scénáři nejsou obsaženy žádná data
                Nonce = dataKey.Nonce,
                Tag = dataKey.Tag,
            };
        }
    }
}
