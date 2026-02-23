using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RevitIfcManager.Json
{
    public class ExcelSettings
    {
        public string PropertySetNameColumn { get; set; }
        public string PropertyNameColumn { get; set; }
        public string DataTypeColumn { get; set; }
        public int HeaderRowIndex { get; set; }
        public string PropertiesSheetName { get; set; }
        public string PicklistSheetName { get; set; }
        public List<string> PropertyMatchSheets { get; set; } = new List<string>();
        public string ExpressionsSheetName { get; set; }
        public string ComposedSheetName { get; set; }
        public List<string> PropertyExactMatchSheets { get;  set; } = new List<string>();
    }
}
