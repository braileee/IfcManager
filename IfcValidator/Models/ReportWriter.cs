using IfcManager.BL.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfcValidator.Models
{
    public class ReportWriter
    {
        public ReportWriter(List<IfcFile> ifcFiles, string reportFilePath, List<PropertySetItem> propertySetItems, List<PicklistGroup> picklistGroups, List<PropertyValueMatch> propertyValueMatches, List<ExpressionItem> expressions, List<LayerMappingItem> layerMappingItems, List<ComposedPropertyItem> composedPropertyItems)
        {
            IfcFiles = ifcFiles;
            ReportFilePath = reportFilePath;
            PropertySetItems = propertySetItems;
            PicklistGroups = picklistGroups;
            PropertyValueMatches = propertyValueMatches;
            Expressions = expressions;
            LayerMappingItems = layerMappingItems;
            ComposedPropertyItems = composedPropertyItems;
        }

        public List<IfcFile> IfcFiles { get; } = new List<IfcFile>();
        public string ReportFilePath { get; }
        public List<PropertySetItem> PropertySetItems { get; } = new List<PropertySetItem>();
        public List<PicklistGroup> PicklistGroups { get; } = new List<PicklistGroup>();
        public List<PropertyValueMatch> PropertyValueMatches { get; } = new List<PropertyValueMatch>();
        public List<ExpressionItem> Expressions { get; } = new List<ExpressionItem>();
        public List<LayerMappingItem> LayerMappingItems { get; }
        public List<ComposedPropertyItem> ComposedPropertyItems { get; }

        public void Create()
        {
            if (IfcFiles.Count == 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(ReportFilePath))
            {
                return;
            }

            IfcFileDataFilter ifcFileDataFilter = new IfcFileDataFilter(IfcFiles, PropertySetItems, PicklistGroups);

            List<IfcFile> ifcFilesMissingProperties = ifcFileDataFilter.GetMissingPropertiesData(IfcFiles, PropertySetItems);

            List<IfcFile> filteredIfcFiles = ifcFileDataFilter.GetPerPropertySetItems();
            List<IfcFile> ifcFileWithEmptyValues = ifcFileDataFilter.GetWithEmptyValues(filteredIfcFiles);
            List<IfcFile> ifcFilesPicklistCheck = ifcFileDataFilter.GetPicklistCheck(filteredIfcFiles, PicklistGroups);

            List<IfcFile> ifcFileNonMatchList = ifcFileDataFilter.GetNonMatchData(filteredIfcFiles, PropertyValueMatches);
            List<IfcFile> ifcFileExpressions = ifcFileDataFilter.GetWrongExpressions(filteredIfcFiles, Expressions);
            List<IfcFile> wrongMappings = ifcFileDataFilter.GetWrongLayerMappings(filteredIfcFiles, LayerMappingItems);
            List<IfcFile> wrongComposedData = ifcFileDataFilter.GetWrongComposedData(filteredIfcFiles, ComposedPropertyItems);

            IWorkbook workbook = new XSSFWorkbook();

            List<string> mainHeaders =
                        new List<string>
                        {
                            "FilePath", "Guid", "IfcEntity", "Tag", "Layer",
                            "PropertySetName", "PropertyName", "Value"
                        };

            List<string> picklistHeaders = mainHeaders.ToList();
            picklistHeaders.Add("Is From Picklist");

            ISheet allPropertiesSheet = workbook.CreateSheet("All Properties");
            WriteAllData(allPropertiesSheet, mainHeaders, IfcFiles);

            ISheet missedPropertiesSheet = workbook.CreateSheet("Missed properties");
            WriteAllData(missedPropertiesSheet, mainHeaders, ifcFilesMissingProperties);

            ISheet filteredPropertiesSheet = workbook.CreateSheet("Filtered Properties");
            WriteAllData(filteredPropertiesSheet, mainHeaders, filteredIfcFiles);

            ISheet emptyValuesSheet = workbook.CreateSheet("Empty Values Data");
            WriteAllData(emptyValuesSheet, mainHeaders, ifcFileWithEmptyValues);

            ISheet picklistCheckSheet = workbook.CreateSheet("Picklist Check");
            WriteAllDataPicklist(picklistCheckSheet, picklistHeaders, ifcFilesPicklistCheck);

            ISheet nonMatchSheet = workbook.CreateSheet("Picklist Groups Check");
            WriteAllData(nonMatchSheet, mainHeaders, ifcFileNonMatchList);

            ISheet wrongExpressionsShseet = workbook.CreateSheet("Wrong Expressions");
            WriteAllData(wrongExpressionsShseet, mainHeaders, ifcFileExpressions);

            ISheet wrongLayerMappings = workbook.CreateSheet("Wrong layer mappings");
            WriteAllData(wrongLayerMappings, mainHeaders, wrongMappings);

            // --- Save to disk ---
            using (var fs = new FileStream(ReportFilePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }

            workbook.Close();

        }

        private void WriteAllData(ISheet sheet, List<string> headers, List<IfcFile> ifcFiles)
        {
            // --- Header row ---
            int rowIndex = AddHeaders(sheet, headers);

            foreach (var file in ifcFiles)
            {
                if (file?.IfcElements == null) continue;

                foreach (var element in file.IfcElements)
                {
                    // If element has no properties, still output a row (optional, but often useful)
                    var props = element?.IfcProperties;
                    if (props == null || props.Count == 0)
                    {
                        var row = sheet.CreateRow(rowIndex++);
                        WriteRow(row, file, element, psetName: null, propName: null, value: null);
                        continue;
                    }

                    // One row per property
                    foreach (var prop in props)
                    {
                        var row = sheet.CreateRow(rowIndex++);
                        WriteRow(
                            row,
                            file,
                            element,
                            prop?.PropertySetName,
                            prop?.PropertyName,
                            prop?.Value
                        );
                    }
                }
            }

            FormatSheet(sheet, headers, rowIndex);
        }

        private void WriteAllDataPicklist(ISheet sheet, List<string> headers, List<IfcFile> ifcFiles)
        {
            // --- Header row ---
            int rowIndex = AddHeaders(sheet, headers);

            foreach (var file in ifcFiles)
            {
                if (file?.IfcElements == null) continue;

                foreach (var element in file.IfcElements)
                {
                    // If element has no properties, still output a row (optional, but often useful)
                    var props = element?.IfcProperties;
                    if (props == null || props.Count == 0)
                    {
                        var row = sheet.CreateRow(rowIndex++);
                        WriteRow(row, file, element, psetName: null, propName: null, value: null);
                        continue;
                    }

                    // One row per property
                    foreach (var prop in props)
                    {
                        var row = sheet.CreateRow(rowIndex++);
                        WriteRow(
                            row,
                            file,
                            element,
                            prop?.PropertySetName,
                            prop?.PropertyName,
                            prop?.Value,
                            prop.IsValueFromPicklist
                        );
                    }
                }
            }

            FormatSheet(sheet, headers, rowIndex);
        }

        private static int AddHeaders(ISheet sheet, List<string> headers)
        {
            var headerRow = sheet.CreateRow(0);
            for (int i = 0; i < headers.Count; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
            }

            int rowIndex = 1;
            return rowIndex;
        }

        private static void FormatSheet(ISheet sheet, List<string> headers, int rowIndex)
        {
            // --- Make it readable ---
            sheet.CreateFreezePane(0, 1); // Freeze header row


            int lastRow = rowIndex - 1;           // last row with data
            int lastCol = headers.Count - 1;     // last column index

            // Apply AutoFilter to header + data
            sheet.SetAutoFilter(new NPOI.SS.Util.CellRangeAddress(0, lastRow, 0, lastCol));

            for (int i = 0; i < headers.Count; i++)
                sheet.AutoSizeColumn(i);
        }

        private static void WriteRow(
       IRow row,
       IfcFile file,
       IfcElement element,
       string psetName,
       string propName,
       object value)
        {
            // Column order:
            // 0 FilePath
            // 1 Guid
            // 2 IfcEntity
            // 3 Tag
            // 4 Layer
            // 5 PropertySetName
            // 6 PropertyName
            // 7 Value

            row.CreateCell(0).SetCellValue(file?.FilePath ?? string.Empty);
            row.CreateCell(1).SetCellValue(element?.Guid.ToString() ?? string.Empty);
            row.CreateCell(2).SetCellValue(element?.IfcEntity ?? string.Empty);
            row.CreateCell(3).SetCellValue(element?.Tag ?? string.Empty);
            row.CreateCell(4).SetCellValue(element?.Layer ?? string.Empty);

            row.CreateCell(5).SetCellValue(psetName ?? string.Empty);
            row.CreateCell(6).SetCellValue(propName ?? string.Empty);

            var valueCell = row.CreateCell(7);
            valueCell.SetCellValue(ToExcelString(value));
        }

        private static void WriteRow(
       IRow row,
       IfcFile file,
       IfcElement element,
       string psetName,
       string propName,
       object value,
       bool isFromPicklist)
        {
            // Column order:
            // 0 FilePath
            // 1 Guid
            // 2 IfcEntity
            // 3 Tag
            // 4 Layer
            // 5 PropertySetName
            // 6 PropertyName
            // 7 Value
            WriteRow(row, file, element, psetName, propName, value);
            row.CreateCell(8).SetCellValue(isFromPicklist);
        }

        private static string ToExcelString(object value)
        {
            if (value == null) return string.Empty;

            // If you have IFC types here, you might want to unwrap them.
            // For now we just stringify with some sensible handling.
            switch (value)
            {
                case DateTime dt:
                    return dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                case IFormattable fmt:
                    return fmt.ToString(null, CultureInfo.InvariantCulture);

                default:
                    return value.ToString() ?? string.Empty;
            }
        }
    }
}
