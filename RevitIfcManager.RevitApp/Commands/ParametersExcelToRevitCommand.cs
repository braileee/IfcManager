using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using IfcManager.BL.Json;
using IfcManager.BL.Models;
using RevitIfcManager.EventHandlers;
using RevitIfcManager.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace RevitIfcManager.RevitApp.Commands
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
                string excelFilePath = ExcelDataLoader.LoadOrPromptExcelFilePath();

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

                List<PropertySetItem> propertySetItems = ExcelDataLoader.LoadPropertySetItems(excelFilePath, settingsRoot.ExcelSettings.PropertiesSheet);

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
