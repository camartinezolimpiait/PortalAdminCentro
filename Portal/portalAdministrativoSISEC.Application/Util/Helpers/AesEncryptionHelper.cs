using Microsoft.Extensions.Options;
using portalAdministrativoSISEC.Aplication;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace portalAdministrativoSISEC.Util.Helpers
{
    public class AesEncryptionHelper : IAesEncryptionHelper
    {
        #region Fields

        protected readonly IOptions<AppSettings> _appSettings;
        private const int KeySize = 256;
        private const int BlockSize = 128;
        private readonly string Key;
        private readonly string Iv;
        private readonly PaddingMode PaddingMode = PaddingMode.PKCS7;
        private readonly CipherMode CipherMode = CipherMode.CBC;

        #endregion Fields

        #region Public Constructors

        public AesEncryptionHelper(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
            Key = _appSettings.Value.ApiFrontMiLicencia.Encrypt.Key;
            Iv = _appSettings.Value.ApiFrontMiLicencia.Encrypt.Iv;
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task<string> Encrypt(string plainText)
        {
            byte[] encryptedBytes;
            byte[] ivBytes = Encoding.UTF8.GetBytes(Iv);
            byte[] keyBytes = Encoding.UTF8.GetBytes(Key);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = KeySize;
                aesAlg.BlockSize = BlockSize;
                aesAlg.Padding = PaddingMode;
                aesAlg.Mode = CipherMode;
                aesAlg.Key = keyBytes;
                aesAlg.IV = ivBytes;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msEncrypt = new MemoryStream();
                using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                encryptedBytes = msEncrypt.ToArray();
            }
            return await Task.FromResult(Convert.ToBase64String(encryptedBytes));
        }

        public async Task<string> Decrypt(string cipherText)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] ivBytes = Encoding.UTF8.GetBytes(Iv);
            byte[] keyBytes = Encoding.UTF8.GetBytes(Key);

            string plainText;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = KeySize;
                aesAlg.BlockSize = BlockSize;
                aesAlg.Padding = PaddingMode;
                aesAlg.Mode = CipherMode;
                aesAlg.Key = keyBytes;
                aesAlg.IV = ivBytes;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msDecrypt = new MemoryStream(cipherBytes);
                using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new StreamReader(csDecrypt);
                plainText = srDecrypt.ReadToEnd();
            }
            return await Task.FromResult(plainText);
        }

        #endregion Public Methods
    }
}