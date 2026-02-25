using IfcManager.BL.Json;
using IfcManager.Utils;
using Newtonsoft.Json;
using System.IO;

namespace IfcManager.BL.Models
{
    public static class SettingsLoader
    {
        public static SettingsRoot Load(string filePath)
        {
            string json = File.ReadAllText(filePath);

            SettingsRoot settingsRoot = JsonConvert.DeserializeObject<SettingsRoot>(json);

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

        public static string GetPath()
        {
            return Properties.Settings.Default.SettingsFilePath;
        }

        public static void SavePath(string path)
        {
            Properties.Settings.Default.SettingsFilePath = path;
            Properties.Settings.Default.Save();
        }


        public static string GetSettingsFilePath()
        {
            string assemblyFolder = AssemblyUtils.GetFolder(typeof(SettingsLoader));
            string defaultSettingsPath = Path.Combine(assemblyFolder, Constants.FilesFolder, Constants.IfcManagerFolder, Constants.JsonSettingsFileName);
            return defaultSettingsPath;
        }
    }
}
