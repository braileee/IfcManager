using IfcManager.BL.Json;
using IfcManager.BL.Models;
using MyApp.ViewModels;
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
            ExactMatchesViewModel = new ExactMatchesViewModel(Settings);
            MatchesViewModel = new MatchesViewModel(Settings);
            ExpressionEditorViewModel = new ExpressionEditorViewModel(Settings);
        }

        public SettingsRoot Settings { get; }
        public PathsViewModel PathsViewModel { get; set; }
        public PropertiesViewModel PropertiesViewModel { get; set; }
        public PicklistViewModel PicklistViewModel { get; set; }
        public ExactMatchesViewModel ExactMatchesViewModel { get; set; }
        public MatchesViewModel MatchesViewModel { get; }
        public ExpressionEditorViewModel ExpressionEditorViewModel { get; }
    }
}
