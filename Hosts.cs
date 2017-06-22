using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Spotify_Ads_Block
{
    class Hosts
    {
        private static string PATH_SYSTEM32 = Environment.GetFolderPath(Environment.SpecialFolder.System);
        private static string FILE_HOSTS = Path.Combine(PATH_SYSTEM32, @"drivers\etc\hosts");

        private static List<string> ReadFile()
        {
            bool pause = false;
            string lastString = string.Empty;
            List<string> result = new List<string>();
            foreach (string line in File.ReadAllLines(FILE_HOSTS))
            {
                if (line.Contains("#spotify-start"))
                    pause = true;

                if (!pause && (lastString != line))
                {
                    result.Add(line);
                    lastString = line;
                }

                if (line.Contains("#spotify-end"))
                    pause = false;
            }

            if (lastString == string.Empty)
                result.RemoveAt(result.Count - 1);

            return result;
        }

        public static void Block(List<string> adslist)
        {
            Modify(true);

            List<string> currentList = ReadFile();

            // remove duplicate
            List<string> cleanList = new List<string>();
            foreach (string line in adslist)
            {
                bool isDuplicate = false;
                foreach (string text in currentList)
                {
                    if (line == text)
                        isDuplicate = true;
                }
                if (!isDuplicate)
                    cleanList.Add(line);
            }

            // merge into current
            currentList.Add("");
            currentList.Add("#spotify-start");
            currentList.AddRange(cleanList);
            currentList.Add("#spotify-end");

            File.WriteAllLines(FILE_HOSTS, currentList);

            Modify(false);
            FlushDNS();
        }

        public static void Reset()
        {
            Modify(true);

            File.WriteAllLines(FILE_HOSTS, ReadFile());

            Modify(false);
            FlushDNS();
        }

        private static void Modify(bool state)
        {
            FileAttributes attributes = File.GetAttributes(FILE_HOSTS);
            if (state)
            {
                // make the file RW
                File.SetAttributes(FILE_HOSTS, attributes & ~FileAttributes.ReadOnly);
            }
            else
            {
                // make the file RO
                File.SetAttributes(FILE_HOSTS, attributes | FileAttributes.ReadOnly);
            }
        }

        private static void FlushDNS()
        {
            ProcessStartInfo prog = new ProcessStartInfo();
            prog.FileName = Path.Combine(PATH_SYSTEM32, "ipconfig.exe");
            prog.Arguments = "/flushdns";
            prog.WindowStyle = ProcessWindowStyle.Hidden;
            Process exec = Process.Start(prog);
            exec.WaitForExit();
            exec.Dispose();
        }
    }
}
