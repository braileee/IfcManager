using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PSURevitApps.BL.Utils;
using RevitIfcManager.Json;
using RevitIfcManager.Models;
using RevitIfcManager.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RevitIfcManager.Commands
{
    [Transaction(TransactionMode.Manual)]

    public class ParametersSettingsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
               
                SettingsRoot settingsRoot = SettingsLoader.LoadExistingOrDefault();
                string excelFilePath = ExcelDataLoader.LoadExistingOrPrompt();

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

                ParametersSettingsView settingsView = new ParametersSettingsView(commandData);
                settingsView.ShowDialog();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error catched on showing start view", ex.Message);
            }

            return Result.Succeeded;
        }
    }
}
