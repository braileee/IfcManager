using IfcManager.BL.Json;
using IfcManager.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfcManager.Settings.ViewModels
{
    public class IfcManagerSettingsViewModel
    {
        public IfcManagerSettingsViewModel()
        {
            Settings = SettingsLoader.LoadExistingOrDefault();
            PathsViewModel = new PathsViewModel(Settings);
            PropertiesViewModel = new PropertiesViewModel(Settings);
        }

        public SettingsRoot Settings { get; }
        public PathsViewModel PathsViewModel { get; }
        public PropertiesViewModel PropertiesViewModel { get; }
    }
}
