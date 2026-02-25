using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using IfcManager.BL;
using IfcManager.BL.Json;
using IfcManager.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RevitIfcManager.RevitApp.Commands
{
    [Transaction(TransactionMode.Manual)]

    public class ParametersTagElementsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                SettingsRoot settingsRoot = SettingsLoader.LoadExistingOrDefault();

                // load excel data
                string excelFilePath = ExcelDataLoader.LoadOrPromptExcelFilePath();

                if (string.IsNullOrEmpty(excelFilePath))
                {
                    MessageBox.Show($"No excel file selected", "Error");
                    return Result.Cancelled;
                }

                if(settingsRoot == null)
                {
                    MessageBox.Show($"No settings file found", "Error");
                    return Result.Cancelled;
                }

                List<PropertySetItem> propertySetItems = ExcelDataLoader.LoadPropertySetItems(excelFilePath, settingsRoot.ExcelSettings);
                List<PicklistGroup> picklistGroups = ExcelDataLoader.LoadPicklistGroups(excelFilePath, settingsRoot.ExcelSettings);

                Dictionary<string, List<string>> propertiesWithValues = picklistGroups.ToDictionary(item => item.GroupName, item => item.Values);

                List<PropertyItem> properties = propertySetItems.SelectMany(item => item.PropertyItems).ToList();

                DockablePaneId id = new DockablePaneId(new Guid(Constants.ParametersTagElementsPaneId));
                DockablePane dockableWindow = commandData.Application.GetDockablePane(id);

                if (dockableWindow.IsShown())
                {
                    dockableWindow.Hide();
                }
                else
                {
                    dockableWindow.Show();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error catched", ex.Message);
            }

            return Result.Succeeded;
        }
    }
}
