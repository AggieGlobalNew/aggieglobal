/* ========================================================================
 * Includes    : FormatterConfig.cs
 * Website     : http://www.atlassoft.com
 * Create Date : May, May 08, 2019 by Sudipta Sarkar
 * Details     : Implements various data type formatter registration logics for AggieGlobal.WebApi.
 * Copyright © 2019 Atlas Software Technologies All rights reserved
 * ========================================================================
 * Release History
 * ---------------
 * DESCRIPTION					CREATED / MODIFIED BY
 * -----------                  ---------------------
 * Initial Implementation		Sudipta Sarkar
 * ========================================================================
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;

namespace AggieGlobal.WebApi.Infrastructure
{
    public static class EncryptionHelper
    {
        public static string MD5Encryption(string stringToEncrypt, EncryptionKey encryptionKey)
        {
            byte[] sourceBytes;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider mcsp = new MD5CryptoServiceProvider();
            byte[] TDESByte = mcsp.ComputeHash(UTF8.GetBytes(encryptionKey.GetCustomDescription()));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESByte;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] dataToEncrypt = UTF8.GetBytes(stringToEncrypt);

            // Step 5. Attempt to encrypt the string
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                sourceBytes = Encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                mcsp.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string
            return Convert.ToBase64String(sourceBytes);
        }

        public static string MD5Decryption(string stringToDecrypt, EncryptionKey encryptionKey)
        {
            byte[] sourceBytes;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider mcsp = new MD5CryptoServiceProvider();
            byte[] TDESByte = mcsp.ComputeHash(UTF8.GetBytes(Utility.GetCustomDescription(encryptionKey)));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESByte;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] dataToDecrypt = Convert.FromBase64String(stringToDecrypt);

            // Step 5. Attempt to decrypt the string
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                sourceBytes = Decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                mcsp.Clear();
            }

            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString(sourceBytes);
        }

        public static string AesEncryption(string stringToEncrypt, EncryptionKey encryptionKey)
        {
            using (AesCryptoServiceProvider acsp = GetProvider(Encoding.Default.GetBytes(encryptionKey.GetCustomDescription().MD5HexString())))
            {
                byte[] sourceBytes = Encoding.ASCII.GetBytes(stringToEncrypt);
                ICryptoTransform ict = acsp.CreateEncryptor();

                using (MemoryStream ms = new MemoryStream())
                using(CryptoStream cs = new CryptoStream(ms, ict, CryptoStreamMode.Write))
                {
                    cs.Write(sourceBytes, 0, sourceBytes.Length);
                    cs.FlushFinalBlock();

                    byte[] encryptedBytes = ms.ToArray(); /***** .ToArray() is important, don't mess with the buffer *****/
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public static string MD5HexString(this string stringToConvert)
        {
            HashAlgorithm hashAlgorithm = MD5.Create();
            Encoding encoding = new UTF8Encoding();
            var convertedString = new StringBuilder();

            foreach (var element in hashAlgorithm.ComputeHash(encoding.GetBytes(stringToConvert)))
            {
                convertedString.Append(element.ToString("x2"));
            }

            return convertedString.ToString();
        }

        private static AesCryptoServiceProvider GetProvider(byte[] key)
        {
            AesCryptoServiceProvider acsp = new AesCryptoServiceProvider();
            acsp.BlockSize = 128;
            acsp.KeySize = 128;
            acsp.Mode = CipherMode.CBC;
            acsp.Padding = PaddingMode.PKCS7;

            acsp.GenerateIV();
            acsp.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            byte[] realKey = GetKey(key, acsp);
            //acsp.Key = realKey; /***** for 16 byte *****/
            acsp.Key = key; /***** for 32 byte *****/

            return acsp;
        }

        private static byte[] GetKey(byte[] suggestedKey, SymmetricAlgorithm sa)
        {
            byte[] rawByte = suggestedKey;
            List<byte> listByte = new List<byte>();

            for (int count = 0; count < sa.LegalKeySizes[0].MinSize; count += 8)
            {
                listByte.Add(rawByte[(count / 8) % rawByte.Length]);
            }
            byte[] returnByte = listByte.ToArray();
            return returnByte;
        }

        public static string AesDecryption(string base64StringToDecrypt, EncryptionKey encryptionKey)
        {
            using (AesCryptoServiceProvider acsp = GetProvider(Encoding.Default.GetBytes(encryptionKey.GetCustomDescription().MD5HexString())))
            {
                byte[] rawBytes = Convert.FromBase64String(base64StringToDecrypt);
                ICryptoTransform ict = acsp.CreateDecryptor();

                using(MemoryStream ms = new MemoryStream(rawBytes, 0, rawBytes.Length))
                using(CryptoStream cs = new CryptoStream(ms, ict, CryptoStreamMode.Read))
                    return (new StreamReader(cs)).ReadToEnd();
            }
        }

        public static string Encrypt(string name, string userData)
        {
            FormsAuthenticationTicket formsTicket = new FormsAuthenticationTicket(1, name, DateTime.Now,
                DateTime.Now.AddMinutes(30), false, userData);

            return FormsAuthentication.Encrypt(formsTicket);
        }

        public static string Decrypt(string encryptedTicket)
        {
            try
            {
                FormsAuthenticationTicket formsTicket = FormsAuthentication.Decrypt(encryptedTicket);
                return !formsTicket.Expired ? formsTicket.UserData : null;
            }
            catch (Exception)
            {
                // ArgumentException & FormatException
                // encryptedTicket is null.- or -encryptedTicket is an empty string or is of an invalid format.
                return null;
            }
        }

        public static string Encrypt(this string NormalString)
        {
            return new ColEncryption().Scramble(NormalString);
        }
        public static string Decrypt(string NormalString,int check)
        {
            string retString = string.Empty;
            if (!string.IsNullOrEmpty(NormalString))
                try
                {
                    retString = new ColEncryption().Descramble(NormalString);
                }
                catch (Exception ex)
                {
                }
            return retString;
        }
        public static string EncryptInt(string inputText)
        {
            return new ColEncryption().Scramble(inputText.ToString());
        }
        public static int DecryptInt(string inputText)
        {
            int retInt = default(int);
            if (!string.IsNullOrEmpty(inputText))
                try
                {
                    int.TryParse(new ColEncryption().Descramble(inputText), out retInt);
                }
                catch (Exception ex)
                {
                }
            return retInt;
        }
        public static string DecryptHeader(HttpRequestHeaders headers, string headerkey)
        {
            var value = headers.GetValues(headerkey).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new WebApiException(ErrorList.UnableToProcess, System.Net.HttpStatusCode.PreconditionFailed);
            }

            return headers.Contains("Decrypt") && headers.GetValues("Decrypt").First().Equals("false") ? value : EncryptionHelper.AesDecryption(value.Replace("%3D", "=").Replace("%3d", "="), EncryptionKey.LOG);
        }
    }

    internal class ColEncryption
    {
        byte[] ScrambleKey = null;
        byte[] ScrambleIV = null;

        private string key = "AggieGlobalKey";

        public ColEncryption()
        {
            ScrambleKey = GetKey();
            ScrambleIV = GetKeyIV();
        }

        public ColEncryption(string Key)
        {
            this.key = Key;

            ScrambleKey = GetKey();
            ScrambleIV = GetKeyIV();
        }

        private byte[] GetKey()
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(this.key);
        }
        private byte[] GetKeyIV()
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(this.key);
        }

        public string Scramble(NameValueCollection Data)
        {
            string normalData = string.Empty;

            foreach (string itemKey in Data.Keys)
            {
                if (string.IsNullOrEmpty(normalData))
                    normalData = string.Format("{0}={1}", itemKey, Data[itemKey]);
                else
                    normalData += string.Format("&{0}={1}", itemKey, Data[itemKey]);
            }

            return Scramble(normalData);
        }

        /// <summary>
        /// Scramble a message using a session-specific key
        /// </summary>
        /// <param name="message">plaintext string</param>
        /// <returns>Base64 encoded string</returns>
        public string Scramble(string message)
        {
            UTF8Encoding textConverter = new UTF8Encoding();
            RC2CryptoServiceProvider rc2CSP = new RC2CryptoServiceProvider();

            //Convert the data to a byte array.
            byte[] toEncrypt = textConverter.GetBytes(message);

            //Get an encryptor.
            ICryptoTransform encryptor = rc2CSP.CreateEncryptor(ScrambleKey, ScrambleIV);

            //Encrypt the data.
            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            //Write all data to the crypto stream and flush it.
            // Encode length as first 4 bytes
            byte[] length = new byte[4];
            length[0] = (byte)(message.Length & 0xFF);
            length[1] = (byte)((message.Length >> 8) & 0xFF);
            length[2] = (byte)((message.Length >> 16) & 0xFF);
            length[3] = (byte)((message.Length >> 24) & 0xFF);
            csEncrypt.Write(length, 0, 4);
            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();

            //Get encrypted array of bytes.
            byte[] encrypted = msEncrypt.ToArray();

            // Convert to Base64 string
            string b64 = Convert.ToBase64String(encrypted);

            // Protect against URLEncode/Decode problem
            string b64mod = b64.Replace('+', '@');

            // Return a URL encoded string
            return HttpUtility.UrlEncode(b64mod);
        }

        /// <summary>
        /// Descramble a message in a session-specific key
        /// </summary>
        /// <param name="scrambledMessage">Base64 string</param>
        /// <returns>plaintext string</returns>
        public string Descramble(string scrambledMessage)
        {
            UTF8Encoding textConverter = new UTF8Encoding();
            RC2CryptoServiceProvider rc2CSP = new RC2CryptoServiceProvider();
            // URL decode , replace and convert from Base64
            string b64mod = HttpUtility.UrlDecode(scrambledMessage);
            // Replace '@' back to '+' (avoid URLDecode problem)
            string b64 = b64mod.Replace('@', '+');
            // Base64 decode
            byte[] encrypted = Convert.FromBase64String(b64);

            //Get a decryptor that uses the same key and IV as the encryptor.
            ICryptoTransform decryptor = rc2CSP.CreateDecryptor(ScrambleKey, ScrambleIV);

            //Now decrypt the previously encrypted message using the decryptor
            // obtained in the above step.
            MemoryStream msDecrypt = new MemoryStream(encrypted);
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            byte[] fromEncrypt = new byte[encrypted.Length - 4];

            //Read the data out of the crypto stream.
            byte[] length = new byte[4];
            csDecrypt.Read(length, 0, 4);
            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
            int len = (int)length[0] | (length[1] << 8) | (length[2] << 16) | (length[3] << 24);

            //Convert the byte array back into a string.
            return textConverter.GetString(fromEncrypt).Substring(0, len);
        }

        /// <summary>
        /// Descramble the query string and return a NameValueCollection containing key and value
        /// pairs corresponding to the original query string.
        /// </summary>
        /// <param name="scrambledMessage">scrambled query string</param>
        /// <returns>NameValue continaing query string values</returns>
        public NameValueCollection DescrambleQueryString(string scrambledMessage)
        {
            // Decode the query string
            string queryString = Descramble(scrambledMessage);
            NameValueCollection result = new NameValueCollection();
            char[] splitChar = new char[] { '&' };
            char[] equalChar = new char[] { '=' };
            // Split query string to components
            foreach (string s in queryString.Split(splitChar))
            {
                // split each component to key and value
                string[] keyVal = s.Split(equalChar, 2);
                string key = keyVal[0];
                string val = String.Empty;
                if (keyVal.Length > 1) val = keyVal[1];
                // Add to the hashtable
                result.Add(key, val);
            }

            // return the resulting hashtable
            return result;
        }
    }

    internal class EncriptionManager
    {
        private const string PARAMETER_NAME = "enc=";
        private const string ENCRYPTION_KEY = "key";
        /// <summary>
        /// Parses the current URL and extracts the virtual path without query string.
        /// </summary>
        /// <returns>The virtual path of the current URL.</returns>
        private static string GetVirtualPath()
        {
            string path = HttpContext.Current.Request.RawUrl;
            path = path.Substring(0, path.IndexOf("?"));
            path = path.Substring(path.LastIndexOf("/") + 1);
            return path;
        }

        /// <summary>
        /// Parses a URL and returns the query string.
        /// </summary>
        /// <param name="url">The URL to parse.</param>
        /// <returns>The query string without the question mark.</returns>
        private static string ExtractQuery(string url)
        {
            int index = url.IndexOf("?") + 1;
            return url.Substring(index);
        }
        #region Encryption/decryption

        /// <summary>
        /// The salt value used to strengthen the encryption.
        /// </summary>
        private readonly static byte[] SALT = Encoding.ASCII.GetBytes(ENCRYPTION_KEY.Length.ToString());

        /// <summary>
        /// Encrypts any string using the Rijndael algorithm.
        /// </summary>
        /// <param name="inputText">The string to encrypt.</param>
        /// <returns>A Base64 encrypted string.</returns>
        public static string Encrypt(string inputText)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            byte[] plainText = Encoding.Unicode.GetBytes(inputText);
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(ENCRYPTION_KEY, SALT);

            using (ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16)))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainText, 0, plainText.Length);
                        cryptoStream.FlushFinalBlock();
                        return "?" + PARAMETER_NAME + Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts a previously encrypted string.
        /// </summary>
        /// <param name="inputText">The encrypted string to decrypt.</param>
        /// <returns>A decrypted string.</returns>
        public static string Decrypt(string inputText)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            byte[] encryptedData = Convert.FromBase64String(inputText);
            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(ENCRYPTION_KEY, SALT);

            using (ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16)))
            {
                using (MemoryStream memoryStream = new MemoryStream(encryptedData))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] plainText = new byte[encryptedData.Length];
                        int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);
                        return Encoding.Unicode.GetString(plainText, 0, decryptedCount);
                    }
                }
            }
        }
        #endregion
    }

    public enum EncryptionKey
    {
        [Description("LOG")]
        LOG = 1,
        [Description("REG")]
        REG = 2,
        [Description("None")]
        None = 3
    }
}