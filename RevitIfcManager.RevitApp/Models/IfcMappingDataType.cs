using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitIfcManager.Models
{
    public static class IfcMappingDataType
    {
        public static string Get(string dataType)
        {
            if (string.IsNullOrEmpty(dataType))
            {
                return "Text";
            }

            return dataType.ToLower() switch
            {
                "string" => "Text",
                "int" => "Integer",
                "double" => "Real",
                "bool" => "Boolean",
                _ => "Text",
            };
        }
    }
}
