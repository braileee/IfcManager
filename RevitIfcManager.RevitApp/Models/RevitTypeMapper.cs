using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace RevitIfcManager.Models
{
    public static class RevitTypeMapper
    {
        public static ForgeTypeId GetSpecType(string dataType)
        {
            if (string.IsNullOrEmpty(dataType))
            {
                return SpecTypeId.String.Text;
            }

            return dataType.ToLower() switch
            {
                "string" => SpecTypeId.String.Text,
                "int" => SpecTypeId.Int.Integer,
                "double" => SpecTypeId.Number,
                "bool" => SpecTypeId.Boolean.YesNo,
                _ => SpecTypeId.String.Text,
            };
        }
    }
}

