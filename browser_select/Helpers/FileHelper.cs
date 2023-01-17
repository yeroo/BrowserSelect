using browser_select.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace browser_select.Helpers
{
    public class FileHelper
    {
        public static string GetSettingsFileName()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\yeroo\\browser_select\\settings.json";
        }

        public static void SaveSettingsFile(SettingsFile settings)
        {
            var settingsFileName = GetSettingsFileName();
            var settingsFileDirectory = Path.GetDirectoryName(settingsFileName);
            if (!Directory.Exists(settingsFileDirectory))
            {
                Directory.CreateDirectory(settingsFileDirectory);
            }
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(settingsFileName, json);
        }

        public static SettingsFile GetSettingsFile()
        {
            var settingsFileName = GetSettingsFileName();

            if (File.Exists(settingsFileName))
            {
                var json = File.ReadAllText(settingsFileName);
                return JsonConvert.DeserializeObject<SettingsFile>(json);
            }
            else
            {
                return new SettingsFile
                {
                    AutoAppsAndSites = new Dictionary<string, Dictionary<string, List<string>>>()
                };
            }
        }
    }
}
