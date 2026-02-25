using System.Collections.Generic;

namespace IfcManager.BL.Json
{
    public class ExcelSettings
    {
        public string PropertySetNameColumn { get; set; }
        public string PropertyNameColumn { get; set; }
        public string DataTypeColumn { get; set; }
        public int HeaderRowIndex { get; set; }
        public string PropertiesSheetName { get; set; }
        public string PicklistSheetName { get; set; }
        public string MicrostationLayersMappingSheetName { get; set; }
        public string MicrostationLayerColumnName { get; set; }
        public List<string> PropertyMatchSheets { get; set; } = new List<string>();
        public List<string> PropertyExactMatchSheets { get; set; } = new List<string>();
        public string ExpressionsSheetName { get; set; }
        public string ComposedSheetName { get; set; }
    }
}
