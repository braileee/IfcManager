using IfcManager.BL.Models;
using System.Collections.Generic;
using System.Text;

namespace RevitIfcManager.Models
{
    public class IfcMappingContent
    {
        public IfcMappingContent(List<PropertySetItem> propertySetItems)
        {
            PropertySetItems = propertySetItems;
        }

        public List<PropertySetItem> PropertySetItems { get; } = new List<PropertySetItem>();

        public string ToRevitTxtFormat()
        {
            StringBuilder stringBuilder = new StringBuilder();
            char tab = '\t';
            string instanceKind = "I";
            List<string> ifcTypes = new List<string> { "IfcElement", "IfcElementType" };
            string ifcTypesJoined = string.Join(", ", ifcTypes);

            foreach (var customPropertySet in PropertySetItems)
            {
                stringBuilder.AppendLine($"PropertySet:{tab}{customPropertySet.PropertySetName}{tab}{instanceKind}{tab}{ifcTypesJoined}");

                foreach (var property in customPropertySet.PropertyDefinitions)
                {
                    string type = IfcMappingDataType.Get(property.DataType);
                    stringBuilder.AppendLine($"{property.PropertyName}{tab}{type}{tab}{property.PropertyName}");
                }
            }

            return stringBuilder.ToString();
        }
    }
}
