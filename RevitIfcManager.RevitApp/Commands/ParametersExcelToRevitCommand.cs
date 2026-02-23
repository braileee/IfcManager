using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MullerWust.Revit.Common.Utils;
using RevitIfcManager.EventHandlers;
using RevitIfcManager.Json;
using RevitIfcManager.Models;
using RevitIfcManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RevitIfcManager.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class ParametersExcelToRevitCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Load settings
                SettingsRoot settingsRoot = SettingsLoader.LoadExistingOrDefault();

                // load excel data
                string excelFilePath = ExcelDataLoader.LoadExistingOrPrompt();

                if (string.IsNullOrEmpty(excelFilePath))
                {
                    MessageBox.Show($"No excel file selected", "Error");
                    return Result.Cancelled;
                }

                if (settingsRoot == null)
                {
                    MessageBox.Show($"No settings file found", "Error");
                    return Result.Cancelled;
                }

                List<PropertySetItem> propertySetItems = ExcelDataLoader.LoadPropertySetItems(excelFilePath, settingsRoot.ExcelSettings);

                ParametersExcelToRevitEventHandler parametersExcelToRevitEventHandler = new ParametersExcelToRevitEventHandler();
                parametersExcelToRevitEventHandler.Raise(new ParametersExcelToRevitOptions
                {
                    PropertySetItems = propertySetItems
                });
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error catched on showing start view", ex.Message);
            }

            return Result.Succeeded;
        }
    }
}
