using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using IfcManager.BL.Json;
using IfcManager.BL.Models;
using IfcManager.Settings.ViewModels;
using IfcManager.Settings.Views;
using RevitIfcManager.Models;
using RevitIfcManager.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace RevitIfcManager.RevitApp.Commands
{
    [Transaction(TransactionMode.Manual)]

    public class ParametersSettingsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
               
                SettingsRoot settingsRoot = SettingsLoader.LoadExistingOrDefault();
                string excelFilePath = ExcelDataLoader.LoadOrPromptExcelFilePath(settingsRoot.ExcelSettings.FileLinkSettings);

                if (string.IsNullOrEmpty(excelFilePath))
                {
                    MessageBox.Show($"No excel file selected", "Error");
                    return Result.Failed;
                }

                if(settingsRoot == null)
                {
                    MessageBox.Show($"No settings file found", "Error");
                    return Result.Failed;
                }

                IfcManagerSettingsView view = new IfcManagerSettingsView();
                IfcManagerSettingsViewModel viewModel = new IfcManagerSettingsViewModel();
                view.DataContext = viewModel;
                view.ShowDialog();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error catched on showing start view", ex.Message);
            }

            return Result.Succeeded;
        }
    }
}
