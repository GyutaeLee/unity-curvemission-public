using UnityEngine;
using System.Security.Cryptography;
using System.Text;

namespace Services.Security
{
    public class PlayerPrefs : MonoBehaviour
    {
        private static string saltForKey;

        private static byte[] keys;
        private static byte[] iv;
        private static int keySize = 256;
        private static int blockSize = 128;
        private static int hashLen = 32;

        static PlayerPrefs()
        {
            // 8 바이트로 하고, 변경해서 쓸것
            byte[] saltBytes = new byte[] { security - related };

            // 길이 상관 없고, 키를 만들기 위한 용도로 씀
            //string randomSeedForKey = "security-related";
            string randomSeedForKey = "security-related";

            // 길이 상관 없고, aes에 쓸 key 와 iv 를 만들 용도
            //string randomSeedForValue = "security-related";
            string randomSeedForValue = "security-related";

            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(randomSeedForKey, saltBytes, 1000);
                saltForKey = System.Convert.ToBase64String(key.GetBytes(blockSize / 8));
            }

            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(randomSeedForValue, saltBytes, 1000);
                keys = key.GetBytes(keySize / 8);
                iv = key.GetBytes(blockSize / 8);
            }
        }

        public static string MakeHash(string original)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(original);
                byte[] hashBytes = md5.ComputeHash(bytes);

                string hashToString = "";
                for (int i = 0; i < hashBytes.Length; ++i)
                    hashToString += hashBytes[i].ToString("x2");

                return hashToString;
            }
        }

        public static byte[] Encrypt(byte[] bytesToBeEncrypted)
        {
            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.KeySize = keySize;
                aes.BlockSize = blockSize;

                aes.Key = keys;
                aes.IV = iv;

                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform ct = aes.CreateEncryptor())
                {
                    return ct.TransformFinalBlock(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                }
            }
        }

        public static byte[] Decrypt(byte[] bytesToBeDecrypted)
        {
            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.KeySize = keySize;
                aes.BlockSize = blockSize;

                aes.Key = keys;
                aes.IV = iv;

                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform ct = aes.CreateDecryptor())
                {
                    return ct.TransformFinalBlock(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                }
            }
        }

        public static string Encrypt(string input)
        {
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            byte[] bytesEncrypted = Encrypt(bytesToBeEncrypted);

            return System.Convert.ToBase64String(bytesEncrypted);
        }

        public static string Decrypt(string input)
        {
            byte[] bytesToBeDecrypted = System.Convert.FromBase64String(input);
            byte[] bytesDecrypted = Decrypt(bytesToBeDecrypted);

            return Encoding.UTF8.GetString(bytesDecrypted);
        }

        private static void SetSecurityValue(string key, string value)
        {
            string hideKey = MakeHash(key + saltForKey);
            string encryptValue = Encrypt(value + MakeHash(value));

            UnityEngine.PlayerPrefs.SetString(hideKey, encryptValue);
        }

        private static string GetSecurityValue(string key)
        {
            string hideKey = MakeHash(key + saltForKey);

            string encryptValue = UnityEngine.PlayerPrefs.GetString(hideKey);
            if (true == string.IsNullOrEmpty(encryptValue))
                return string.Empty;

            string valueAndHash = Decrypt(encryptValue);
            if (hashLen > valueAndHash.Length)
                return string.Empty;

            string savedValue = valueAndHash.Substring(0, valueAndHash.Length - hashLen);
            string savedHash = valueAndHash.Substring(valueAndHash.Length - hashLen);

            if (MakeHash(savedValue) != savedHash)
                return string.Empty;

            return savedValue;
        }

        public static void DeleteKey(string key)
        {
            UnityEngine.PlayerPrefs.DeleteKey(MakeHash(key + saltForKey));
        }

        public static void DeleteAll()
        {
            UnityEngine.PlayerPrefs.DeleteAll();
        }

        public static void Save()
        {
            UnityEngine.PlayerPrefs.Save();
        }

        public static void SetBool(string key, bool value)
        {
            SetSecurityValue(key, value.ToString());
        }

        public static void SetInt(string key, int value)
        {
            SetSecurityValue(key, value.ToString());
        }

        public static void SetLong(string key, long value)
        {
            SetSecurityValue(key, value.ToString());
        }

        public static void SetFloat(string key, float value)
        {
            SetSecurityValue(key, value.ToString());
        }

        public static void SetString(string key, string value)
        {
            SetSecurityValue(key, value);
        }

        public static bool GetBool(string key, bool defaultValue)
        {
            string originalValue = GetSecurityValue(key);
            if (true == string.IsNullOrEmpty(originalValue))
                return defaultValue;

            bool result = defaultValue;
            if (false == bool.TryParse(originalValue, out result))
                return defaultValue;

            return result;
        }

        public static int GetInt(string key, int defaultValue)
        {
            string originalValue = GetSecurityValue(key);
            if (true == string.IsNullOrEmpty(originalValue))
                return defaultValue;

            int result = defaultValue;
            if (false == int.TryParse(originalValue, out result))
                return defaultValue;

            return result;
        }

        public static long GetLong(string key, long defaultValue)
        {
            string originalValue = GetSecurityValue(key);
            if (true == string.IsNullOrEmpty(originalValue))
                return defaultValue;

            long result = defaultValue;
            if (false == long.TryParse(originalValue, out result))
                return defaultValue;

            return result;
        }

        public static float GetFloat(string key, float defaultValue)
        {
            string originalValue = GetSecurityValue(key);
            if (true == string.IsNullOrEmpty(originalValue))
                return defaultValue;

            float result = defaultValue;
            if (false == float.TryParse(originalValue, out result))
                return defaultValue;

            return result;
        }

        public static string GetString(string key, string defaultValue)
        {
            string originalValue = GetSecurityValue(key);
            if (true == string.IsNullOrEmpty(originalValue))
                return defaultValue;

            return originalValue;
        }
    }
}