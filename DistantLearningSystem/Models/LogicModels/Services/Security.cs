using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Web;

namespace DistantLearningSystem.Models.LogicModels.Services
{
    public class Security
    {

        /// <summary>
        /// MD5 hash
        /// </summary>
        /// <param name="s">строку, которую кодируем в MD5 хеш</param>
        /// <returns></returns>
        public static string GetHashString(string s)
        {
            try
            {
                byte[] bytes = Encoding.Unicode.GetBytes(s);
                var csp = new MD5CryptoServiceProvider();
                byte[] byteHash = csp.ComputeHash(bytes);
                return byteHash.Aggregate(string.Empty, (current, b) => current + string.Format("{0:x2}", b));
            }
            catch
            {
                return null;
            }
        }

        public static bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
                return true;
            var formats = new[] { ".jpg", ".png", ".gif", ".jpeg" };
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
    }
}
