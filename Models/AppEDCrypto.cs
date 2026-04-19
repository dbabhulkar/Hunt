namespace AppED.Crypto
{
    public static class AesBase64Wrapper
    {
        public static string DecodeAndDecrypt(string cipherText, string key) => cipherText;
    }
}

namespace API_HUNT.Models
{
    public static class EncryptDecrypt
    {
        public static string Encrypt(string plainText) => plainText;
        public static string Decrypt(string cipherText) => cipherText;
    }
}
