using IfcValidator.Json;
using IfcValidator.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IfcValidator.Models
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
            string defaultSettingsPath = GetSettingsFilePath();
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

        public static string GetSettingsFilePath()
        {
            string assemblyFolder = AssemblyUtils.GetFolder(typeof(SettingsLoader));
            string defaultSettingsPath = Path.Combine(assemblyFolder, "Files", "Settings.json");
            return defaultSettingsPath;
        }
    }
}
