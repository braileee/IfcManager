using IfcManager.BL.Enums;
using IfcManager.BL.Json;
using IfcManager.Json;
using IfcManager.Models;
using IfcManager.Utils;
using NPOI.SS.UserModel;
using NPOI.Util.Collections;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfcManager.BL.Models
{
    public static class ExcelDataLoader
    {
        public static List<PropertySetItem> LoadPropertySetItems(string filePath, ExcelSettings settings)
        {
            var rows = new List<PropertyDataRow>();

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);
                ISheet sheet = workbook.GetSheet(settings.PropertiesSheetName);

                if (sheet == null)
                {
                    return new List<PropertySetItem>();
                }

                // Assuming first row is header
                int headerRowIndex = settings.HeaderRowIndex;
                IRow headerRow = sheet.GetRow(headerRowIndex);

                int colSet = headerRow.Cells.FindIndex(c => c.StringCellValue == settings.PropertySetNameColumn);
                int colName = headerRow.Cells.FindIndex(c => c.StringCellValue == settings.PropertyNameColumn);
                int colType = headerRow.Cells.FindIndex(c => c.StringCellValue == settings.DataTypeColumn);

                for (int i = headerRowIndex + 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    var item = new PropertyDataRow
                    {
                        PropertySetName = row.GetCell(colSet)?.ToString()?.Trim(),
                        PropertyName = row.GetCell(colName)?.ToString()?.Trim(),
                        DataType = row.GetCell(colType)?.ToString()?.Trim()
                    };

                    // Skip empty rows
                    if (string.IsNullOrWhiteSpace(item.PropertySetName) &&
                        string.IsNullOrWhiteSpace(item.PropertyName))
                        continue;

                    rows.Add(item);
                }
            }


            var propertySets = rows.GroupBy(r => r.PropertySetName).Select(g => new PropertySetItem
            {
                PropertySetName = g.Key,
                PropertyDefinitions = g
                                        .Select(r => new PropertyItem
                                        {
                                            PropertyName = r.PropertyName,
                                            DataType = r.DataType
                                        })
                                        .ToList()
            })
                                .ToList();

            return propertySets;

        }

        public static List<PicklistGroup> ReadAllGroups(string filePath, ExcelSettings settings)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);

                ISheet sheet = workbook.GetSheet(settings.PicklistSheetName);
                if (sheet == null)
                    return new List<PicklistGroup>();

                // --- Header row ---
                IRow headerRow = sheet.GetRow(0);
                if (headerRow == null)
                    return new List<PicklistGroup>();

                int columnCount = headerRow.LastCellNum;

                // --- Prepare groups ---
                var groups = new Dictionary<int, PicklistGroup>();

                for (int c = 0; c < columnCount; c++)
                {
                    string header = headerRow.GetCell(c)?.ToString()?.Trim();
                    if (string.IsNullOrWhiteSpace(header))
                        continue;

                    groups[c] = new PicklistGroup
                    {
                        GroupName = header
                    };
                }

                // --- Read data rows ---
                for (int r = 1; r <= sheet.LastRowNum; r++)
                {
                    IRow row = sheet.GetRow(r);
                    if (row == null)
                        continue;

                    foreach (var kvp in groups)
                    {
                        int colIndex = kvp.Key;
                        PicklistGroup group = kvp.Value;

                        string value = row.GetCell(colIndex)?.ToString()?.Trim();
                        if (!string.IsNullOrWhiteSpace(value))
                            group.Values.Add(value);
                    }
                }

                // --- Deduplicate & sort ---
                foreach (var group in groups.Values)
                {
                    group.Values = group.Values
                        .Distinct()
                        .OrderBy(v => v)
                        .ToList();
                }

                return groups.Values.ToList();
            }

        }

        public static List<LayerMappingItem> ReadLayerMappings(
                                           string filePath,
                                          ExcelSettings settings,
                                           string layerNameColumn)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);

                ISheet sheet = workbook.GetSheet(settings.MicrostationLayersMappingSheetName);
                if (sheet == null)
                    return new List<LayerMappingItem>();

                IRow headerRow = sheet.GetRow(0);
                if (headerRow == null)
                    return new List<LayerMappingItem>();

                int columnCount = headerRow.LastCellNum;

                // Map column index → column name
                var columns = new Dictionary<int, string>();
                int layerNameColIndex = -1;

                for (int c = 0; c < columnCount; c++)
                {
                    string header = headerRow.GetCell(c)?.ToString()?.Trim();
                    if (string.IsNullOrWhiteSpace(header))
                        continue;

                    columns[c] = header;

                    if (header == layerNameColumn)
                        layerNameColIndex = c;
                }

                if (layerNameColIndex == -1)
                    return new List<LayerMappingItem>();

                var result = new List<LayerMappingItem>();

                // Read rows
                for (int r = 1; r <= sheet.LastRowNum; r++)
                {
                    IRow row = sheet.GetRow(r);
                    if (row == null)
                        continue;

                    string layerName = row.GetCell(layerNameColIndex)?.ToString()?.Trim();
                    if (string.IsNullOrWhiteSpace(layerName))
                        continue;

                    var item = new LayerMappingItem
                    {
                        LayerName = layerName
                    };

                    foreach (var col in columns)
                    {
                        if (col.Key == layerNameColIndex)
                            continue;

                        string value = row.GetCell(col.Key)?.ToString()?.Trim();
                        if (!string.IsNullOrWhiteSpace(value))
                            item.PropertiesWithValues[col.Value] = value;
                    }

                    result.Add(item);
                }

                return result;
            }
        }

        public static List<PropertyValueMatch> LoadPropertiesValueMatches(string filePath, ExcelSettings settings)
        {
            List<PropertyValueMatch> propertyValueMatches = new List<PropertyValueMatch>();

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);

                List<string> sheetNames = settings.PropertyMatchSheets;

                foreach (string sheetName in sheetNames)
                {
                    ISheet sheet = workbook.GetSheet(sheetName);
                    if (sheet == null)
                    {
                        continue;
                    }

                    // Read header row
                    IRow header = sheet.GetRow(0);
                    string propertyNameSource = header.GetCell(0).ToString();
                    string propertyNameTarget = header.GetCell(1).ToString();

                    // Iterate rows
                    for (int i = 1; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;

                        string valueSource = row.GetCell(0)?.ToString();
                        string valueTarget = row.GetCell(1)?.ToString();

                        if (string.IsNullOrWhiteSpace(valueSource) || string.IsNullOrWhiteSpace(valueTarget))
                            continue;

                        propertyValueMatches.Add(new PropertyValueMatch
                        {
                            PropertyNameSource = propertyNameSource,
                            PropertyValueSource = valueSource,
                            PropertyNameTarget = propertyNameTarget,
                            PropertyValueTarget = valueTarget
                        });
                    }
                }
            }

            return propertyValueMatches;
        }

        public static string LoadOrPromptExcelFilePath()
        {
            string excelFilePath = Properties.Settings.Default.ExcelFilePath;

            if (string.IsNullOrEmpty(excelFilePath) || !File.Exists(excelFilePath))
            {
                string assemblyFolder = AssemblyUtils.GetFolder(typeof(ExcelDataLoader));
                excelFilePath = System.IO.Path.Combine(assemblyFolder, Constants.FilesFolder, Constants.IfcManagerFolder, Constants.ExcelSettingsFileName);
                Properties.Settings.Default.ExcelFilePath = excelFilePath;
                Properties.Settings.Default.Save();
            }

            if (string.IsNullOrEmpty(excelFilePath) || !File.Exists(excelFilePath))
            {
                excelFilePath = FilePromptUtils.GetFilePath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*");
                Properties.Settings.Default.ExcelFilePath = excelFilePath;
                Properties.Settings.Default.Save();
            }

            return excelFilePath;
        }

        public static List<ExpressionItem> LoadExpressions(string excelFilePath, ExcelSettings excelSettings)
        {
            var expressions = new List<ExpressionItem>();
            using (var fs = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);
                ISheet sheet = workbook.GetSheet(excelSettings.ExpressionsSheetName);
                if (sheet == null)
                    return expressions;
                // Assuming first row is header
                int headerRowIndex = excelSettings.HeaderRowIndex;
                IRow headerRow = sheet.GetRow(headerRowIndex);
                int colSourceProperty = headerRow.Cells.FindIndex(c => c.StringCellValue == "Source Property Name");
                int colTargetProperty = headerRow.Cells.FindIndex(c => c.StringCellValue == "Target Property Name");
                int colFunctionType = headerRow.Cells.FindIndex(c => c.StringCellValue == "Function");
                int colValue = headerRow.Cells.FindIndex(c => c.StringCellValue == "Value");
                for (int i = headerRowIndex + 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    string sourcePropertyName = row.GetCell(colSourceProperty)?.ToString()?.Trim();
                    string targetPropertyName = row.GetCell(colTargetProperty)?.ToString()?.Trim();
                    string functionTypeStr = row.GetCell(colFunctionType)?.ToString()?.Trim();
                    string value = row.GetCell(colValue)?.ToString()?.Trim();
                    if (string.IsNullOrWhiteSpace(sourcePropertyName) || string.IsNullOrWhiteSpace(targetPropertyName))
                        continue;
                    ExpressionFunctionType functionType;
                    if (!Enum.TryParse(functionTypeStr, out functionType))
                    {
                        functionType = ExpressionFunctionType.Undefined;
                    }
                    expressions.Add(new ExpressionItem
                    {
                        SourcePropertyName = sourcePropertyName,
                        TargetPropertyName = targetPropertyName,
                        ExpressionFunctionType = functionType,
                        Value = value
                    });
                }
            }
            return expressions;
        }

        public static List<ComposedPropertyItem> LoadComposed(string excelFilePath, ExcelSettings excelSettings)
        {
            var composedItems = new List<ComposedPropertyItem>();
            using (var fs = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);
                ISheet sheet = workbook.GetSheet(excelSettings.ComposedSheetName);

                if (sheet == null)
                {
                    return composedItems;
                }

                int headerRowIndex = excelSettings.HeaderRowIndex;
                IRow headerRow = sheet.GetRow(headerRowIndex);
                int cellIndexName = headerRow.Cells.FindIndex(c => c.StringCellValue == "Composed Property Name");
                int cellIndexFormula = headerRow.Cells.FindIndex(c => c.StringCellValue == "Formula");

                for (int i = headerRowIndex + 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }

                    string valuePropertyName = row.GetCell(cellIndexName)?.ToString()?.Trim();
                    string valueFormula = row.GetCell(cellIndexFormula)?.ToString()?.Trim();

                    if (string.IsNullOrEmpty(valuePropertyName) || string.IsNullOrEmpty(valueFormula))
                    {
                        continue;
                    }

                    composedItems.Add(new ComposedPropertyItem
                    {
                        ComposedPropertyName = valuePropertyName,
                        Formula = valueFormula
                    });
                }
            }
            return composedItems;
        }

        public static List<PropertyValueMatch> LoadPropertiesExactValueMatches(string filePath, ExcelSettings settings)
        {
            List<PropertyValueMatch> propertyValueMatches = new List<PropertyValueMatch>();

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);

                List<string> sheetNames = settings.PropertyExactMatchSheets;

                foreach (string sheetName in sheetNames)
                {
                    ISheet sheet = workbook.GetSheet(sheetName);
                    if (sheet == null)
                    {
                        continue;
                    }

                    // Read header row
                    IRow header = sheet.GetRow(0);
                    string propertyNameSource = header.GetCell(0).ToString();
                    string propertyNameTarget = header.GetCell(1).ToString();

                    // Iterate rows
                    for (int i = 1; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;

                        string valueSource = row.GetCell(0)?.ToString();
                        string valueTarget = row.GetCell(1)?.ToString();

                        if (string.IsNullOrWhiteSpace(valueSource) || string.IsNullOrWhiteSpace(valueTarget))
                            continue;

                        propertyValueMatches.Add(new PropertyValueMatch
                        {
                            PropertyNameSource = propertyNameSource,
                            PropertyValueSource = valueSource,
                            PropertyNameTarget = propertyNameTarget,
                            PropertyValueTarget = valueTarget
                        });
                    }
                }
            }

            return propertyValueMatches;
        }

        public static void SavePath(string path)
        {
            Properties.Settings.Default.ExcelFilePath = path;
            Properties.Settings.Default.Save();
        }
    }
}
