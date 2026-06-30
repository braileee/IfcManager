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
            PathsViewModel = new PathsViewModel();
            PropertiesViewModel = new PropertiesViewModel(Settings);
            PicklistViewModel = new PicklistViewModel(Settings);
        }

        public SettingsRoot Settings { get; }
        public PathsViewModel PathsViewModel { get; set; }
        public PropertiesViewModel PropertiesViewModel { get; set; }
        public PicklistViewModel PicklistViewModel { get; set; }
    }
}
