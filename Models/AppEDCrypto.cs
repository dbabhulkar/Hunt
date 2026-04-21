using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace AppED.Crypto
{
    public static class AesBase64Wrapper
    {
        private static readonly byte[] DefaultKey = Encoding.UTF8.GetBytes("HdC_Hunt2024SecK"); // 16 bytes for AES-128

        public static string DecodeAndDecrypt(string cipherText, string key)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            // Dual-read: try decrypt first, fall back to plaintext for un-migrated data
            try
            {
                byte[] keyBytes = DeriveKey(key);
                byte[] combined = Convert.FromBase64String(cipherText);
                if (combined.Length < 17) // minimum: 16-byte IV + 1 byte data
                    return cipherText;

                byte[] iv = new byte[16];
                byte[] encrypted = new byte[combined.Length - 16];
                Buffer.BlockCopy(combined, 0, iv, 0, 16);
                Buffer.BlockCopy(combined, 16, encrypted, 0, encrypted.Length);

                using var aes = Aes.Create();
                aes.Key = keyBytes;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor();
                byte[] plainBytes = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch
            {
                // Data is not encrypted yet (pre-migration plaintext)
                return cipherText;
            }
        }

        public static string EncryptAndEncode(string plainText, string key)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            byte[] keyBytes = DeriveKey(key);

            using var aes = Aes.Create();
            aes.Key = keyBytes;
            aes.GenerateIV();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // Prepend IV to ciphertext
            byte[] combined = new byte[aes.IV.Length + encrypted.Length];
            Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
            Buffer.BlockCopy(encrypted, 0, combined, aes.IV.Length, encrypted.Length);

            return Convert.ToBase64String(combined);
        }

        private static byte[] DeriveKey(string key)
        {
            // Pad or truncate key to 16 bytes (AES-128)
            byte[] keyBytes = new byte[16];
            byte[] raw = Encoding.UTF8.GetBytes(key ?? string.Empty);
            Buffer.BlockCopy(raw, 0, keyBytes, 0, Math.Min(raw.Length, 16));
            return keyBytes;
        }
    }
}

namespace API_HUNT.Models
{
    public static class EncryptDecrypt
    {
        private static readonly byte[] Key;

        static EncryptDecrypt()
        {
            // Use a fixed key derived from a known secret. In production, load from configuration.
            // This matches the legacy key pattern used by the application.
            Key = Encoding.UTF8.GetBytes("HuntApp2024SecKy"); // 16 bytes for AES-128
        }

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using var aes = Aes.Create();
            aes.Key = Key;
            aes.GenerateIV();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor();
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // Prepend IV to ciphertext
            byte[] combined = new byte[aes.IV.Length + encrypted.Length];
            Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
            Buffer.BlockCopy(encrypted, 0, combined, aes.IV.Length, encrypted.Length);

            return Convert.ToBase64String(combined);
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            // Dual-read: try decrypt first, fall back to plaintext for un-migrated data
            try
            {
                byte[] combined = Convert.FromBase64String(cipherText);
                if (combined.Length < 17)
                    return cipherText;

                byte[] iv = new byte[16];
                byte[] encrypted = new byte[combined.Length - 16];
                Buffer.BlockCopy(combined, 0, iv, 0, 16);
                Buffer.BlockCopy(combined, 16, encrypted, 0, encrypted.Length);

                using var aes = Aes.Create();
                aes.Key = Key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor();
                byte[] plainBytes = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch
            {
                // Data is not encrypted yet (pre-migration plaintext)
                return cipherText;
            }
        }
    }
}
