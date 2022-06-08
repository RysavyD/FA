using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using _3F.Model.Model;

namespace _3F.Web.Utils
{
    public class Utilities
    {
        public static bool Validate(OldPassword oldLogin, string password)
        {
            using (var hmac = new HMACSHA512(oldLogin.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (oldLogin.PasswordHash[i] != computedHash[i])
                        return false;
                }
            }
            return true;
        }

        public static string Crypt(string toCrypt)
        {
            RijndaelManaged aes = new RijndaelManaged();

            // Nastavit CBC block mode
            aes.Mode = CipherMode.CBC;

            // Nastavit standardní PKCS7 padding posledního bloku
            aes.Padding = PaddingMode.PKCS7;

            // Vygenerovat náhodný klíč
            aes.Key = Convert.FromBase64String("JBZZ4e4gIL71jigI2rg4lGsiP/2B/i78ujUkoeTx3l8=");

            // Vygenerovat inicializační vektor (IV)
            aes.IV = Convert.FromBase64String("DYRjyZT85IMEVKL4KSnR9g==");

            // Převést text na pole bajtů
            byte[] plainData = Encoding.UTF8.GetBytes(toCrypt);

            // Zašifrovat data
            byte[] cipherData;
            using (ICryptoTransform enc = aes.CreateEncryptor())
            {
                cipherData = enc.TransformFinalBlock(plainData, 0, plainData.Length);
            }

            return Convert.ToBase64String(cipherData);
        }

        public static string Decrypt(string toDecrypt)
        {
            // Vytvořit instanci AES/Rijdael algoritmu
            RijndaelManaged aes = new RijndaelManaged();

            // Nastavit CBC block mode
            aes.Mode = CipherMode.CBC;

            // Nastavit standardní PKCS7 padding posledního bloku
            aes.Padding = PaddingMode.PKCS7;

            // Vygenerovat náhodný klíč
            aes.Key = Convert.FromBase64String("JBZZ4e4gIL71jigI2rg4lGsiP/2B/i78ujUkoeTx3l8=");

            // Vygenerovat inicializační vektor (IV)
            aes.IV = Convert.FromBase64String("DYRjyZT85IMEVKL4KSnR9g==");

            // Načíst  šifrovaná data
            byte[] cipherData = Convert.FromBase64String(toDecrypt);

            // Dešifrovat data
            byte[] plainData;
            using (ICryptoTransform dec = aes.CreateDecryptor())
            {
                plainData = dec.TransformFinalBlock(cipherData, 0, cipherData.Length);
            }

            return Encoding.UTF8.GetString(plainData);
        }

        public static string Url(string url)
        {
            return Url(url, string.Empty);
        }

        public static string Url(string url, bool toAbsolute)
        {
            return Url(url, string.Empty, toAbsolute);
        }

        public static string Url(string url, HttpContextBase context)
        {
            return Url(url, string.Empty, context, false);
        }

        public static string Url(string url, HttpContextBase context, bool toAbsolute)
        {
            return Url(url, string.Empty, context, toAbsolute);
        }

        public static string Url(string url, string parameter)
        {
            return Url(url, parameter, HttpContext.Current.Request.RequestContext.HttpContext, false);
        }

        public static string Url(string url, string parameter, bool toAbsolute)
        {
            return Url(url, parameter, HttpContext.Current.Request.RequestContext.HttpContext, toAbsolute);
        }

        public static string Url(string url, string parameter, HttpContextBase context)
        {
            return Url(url, parameter, context, false);
        }

        public static string Url(string url, string parameter, HttpContextBase context, bool toAbsolute)
        {
            var path = UrlHelper.GenerateContentUrl(url + parameter, context);
            var uri = new Uri(HttpContext.Current.Request.Url, path);

            return toAbsolute ? uri.AbsoluteUri : path;
        }
    }
}