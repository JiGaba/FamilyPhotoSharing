using EncryptionLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionLayer.Photo
{
    public interface IAesGcmCipher
    {
        EncryptedDataModel EncryptData(byte[] plainText);
    }
    public class AesGcmCipher : CryptoBase, IAesGcmCipher
    {
        public byte[] AesKeyPlain { get; set; }
        public byte[] AesKeyEncrypt { get; set; }
        public byte[] Nonce { get; set; }
        public byte[] Tag { get; set; }
        public byte[] EncrptData { get; set; }
        private const Int16 TagSize = 16;
        public AesGcmCipher()
        {
            Reload();
        }
        public EncryptedDataModel EncryptData(byte[] plainText)
        {
            EncrptData = new byte[plainText.Length];

            using var aes = new AesGcm(AesKeyPlain, TagSize);
            aes.Encrypt(Nonce, plainText, EncrptData, Tag);

            var encryptedData = new EncryptedDataModel
            {
                EncrptData = EncrptData,
                Nonce = Nonce,
                Tag = Tag,
                AesKeyPlain = AesKeyPlain
            };

            Reload();

            return encryptedData;
        }

        public static EncryptedDataModel EncryptData(byte[] plainText, byte[] aesKey)
        {
            var encrptData = new byte[plainText.Length];
            var nonce = RandomNumberGenerator.GetBytes(12);
            var tag = new byte[16];

            using var aes = new AesGcm(aesKey, TagSize);
            aes.Encrypt(nonce, plainText, encrptData, tag);

            return new EncryptedDataModel
            {
                EncrptData = encrptData,
                Nonce = nonce,
                Tag = tag,
                AesKeyPlain = aesKey
            };
        }

        public static byte[] DecryptData(EncryptedDataModel file)
        {
            byte[] plaintext = new byte[file.EncrptData.Length];

            try
            {
                using var aes = new AesGcm(file.AesKeyPlain, TagSize);
                aes.Decrypt(file.Nonce, file.EncrptData, file.Tag, plaintext);

                return plaintext;
            }
            catch (CryptographicException) // Pokud je nevalidní klíč nebo je chyba v konzistenci dat
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string GetAesKeyBase64() => BytesToBase64(RandomNumberGenerator.GetBytes(32));
        private void Reload()
        {
            AesKeyPlain = RandomNumberGenerator.GetBytes(32); // AES-256
            Nonce = RandomNumberGenerator.GetBytes(12);
            Tag = new byte[16];
        }
    }
}
