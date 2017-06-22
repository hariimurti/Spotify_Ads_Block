using System;
using System.Collections.Generic;
using System.IO;

namespace Spotify_Ads_Block
{
    class Preferences
    {
        private static string PATH_USER = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        private static string FILE_PREFS = Path.Combine(PATH_USER, "AppData\\Roaming\\Spotify\\prefs");

        public static bool isExist()
        {
            return File.Exists(FILE_PREFS);
        }

        public static string Get_StorageSize()
        {
            if (isExist())
            {
                foreach(string line in File.ReadAllLines(FILE_PREFS))
                {
                    if (line.Contains("storage.size="))
                    {
                        return line.Replace("storage.size=", "");
                    }
                }
            }
            return "1000";
        }

        public static bool Set_StorageSize(string size)
        {
            if (isExist())
            {
                bool isSet = false;
                List<string> prefs = new List<string>();
                foreach (string line in File.ReadAllLines(FILE_PREFS))
                {
                    if (line.StartsWith("storage.size="))
                    {
                        prefs.Add($"storage.size={size}");
                        isSet = true;
                    }
                    else if (line != string.Empty)
                    {
                        prefs.Add(line);
                    }
                }
                if (!isSet) prefs.Add($"storage.size={size}");

                File.WriteAllLines(FILE_PREFS, prefs);
                return true;
            }
            return false;
        }
    }
}
