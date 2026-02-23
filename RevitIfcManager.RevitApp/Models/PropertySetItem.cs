using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitIfcManager.Models
{
    public class PropertySetItem
    {
        public string PropertySetName { get; set; }
        public List<PropertyItem> PropertyDefinitions { get; set; }
    }
}
