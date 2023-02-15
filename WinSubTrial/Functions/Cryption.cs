using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WinSubTrial
{
    class Cryption
    {
        static readonly string key = "winteam@tank@key";
        static readonly string iv = "winteam2020@tank";

        public static string Encrypt(string plainText)
        {
            //Console.WriteLine($"Plain text: {plainText}");

            string encrypted = Utils.Cryption.PKCS7.Encrypt128(key, iv, plainText);

            //Console.WriteLine($"Encryped text: {encrypted}");

            return encrypted;
        }
        public static string DecryptWinChanger(string encrypted)
        {
            //Console.WriteLine($"Encryped text: {encrypted}");

            if (!IsBase64Encoded(encrypted)) return encrypted;

            string plainText = Utils.Cryption.PKCS7.Decrypt128(encrypted, key, iv);

            //Console.WriteLine($"Plain text: {plainText}");
            return plainText;
        }
        public static bool IsBase64Encoded(string str)
        {

            try

            {
                // If no exception is caught, then it is possibly a base64 encoded string
                byte[] data = Convert.FromBase64String(str);
                // The part that checks if the string was properly padded to the
                // correct length was borrowed from d@anish's solution
                return (str.Replace(" ", "").Length % 4 == 0);
            }
            catch
            {
                // If exception is caught, then it is not a base64 encoded string
                return false;
            }

        }
    }
}
