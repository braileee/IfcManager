using Autodesk.Revit.UI;
using IfcManager.BL.Json;
using IfcManager.BL.Models;
using IfcManager.Utils;
using Prism.Commands;
using Prism.Mvvm;
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

            ExcelFilePath = ExcelDataLoader.LoadOrPromptExcelFilePath();

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

            return SettingsLoader.GetPath();
        }

        private string settingsFilePath;
        private string excelFilePath;

        public string SettingsFilePath
        {
            get { return settingsFilePath; }
            set
            {
                settingsFilePath = value;
                SettingsLoader.SavePath(value); 
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
                ExcelDataLoader.SavePath(value);
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
