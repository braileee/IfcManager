using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfcValidator.Models
{
    public class PropertySetItem
    {
        public string PropertySetName { get; set; }
        public List<PropertyItem> PropertyDefinitions { get; set; } = new List<PropertyItem>();
    }
}
