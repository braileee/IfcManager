using IfcManager.BL.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfcManager.Settings.ViewModels
{
    public class PathsViewModel
    {
        public PathsViewModel(SettingsRoot settingsRoot)
        {
            Settings = settingsRoot;
        }

        public SettingsRoot Settings { get; }
    }
}
