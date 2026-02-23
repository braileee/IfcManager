using Newtonsoft.Json;
using System.IO;

namespace MicrostationIfcManager.Json
{
    public static class SettingsLoader
    {
        public static SettingsRoot Load(string filePath)
        {
            var json = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<SettingsRoot>(json);
        }
    }
}
