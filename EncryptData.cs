using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Configuration;
using System.Text;
using DMVCS.DataWorker;

namespace DMVCS.BusinessLogic
{
    public static class EncryptData
    {
        //based on the following url for help:  http://www.deltasblog.co.uk/code-snippets/basic-encryptiondecryption-c/

        /// <summary>
        /// Encrypt strings using key stored in web.config or one retrieved from database
        /// if bool database is false, key is web.config
        /// model.InputToHash.EncryptWithStoredKey(false);
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EncryptWithStoredKey(this string input, bool database)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            byte[] result;
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                if (!database)
                {
                    aes.Key = UTF8Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Enycrpt"].ToString());
                }
                else
                {
                    DataAccessLayer dal = new DataAccessLayer();
                    aes.Key = UTF8Encoding.UTF8.GetBytes(dal.GetEncryptionKey(string.Empty)); //to get key from settings table
                }
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryTrans = aes.CreateEncryptor())
                {
                    result = cryTrans.TransformFinalBlock(inputArray, 0, inputArray.Length);
                }
                aes.Clear();
            }
            return Convert.ToBase64String(result, 0, result.Length);
        }

        /// <summary>
        /// Decrypt strings using key stored in web.config, or retrieved from database
        /// if bool database is false, then key in web.config
        /// model.OuputHashed.DecryptWithStoredKey(false);
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DecryptWithStoredKey(this string input, bool database)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            byte[] result;
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                if (!database)
                {
                    aes.Key = UTF8Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Enycrpt"].ToString());
                }
                else
                {
                    //database hit here to get the key
                    DataAccessLayer dal = new DataAccessLayer();
                    aes.Key = UTF8Encoding.UTF8.GetBytes(dal.GetEncryptionKey(string.Empty)); //to get key from settings table
                }
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryTrans = aes.CreateDecryptor())
                {
                    result = cryTrans.TransformFinalBlock(inputArray, 0, inputArray.Length);
                }
                aes.Clear();
            }
            return UTF8Encoding.UTF8.GetString(result);
        }

        /// <summary>
        /// randomn key used for each record in database.
        /// you must have the key for passthrough
        /// how to generate a randomn key for aes
        /// string key = string.Empty;
        /// key = key.RandomnKey();
        /// </summary>
        /// <param name="input"></param>
        /// <param name="randomnKey"></param>
        /// <returns></returns>
        public static string EncryptWithKeyOnHand(this string input, string randomnKey)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            byte[] result;
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = UTF8Encoding.UTF8.GetBytes(randomnKey);
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryTrans = aes.CreateEncryptor())
                {
                    result = cryTrans.TransformFinalBlock(inputArray, 0, inputArray.Length);
                }
                aes.Clear();
            }
            return Convert.ToBase64String(result, 0, result.Length);
        }

        /// <summary>
        /// you must have the key for each pass through
        /// get the key from the database record
        /// </summary>
        /// <param name="input"></param>
        /// <param name="randomKey"></param>
        /// <returns></returns>
        public static string DecryptWithKeyOnHand(this string input, string randomKey)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            byte[] result;
            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = UTF8Encoding.UTF8.GetBytes(randomKey);
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryTrans = aes.CreateDecryptor())
                {
                    result = cryTrans.TransformFinalBlock(inputArray, 0, inputArray.Length);
                }
                aes.Clear();
            }
            return UTF8Encoding.UTF8.GetString(result);
        }

        /// <summary>
        /// generates a 24 character randomn key
        /// always send an empty string to it for it to work like key = key.RandomnKey();
        /// </summary>
        /// <returns></returns>
        public static string RandomnKey(this string input)
        {
            string key = string.Empty;
            Random rand = new Random();
            char[] arr = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

            for (int i = 0; i < 12; i++)
            {

                key = key + arr[rand.Next(26)].ToString() + rand.Next(10).ToString();

            }

            return key;
        }

    }
}