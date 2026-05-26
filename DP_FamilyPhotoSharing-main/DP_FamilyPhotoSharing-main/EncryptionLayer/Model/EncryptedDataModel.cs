using ModelLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionLayer.Model
{
    public class EncryptedDataModel
    {
        public int UserId { get; set; }
        /// <summary>
        /// Klíč AES zašifrovaný RSA klíčem uživatele
        /// </summary>
        public byte[] AesKeyEncrypted { get; set; }
        /// <summary>
        /// Klíč AES bez šifrování
        /// </summary>
        public byte[] AesKeyPlain { get; set; }
        /// <summary>
        ///  Sůl pro šifrování dat - unikátní pro každý soubor
        /// </summary>
        public byte[] Nonce { get; set; }
        /// <summary>
        /// Digitální otisk nad daty - určení zda došlo k manipulaci s daty
        /// </summary>
        public byte[] Tag { get; set; }
        /// <summary>
        /// Zašifrovaná samotná data
        /// </summary>
        public byte[] EncrptData { get; set; }
        public static EncryptedDataModel GetEncryptedDataFromPhotoEncryptModel(PhotoEncryptModel photoEncryptModel) => new EncryptedDataModel
        {
            AesKeyEncrypted = photoEncryptModel.Aes,
            Nonce = photoEncryptModel.Nonce,
            Tag = photoEncryptModel.Tag,
            UserId = photoEncryptModel.UserId,
        };
    }
}
