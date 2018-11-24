using Newtonsoft.Json;
using System;
using System.Linq;
using System.Windows.Forms;

namespace VgcApis.Libs
{
    public static class Utils
    {
        #region Json
        /// <summary>
        /// return null if fail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string content) where T : class
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            try
            {
                var result = JsonConvert.DeserializeObject<T>(content);
                if (result != null)
                {
                    return result;
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// return null if fail
        /// </summary>
        /// <param name="serializeObject"></param>
        /// <returns></returns>
        public static string SerializeObject(object serializeObject)
        {
            if (serializeObject == null)
            {
                return null;
            }
            return JsonConvert.SerializeObject(serializeObject);
        }

        /// <summary>
        /// return null if fail
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a"></param>
        /// <returns></returns>
        public static T Clone<T>(T a) where T : class
        {
            if (a == null)
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(
                    JsonConvert.SerializeObject(a));
            }
            catch { }
            return null;
        }
        #endregion


        #region Misc
        public static bool CopyToClipboard(string content)
        {
            try
            {
                Clipboard.SetText(content);
                return true;
            }
            catch { }
            return false;
        }

        public static string GetAppDir()
        {
            // The code below will fail in test.
            // return Path.GetDirectoryName(Application.ExecutablePath);

            // https://stackoverflow.com/questions/6041332/best-way-to-get-application-folder-path/35295609
            return System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static int Clamp(int value, int min, int max)
        {
            return Math.Max(Math.Min(value, max - 1), min);
        }

        public static string RandomHex(int length)
        {
            //  https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings-in-c
            if (length <= 0)
            {
                return string.Empty;
            }

            Random random = new Random();
            const string chars = "0123456789abcdef";
            return new string(
                Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
        }

        public static int Str2Int(string value)
        {
            if (float.TryParse(value, out float f))
            {
                return (int)Math.Round(f);
            };
            return 0;
        }
        #endregion
    }
}
