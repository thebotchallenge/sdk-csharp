using System;
using System.IO;
using System.Security.Cryptography;

namespace vikebot.Security
{
    /// <summary>
    /// Summary description for Aes256
    /// </summary>
    internal sealed class AesCrypt : IDisposable
    {
        private Aes rijndael;
        private ICryptoTransform encryptor;
        private ICryptoTransform decryptor;
        public byte[] Key
        {
            get { return this.rijndael.Key; }
        }
        public byte[] IV
        {
            get { return this.rijndael.IV; }
        }

        public AesCrypt(string base64Key, string base64IV, CipherMode cipherMode, int keySize, int blockSize)
        {
            this.rijndael = Aes.Create();
            this.rijndael.Mode = cipherMode;
            this.rijndael.KeySize = keySize;
            this.rijndael.BlockSize = blockSize;
            this.rijndael.Key = Convert.FromBase64String(base64Key);
            this.rijndael.IV = Convert.FromBase64String(base64IV);
            this.InitialiseCryptoTransformer();
        }

        private void InitialiseCryptoTransformer()
        {
            this.encryptor = this.rijndael.CreateEncryptor(this.rijndael.Key, this.rijndael.IV);
            this.decryptor = this.rijndael.CreateDecryptor(this.rijndael.Key, this.rijndael.IV);
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
        public byte[] Encrypt(byte[] plain)
        {
            return this.InternalCrypt(plain, this.encryptor);
        }
        public byte[] Decrypt(byte[] cipher)
        {
            return this.InternalCrypt(cipher, this.decryptor);
        }

        public void Dispose()
        {
            this.encryptor.Dispose();
            this.decryptor.Dispose();
            this.rijndael.Dispose();
        }
    }
}