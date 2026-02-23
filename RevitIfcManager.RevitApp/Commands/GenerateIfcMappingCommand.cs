using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MullerWust.Revit.Common.Utils;
using RevitIfcManager.Json;
using RevitIfcManager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RevitIfcManager.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class GenerateIfcMappingCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Load settings
                string settingsFilePath = Properties.Settings.Default.SettingsFilePath;

                SettingsRoot settingsRoot = SettingsLoader.Load(settingsFilePath);

                // load excel data

                string excelFilePath = Properties.Settings.Default.ExcelFilePath;

                if (string.IsNullOrEmpty(Properties.Settings.Default.ExcelFilePath))
                {
                    excelFilePath = FilePromptUtils.GetFilePath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*");
                    Properties.Settings.Default.ExcelFilePath = excelFilePath;
                    Properties.Settings.Default.Save();
                }

                if (string.IsNullOrEmpty(excelFilePath))
                {
                    return Result.Cancelled;
                }

                List<PropertySetItem> propertyDefinitions = ExcelDataLoader.LoadPropertySetItems(excelFilePath, settingsRoot.ExcelSettings);

                string mappingFilePath = FilePromptUtils.SaveFileToFolder(Environment.SpecialFolder.Desktop, "IfcMapping", "txt");

                IfcMappingContent ifcMappingContent = new IfcMappingContent(propertyDefinitions);
                string mappingData = ifcMappingContent.ToRevitTxtFormat();

                System.IO.File.WriteAllText(mappingFilePath, mappingData, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error catched on showing start view", ex.Message);
            }

            return Result.Succeeded;
        }
    }
}
