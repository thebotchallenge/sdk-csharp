using System;
using System.IO;
using System.Security.Cryptography;

namespace vikebot.Security
{
    /// <summary>
    /// Helper class to easily use Aes (alias Rijndael Algorithm).
    /// </summary>
    internal sealed class AesHelper : IDisposable
    {
        private Aes rijndael;
        private ICryptoTransform encryptor;
        private ICryptoTransform decryptor;

        internal byte[] Key => this.rijndael.Key;
        internal byte[] IV => this.rijndael.IV;

        internal AesHelper(string base64Key, string base64IV, CipherMode cipherMode, int keySize, int blockSize)
        {
            this.rijndael = Aes.Create();
            this.rijndael.Mode = cipherMode;
            this.rijndael.KeySize = keySize;
            this.rijndael.BlockSize = blockSize;
            this.rijndael.Key = Convert.FromBase64String(base64Key);
            this.rijndael.IV = Convert.FromBase64String(base64IV);

            this.encryptor = this.rijndael.CreateEncryptor(this.rijndael.Key, this.rijndael.IV);
            this.decryptor = this.rijndael.CreateDecryptor(this.rijndael.Key, this.rijndael.IV);
        }

        internal byte[] Encrypt(byte[] plain)
        {
            return this.InternalCrypt(plain, this.encryptor);
        }
        internal byte[] Decrypt(byte[] cipher)
        {
            return this.InternalCrypt(cipher, this.decryptor);
        }

        private byte[] InternalCrypt(byte[] input, ICryptoTransform ct)
        {
            byte[] buffer;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, ct, CryptoStreamMode.Write))
                    cryptoStream.Write(input, 0, input.Length);
                buffer = memoryStream.ToArray();
            }
            return buffer;
        }

        public void Dispose()
        {
            this.encryptor.Dispose();
            this.decryptor.Dispose();
            this.rijndael.Dispose();
        }
    }
}