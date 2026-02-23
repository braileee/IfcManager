using PSURevitApps.BL.Utils;
using RevitIfcManager.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RevitIfcManager.Json
{
    public static class SettingsLoader
    {
        public static SettingsRoot Load(string filePath)
        {
            string json = File.ReadAllText(filePath);

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            SettingsRoot settingsRoot = JsonSerializer.Deserialize<SettingsRoot>(json, options);

            Properties.Settings.Default.SettingsFilePath = filePath;
            Properties.Settings.Default.Save();

            return settingsRoot;
        }

        public static SettingsRoot LoadExistingOrDefault()
        {
            string assemblyFolder = AssemblyUtils.GetFolder(typeof(ParametersSettingsCommand));
            string defaultSettingsPath = Path.Combine(assemblyFolder, "Files", "RevitIfcManager", "Settings.json");
            string currentSettingsPath = Properties.Settings.Default.SettingsFilePath;

            if (string.IsNullOrEmpty(currentSettingsPath) || !File.Exists(currentSettingsPath))
            {
                currentSettingsPath = defaultSettingsPath;
            }

            if (!File.Exists(currentSettingsPath))
            {
                return null;
            }

            return Load(currentSettingsPath);
        }
    }
}
