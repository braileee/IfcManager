using Autodesk.Revit.UI;
using MullerWust.Revit.Common.Utils;
using Prism.Commands;
using Prism.Mvvm;
using PSURevitApps.BL.Utils;
using RevitIfcManager.Json;
using System;
using System.IO;

namespace RevitIfcManager.ViewModels
{
    internal class ParametersSettingsViewModel : BindableBase
    {
        public ParametersSettingsViewModel(UIApplication uiapp)
        {
            SettingsFilePath = LoadSettings();
            LoadSettingsCommand = new DelegateCommand(OnLoadSettingsCommand);

            ExcelFilePath = Properties.Settings.Default.ExcelFilePath;

            LoadExcelCommand = new DelegateCommand(OnLoadExcelCommand);
        }

        private void OnLoadExcelCommand()
        {
            string excelFilePath = FilePromptUtils.GetFilePath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Excel files (*.xlsx)|*.xlsx");

            if (string.IsNullOrEmpty(excelFilePath) || !File.Exists(excelFilePath))
            {
                return;
            }

            ExcelFilePath = excelFilePath;
        }

        private void OnLoadSettingsCommand()
        {
            string settingsFilePath = FilePromptUtils.GetFilePath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "json files (*.json)|*.json");

            if (string.IsNullOrEmpty(settingsFilePath) || !File.Exists(settingsFilePath))
            {
                return;
            }

            SettingsFilePath = settingsFilePath;
        }

        private string LoadSettings()
        {
            SettingsRoot settingsRoot = SettingsLoader.LoadExistingOrDefault();

            if(settingsRoot == null)
            {
                return "N/A";   
            }

            return Properties.Settings.Default.SettingsFilePath;
        }

        private string settingsFilePath;
        private string excelFilePath;

        public string SettingsFilePath
        {
            get { return settingsFilePath; }
            set
            {
                settingsFilePath = value;
                Properties.Settings.Default.SettingsFilePath = settingsFilePath;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public DelegateCommand LoadSettingsCommand { get; }
        public string ExcelFilePath
        {
            get
            {
                return excelFilePath;
            }
            set
            {
                excelFilePath = value;
                Properties.Settings.Default.ExcelFilePath = excelFilePath;
                Properties.Settings.Default.Save();
                RaisePropertyChanged();
            }
        }

        public DelegateCommand LoadExcelCommand { get; }

        public event EventHandler CloseRequest;

        public void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
