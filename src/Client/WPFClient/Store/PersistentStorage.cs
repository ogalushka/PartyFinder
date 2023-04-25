using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text.Json;

namespace WPFClient.Store
{
    public class PersistentStorage
    {
        private const string AppName = "PartyFinder";

        private readonly string CookiePath;
        private readonly string StoragePath;
        
        public PersistentStorage()
        {
            var rootFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            StoragePath = $"{rootFolder}/{AppName}";
            CookiePath = $"{StoragePath}/cookies";
        }

        public void SaveCookies(IEnumerable<Cookie> cookies)
        {
            Directory.CreateDirectory(StoragePath);
            ClearCookies();

            using var streamWriter = new StreamWriter(CookiePath);
            var serializedCookies = JsonSerializer.Serialize(cookies.ToArray());
            streamWriter.Write(serializedCookies);
        }

        public void ClearCookies()
        {
            try
            {
                File.Delete(CookiePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public Cookie[] GetCookies()
        {
            try
            {
                using var fileStream = new StreamReader(CookiePath);
                var s = fileStream.ReadToEnd();
                var result = JsonSerializer.Deserialize<Cookie[]>(s);
                if (result == null)
                {
                    return Array.Empty<Cookie>();
                }

                for (var i = 0; i < result.Length; i++)
                {
                    var cookie = result[i];
                    // Reacreating because deserialized cookie objects don't work
                    result[i] = new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);
                }

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Array.Empty<Cookie>();
            }
        }
    }
}
