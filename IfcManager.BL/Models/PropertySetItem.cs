using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfcManager.BL.Models
{
    public class PropertySetItem
    {
        public string PropertySetName { get; set; }
        public List<PropertyItem> PropertyItems { get; set; } = new List<PropertyItem>();
    }
}
